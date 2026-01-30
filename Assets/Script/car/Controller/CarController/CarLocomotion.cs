using UnityEngine;

// 移動：自動前進 + 左右
[RequireComponent(typeof(Rigidbody))]
public class CarLocomotion : CarComponent
{
    [Header("Auto Move")]
    [Range(0f, 1f)]
    [SerializeField] private float autoForwardInput = 0.4f;

    [Header("Speed")]
    [SerializeField] private float turnSpeed = 3f;

    [Header("Steer Smoothing")]
    [SerializeField] private float steerSmooth = 6f;

    private float inputV;
    private float inputH;
    private float smoothH;

    private Rigidbody Rigidbody => car.Rigidbody;
    private PlayerStats PlayerStats => car.PlayerStats;

    private CarGroundChecker Ground => GetComponent<CarGroundChecker>();
    private CarDriftModule Drift => GetComponent<CarDriftModule>();

    public void SetSteer(float steer01) => inputH = Mathf.Clamp(steer01, -1f, 1f);

    //  CarController call：更新 inputV（自動前進）
    public void TickInputs(bool isGrounded)
    {
        inputV = isGrounded ? autoForwardInput : 0f;
        smoothH = Mathf.Lerp(smoothH, inputH, steerSmooth * Time.deltaTime);

        // drift 判定開始
        Drift?.TickStartCondition(inputV, inputH);
    }

    private void FixedUpdate()
    {
        // CarControllerでcanControl管理する
        if (car.Controller != null && !car.Controller.canControl) return;

        bool grounded = Ground != null && Ground.IsGrounded;
        var drift = Drift;

        // drift 状態
        drift?.TickStateMachine();

        const float KMH_TO_MS = 1f / 3.6f;

        float speedMul = drift != null ? drift.SpeedMultiplier : 1f;
        float turnMul = drift != null ? drift.TurnMultiplier : 1f;

        float accel = PlayerStats != null ? PlayerStats.acceleration : 0f;
        float maxSp = PlayerStats != null ? PlayerStats.maxSpeed * KMH_TO_MS : 20f * KMH_TO_MS;

        // 前進
        float currentSpeed = Rigidbody.linearVelocity.magnitude;
        float currentMax = maxSp * speedMul;

        if (grounded && currentSpeed < currentMax)
        {
            Rigidbody.AddForce(transform.forward * inputV * accel , ForceMode.Acceleration);

            if (Rigidbody.IsSleeping())
            {
                Rigidbody.WakeUp();
            }
        }

        Debug.Log(transform.forward * inputV * accel);

        //速度制限  
        Vector3 v = Rigidbody.linearVelocity;
        float speed = v.magnitude;
        if (speed > currentMax && speed > 0.01f)
        {
            Rigidbody.linearVelocity = v.normalized * currentMax;
        }
        // 左右
        if (grounded)
        {
            float yawDegPerSec = turnSpeed * 120f * turnMul;
            float yawThisStep = smoothH * yawDegPerSec * Time.fixedDeltaTime;
            Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(0f, yawThisStep, 0f));
        }
    }

    public void SetVelocity(Vector3 v)
    {
        car.Rigidbody.linearVelocity = v;
    }
}