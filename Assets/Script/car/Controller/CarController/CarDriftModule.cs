using UnityEngine;
using System;
using UnityEngine.InputSystem;

// Drift：drift 状態、倍率/boost 発動
public class CarDriftModule : CarComponent
{
    public event Action DriftChargeStarted; //  Preparing（charge）
    public event Action DriftBoostStarted;  //  Drifting（boost）
    public float PrepareTime => prepareTime;

    private enum DriftState { None, Preparing, Drifting, Cooldown }

    [Header("Drift")]
    [SerializeField] private KeyCode driftKey = KeyCode.Space;

    [SerializeField] private bool enableGamepadDrift = true; //パット


    [SerializeField] private float driftMinSpeed = 30f;

    [SerializeField] private float prepareTime = 0.5f;
    [SerializeField] private float driftTime = 1.2f;
    [SerializeField] private float cooldownTime = 0.6f;

    [SerializeField] private float prepareSpeed = 0.85f;
    [SerializeField] private float driftSpeed = 1.5f;
    [SerializeField] private float cooldownSpeed = 0.9f;

    [SerializeField] private float driftTurnMultiplier = 1.2f;

    private DriftState state = DriftState.None;
    private float timer;

    private Rigidbody Rigidbody => car.Rigidbody;
    private CarAudio CarAudio => car.CarAudio;

    public float SpeedMultiplier =>
        state switch
        {
            DriftState.Preparing => prepareSpeed,
            DriftState.Drifting => driftSpeed,
            DriftState.Cooldown => cooldownSpeed,
            _ => 1f
        };

    public float TurnMultiplier => (state == DriftState.Drifting) ? driftTurnMultiplier : 1f;

    public void TickStartCondition(float inputV, float inputH)
    {
        if (state != DriftState.None) return;

        float speed = Rigidbody.linearVelocity.magnitude;
        bool isAccelerating = inputV > 0.1f;
        bool isTurning = Mathf.Abs(inputH) > 0.1f;
        bool pressed = Input.GetKeyDown(driftKey) || GamepadDriftPressed(); ;

        if (speed >= driftMinSpeed && isAccelerating  && pressed)
        {
            state = DriftState.Preparing;
            timer = 0f;
            DriftChargeStarted?.Invoke();//chargeからsparke start
            CarAudio?.StartDriftCharge();
        }
    }

    private bool GamepadDriftPressed()
    {
        if (!enableGamepadDrift) return false;
        var g = Gamepad.current;
        if (g == null) return false;

        //Y =ドリフト
        return g.buttonNorth.wasPressedThisFrame; 
    }


    public void TickStateMachine()//drift状態機
    {
        if (state == DriftState.None) return;

        timer += Time.deltaTime;

        switch (state)
        {
            case DriftState.Preparing:
                if (timer >= prepareTime)
                {
                    state = DriftState.Drifting;
                    timer = 0f;

                    // Drifting入る： boost
                    var vel = Rigidbody.linearVelocity;
                    if (vel.sqrMagnitude > 0.001f)
                    {
                        Rigidbody.linearVelocity = vel.normalized * vel.magnitude * driftSpeed;
                        CarAudio?.StopDriftCharge();
                        CarAudio?.PlayBoost();
                        DriftBoostStarted?.Invoke();

                    }
                }
                break;

            case DriftState.Drifting:
                if (timer >= driftTime)
                {
                    state = DriftState.Cooldown;
                    timer = 0f;
                }
                break;

            case DriftState.Cooldown:
                if (timer >= cooldownTime)
                {
                    state = DriftState.None;
                    timer = 0f;
                }
                break;
        }
    }
}