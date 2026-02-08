using UnityEngine;
//マルチプレイのためのヘッダ
//using Fusion;

//Carの機能実装するための親クラス
public class EnemyComponent : MonoBehaviour
{
    public EnemyEntity car {  get; private set; }

    public virtual void Init(EnemyEntity _car)
    {
        car = _car;
    }

    public virtual void OnRaceStart() { }

    public virtual void OnRaceEnd() { }

    public virtual void OnAbility(Ability _ability,float _timeUntilCanUse) { }
}
