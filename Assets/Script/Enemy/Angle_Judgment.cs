using Fusion;
using UnityEngine;
public enum Direction
{
    None,
    Front,
    Back,
    Left,
    Right
}
public class Angle_Judgment: MonoBehaviour
{
    //目的地が正面か調べる(double rangeは正面とする範囲を角度で入力1～180)
    public static bool Front_Judgment(GameObject car, Vector3 target, float range = 0)
    {
        if(range > 180) { range = 180; };
        if(range < 0) { range = 0; };
        float angle = range;  //正面とする範囲1～180を1～-1に変換
        bool result = false;    //結果代入用
        
        //前方ベクトルを生成
        Vector3 Forward = car.transform.forward;
        Forward.y = 0;

        //targetとcarから目的地へのベクトルを生成
        Vector3 toTarget = target - car.transform.position;
        toTarget.y = 0;

        //正面か計算
        float dot = Vector3.SignedAngle(Forward, toTarget, car.transform.up);
        dot = Mathf.Abs(dot);
        if(dot <= angle) { result = true; }
        //Debug.Log(result + "現在角度" + dot + "設定角度" + angle + "誤差" + ((1.0f - dot)));
        return result;
        
    }

    //目的地が前後どちらか調べる
    public static Direction Forward_BackJudgment(GameObject car, Vector3 target)
    {
        Direction result = Direction.None;

        //前方ベクトルを生成
        Vector3 Forward = car.transform.forward;
        Forward.y = 0;

        //targetとcarから目的地へのベクトルを生成
        Vector3 toTarget = target - car.transform.position;
        toTarget.y = 0;
        //前後判定
        float dot = Vector3.Dot(Forward.normalized, toTarget.normalized);
        if(dot > 0f)
        {
            result = Direction.Front;
        }
        if (dot < 0f)
        {
            result = Direction.Back;
        }
        return result;
    }

    //目的地が左右どちらか調べる
    public static Direction Left_RightJudgment(GameObject car,Vector3 target)
    {
        Direction result = Direction.None;

        //前方ベクトルを生成
        Vector3 Forward = car.transform.forward;
        Forward.y = 0;

        //targetとcarから目的地へのベクトルを生成
        Vector3 toTarget = target - car.transform.position;
        toTarget.y = 0;

        //目的地が左右どちらにあるか判定する
        float crossY = Vector3.Cross(toTarget.normalized, Forward.normalized).y;
        //左にある場合
        if (crossY > 0)
        {
            result = Direction.Left;
        }
        //右にある場合
        else if (crossY < 0)
        {
            result = Direction.Right;
        }
        return result;
    }
}
