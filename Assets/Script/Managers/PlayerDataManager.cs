using System.IO;
using System.Resources;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    //シングルトン
    public static PlayerDataManager Instance => Singleton<PlayerDataManager>.Instance;

    //一人プレイに仮定する
    public string playerName = "Player1";
    public int playerID = 1;
    public string[] PartsName = new string[3];
    public string abilityName = "None";

    private PlayerStats playerStats { get; set; }   
    //Player player { get; private set; }
    public string result { get; private set; }

    private string savePath => Path.Combine(Application.dataPath, "SaveData/playData.json");

    void Awake()
    {
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
        PlayerSaveData saveData = new PlayerSaveData(
            playerName,
            playerID,
            PartsName,
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

    private void Update()
    {
    }

}