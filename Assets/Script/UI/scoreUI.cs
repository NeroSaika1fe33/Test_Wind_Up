using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public CarSituation carSituation;
    public TextMeshProUGUI TimeText;
    public Goal Goal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carSituation.Get_Steat() != CarSituation.Steat.Goal)
        TimeText.text = $"{Goal.Get_Time_m():D2}:{Goal.Get_Time_s():D2}:{Goal.Get_Time_ms():D3}";
    }
}
