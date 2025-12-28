using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    private CarRaceController CarRaceController;
	public TextMeshProUGUI TimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject resultObj = GameObject.Find("Car");

        Debug.Log(resultObj);
        if (resultObj != null)
        {
            CarRaceController = resultObj.GetComponent<CarRaceController>();
            TimeText.text = $"{CarRaceController.Get_Time_m():D2}:{CarRaceController.Get_Time_s():D2}:{CarRaceController.Get_Time_ms():D3}";
        }
        else
        {
            TimeText.text = $"__:__:___";
        }
	}

    // Update is called once per frame
    void Update()
    {
       
    }
}
