using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Carを再起動するためのA/D連打
public class QTEController : CarComponent
{
    int currentCount = 0;// 現在の連打回数
    int targetCount = 20;// 目標連打回数

    bool isRunning = false;// QTE実行中flag

    [Header("Start")]
    bool isStartGameQTE = false; //ゲーム開始時のQTEかどうか
    public float startQTETime = 3f;/// 開始QTEの表示用
    public float timeLimit = 5f; // 制限時間
    float timer = 0f;            // 残り時間
    float prevStickX = 0f;      //ゲームパッド対応

    //読みやすくするため
    private CarController Controller => car.Controller;
    private PlayerStats PlayerStats => car.PlayerStats;
    private CarRaceController RaceController => car.RaceController;
    private InGameUI Hud => car.Hud;

    void Update()
    {
        // 実行中でなければ何もしない
        if (!isRunning) return;
        // 残り時間を減らす
        timer -= Time.deltaTime;

        if(!Hud) return;

        //カウントダウン
        if (isStartGameQTE && Hud.timerText != null)
        {
            if (timer > 0f)
            {
                int display = Mathf.CeilToInt(timer);    // 2.8→3, 1.2→2, 0.3→1 のように切り上げ表示
                if (display < 0) display = 0;
                Hud.timerText.text = display.ToString();
            }
            else
            {
                Hud.timerText.text = "GO!";
            }
        }

        //UI設定
        if (Hud.timerText != null)
        {
            Hud.timerText.text = Mathf.CeilToInt(timer).ToString();
        }

        //再起動判定
        if (timer <= 0f)
        {
            if (currentCount >= targetCount)
                Success();
            else
                Fail();

            return;
        }

        // A/Dキー押下でカウント
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            currentCount++;
            UpdateUI();

            // 目標達成で即成功
            if (currentCount >= targetCount)
            {
                Success();  
            }
        }
    }

    //QTE起動とUI制御
    public void Minigame()
    {
        Debug.Log("minigame start");
        isRunning = true;
        currentCount = 0;
        timer = isStartGameQTE ? startQTETime : 9999f;

        timer = timeLimit; //time reset

        UpdateUI();

        if (Hud.QTEPanel != null)
            Hud.QTEPanel.SetActive(true);

        if (Hud.InfoText != null) Hud.InfoText.text = "A D Key Press repeatedly!";

        if (isStartGameQTE && Hud.timerText != null)
        {
            Hud.timerText.gameObject.SetActive(true);
            Hud.timerText.text = Mathf.CeilToInt(timer).ToString();   // 3
        }
    }
    //再起動成功の処理
    void Success()
    {

        isRunning = false;
        if (Hud.QTEPanel != null)
            Hud.QTEPanel.SetActive(false);
        if (Hud.timerText != null)
        {
            Hud.timerText.text = "";
            Hud.timerText.gameObject.SetActive(false);
        }
        Debug.Log("QTE Success!");

        if (isStartGameQTE)
        {
            isStartGameQTE = false;

            if (Controller != null)
                Controller.OnStartQTESuccess();  // boost
            RaceController.start_count();

            return;   // carhealth is not run
        }

        
            Controller?.SetCanControl(true);
        

        PlayerStats.ResetHp();

    }

    void UpdateUI()
    {
        if (!Hud) return;

        if (Hud.progressBar != null)
        {
            Hud.progressBar.maxValue = targetCount;
            Hud.progressBar.value = currentCount;
        }
    }

    public void StartGameQTE()
    {
        isStartGameQTE = true;
        Minigame();
    }

    //再起動失敗の処理
    void Fail()
    {
        isRunning = false;
        if (Hud.QTEPanel != null)
            Hud.QTEPanel.SetActive(false);

        if (Hud.timerText != null)
        {
            Hud.timerText.text = "";
            Hud.timerText.gameObject.SetActive(false);
        }
        Debug.Log("QTE Fail!");

        if (isStartGameQTE)
        {
            isStartGameQTE = false;

            if (Controller != null)
                Controller.OnStartQTEFail();
            RaceController.start_count();

        }
        else
        {
            //if (Controller != null && !Controller.canControl)
            //    Controller.canControl = true;
        }
    }
}
