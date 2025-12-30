using UnityEngine;
using UnityEngine.SceneManagement;
public class CarRaceController : CarComponent
{
    [Header("RaceTotalTime")]
    float SceneCount = 0;
    float Timer = 0;    //タイマー
    bool count = false; //カウントするかどうか
    float Time1000 = 0; //タイマーを整数にしたもの(誤差修正用)

    [Header("Checkpoint")]
    private int CheckpointIndex { get; set; } = -1;

    private GoalTime GoalTime { get; set; }

    private void Start()
    {
        GoalTime = new GoalTime();
    }

    private void Update()
    {
        CountTotalTime();

        //ゴール後カウントダウンの後シーン遷移
        AfterGaol();
    }

    //トータルタイマー
    private void CountTotalTime()
    {
        if (count)
        {
            Timer += Time.deltaTime;
            Time1000 = Timer * 1000;
            float rest = Time1000 % 1;
            Time1000 -= rest;
        }
    }
    //ゴール後カウントダウン
    private void AfterGaol()
    {
        if (SceneCount > 0)
        {
            SceneCount -= Time.deltaTime;
            if (SceneCount <= 0)
            {
                LevelManager.Instance.LoadScene(SceneList.Result);
                SceneCount = 0;
            }
        }
    }
    #region Start
    public void start_count()
    {
        count = true;
    }
    public void stop_count()
    {
        count = false;
    }
    #endregion
    #region TotalTime
    public short Get_Time_ms()//タイマーのミリ秒を返す
    {
        return (short)(Time1000 % 1000);
    }
    public short Get_Time_s()//タイマーの秒を返す
    {
        return (short)(Timer / 1);
    }
    public short Get_Time_m()//タイマーの分を返す
    {
        return (short)(Timer / 60);
    }
    public short Get_Time_h()//タイマーの時を返す
    {
        return (short)(Timer / 3600);
    }
    public void Set_Time(short m, short s, short ms)
    {
        GoalTime.m = m;
        GoalTime.s = s;
        GoalTime.ms = ms;
    }
    #endregion
    //ゴール後にResultに行くまでの時間
    public void SetCountDown(float f)
    {
        SceneCount = f;
    }

    public void ProcessGoal(Goal _goal)
    {
        car.Controller.canControl = false;
        stop_count();
        //ここのGoalTime更新
        Set_Time(Get_Time_m(), Get_Time_s(), Get_Time_ms());
        //プレイデータの部分も更新
        PlayerDataManager.Instance.SetResult(GoalTime);
        SetCountDown(3.0f);
    }

    public void ProcessCheckpoint(Checkpoint _checkpoint)
    {
        if (CheckpointIndex == _checkpoint.index - 1)
        {
            CheckpointIndex++;
        }
    }

    //未使用
    //public short Get_Goal_Time_m()
    //{
    //    return _GoalTime.m;
    //}
    //public short Get_Goal_Time_s()
    //{
    //    return _GoalTime.s;
    //}
    //public short Get_Goal_Time_ms()
    //{
    //    return _GoalTime.ms;
    //}
}
