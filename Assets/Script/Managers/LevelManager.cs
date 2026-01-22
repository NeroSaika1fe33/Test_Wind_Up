using System;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneList : int
{
    Title,
    Car_Selection,
    Tutorial,
    In_Game,
    Result,
    Ranking,
    Track_Selection
}

public class LevelManager : MonoBehaviour
{
    //シングルトン
    public static LevelManager Instance => Singleton<LevelManager>.Instance;

    public SceneList CurrentScene;

    public SceneList InitScene = SceneList.Title;

    public GameObject Car { get; set; } = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        //シーン更新伴う処理
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneloaded;
    }

    private void OnSceneUnloaded(Scene _CurrentScene)
    {
    }

    private void OnSceneloaded(Scene _CurrentScene, LoadSceneMode _mode)
    {
        //違うシーンに違う処理をする
        switch (ConvertSceneNameToEnum(_CurrentScene.name))
        {
            case SceneList.Title:
                Debug.Log("Titleに入た");
                break;
            case SceneList.Car_Selection:
                Debug.Log("CarSelectionに入た");
                break;
            case SceneList.In_Game:
                Debug.Log("InGameに入た");
                //GameManager.Instance.SetCurrentTrack(FindFirstObjectByType<Track>());
                GameManager.Instance.InitCurrentTrack();
                GameManager.Instance.InitPlayerInGame();
                break;
            case SceneList.Result:
                Debug.Log("Resultに入た");
                break;
            case SceneList.Ranking:
                Debug.Log("Rankingに入た");
                break;
            case SceneList.Track_Selection:
                Debug.Log("TrackSelectionに入た");
                break;
        }
    }

    private void Start()
    {
        CurrentScene = InitScene;
    }
    //シーン遷移判定
    public void LoadScene(SceneList _NextSceneName)
    {
        switch (_NextSceneName)
        {
            case SceneList.Title:
                SceneManager.LoadScene("Title");
                break;
            case SceneList.Car_Selection:
                SceneManager.LoadScene("CarSelection");
                break;
            case SceneList.Tutorial: 
                SceneManager.LoadScene("Tutorial");
                break;
            case SceneList.In_Game:
                SceneManager.LoadScene("InGame");
                break;
            case SceneList.Result:
                SceneManager.LoadScene("Result");
                break;
            case SceneList.Ranking:
                SceneManager.LoadScene("Ranking");
                break;
            case SceneList.Track_Selection:
                SceneManager.LoadScene("TrackSelection");
                break;
        }
        CurrentScene = _NextSceneName;
    }

    private void Update()
    {
        
        //シーン遷移操作
        if (Input.GetKeyDown(KeyCode.Return) && CurrentScene == SceneList.Track_Selection)
        {
            LoadScene(SceneList.In_Game);
        }

        if (Input.GetKeyDown(KeyCode.Return) && CurrentScene == SceneList.Car_Selection)
        {
            LoadScene(SceneList.Track_Selection);
        }

        if (Input.anyKeyDown && CurrentScene == SceneList.Result)
        {
            LoadScene(SceneList.Title);
            PlayerDataManager.Instance.Save();
        } 
    }

    public SceneList GetCurrentScene()
    {
        return CurrentScene;
    }

    //ゲームシーンの名前転換
    private SceneList ConvertSceneNameToEnum(string sceneName)
    {
        return sceneName switch
        {
            "Title" => SceneList.Title,
            "CarSelection" => SceneList.Car_Selection,
            "InGame" => SceneList.In_Game,
            "Result" => SceneList.Result,
            "Ranking" => SceneList.Ranking,
            "Tutotial"=>SceneList.Tutorial,
            "TrackSelection"=>SceneList.Track_Selection,
            "InGame_ForDebug"=>SceneList.In_Game,   //debug用
            _ => throw new ArgumentOutOfRangeException(nameof(sceneName), $"不明なシーン名: {sceneName}")
        };
    }

    public void OnClickTutorial()
    {
        Debug.Log("Tutorial Button Clicked!");
        LoadScene(SceneList.Tutorial);
    }

    public void OnClickSelect()
    {
        LoadScene(SceneList.Car_Selection);
    }
}
