using UnityEngine;

public class EnemyStats : EnemyComponent
{
    public EnemyPartsContainer PartsContainer;

    public float maxSpeed { get; set; }
    public float acceleration { get; set; }
    public float weight { get; set; }

    public Part[] parts { get; private set; } = new Part[3];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //配列初期化
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = new Part();
            //parts[i].abilityName = "None";
        }

        InitParts();
        UpdatePartsStats();
        Debug.Log(maxSpeed + "  " + acceleration + "  " + weight);
        Debug.Log(PartsContainer.BodyPrefabName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void InitParts()
    {
        if (parts != null)
        {
            parts[0].partsName = PartsContainer.BodyPrefabName;
            parts[1].partsName = PartsContainer.MainspringPrefabName;
            parts[2].partsName = PartsContainer.FrontWheelPrefabName;
            for (int i = 0; i < parts.Length; i++)
            {
                string Name = PartsDataManager.Instance.Get_PartsName(parts[i].partsName);
                parts[i].maxSpeed = PartsDataManager.Instance.Get_PartsData_int(Name, "最高速度");
                parts[i].acceleration = PartsDataManager.Instance.Get_PartsData_int(Name, "加速度");
                parts[i].weight = PartsDataManager.Instance.Get_PartsData_int(Name, "重量");
                //parts[i].abilityName = PartsDataManager.Instance.Get_PartsData_string(parts[i].partsName, "アビリティ");

            }
        }
        else
        {
            Debug.LogError("CarPartsがアサインせれていない！！");
        }
    }
    //パーツステータス更新
    protected void UpdatePartsStats()
    {
        maxSpeed = parts[0].maxSpeed + parts[1].maxSpeed + parts[2].maxSpeed;
        acceleration = parts[0].acceleration + parts[1].acceleration + parts[2].acceleration;
        weight = parts[0].weight + parts[1].weight + parts[2].weight;
    }
}
