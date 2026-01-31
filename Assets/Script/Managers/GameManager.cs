using System.Resources;
using Unity.Cinemachine;
using UnityEngine;

//TODO:ゲーム再開の開発、関数のまとめ  12.29
public class GameManager : MonoBehaviour
{
    //private GameObject Car;

    private CinemachineCamera cam;       //カメラ制御のため

    public Track CurrentTrack;

    public int TrackID;          //トラックの設定

    public GameObject car;

    public GameObject MyTrack;

    //一名プレイ仮定
    private int playerNum = 1;

    //シングルトン
    public static GameManager Instance => Singleton<GameManager>.Instance;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void InitSelectCarParts()
    {
    }

    public void InitPlayerInGame()
    {
        //プリハブ初期化
        car = Instantiate(ResourceManager.Instance.carPrefab[0], CurrentTrack.startLinePos[0].position, CurrentTrack.startLinePos[0].transform.rotation);

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

    public void SetCurrentTrackByID(int _Trackid)
    {
        TrackID = _Trackid;
    }

    public void InitCurrentTrack()
    {
        //var Track = Instantiate(ResourceManager.Instance.TrackPrefab[TrackID], new Vector3(0f, 0f, 0f), Quaternion.identity);
        MyTrack = Instantiate(ResourceManager.Instance.TrackPrefab[TrackID], new Vector3(0f, 0f, 0f), Quaternion.identity);
    }

    public void InitResult()
    {
    }

    public void InitRanking()
    {
    }

    public void OnDestroyMyTrack()
    {
        if (MyTrack != null)
            Destroy(MyTrack);

    }
    public void OnDestroyCar()
    {
        if (car != null)
            Destroy(car);
    }
}
