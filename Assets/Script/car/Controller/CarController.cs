using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : CarComponent
{
    public bool canControl = true;



    private CarGroundChecker Ground => GetComponent<CarGroundChecker>();
    private CarLocomotion Locomotion => GetComponent<CarLocomotion>();
    private CarCrashHandler Crash => GetComponent<CarCrashHandler>();
    private QTEController QTE => car.QTEController;
    private InGameUI Hud => car.Hud;
    private PlayerStats Stats =>GetComponent<PlayerStats>();
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

 
    }