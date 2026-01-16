using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CarController : CarComponent
{
    public bool canControl = true;



    private CarGroundChecker Ground => GetComponent<CarGroundChecker>();
    private CarLocomotion Locomotion => GetComponent<CarLocomotion>();
    private CarCrashHandler Crash => GetComponent<CarCrashHandler>();
    private QTEController QTE => car.QTEController;
    private InGameUI Hud => car.Hud;
    private PlayerStats Stats =>GetComponent<PlayerStats>();
    private CarAudio Audio => GetComponent<CarAudio>();
  

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


    void Start()
    {
        //GameStart-> QTE 
        canControl = false;
        if (QTE != null) QTE.StartGameQTE(); 
        else canControl = true;
    }

    void Update()
    {
        if (!canControl) return;

        //QTE UI開けたらreturn 
        bool qteShowing = (Hud != null && Hud.QTEPanel != null && Hud.QTEPanel.activeInHierarchy);
        if (qteShowing) return;

        CheckGround();

        //左右input
        float steer = GetHorizontalInput();
        Locomotion?.SetSteer(steer);

        // 自動前進input 更新、drift start 判定
        Locomotion?.TickInputs(Ground != null && Ground.IsGrounded);

        // crash check
        Crash?.TickCrashCheck();

        //wallKnock
        // 壁ノックバック中：入力を無効化し、一定時間で解除
        if (isWallKnockback)
        {
            Audio?.StopAccel();
            Audio?.PlayCrash();
            car.Rigidbody.linearVelocity = Vector3.zero;
            car.Rigidbody.angularVelocity = Vector3.zero;

            //タイマー
            wallKnockbackTimer += Time.deltaTime;
            if (wallKnockbackTimer >= wallKnockbackLockTime)
            {
                isWallKnockback = false;
            }
            return; //HandleDriftInput動作しない場合
        }
        //
        //無敵
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                Debug.Log("Invincible end");
            }
        }
    }

   
    public void CheckGround() => Ground?.CheckGround();

    public void CarCrash() => Crash?.TriggerCrash();

    public void SetCanControl(bool enabled) => canControl = enabled;

    public void OnStartQTESuccess()
    {
        canControl = true;
        // StartQTE Boost
        var rb = car.Rigidbody;
        Vector3 dir = transform.forward; dir.y = 0f; dir.Normalize();
        rb.linearVelocity = dir * 40f;
    }

    public void OnStartQTEFail(){
        canControl = true;

        var rb = car.Rigidbody;
        Vector3 dir = transform.forward; dir.y = 0f; dir.Normalize();
        rb.linearVelocity = dir * 5f;
    }

    float GetHorizontalInput() //Car input
    {
        // Gamepad
        if (Gamepad.current != null)
        {
            float h = Gamepad.current.leftStick.x.ReadValue();
            if (Mathf.Abs(h) < 0.15f) h = 0f;
            return Mathf.Clamp(h, -1f, 1f);
        }

        // Keyboard
        float k = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) k -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) k += 1f;
        }
        return Mathf.Clamp(k, -1f, 1f);
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
        car.Rigidbody.linearVelocity = Vector3.zero;

        // 後退＋上方向の速度を付与
        Vector3 newVel = knockDir * wallKnockbackSpeed + Vector3.up * wallKnockbackUp;
        car.Rigidbody.linearVelocity = newVel;


        wallKnockbackTimer = 0f;
        // X回転ロック（転倒防止目的など）
        //Rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
    }

    //ダメージする時エフェクトや効果あったらここに
    void GetDamage()
    {
        Stats?.TakeDamage(1);
        //無敵on
        isInvincible = true;
        invincibleTimer = invincibleTime;
    }
}