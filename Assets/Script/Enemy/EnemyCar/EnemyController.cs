using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Windows;

public class EnemyController : EnemyComponent
{
    private EnemyStats EnemyStats => GetComponent<EnemyStats>();
    private Rigidbody Rigidbody => car.Rigidbody;

    public bool canControl = false;
    public float timer = 5.0f;
    [SerializeField] private LayerMask wallLayer;

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


    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1.0f;
    [SerializeField] private float slopeLimitDeg = 60f;

    public bool IsGrounded { get; private set; }
    private bool groundedFront;
    private bool groundedBack;

    public void CheckGround()
    {
        var t = transform;
        groundedFront = RayIsGround(t.position + t.forward * 0.8f);
        groundedBack = RayIsGround(t.position - t.forward * 0.8f);
        IsGrounded = groundedFront || groundedBack;
    }

    private bool RayIsGround(Vector3 origin)
    {
        Vector3 dir = -transform.up;
        bool hit = Physics.Raycast(origin, dir, out RaycastHit info, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

        Debug.DrawRay(origin, dir * groundCheckDistance, hit ? Color.green : Color.red);

        if (!hit) return false;

        float angle = Vector3.Angle(info.normal, Vector3.up);
        return angle <= slopeLimitDeg;
    }

    void Update()
    {
        const float KMH_TO_MS = 1f / 3.6f;

        float accel = EnemyStats != null ? EnemyStats.acceleration : 0f;
        float maxSp = EnemyStats != null ? EnemyStats.maxSpeed * KMH_TO_MS : 20f * KMH_TO_MS;

        // 残り時間を減らす
        if (timer > 0f)
            timer -= Time.deltaTime;

        if (timer < 0f)
        {
            canControl = true;
            Rigidbody.linearVelocity = transform.forward * 0.4f * accel;
            timer = 0f;
        }

        if (!canControl) return;

        CheckGround();

        // 自動前進input 更新、drift start 判定
        if (!IsGrounded) return;
        bool grounded = IsGrounded;



        // 前進
        float currentSpeed = Rigidbody.linearVelocity.magnitude;
        float currentMax = maxSp *0.8f;

        if (grounded && currentSpeed < currentMax)
        {
            Rigidbody.AddForce(transform.forward * 0.4f * accel, ForceMode.Acceleration);
        }
        //速度制限  
        Vector3 v = Rigidbody.linearVelocity;
        float speed = v.magnitude;
        if (speed > currentMax && speed > 0.01f)
        {
            Rigidbody.linearVelocity = v.normalized * currentMax;
        }

        //wallKnock
        // 壁ノックバック中：入力を無効化し、一定時間で解除
        if (isWallKnockback)
        {
            //タイマー
            wallKnockbackTimer += Time.deltaTime;
            if (wallKnockbackTimer >= wallKnockbackLockTime)
            {
                isWallKnockback = false;
                wallKnockbackTimer = 0.0f;
            }
            return; //HandleDriftInput動作しない場合
        }

        //無敵
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                isWallKnockback = false;
                Debug.Log("Invincible end");
            }
        }
    }

    bool IsWall(GameObject obj)
    {
        return (wallLayer.value & (1 << obj.layer)) != 0;
    }

    void OnCollisionStay(Collision collision)
    {

        // 壁（tag=wall）に当たった時のみ処理
        if (IsWall(collision.gameObject))
        {
            Debug.Log("HIT WALL: " + collision.gameObject.name);
            //無敵中はノックバックしない
            if (IsInvincible)
            {
                return;
            }
            else if (!isInvincible && IsWall(collision.gameObject))       //無敵状態でなく、かつ衝突相手のタグが「Wall」のときだけダメージ処理
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
        //Stats?.TakeDamage(1);
        //無敵on
        isInvincible = true;
        invincibleTimer = invincibleTime;
    }
}