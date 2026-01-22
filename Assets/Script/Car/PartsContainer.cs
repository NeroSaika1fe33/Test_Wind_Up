using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

//プレイヤーのパーツ管理
public class PartsContainer : CarComponent
{
    public string BodyPrefabName = "Car_body1";
    public string MainspringPrefabName = "Car_Zenmai1";
    public string FrontWheelPrefabName = "Car_Tire1";
    public string BackWheelPrefabName = "Car_Tire1";

    public Transform Installation_Location_Body;
    [SerializeField] Transform Installation_Location_MainSpring;
    public Transform Installation_Location_Wheel_FrontLeft;
    public Transform Installation_Location_Wheel_BackLeft;
    public Transform Installation_Location_Wheel_FrontRight;
    public Transform Installation_Location_Wheel_BackRight;

    private void Awake()
    {
        //Playerがセットされたら、プレイデータからパーツ更新
        if (PlayerDataManager.Instance.CustomizeList != null)
        {
            BodyPrefabName = PartsDataManager.Instance.Get_PartsID(PlayerDataManager.Instance.CustomizeList[0]);
            MainspringPrefabName = PartsDataManager.Instance.Get_PartsID(PlayerDataManager.Instance.CustomizeList[1]);
            FrontWheelPrefabName = PartsDataManager.Instance.Get_PartsID(PlayerDataManager.Instance.CustomizeList[2]);
            BackWheelPrefabName = PartsDataManager.Instance.Get_PartsID(PlayerDataManager.Instance.CustomizeList[2]);
        }
        PartsArrangement(BodyPrefabName, Installation_Location_Body);
        PartsArrangement(MainspringPrefabName, Installation_Location_MainSpring);
        PartsArrangement(FrontWheelPrefabName, Installation_Location_Wheel_FrontLeft);
        PartsArrangement(FrontWheelPrefabName, Installation_Location_Wheel_FrontRight);
        PartsArrangement(BackWheelPrefabName, Installation_Location_Wheel_BackLeft);
        PartsArrangement(BackWheelPrefabName, Installation_Location_Wheel_BackRight);
    }

    void Start()
    {
    }

    public void PartsArrangement(string PartsName, Transform Installation_Location)
    {
        //パーツタイプ判別(未使用)
        string PartsType = PartsDataManager.Instance.Get_PartsType(PartsName);
        // コード上では拡張子を付けない
        GameObject prefab = Resources.Load<GameObject>(PartsName);

        if (prefab == null)
        {
            Debug.LogError("Prefabが見つかりません: " + PartsName);
        }
        GameObject childObject = Instantiate(prefab, Installation_Location);
        childObject.transform.localPosition = new Vector3(0, 0, 0);
        //childObject.transform.localRotation = Quaternion.identity;
        childObject.transform.localScale = new Vector3(1, 1, 1);

    }

    //パーツ名更新とプリハブ更新
    public void UpdateBodyParts(string PartsName)
    {
        BodyPrefabName = PartsName;
        Destroy(Installation_Location_Body.GetChild(0).gameObject);
        PartsArrangement(BodyPrefabName, Installation_Location_Body);
    }
    public void UpdateTireParts(string PartsName)
    {
        FrontWheelPrefabName = PartsName;
        BackWheelPrefabName = PartsName;
        if (Installation_Location_Wheel_FrontLeft.childCount != 0)
        {
            Destroy(Installation_Location_Wheel_FrontLeft.GetChild(0).gameObject);
            Destroy(Installation_Location_Wheel_FrontRight.GetChild(0).gameObject);
            Destroy(Installation_Location_Wheel_BackLeft.GetChild(0).gameObject);
            Destroy(Installation_Location_Wheel_BackRight.GetChild(0).gameObject);
        }

        PartsArrangement(FrontWheelPrefabName, Installation_Location_Wheel_FrontLeft);
        PartsArrangement(FrontWheelPrefabName, Installation_Location_Wheel_FrontRight);
        PartsArrangement(BackWheelPrefabName, Installation_Location_Wheel_BackLeft);
        PartsArrangement(BackWheelPrefabName, Installation_Location_Wheel_BackRight);
    }
    public void UpdateMainSpringParts(string PartsName)
    {
        Debug.Log(Installation_Location_MainSpring);
        MainspringPrefabName = PartsName;
        if (Installation_Location_MainSpring.childCount != 0)
        {
            Destroy(Installation_Location_MainSpring.GetChild(0).gameObject);
        }

        PartsArrangement(MainspringPrefabName, Installation_Location_MainSpring);
    }
    //初期化パーツ
    public void InitialSettingsParts(string Body, string Wheel, string Mainspring)
    {
        BodyPrefabName = Body;
        FrontWheelPrefabName = Wheel;
        BackWheelPrefabName = Wheel;
        MainspringPrefabName = Mainspring;
    }
}
