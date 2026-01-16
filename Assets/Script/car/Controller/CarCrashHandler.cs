using UnityEngine;

//HP0 => lock + QTE
public class CarCrashHandler : CarComponent
{
    private PlayerStats PlayerStats => car.PlayerStats;
    private QTEController QTE => car.QTEController;

    public void TickCrashCheck()
    {
        if (PlayerStats == null) return;
        if (PlayerStats.currentHP > 0) return;

        TriggerCrash();
    }

    public void TriggerCrash()
    {
        if (car.Controller != null) car.Controller.SetCanControl(false);
        QTE?.Minigame(); 
        Debug.Log("Car is crash");
    }
}