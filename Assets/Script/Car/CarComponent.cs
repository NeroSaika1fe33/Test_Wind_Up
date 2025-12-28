using UnityEngine;
//マルチプレイのためのヘッダ
//using Fusion;

//Carの機能実装するための親クラス
public class CarComponent : MonoBehaviour
{
    public CarEntity car {  get; private set; }

    public virtual void Init(CarEntity _car)
    {
        car = _car;
    }

    public virtual void OnRaceStart() { }

    public virtual void OnRaceEnd() { }

    public virtual void OnAbility(Ability _ability,float _timeUntilCanUse) { }
}
