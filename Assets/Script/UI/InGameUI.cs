using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    //CarEntityインターフェース
    public interface IGameUIComponent
    {
        void Init(CarEntity entity);
    }
    public CarEntity car { get; set; }

    [Header("HPUI")]
    public Image[] hp; // 3 heart image
    public TextMeshProUGUI TimeText;

    [Header("SpeedUI")]
    public TMP_Text speedText;

    [Header("QTEUI")]
    public GameObject QTEPanel;
    public Slider progressBar;
    public TMP_Text InfoText;
    public TMP_Text timerText;

    //読みやすくするため
    private Rigidbody Rigidbody => car.Rigidbody;
    private PlayerStats PlayerStats => car.PlayerStats;
    private CarSituation Situation => car.Situation;
    private CarRaceController RaceController => car.RaceController;

    //todo:make fuctions
    void Update()
    {
        if (!car)return;

        UpdateHP(PlayerStats.currentHP);
        UpadateTimerText();
        UpdateSpeedText();
    }

    private void UpdateSpeedText()
    {
        //m/s単位からkm/s単位転換
        float currentSpeed = Rigidbody.linearVelocity.magnitude * 3.6f;

        speedText.text = "Speed: " + Mathf.Round(currentSpeed).ToString() + " km/h";
    }

    private void UpadateTimerText()
    {
        if (Situation.Get_Steat() != CarSituation.Steat.Goal)
            TimeText.text = $"{RaceController.Get_Time_m():D2}:{RaceController.Get_Time_s():D2}:{RaceController.Get_Time_ms():D3}";
    }

    public void UpdateHP(int currentHP)
    {
        for (int i = 0; i < hp.Length; i++)
        {
            hp[i].enabled = (i < currentHP);
        }
    }

    //UI全部を初期化
    public void Init(CarEntity _car)
    {
        car = _car;

        var uis = GetComponentsInChildren<IGameUIComponent>(true);
        foreach (var ui in uis) ui.Init(car);
    }

}
