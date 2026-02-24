/*using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;*/
using UnityEngine;
using UnityEngine.InputSystem;
/*using UnityEngine.UIElements.Experimental;*/

[System.Serializable]
public class PlayerStats : MonoBehaviour, IStats
{
    [Header("Stick")]
    private bool _stickUsed;
    bool stickRight = false;
    bool stickLeft = false;

    [Header("PlayData")]
    public float maxSpeed { get; set; }
    public float acceleration { get; set; }
    public float weight { get; set; }
    public string abilityName { get; set; } = "None";
    public string result {  get; set; }

    private PlayerPartsContainer Car;
    private CarSelectionUI SelectionUI;
    public Part[] parts { get; private set; } = new Part[3];

    [Header("InGame")]
    public int maxHP = 3;                // max HP = 3
    public int currentHP;                //現在のHP（計算用）

    //ダメージ受け
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0; //HP>0確保
    }

    public void ResetHp()
    {
        currentHP = 3;
    }

    private void Awake()
    {
        Car = GetComponentInParent<PlayerPartsContainer>();
        //配列初期化
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = new Part();
            //parts[i].abilityName = "None";
        }

        InitParts();
        UpdatePartsStats();

        currentHP = maxHP;
    }

    protected void InitParts()
    {
        if (parts != null)
        {
            parts[0].partsName = Car.BodyPrefabName;
            parts[1].partsName = Car.MainspringPrefabName;
            parts[2].partsName = Car.FrontWheelPrefabName;
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
    //パーツ内容の更新
    protected void UpdateParts()
    {
        parts[0].partsName = Car.BodyPrefabName;
        parts[1].partsName = Car.MainspringPrefabName;
        parts[2].partsName = Car.FrontWheelPrefabName;
        for (int i = 0; i < parts.Length; i++)
        {
            string Name = PartsDataManager.Instance.Get_PartsName(parts[i].partsName);
            parts[i].maxSpeed = PartsDataManager.Instance.Get_PartsData_int(Name, "最高速度");
            parts[i].acceleration = PartsDataManager.Instance.Get_PartsData_int(Name, "加速度");
            parts[i].weight = PartsDataManager.Instance.Get_PartsData_int(Name, "重量");
            //parts[i].abilityName = PartsDataManager.Instance.Get_PartsData_string(parts[i].partsName, "アビリティ");
        }
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            float x = Gamepad.current.leftStick.x.ReadValue();

            if (!_stickUsed && x > 0.6f) { stickRight = true; _stickUsed = true; }
            if (!_stickUsed && x < -0.6f) { stickLeft = true; _stickUsed = true; }
            if (Mathf.Abs(x) < 0.2f) _stickUsed = false; // 戻ったら解除
        }

        if (LevelManager.Instance.GetCurrentScene() == SceneList.Car_Selection)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
               UpdateParts();
               UpdatePartsStats();
            }
            if (stickLeft || stickRight)
            {
                UpdateParts();
                UpdatePartsStats();
             }
            }

        if (LevelManager.Instance.GetCurrentScene() == SceneList.Result)
        {
            PlayerDataManager.Instance.SetPlayer(this);
        }
    }
}
