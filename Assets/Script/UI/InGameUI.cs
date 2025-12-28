using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public interface IGameUIComponent
    {
        void Init(CarEntity entity);
    }
    public Image[] hp; // 3 heart image
    public TextMeshProUGUI TimeText;
    public CarEntity car { get; set; }

    void Update()
    {
        UpdateHP(car.PlayerStats.currentHP);
        Debug.Log("Hit wall! HP = " + car.PlayerStats.currentHP);

        if (car.Situation.Get_Steat() != CarSituation.Steat.Goal)
            TimeText.text = $"{car.RaceController.Get_Time_m():D2}:{car.RaceController.Get_Time_s():D2}:{car.RaceController.Get_Time_ms():D3}";
    }

    public void UpdateHP(int currentHP)
    {
        for (int i = 0; i < hp.Length; i++)
        {

            hp[i].enabled = (i < currentHP);
        }

    }

    public CarController carController => car.Controller;

    //UIëSïîÇèâä˙âª
    public void Init(CarEntity _car)
    {
        car = _car;

        var uis = GetComponentsInChildren<IGameUIComponent>(true);
        foreach (var ui in uis) ui.Init(car);
    }

}
