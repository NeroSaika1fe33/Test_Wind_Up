using UnityEngine;

public class CarEntity : CarComponent
{



    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
