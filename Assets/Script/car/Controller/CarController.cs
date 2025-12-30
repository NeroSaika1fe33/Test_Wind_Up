using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]

public class CarController : CarComponent
{
    //ドリフト状態 
    enum DriftState
    {
        None,
        Preparing,
        Drifting,
        Cooldown
    }
    //車の角度制御
    [Header("Ground Normal Align")]
    [SerializeField] private Transform carModel;   
    [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private float groundAlignSpeed = 7.5f;
    [SerializeField] private LayerMask groundMask;

    [Header("Auto Move")]
    [Range(0f, 1f)]
    public float autoForwardInput = 0.4f;               //自動前進の力 return->inputV
    float inputV;                                       //自動前進
    float inputH;                                       //左右

    [Header("Speed")]
    [SerializeField] private int energy = 1000;         //エネルギー、未実装
    [SerializeField] private float turnSpeed = 3f;       //左右旋回の基本速度（FixedUpdateでyaw量に変換）、ステータスから未実装
    private float acceleration;                         //加速力
    private float maxSpeed;                             //最大速度(M/S)

    [Header("Gravity")] //重力
    public float airGravityMultiplier = 2f;             // 空中時の重力増幅 (推奨2~6)
    public float downforcePerSpeed = 0.3f;              // 地面に掴む力(ウンフォース)(0.1~0.6)
    public float maxDownforce = 10f;                    //ウンフォース上限 (5~20)

    [Header("Drift")]
    public KeyCode driftKey = KeyCode.E;                //ドリフトきー
    public float driftMinSpeed = 30f;                   //発動最低速度

    public float prepareTime = 2f;                      //発動掛かる時間
    public float driftTime = 2f;                        //発動時間
    public float cooldownTime = 2f;                     //冷却時間

    public float prepareSpeed = 0.85f;                  //発動準備時、現時点速度下がる
    public float driftSpeed = 1.5f;                     //ドリフト現時点速度掛ける
    public float cooldownSpeed = 0.75f;
    public float boostedSpeed = 0f;                     //ドリフト開始時、スピード計算用

    [Header("Wall Knockback")]
    public string wallTag = "Wall";                     //unity tag
    public float wallKnockbackSpeed = 20f;              //後退速度
    public float wallKnockbackUp = 2f;                  //ぶつかった後、Ｙ軸増やす
    public float wallKnockbackLockTime = 0.2f;          //壁をぶつかる後、CAR LOCK時間
    bool isWallKnockback = false;                       //発動FLAG
    float wallKnockbackTimer = 0f;

    [Header("InvicibleControl")]
    bool isInvincible = false;                          //無敵状態
    float invincibleTimer = 0f;                         //無敵状態カウントダウン
    public float invincibleTime = 2f;                   //無敵時間
    public bool IsInvincible => isInvincible;

    public float steerSmooth = 6f;                      // 左右入力の追従（大きいほど即反応／小さいほど鈍い）
    float smoothH = 0f;                                 // 平滑化された左右入力 fixedUpdate用

    [Header("Start QTE")]
    public float startQTECountdown = 3f;                // gamestart 3,2,1カウント
    public float startBoostSpeed = 40f;                 //成功後、BOOSTの速度

    [Header("Child")]//使ってない
    private bool groundedFront;
    private bool groundedBack;

    [Header("Ground Check")]
    public LayerMask groundLayer;                       // 地面レイヤー
    public float groundCheckDistance = 1.0f;            // レイの長さ
    [SerializeField] private bool isGrounded;           // 接地状態check
    private Transform groundFront; //使ってない
    private Transform groundBack;//使ってない
    bool wasGrounded;//使ってない
    public float landingDamp = 0.3f;　                  // 着地時に落下速度（y）を減衰させる係数
    public float angularDampOnGround = 6f;　            // 接地時の回転（x/z）減衰

    [Header("Grip (Handling)")]
    [Range(0f, 1f)]
    public float lateralGrip = 0.2f;                    // 横滑り抑制
    public float gripOnlyWhenGrounded = 1f;             // 1=contact Ground = ture；

    [Header("Anti-Lift (Downforce)")]
    public float antiLiftForce = 3f;                    // 片側だけ接地している時に下へ押す力（アンチリフト）3~10， -transform.up * 5f
    public float maxDownforceSpeed = 50f;               // 速度に対する係数kの上限計算用

    [Header("Steering Physics")]
    public float yawTorque = 5f;                        //使ってない回転はMoveRotationで行っている）
    public float yawDamp = 4f;　　　　                  //使ってない
    DriftState driftState = DriftState.None;
    float driftTimer = 0f;

    public bool canControl = true;                      // 操作可能flag

    GameObject[] frontWheelObjects;

    //読みやすくするため
    private Rigidbody Rigidbody => car.Rigidbody;
    private QTEController QTEController => car.QTEController;
    private PlayerStats PlayerStats => car.PlayerStats;
    private CarAudio CarAudio => car.CarAudio;
    private InGameUI Hud => car.Hud;
    private PartsContainer PartsContainer => car.PartsContainer;

    void Start()
    {
        //パネル off
        //if (car.Hud.QTEPanel != null)
        //    car.Hud.QTEPanel.SetActive(false);

        carModel = transform.Find("PartsPos");

        if (carModel == null)
        {
            Debug.LogError("CarController: Part が見つかりません！");
        }

        frontWheelObjects = new GameObject[2];
        frontWheelObjects[0] = PartsContainer.Installation_Location_Wheel_FrontRight.GetChild(0).gameObject;
        frontWheelObjects[1] = PartsContainer.Installation_Location_Wheel_FrontLeft.GetChild(0).gameObject;

        //Rigidbody初期化（補間＆連続衝突：高速時の抜け対策）
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rigidbody.maxAngularVelocity = 20f;

        //QTEが終わるまで操作不可
        canControl = false;

        if (QTEController != null)
        {
            //ゲーム開始QTE開始（成功/失敗でコールバック)
            QTEController.StartGameQTE();
        }
        else
        {
            canControl = true;   //QTEなしなら即開始
        }

        // 選択データ反映
        acceleration = PlayerStats.acceleration;
        maxSpeed = PlayerStats.maxSpeed;
        Debug.Log("Car Start:加速度:" + acceleration + " 最高速度:" + maxSpeed);

    }

    void Update()
    {
        if (!canControl) return;// 操作不可なら終了（例：HP=0など）

        bool qteShowing = (Hud.QTEPanel != null && Hud.QTEPanel.activeInHierarchy);//qteあるか確認

        if (qteShowing)//qte出現時、accel音声stop,zenmai音声start
        {
            CarAudio?.StopAccel();
            CarAudio?.StartZenmai();
            return;
        }

        //毎フレーム接地判定更新
        CheckGround();

        // 壁ノックバック中：入力を無効化し、一定時間で解除
        if (isWallKnockback)
        {
            CarAudio?.StopAccel();
            CarAudio?.PlayCrash();
            inputV = 0f;
            inputH = 0f;

            //タイマー
            wallKnockbackTimer += Time.deltaTime;
            if (wallKnockbackTimer >= wallKnockbackLockTime)
            {
                isWallKnockback = false;
            }
            return; //HandleDriftInput動作しない場合
        }

        // 接地中は自動前進、空中は前進入力0
        if (isGrounded)
        {
            inputV = autoForwardInput;
        }
        else
        {
            inputV = 0f;    // 地面に接触していない
        }

        inputH = GetHorizontalInput(); // 左右入力（-1～1）A/D
        //wheelcollider制御
        wheelSteerAngle(inputH);
        // 入力平滑化（追従速度はsteerSmooth）
        smoothH = Mathf.Lerp(smoothH, inputH, steerSmooth * Time.deltaTime);

        //無敵Timer
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                Debug.Log("Invincible end");
            }
        }

        //クラッシュ判定
        if (PlayerStats.currentHP <= 0)
        {
            CarCrash();
        }

        // ドリフト開始判定
        HandleDriftInput();

        bool inCharge = (driftState == DriftState.Preparing);

        if (inCharge)
        {
            // 優先的にchargeしばらくaccel loopを止める
            CarAudio?.StopAccel();
            CarAudio?.StartDriftCharge();
        }
        else
        {
            CarAudio?.StopDriftCharge();
            bool shouldPlayAccel = isGrounded && inputV > 0.1f;
            if (shouldPlayAccel) CarAudio?.StartAccel();
            else CarAudio?.StopAccel();
        }

        GroundNormalRotation();
    }

    void CheckGround()
    {
        // 前後の位置からレイで接地判定
        groundedFront = RayIsGround(groundFront != null ? groundFront.position : (transform.position + transform.forward * 0.8f));
        groundedBack = RayIsGround(groundBack != null ? groundBack.position : (transform.position - transform.forward * 0.8f));

        // どちらかが接地なら接地扱い
        isGrounded = groundedFront || groundedBack;
    }

    bool RayIsGround(Vector3 origin)
    {
        // 車体の下方向へレイ（傾きに追従）
        Vector3 dir = -transform.up;
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(origin, dir, out hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

        // デバッグ描画（緑=接地、赤=未接地
        Debug.DrawRay(origin, dir * groundCheckDistance, hitSomething ? Color.green : Color.red);

        if (!hitSomething) return false;

        // 斜面角制限（法線と上方向の角度が60度以下なら接地とみなす）
        float angle = Vector3.Angle(hit.normal, Vector3.up);
        return angle <= 60f;
    }

    void FixedUpdate()
    {
        // FreezeRotationYが設定されているか（※この変数は現状未使用）
        bool freezeY = (Rigidbody.constraints & RigidbodyConstraints.FreezeRotationY) != 0;


        // 操作不可なら終了
        if (!canControl) return;//hp = 0 -> car can"t  control

        // 着地直後：落下速度yを減衰させてバウンドを抑える
        if (!wasGrounded && isGrounded)
        {
            Vector3 v = Rigidbody.linearVelocity;
            if (v.y < 0f) v.y *= landingDamp;
            Rigidbody.linearVelocity = v;
        }

        // 接地中：回転（x/z）を減衰して安定化
        if (isGrounded)
        {
            Vector3 av = Rigidbody.angularVelocity;

            av.x = Mathf.Lerp(av.x, 0f, angularDampOnGround * Time.fixedDeltaTime);
            av.z = Mathf.Lerp(av.z, 0f, angularDampOnGround * Time.fixedDeltaTime);

            Rigidbody.angularVelocity = av;
        }

        wasGrounded = isGrounded;

        // ドリフト状態更新
        UpdateDriftState(); //drift
                            //drift によって加速/最高速倍率が変わる

        float speedMul = GetCurrentSpeedMultiplier(); //状態ごとの倍率を取得

        //現在速度（m/s）
        //float currentSpeed = Rigidbody.linearVelocity.magnitude;  // liner=Rigidbody(x,y,z) magnitude= longspeed sqrt()->m/s
        Vector3 vel = Rigidbody.linearVelocity;
        float currentSpeed = vel.magnitude;

        //状態込みの最高速
        float currentMaxSpeed = maxSpeed * speedMul;

        //ブースト速度計算（※固定ベクトルのmagnitude * driftSpeed）
        boostedSpeed = (new Vector3(20.0f, 20.0f, 20.0f)).magnitude * driftSpeed;

        // 最高速未満なら前進加速
        if (currentSpeed < currentMaxSpeed)
        {
            Rigidbody.AddForce(transform.forward * inputV * acceleration * speedMul, ForceMode.Acceleration);
        }


        //横滑り抑制（lateralGripは「小さいほど横滑りが減る」仕様）
        ApplyLateralGrip();

        // ドリフト中は旋回倍率UP
        float turnMul = (driftState == DriftState.Drifting) ? 1.2f : 1f;

        // 接地中のみ旋回＆ダウンフォース
        if (isGrounded)
        {

            float yawDegPerSec = turnSpeed * 120f * turnMul;
            float yawThisStep = smoothH * yawDegPerSec * Time.fixedDeltaTime;

            float speed = Rigidbody.linearVelocity.magnitude;
            float down = Mathf.Clamp(speed * downforcePerSpeed, 0f, maxDownforce);
            Rigidbody.AddForce(-transform.up * down, ForceMode.Acceleration);

            Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(0f, yawThisStep, 0f));

            Vector3 av = Rigidbody.angularVelocity;
            av.y = 0f;
            Rigidbody.angularVelocity = av;
        }


        // 前後どちらかだけ接地している時のアンチリフト
        ApplyAntiLift();

        // 空中：重力を強める
        if (!isGrounded)
        {
            Rigidbody.AddForce(Physics.gravity * airGravityMultiplier, ForceMode.Acceleration);
        }
    }

    void ApplyLateralGrip()
    {
        // 接地してないなら何もしない
        if (!isGrounded) return;

        // ワールド速度→ローカル速度へ変換
        Vector3 v = Rigidbody.linearVelocity;
        Vector3 localV = transform.InverseTransformDirection(v);

        // 横方向（x）だけ減衰：localV.x *= lateralGrip
        // 例：lateralGrip=0.2 なら横滑りが20%だけ残る（=かなり粘る）
        localV.x *= lateralGrip;

        // ローカル→ワールドへ戻す
        Rigidbody.linearVelocity = transform.TransformDirection(localV);
    }

    void ApplyAntiLift()
    {
        // 前後とも未接地なら無視
        if (!groundedFront && !groundedBack) return;

        // XOR：片側だけ接地している（車体が前後どちらか浮いている）
        bool tilt = groundedFront ^ groundedBack;
        if (!tilt) return;

        // 速度に応じた係数k（0～1）
        float speed = Rigidbody.linearVelocity.magnitude;
        float k = Mathf.Clamp01(speed / Mathf.Max(1f, maxDownforceSpeed));

        Rigidbody.AddForce(-transform.up * antiLiftForce * k, ForceMode.Acceleration);
    }

    void HandleDriftInput()
    {
        // すでにドリフト中（準備/本体/クールダウン）なら開始判定しない
        if (driftState != DriftState.None)
            return;

        float currentSpeed = Rigidbody.linearVelocity.magnitude;

        // m/s → km/h
        float kmh = currentSpeed * 3.6f;

        if (Input.GetKeyDown(driftKey))
        {
            Debug.Log($"Speed: {kmh:F1} km/h | State: {driftState} | V={inputV} H={inputH}");
        }

        // ドリフト条件：加速中＆回転中＆キー押下＆最低速度以上
        bool isAccelerating = inputV > 0.1f;
        bool isTurning = Mathf.Abs(inputH) > 0.1f;
        bool driftPressed = Input.GetKeyDown(driftKey);

        if (Input.GetKeyDown(driftKey))
        {
            Debug.Log($"Speed={currentSpeed}  accel={isAccelerating}  turn={isTurning}  pressed={driftPressed}");
        }

        if (currentSpeed >= driftMinSpeed && isAccelerating && isTurning && driftPressed)
        {
            // 準備状態へ
            driftState = DriftState.Preparing;
            driftTimer = 0f;
            Debug.Log("Drift prepare start");
        }
    }

    void UpdateDriftState()
    {
        if (driftState == DriftState.None)
            return;

        // 状態タイマー更新（※deltaTime使用：現状の実装を維持）
        driftTimer += Time.deltaTime;

        switch (driftState)
        {
            case DriftState.Preparing:
                if (driftTimer >= prepareTime)
                {
                    driftState = DriftState.Drifting;
                    driftTimer = 0f;
                    Debug.Log("Drift start!");// Debug.Log("Drift start!");

                    //ブースト：現在速度の方向に boostedSpeed を乗せる
                    Vector3 boostedVel = Rigidbody.linearVelocity.normalized * boostedSpeed;

                    Rigidbody.linearVelocity = boostedVel;
                    CarAudio?.PlayBoost();
                    Debug.Log("boosted speed = " + boostedVel.magnitude);
                }
                break;

            case DriftState.Drifting:
                if (driftTimer >= driftTime)
                {
                    driftState = DriftState.Cooldown;
                    driftTimer = 0f;
                    Debug.Log("Drift cooldown");// Debug.Log("Drift cooldown");
                }
                break;

            case DriftState.Cooldown:
                if (driftTimer >= cooldownTime)
                {
                    driftState = DriftState.None;
                    driftTimer = 0f;
                    Debug.Log("Drift end");// Debug.Log("Drift end");
                }
                break;
        }
    }

    float GetCurrentSpeedMultiplier()
    {
        // ドリフト状態ごとの速度倍率
        switch (driftState)
        {
            case DriftState.Preparing:
                return prepareSpeed;
            case DriftState.Drifting:
                return driftSpeed;
            case DriftState.Cooldown:
                return cooldownSpeed;
            default:
                return 1f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        // 壁（tag=wall）に当たった時のみ処理
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("HIT WALL: " + collision.gameObject.name);
            //無敵中はノックバックしない
            if (IsInvincible)
            {
                return;
            }
            else if (!isInvincible && collision.gameObject.CompareTag("Wall"))       //無敵状態でなく、かつ衝突相手のタグが「Wall」のときだけダメージ処理
            {
                // 壁に当たったら 1 ダメージ
                GetDamage();
            }

            // 操作ロック開始
            isWallKnockback = true;
        }

        ContactPoint contact = collision.GetContact(0);

        // ワールド座標の接触点を車ローカルに変換
        // localHitPoint.z > 0 なら車の前側、< 0 なら後側
        Vector3 localHitPoint = transform.InverseTransformPoint(contact.point); //// localHitPoint.z > 0 car infront
                                                                                // localHitPoint.z < 0 car back
                                                                                // 前側以外（z<=0）は無視
        if (localHitPoint.z <= 0f)
        {
            return;
        }

        // 後退方向（-forward）を水平にして正規化
        Vector3 knockDir = -transform.forward;
        knockDir.y = 0f;
        knockDir.Normalize();

        // 速度を一旦0にする（リセット）
        Rigidbody.linearVelocity = Vector3.zero;

        // 後退＋上方向の速度を付与
        Vector3 newVel = knockDir * wallKnockbackSpeed + Vector3.up * wallKnockbackUp;
        Rigidbody.linearVelocity = newVel;


        wallKnockbackTimer = 0f;
        // X回転ロック（転倒防止目的など）
        //Rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
    }

    public void OnStartQTESuccess()
    {
        // QTE成功：操作開始＆初速付与
        canControl = true;
        CarAudio?.PlayBoost();

        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        //初速を設定
        Rigidbody.linearVelocity = dir * startBoostSpeed;
    }

    // QTE失敗でもゲーム開始
    public void OnStartQTEFail()
    {
        canControl = true;
    }

    //HPゼロになるとクラッシュ
    void CarCrash()
    {
        canControl = false;
        //TODO:12.28 make car.QTEController a Prefab 
        QTEController.Minigame();
        Debug.Log("Car is crash");
    }

    //ダメージする時エフェクトや効果あったらここに
    void GetDamage()
    {
        PlayerStats.TakeDamage(1);
        //無敵on
        isInvincible = true;
        invincibleTimer = invincibleTime;
    }

    //車輪を左右旋回
    public void wheelSteerAngle(float steerInput)
    {
        float maxSteerAngle = 45f;

        float angle = steerInput * maxSteerAngle;

        for (int i = 0; i < frontWheelObjects.Length; i++)
        {
            Transform wheel = frontWheelObjects[i].transform;

            Vector3 localEuler = wheel.localEulerAngles;
            localEuler.y = angle;
            wheel.localEulerAngles = localEuler;
        }
    }

    //ゲームパッド対応
    float GetHorizontalInput()
    {
        float h = 0f;

        if (Gamepad.current != null)
        {
            h = Gamepad.current.leftStick.x.ReadValue();

            if (Mathf.Abs(h) < 0.15f)
                h = 0f;
        }

        if (Mathf.Abs(h) < 0.01f)
        {
            h = Input.GetAxis("Horizontal");
        }

        return Mathf.Clamp(h, -1f, 1f);
    }

    void GroundNormalRotation()
    {
        if (!isGrounded) return;

        Vector3 origin = Rigidbody.worldCenterOfMass;

        if (Physics.SphereCast(
            origin,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.magenta);

            Vector3 modelUp = carModel.up;

            Quaternion targetRot =
                Quaternion.FromToRotation(modelUp, hit.normal) * carModel.rotation;

            carModel.rotation = Quaternion.Lerp(
                carModel.rotation,
                targetRot,
                groundAlignSpeed * Time.deltaTime
            );
        }
    }
}


