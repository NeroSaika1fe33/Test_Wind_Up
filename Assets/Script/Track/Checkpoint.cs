using UnityEngine;

//プレイヤーのチェックポイント更新
public class Checkpoint : MonoBehaviour
{
    public int index = -1;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out CarRaceController _car))
        {
            _car.ProcessCheckpoint(this);
        }
    }
}
