using UnityEngine;
using UnityEngine.UI;
public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button GameStartButton;
    [SerializeField] private Button tutorialButton;

    void Start()
    {
        GameStartButton.onClick.RemoveAllListeners();
        GameStartButton.onClick.AddListener(() => LevelManager.Instance.OnClickSelect());

        tutorialButton.onClick.RemoveAllListeners();
        tutorialButton.onClick.AddListener(() => LevelManager.Instance.OnClickTutorial());
    }
}