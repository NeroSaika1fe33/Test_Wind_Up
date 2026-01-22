using UnityEngine;

// ínñ ê⁄êGîªíËÅFGround CheckÇæÇØÅAà⁄ìÆÇµÇ»Ç¢
public class CarGroundChecker : CarComponent
{
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
}