using UnityEngine;

//車体の位置をリセットするためのクラス
public class ResetPlaneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarEntity _car))
        {
            Debug.Log("チェックポイントに戻す");
            _car.RaceController.ResetToCheckPoint();
        }
    }
}
