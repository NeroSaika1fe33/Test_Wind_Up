using System.Resources;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public CinemachineCamera camera;

    public Track CurrentTrack;
    public static GameManager Instance => Singleton<GameManager>.Instance;

    //ˆê–¼ƒvƒŒƒC‰¼’è
    public int playerNum = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < playerNum; i++) {
            if (LevelManager.Instance.GetCurrentScene() == SceneList.In_Game)
            {
                GameObject Car = Instantiate(ResourceManager.Instance.carPrefab[i], new Vector3(60,55,-150), Quaternion.identity);
                var tempComponent = Car.GetComponent<CarEntity>();
                tempComponent.InitInGameUI();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
