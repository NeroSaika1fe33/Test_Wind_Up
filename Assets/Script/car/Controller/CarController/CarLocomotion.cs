using System.Collections;
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

    [Header("Air Control")]
    [SerializeField] private bool allowAirSteer = true;
    [SerializeField, Range(0f, 1f)] private float airTurnMultiplier = 0.35f;

    [Header("Item Buff ")]
    [SerializeField] private float itemMaxSpeedMultiplier = 1f;
    [SerializeField] private float itemAccelerationMultiplier = 1f;

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
        inputV = autoForwardInput;
        smoothH = Mathf.Lerp(smoothH, inputH, steerSmooth * Time.deltaTime);

        // drift 判定開始
        if (isGrounded)
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

        float accelBase = PlayerStats != null ? PlayerStats.acceleration : 0f;
        float maxSpBase = PlayerStats != null ? PlayerStats.maxSpeed * KMH_TO_MS : 20f * KMH_TO_MS;

        //Item buffを使う
        float accel = accelBase * itemAccelerationMultiplier;
        float maxSp = maxSpBase * itemMaxSpeedMultiplier;

        // 前進
        float currentSpeed = Rigidbody.linearVelocity.magnitude;
        float currentMax = maxSp * speedMul;

        if (grounded && currentSpeed < currentMax)
        {
            Rigidbody.AddForce(transform.forward * inputV * accel, ForceMode.Acceleration);
        }
        //速度制限  
        Vector3 v = Rigidbody.linearVelocity;
        float speed = v.magnitude;
        if (speed > currentMax && speed > 0.01f)
        {
            Rigidbody.linearVelocity = v.normalized * currentMax;
        }
        // 左右
        float steerMul = grounded ? 1f : (allowAirSteer ? airTurnMultiplier : 0f);
        if (steerMul > 0f)
        {
            float yawDegPerSec = turnSpeed * 120f * turnMul * steerMul;
            float yawThisStep = smoothH * yawDegPerSec * Time.fixedDeltaTime;
            Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(0f, yawThisStep, 0f));
        }
    }

    public void SetVelocity(Vector3 v)
    {
        car.Rigidbody.linearVelocity = v;
    }

   
    //item buff
    // 例 maxSpeedMultiplier = 1.3f 表示最高速度 +30%
    
    public void ApplyItemBuff(float maxSpeedMultiplier, float accelerationMultiplier)
    {
        itemMaxSpeedMultiplier = Mathf.Max(0.01f, maxSpeedMultiplier);
        itemAccelerationMultiplier = Mathf.Max(0.01f, accelerationMultiplier);

        Debug.Log($"[CarLocomotion] ApplyItemBuff maxSpeed x{itemMaxSpeedMultiplier}, accel x{itemAccelerationMultiplier}");
    }

   
    //  buff delete
    
    public void ClearItemBuff()
    {
        itemMaxSpeedMultiplier = 1f;
        itemAccelerationMultiplier = 1f;

        Debug.Log("[CarLocomotion] ClearItemBuff");
    }

}