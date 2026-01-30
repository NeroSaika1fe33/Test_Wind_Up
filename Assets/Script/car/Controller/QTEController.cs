using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

    [Header("Gamepad Spin (Add-on)")]
    [SerializeField] private bool enableSpin = true;
    [SerializeField] private bool useRightStick = false; // false=左スティック
    [SerializeField] private float stickDeadzone = 0.35f;
    [SerializeField] private float degreesPerHit = 45f;  // 45度=1 hit（1周約8hit）
    [SerializeField] private bool requireConsistentSpin = false;

    private float _lastAngle;
    private bool _hasLastAngle;
    private float _accumDegrees;
    private int _lastSpinSign;
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

        // 追加：ゲームパッド「スティック回転」で連打扱い
        int spinHits = SpinHitsThisFrame();
        if (spinHits > 0)
        {
            currentCount += spinHits;
            UpdateUI();

            if (currentCount >= targetCount)
            {
                Success();
                return;
            }
        }

        // A/Dキー押下でカウント
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            currentCount++;
            UpdateUI();

            //目標達成で即成功
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

        if (Hud.InfoText != null) Hud.InfoText.text = "A Dキーを連打しよう!";

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
    // =========================
    // ? Gamepad Spin Detection
    // =========================
    private int SpinHitsThisFrame()
    {
        if (!enableSpin) return 0;

        var g = Gamepad.current;
        if (g == null)
        {
            ResetSpinState();
            return 0;
        }

        Vector2 v = useRightStick ? g.rightStick.ReadValue() : g.leftStick.ReadValue();

       
        if (v.magnitude < stickDeadzone)
        {
            ResetSpinState();
            return 0;
        }

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg; // -180 ~ 180

        if (!_hasLastAngle)
        {
            _hasLastAngle = true;
            _lastAngle = angle;
            _accumDegrees = 0f;
            _lastSpinSign = 0;
            return 0;
        }

        float delta = Mathf.DeltaAngle(_lastAngle, angle);
        _lastAngle = angle;

        int sign = delta > 0 ? +1 : (delta < 0 ? -1 : 0);

        if (requireConsistentSpin)
        {
            if (sign != 0 && _lastSpinSign != 0 && sign != _lastSpinSign)
            {
                //必ず統一方向回す
                _accumDegrees = 0f;
            }
        }
        if (sign != 0) _lastSpinSign = sign;

        _accumDegrees += Mathf.Abs(delta);

        int hits = 0;
        while (_accumDegrees >= degreesPerHit)
        {
            _accumDegrees -= degreesPerHit;
            hits++;
        }

        return hits;
    }

    private void ResetSpinState()
    {
        _hasLastAngle = false;
        _accumDegrees = 0f;
        _lastSpinSign = 0;
    }
}
