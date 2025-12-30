using Unity.Cinemachine;
using UnityEngine;

public class CarCamera : CarComponent,ICameraController
{
    public Transform drivingVP;

    public bool ControlCamera(Camera cam)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
