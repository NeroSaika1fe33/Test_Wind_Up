using System.IO;
using System.Resources;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    //シングルトン
    public static PlayerDataManager Instance => Singleton<PlayerDataManager>.Instance;

    //一人プレイに仮定する
    private string playerName = "Player1";
    private int playerID = 1;
    private string[] PartsName = new string[3];
    private string abilityName = "None";
    public string result { get; private set; }

    //シーンCarSelectionUIのカスタマイズ情報をここに保存
    public string[] CustomizeList { get; private set; } = { "車1", "ゼンマイ1", "タイヤ1" };//デフォルト設定

    private PlayerStats playerStats { get; set; }

    public GoalTime GoalTime { get; private set; }
    private string savePath => Path.Combine(Application.dataPath, "SaveData/playData.json");

    void Awake()
    {

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void SetPlayer(PlayerStats _playerStats)
    {
        playerStats = _playerStats;
        for (int i = 0; i < PartsName.Length; i++)
        {
            PartsName[i] = _playerStats.parts[i].partsName;
        }
    }

    public void SetMatchResult(string _result)
    {
        result = _result;
    }

    //データ保存用
    public void Save()
    {
        Debug.Log("最高速度"+playerStats.maxSpeed);
        PlayerSaveData saveData = new PlayerSaveData(
            playerName,
            playerID,
            CustomizeList,
            playerStats.maxSpeed,
            playerStats.acceleration,
            playerStats.weight,
            abilityName,
            result
            );

        //jsonに入力
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("保存されたパース：" + savePath);
    }

    //データ読み込む用
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            Debug.Log("プレーデータを読み込む");
        }
        else
        {
            Debug.Log("ファイルを見つからない");
        }
    }

    public void Register(PlayerStats _playerStats)
    {
        this.playerStats = _playerStats;
    }

    public void DataStorage(string Body, string Mainspring, string Wheel)
    {
        CustomizeList[0] = Body;
        CustomizeList[1] = Mainspring;
        CustomizeList[2] = Wheel;
    }

    public void SetResult(GoalTime _goalTime)
    {
        GoalTime = _goalTime;
        result = GoalTime.m + ":" + GoalTime.s + ":" + GoalTime.ms;
    }
}