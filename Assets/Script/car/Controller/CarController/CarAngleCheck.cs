using UnityEngine;

public class CarAngleCheck : MonoBehaviour
{
    [SerializeField] Transform visualModel;
    [SerializeField] LayerMask groundMask;

    public float castDistance = 1f;//判定距離
    public float castRadius = 0.25f;//SphereCastの半径(球体の大きさ)
    public float pitchSmooth = 8f;//大きくすると、速く地面にくっつく
    public float maxPitch = 25f; //最大角度制御

    float currentPitch = 0f;//現在の角度

    Quaternion baseLocalRot;

    void Awake()
    {
        if (visualModel != null)
            baseLocalRot = visualModel.localRotation; // モデルの基準角度を覚える
    }

    void LateUpdate()
    {
        AlignPitchToSlope();
    }

    void AlignPitchToSlope()
    {
        if (visualModel == null) return;

        Vector3 origin = transform.position + Vector3.up * 0.2f;

        if (Physics.SphereCast(origin, castRadius, Vector3.down, out RaycastHit hit, castDistance, groundMask))
        {
            Vector3 n = hit.normal;

            Vector3 right = transform.right;
            Vector3 nInForwardPlane = Vector3.ProjectOnPlane(n, right).normalized;

            float angle = Vector3.SignedAngle(Vector3.up, nInForwardPlane, right);
            float targetPitch = Mathf.Clamp(angle, -maxPitch, maxPitch);

            currentPitch = Mathf.Lerp(currentPitch, targetPitch, pitchSmooth * Time.deltaTime);

            float yaw = visualModel.localEulerAngles.y;

            Quaternion target =
                Quaternion.Euler(0f, yaw, 0f) * baseLocalRot * Quaternion.Euler(currentPitch, 0f, 0f);

            visualModel.localRotation = Quaternion.Slerp(
                visualModel.localRotation,
                target,
                pitchSmooth * Time.deltaTime
            );
        }
        else
        {
            float yaw = visualModel.localEulerAngles.y;

            Quaternion target = Quaternion.Euler(0f, yaw, 0f) * baseLocalRot;

            visualModel.localRotation = Quaternion.Slerp(
                visualModel.localRotation,
                target,
                pitchSmooth * Time.deltaTime);
        }
    }

}
