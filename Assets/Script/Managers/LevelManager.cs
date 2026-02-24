using System;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

    private float inputLockUntil = 0f;
    private bool InputLocked => Time.unscaledTime < inputLockUntil;

    private void LockInput(float seconds = 0.35f)
    {
        inputLockUntil = Time.unscaledTime + seconds;
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        //シーン更新伴う処理
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneloaded;
    }

    private void OnSceneUnloaded(Scene _CurrentScene)
    {
        switch (ConvertSceneNameToEnum(_CurrentScene.name))
        {
            case SceneList.Title:
                Debug.Log("Title から離脱");
                break;
            case SceneList.Car_Selection:
                Debug.Log("CarSelection から離脱");
                break;
            case SceneList.In_Game:
                Debug.Log("InGame から離脱");
                GameManager.Instance.OnDestroyMyTrack();
                GameManager.Instance.OnDestroyCar();
                break;
            case SceneList.Result:
                Debug.Log("Result から離脱");
                break;
            case SceneList.Ranking:
                Debug.Log("Ranking から離脱");
                break;
            case SceneList.Track_Selection:
                Debug.Log("TrackSelection から離脱");
                break;
            case SceneList.Tutorial:
                Debug.Log("Tutorial から離脱");
                break;
        }
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
            case SceneList.Tutorial:
                Debug.Log("Tutorialに入た");
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
        LockInput(0.35f);

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
        if (InputLocked) return;

        //  F1：タイトルに戻る
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            if (CurrentScene != SceneList.Title)
            {
                LoadScene(SceneList.Title);
            }
            return; 
        }
        //シーン遷移操作
        // Confirm（Keyboard Enter / Gamepad buttons）
        if (SubmitPressedThisFrame() && CurrentScene == SceneList.Car_Selection)
        {
            LoadScene(SceneList.In_Game);
        }

       /* if (SubmitPressedThisFrame() && CurrentScene == SceneList.Car_Selection)
        {
            LoadScene(SceneList.Track_Selection);
        }
       */

        
        if (AnyKeyboardOrGamepadPressedThisFrame() && CurrentScene == SceneList.Result)
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
            "Tutorial" => SceneList.Tutorial,
            "TrackSelection" => SceneList.Track_Selection,
            "InGame_ForDebug" => SceneList.In_Game,   //debug用
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

    private bool SubmitPressedThisFrame()
    {
        bool kb = Keyboard.current != null &&
                  (Keyboard.current.enterKey.wasPressedThisFrame ||
                   Keyboard.current.numpadEnterKey.wasPressedThisFrame);

        var g = Gamepad.current;
        bool pad = false;
        if (g != null)
        {
            
            pad = g.buttonSouth.wasPressedThisFrame   // Xbox A / PS × / Switch B
               || g.buttonEast.wasPressedThisFrame    // Xbox B / PS ○ / Switch A
               || g.startButton.wasPressedThisFrame;
        }

        return kb || pad;
    }

    private bool AnyKeyboardOrGamepadPressedThisFrame()
    {
        bool kbAny = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

        bool padAny = false;
        var g = Gamepad.current;
        if (g != null)
        {
            foreach (var c in g.allControls)
            {
                if (c is ButtonControl b && b.wasPressedThisFrame)
                {
                    padAny = true;
                    break;
                }
            }
        }

        return kbAny || padAny;
    }
}
