using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarGroundDown : CarComponent
{
    [Header("References")]
    [SerializeField] private CarGroundChecker ground; 

    [Header("Downforce")]
    [Tooltip("地面接触時DownForce")]
    [SerializeField] private float baseDownforce = 25f;

    [Tooltip("速度より大きくなる：downforce += speed * speedDownforce")]
    [SerializeField] private float speedDownforce = 3f;

    [Header("Anti Pop-up")]
    [Tooltip("地面に離れる時間内，DownForce續ける")]
    [SerializeField] private float coyoteTime = 0.12f;

    [Tooltip("Yのspeedはこの値より大きなら，この値まで減る")]
    [SerializeField] private float maxUpVelocity = 0.5f;

  

    [SerializeField] private float alignSpeed = 6f;

    private Rigidbody rb;
    private float lastGroundedTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (ground == null) ground = GetComponent<CarGroundChecker>();
    }

    void FixedUpdate()
    {
        if (ground == null) return;

        // ground.CheckGround();

        bool grounded = ground.IsGrounded;
        if (grounded) lastGroundedTime = Time.time;

        float speed = rb.linearVelocity.magnitude;

        // 1) 地面接触時強くなる
        bool withinCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        if (grounded || withinCoyote)
        {
            float df = baseDownforce + speed * speedDownforce;
            rb.AddForce(-transform.up * df, ForceMode.Acceleration);
        }

        // 2) 飛ぶ制御
        Vector3 v = rb.linearVelocity;
        if (!grounded && v.y > maxUpVelocity)
        {
            v.y = maxUpVelocity;
            rb.linearVelocity = v;
        }

        
    }
}