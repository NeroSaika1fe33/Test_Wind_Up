using UnityEngine;
using UnityEngine.Windows;

public class EnemyMove : EnemyComponent
{
    public Rigidbody Rigidbody => car.Rigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody.AddForce(transform.forward * 4, ForceMode.Acceleration);
    }
}
