using UnityEngine;
using static UnityEngine.InputSystem.HID.HID;
public class Goal : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarRaceController _car))
        {
			Debug.Log("goal");
            _car.ProcessGoal(this);
        }
    }
}
