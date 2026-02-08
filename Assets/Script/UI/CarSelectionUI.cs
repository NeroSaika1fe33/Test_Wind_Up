using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class CarSelectionUI : MonoBehaviour
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

    [Header("Gamepad/Stick")]
    [SerializeField] private float stickThreshold = 0.5f;
    [SerializeField] private float navCooldown = 0.15f;
    private float nextNavTime = 0f;

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
    private PlayerPartsContainer PartsContainer => car.PlayerPartsContainer;

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
            partTexts[i].color = (i == currentRow) ? Color.yellow : Color.white;
        }
    }
    void Update()
    {
        UpdateStats();
        bool selectionChanged = false;

        //メニュー制御
        Vector2Int dir = ReadNavigateOnce();

        // 上下 row
        if (dir.y > 0)
        {
            currentRow = (currentRow - 1 + 3) % 3;
            updateSignal = true;
            selectionChanged = true;
        }
        else if (dir.y < 0)
        {
            currentRow = (currentRow + 1) % 3;
            updateSignal = true;
            selectionChanged = true;
        }

        // 左右 item
        if (dir.x < 0)
        {
            selected[currentRow]--;
            if (selected[currentRow] < 0)
                selected[currentRow] = items[currentRow].Count - 1;
            updateSignal = true;
            selectionChanged = true;
        }
        else if (dir.x > 0)
        {
            selected[currentRow]++;
            if (selected[currentRow] >= items[currentRow].Count)
                selected[currentRow] = 0;
            updateSignal = true;
            selectionChanged = true;
        }

        //変更あったら UI + 車件更新
        if (selectionChanged)
        {
            UpdatePartsUI();
            UpdateCarInCurrentRow();
        }

        // 確認（Enter / パット A）
        if (SubmitPressed())
        {
            if (PlayerDataManager.Instance.CustomizeList != null)
                PlayerDataManager.Instance.DataStorage(
                    items[0][selected[0]],
                    items[1][selected[1]],
                    items[2][selected[2]]
                );
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
        updateSignal = false;
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

    //車のデータ入力ため、未使用
    public void InitCar(CarEntity _car)
    {
        car = _car;
    }

    // ===== Input Helpers (Keyboard + Gamepad) =====

    private Vector2Int ReadNavigateOnce()
    {
        float now = Time.unscaledTime;
        if (now < nextNavTime) return Vector2Int.zero;

        int x = 0, y = 0;

        // Keyboard
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) y = +1;
            else if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame) y = -1;

            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) x = -1;
            else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) x = +1;
        }

        // Gamepad（Dpad 優先的，次 Stick）
        var g = Gamepad.current;
        if (g != null && x == 0 && y == 0)
        {
            if (g.dpad.up.wasPressedThisFrame) y = +1;
            else if (g.dpad.down.wasPressedThisFrame) y = -1;
            else if (g.dpad.left.wasPressedThisFrame) x = -1;
            else if (g.dpad.right.wasPressedThisFrame) x = +1;
            else
            {
                Vector2 stick = g.leftStick.ReadValue();
                if (stick.y >= stickThreshold) y = +1;
                else if (stick.y <= -stickThreshold) y = -1;
                else if (stick.x <= -stickThreshold) x = -1;
                else if (stick.x >= stickThreshold) x = +1;
            }
        }

        if (x != 0 || y != 0) nextNavTime = now + navCooldown;
        return new Vector2Int(x, y);
    }

    private bool SubmitPressed()
    {
        bool kb = Keyboard.current != null &&
                  (Keyboard.current.enterKey.wasPressedThisFrame ||
                   Keyboard.current.numpadEnterKey.wasPressedThisFrame);

        bool pad = false;
        var g = Gamepad.current;
        if (g != null)
            pad = g.buttonSouth.wasPressedThisFrame || g.buttonEast.wasPressedThisFrame || g.startButton.wasPressedThisFrame;

        return kb || pad;
    }
}
