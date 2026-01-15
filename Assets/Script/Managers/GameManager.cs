using System.Resources;
using Unity.Cinemachine;
using UnityEngine;

//TODO:ゲーム再開の開発、関数のまとめ  12.29
public class GameManager : MonoBehaviour
{
    //private GameObject Car;

    private CinemachineCamera cam;       //カメラ制御のため

    public Track CurrentTrack;          //トラックの設定

    //一名プレイ仮定
    private int playerNum = 1;

    //シングルトン
    public static GameManager Instance => Singleton<GameManager>.Instance;

    public void InitSelectCarParts()
    {
    }

    public void InitPlayerInGame()
    {
        //プリハブ初期化
        var car = Instantiate(ResourceManager.Instance.carPrefab[0], new Vector3(60f, 55f, -150f), Quaternion.identity);

        //Hudの初期化
        var tempEntity = car.GetComponent<CarEntity>();
        if (tempEntity != null)
            tempEntity.InitInGameUI();
        //カメラの追跡
        var tempCarCamera = car.GetComponent<CarCamera>();
        GameObject CustomizeCamera = Instantiate(ResourceManager.Instance.CameraPrefab, tempCarCamera.drivingVP.position, Quaternion.identity);
        cam = CustomizeCamera.GetComponentInChildren<CinemachineCamera>();
        cam.Follow = car.transform;
        cam.LookAt = car.transform;
    }

    public void SetCurrentTrack(Track _track)
    {
        CurrentTrack = _track;
    }

    public void InitResult()
    {
    }

    public void InitRanking()
    {
    }

}
