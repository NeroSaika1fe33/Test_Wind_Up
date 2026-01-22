using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarXYZ : CarComponent
{
    [Header("Refs")]
    [SerializeField] private CarGroundChecker ground; 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayDistance = 2.0f;

    [Header("Stabilize Strength")]
    [SerializeField] private float uprightTorque = 40f;

    [Tooltip("角度制御")]
    [SerializeField] private float angularDamping = 6f;

    [Header("Mode")]
    [Tooltip("true = 地面法線に沿って；false = ワールドアップ")]
    [SerializeField] private bool alignToGroundNormal = true;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (ground == null) ground = GetComponent<CarGroundChecker>();
    }

    void FixedUpdate()
    {
        if (ground == null) return;

        
        bool grounded = ground.IsGrounded;
        if (!grounded) return;

        Vector3 targetUp = Vector3.up;

        if (alignToGroundNormal)
        {
            //rayで targetUp
            Vector3 origin = transform.position + Vector3.up * 0.2f;
            if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, rayDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                targetUp = hit.normal;
            }
        }

        // 1) Upright torque： transform.upから targetUpに変更（roll/pitchだけ影響）
        Vector3 axis = Vector3.Cross(transform.up, targetUp);
        float angle = axis.magnitude;

        if (angle > 0.0001f)
        {
            Vector3 torque = axis.normalized * (angle * uprightTorque);
            rb.AddTorque(torque, ForceMode.Acceleration);
        }

        // 2) Angular damping：転んで制御
        Vector3 av = rb.angularVelocity;
        av.x = Mathf.Lerp(av.x, 0f, angularDamping * Time.fixedDeltaTime);
        av.z = Mathf.Lerp(av.z, 0f, angularDamping * Time.fixedDeltaTime);
        rb.angularVelocity = av;
        
    }
}
