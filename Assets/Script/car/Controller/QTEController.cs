using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Carを再起動するためのA/D連打
public class QTEController : CarComponent
{
    [Header("inspector object")]
    public GameObject qtePanel;
    public Slider progressBar;
    public TMP_Text InfoText;
    public TMP_Text timerText;

    [SerializeField] PlayerStats PlayerStats;
    public CarController carcontroll;
    public Goal Goal;

    int currentCount = 0;// 現在の連打回数
    int targetCount = 20;// 目標連打回数

    bool isRunning = false;// QTE実行中flag

    [Header("Start")]
    bool isStartGameQTE = false; //ゲーム開始時のQTEかどうか
    public float startQTETime = 3f;/// 開始QTEの表示用
    public float timeLimit = 5f; // 制限時間
    float timer = 0f;            // 残り時間


    void Update()
    {
        // 実行中でなければ何もしない
        if (!isRunning) return;
        // 残り時間を減らす
        timer -= Time.deltaTime;

        //カウントダウン
        if (isStartGameQTE && timerText != null)
        {
            if (timer > 0f)
            {
                int display = Mathf.CeilToInt(timer);    // 2.8→3, 1.2→2, 0.3→1 のように切り上げ表示
                if (display < 0) display = 0;
                timerText.text = display.ToString();
            }
            else
            {

                timerText.text = "GO!";
            }
        }

        //UI設定
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timer).ToString();
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

        if (qtePanel != null)
            qtePanel.SetActive(true);

        if (InfoText != null) InfoText.text = "A D Key Press repeatedly!";

        if (isStartGameQTE && timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.CeilToInt(timer).ToString();   // 3
        }
    }
    //再起動成功の処理
    void Success()
    {

        isRunning = false;
        if (qtePanel != null)
            qtePanel.SetActive(false);
        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }
        Debug.Log("QTE Success!");
        Debug.Log(carcontroll.canControl);

        if (isStartGameQTE)
        {
            isStartGameQTE = false;

            if (carcontroll != null)
                carcontroll.OnStartQTESuccess();  // boost
            Goal.start_count();

            return;   // carhealth is not run
        }

        if (PlayerStats != null)
        {
            Debug.Log(PlayerStats.currentHP);
        }

        if (carcontroll.canControl == false)
        {
            carcontroll.canControl = true;
        }

        PlayerStats.ResetHp();

    }

    void UpdateUI()
    {
        if (progressBar != null)
        {
            progressBar.maxValue = targetCount;
            progressBar.value = currentCount;
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
        if (qtePanel != null)
            qtePanel.SetActive(false);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }
        Debug.Log("QTE Fail!");

        if (isStartGameQTE)
        {
            isStartGameQTE = false;

            if (carcontroll != null)
                carcontroll.OnStartQTEFail();
            Goal.start_count();
        }
        else
        {

            if (carcontroll != null && !carcontroll.canControl)
                carcontroll.canControl = true;
        }
    }
}
