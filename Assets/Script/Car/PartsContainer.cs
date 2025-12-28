using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PartsContainer : CarComponent
{
    public string BodyPrefabName = "car_1";
    public string MainspringPrefabName = "Mainspring1";
    public string TirePrefabName = "Normal_Tire";

    public List<Transform> Installation_Location_Body = new List<Transform>();
    public List<Transform> Installation_Location_Wing = new List<Transform>();
    public List<Transform> Installation_Location_Tire = new List<Transform>();

    void Start()
    {
        //Aをi番目の場所に配置(パーツタイプは自動判別)
        PartsArrangement(BodyPrefabName, Installation_Location_Body[0]);
        PartsArrangement(MainspringPrefabName, Installation_Location_Wing[0]);
        PartsArrangement(TirePrefabName, Installation_Location_Tire[0]);
        PartsArrangement(TirePrefabName, Installation_Location_Tire[1]);
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
        childObject.transform.localRotation = Quaternion.identity;
        childObject.transform.localScale = new Vector3(1, 1, 1);

    }

    //パーツ名更新とプリハブ更新
    public void UpdateBodyParts(string PartsName)
    {
        BodyPrefabName = PartsName;
        Destroy(Installation_Location_Body[0].GetChild(0).gameObject);
        PartsArrangement(BodyPrefabName, Installation_Location_Body[0]);
    }
    public void UpdateTireParts(string PartsName)
    {
        TirePrefabName = PartsName;
        Destroy(Installation_Location_Tire[0].GetChild(0).gameObject);
        Destroy(Installation_Location_Tire[1].GetChild(0).gameObject);
        PartsArrangement(TirePrefabName, Installation_Location_Tire[0]);
        PartsArrangement(TirePrefabName, Installation_Location_Tire[1]);
    }
    public void UpdateWingParts(string PartsName)
    {
        MainspringPrefabName = PartsName;
        Destroy(Installation_Location_Wing[0].GetChild(0).gameObject);
        PartsArrangement(MainspringPrefabName, Installation_Location_Wing[0]);
    }
    public void InitialSettingsParts(string Body,string Wheel,string Mainspring)
    {
        BodyPrefabName = Body;
        TirePrefabName = Wheel;
        MainspringPrefabName = Mainspring;
    }
}
