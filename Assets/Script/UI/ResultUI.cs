using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI TimeText;
    void Start()
    {
        if (PlayerDataManager.Instance.GoalTime != null)
        {
            TimeText.text = $"{PlayerDataManager.Instance.GoalTime.m:D2}:{PlayerDataManager.Instance.GoalTime.s:D2}:{PlayerDataManager.Instance.GoalTime.ms:D3}";
        }
        else
        {
            TimeText.text = $"__:__:___";
        }
    }
}
