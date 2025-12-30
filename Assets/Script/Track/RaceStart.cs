using UnityEngine;

//未使用
public class RaceStart : MonoBehaviour
{
    float c = 0;
	public Goal Goal;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        c = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if ( -1 <= c)
        {
             c -= Time.deltaTime; 
        }
        if(c <= 0)
        {
            //car.RaceCotroller.start_count();
        }

	}
    private void OnTriggerStay(Collider other)
    {
        if ( c <= 0)
        {
            var start = other.GetComponent<CarRaceController>();
            var start_move = other.GetComponent<CarInput>();
            //レーススタート時に実行
            if (start != null)
            {
				//Goal.start_count();
                //start_move.start();
                UnityEngine.Debug.Log("test");
            }
        }
    }
}
