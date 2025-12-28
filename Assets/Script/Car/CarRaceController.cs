using UnityEngine;
using UnityEngine.SceneManagement;
public class CarRaceController : CarComponent
{
    [Header("RaceTotalTime")]
    float SceneCount = 0;

    private int CheckpointIndex { get; set; } = -1;
    public struct GoalTime
    {
        public short m;
        public short s;
        public short ms;
    }
    private GoalTime _GoalTime = new GoalTime { m = 1, s = 30, ms = 500 };
    public void Set_Time(short m, short s, short ms)
    {
        _GoalTime.m = m;
        _GoalTime.s = s;
        _GoalTime.ms = ms;
    }
    public short Get_Time_m()
    {
        return _GoalTime.m;
    }
    public short Get_Time_s()
    {
        return _GoalTime.s;
    }
    public short Get_Time_ms()
    {
        return _GoalTime.ms;
    }

    public void SetCountDown(float f)//ƒS[ƒ‹Œã‚ÉResult‚És‚­‚Ü‚Å‚ÌŽžŠÔ
    {
        SceneCount = f;
    }

    public void ProcessGoal(Goal _goal)
    {
        car.Controller.canControl = false;
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

    public void ProcessCheckpoint(Checkpoint _checkpoint)
    {
        if (CheckpointIndex == _checkpoint.index - 1)
        {
            CheckpointIndex++;
        }
    }
}
