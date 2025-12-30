using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    public CarEntity car;//車の入力ため

    [Header("PartsText")]
    public TMP_Text Topic;
    public TextMeshProUGUI[] partTexts;


    [Header("DataText")]
    public TextMeshProUGUI[] dataTexts;

    [Header("StatsData")]

    private string[] statsName = { "MaxSpeed", "Accerleration", "Weight" };

    [Header("Input")]
    private bool updateSignal = false;  //true:入力がある　false:入力がない

    [Header("Item Selection")]
    private string[] categories = { "Body", "Mainspring", "Tire" };
    private List<string>[] items = {
        new List<string>(), //Body
        new List<string>(), //MainSpring
        new List<string>()  //Tire
    };
    private int[] selected = { 0, 0, 0 };
    private int currentRow = 0;

    //読みやすくするため
    private PlayerStats PlayerStats => car.PlayerStats;
    private PartsContainer PartsContainer => car.PartsContainer;

    void Start()
    {
        PartsDataSet();
        InitPartsUI();
        InitDataUI();
    }

    private void InitDataUI()
    {
        dataTexts[0].text = $"{statsName[0]}:   {PlayerStats.maxSpeed}";
        dataTexts[1].text = $"{statsName[1]}:   {PlayerStats.acceleration}";
        dataTexts[2].text = $"{statsName[2]}:   {PlayerStats.weight}";
    }

    private void InitPartsUI()
    {
        Topic.text = $"Parts Selection";
        for (int i = 0; i < 3; i++)
        {
            string prefix = (i == currentRow) ? "> " : "  ";
            string partType = categories[i];
            string itemName = items[i][selected[i]];

            partTexts[i].text = $"{prefix}{categories[i]}:   \n{itemName}";

            if (i == currentRow) partTexts[i].color = Color.yellow;
            else partTexts[i].color = Color.white;
        }
    }
    void Update()
    {
        UpdateStats();
        bool selectionChanged = false;

        //メニュー制御
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentRow = (currentRow - 1 + 3) % 3;
            updateSignal = true;
            selectionChanged = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentRow = (currentRow + 1) % 3;
            updateSignal = true;
            selectionChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selected[currentRow]--;
            if (selected[currentRow] < 0)
                selected[currentRow] = items[currentRow].Count - 1;
            updateSignal = true;
            selectionChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            selected[currentRow]++;
            if (selected[currentRow] >= items[currentRow].Count)
                selected[currentRow] = 0;
            updateSignal = true;
            selectionChanged = true;
        }

        //入力したら更新処理
        if (selectionChanged)
        {
            UpdatePartsUI();
            UpdateCarInCurrentRow();
        }

        //選択確認し、カスタマイズ情報保存
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //シングルトンのプレイデータと共に保存
            if (PlayerDataManager.Instance.CustomizeList != null)
                PlayerDataManager.Instance.DataStorage(items[0][selected[0]], items[1][selected[1]], items[2][selected[2]]);
        }
    }
    //ステータス部分の更新
    private void UpdateStats()
    {
        dataTexts[0].text = $"{statsName[0]}:   {PlayerStats.maxSpeed}";
        dataTexts[1].text = $"{statsName[1]}:   {PlayerStats.acceleration}";
        dataTexts[2].text = $"{statsName[2]}:   {PlayerStats.weight}";
    }
    //パーツ部分の更新
    void UpdatePartsUI()
    {

        for (int i = 0; i < 3; i++)
        {
            string prefix = (i == currentRow) ? "> " : "  ";
            string partType = categories[i];
            string itemName = items[i][selected[i]];

            partTexts[i].text = $"{prefix}{partType}:   \n{itemName}";


            if (i == currentRow) partTexts[i].color = Color.yellow;
            else partTexts[i].color = Color.white;
        }
    }
    //パーツタイプを選択たらCarを更新処理
    void UpdateCarInCurrentRow()
    {
        if (!updateSignal || PartsContainer == null) return;

        string partName = items[currentRow][selected[currentRow]];
        string partID = PartsDataManager.Instance.Get_PartsID(partName);

        switch (currentRow)
        {
            case 0:
                PartsContainer.UpdateBodyParts(partID);
                break;
            case 1:
                PartsContainer.UpdateMainSpringParts(partID);
                break;
            case 2:
                PartsContainer.UpdateTireParts(partID);
                break;
        }
    }

    //パーツデータをPartsDataManagerマネジャーから取得
    void PartsDataSet()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            for (int j = 0; j < PartsDataManager.Instance.Number_of_Parts; j++)
            {
                if (PartsDataManager.Instance.Get_PartsType(PartsDataManager.Instance.Get_PartsName()[j]) == categories[i])
                {
                    items[i].Add(PartsDataManager.Instance.Get_PartsName()[j]);
                }
            }
        }
    }
    //データを次のシーンに持っていくためにCussomList(DDOL)に保存
    private void OnDestroy()
    {

    }

    //車のデータ入力ため、未使用
    public void InitCar(CarEntity _car)
    {
        car = _car;
    }
}
