using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class testenemy : MonoBehaviour
{
    private enemy_manager Enemy_Manager;
    [SerializeField]
    private Transform[] Checkpoint;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 targetPos;
    private void Start()
    {
        if(GameObject.Find("enemy_manager").TryGetComponent<enemy_manager>(out var component)) 
        {
            Enemy_Manager = component;
        }
        else
        {
            UnityEngine.Debug.LogError("enemy_managerスクリプトの認識に失敗しました。");
        }
        target = Checkpoint[0];
        Target_RandomNumber(target);
    }
    private void FixedUpdate()
    {
        if (Enemy_Manager.GetCurrentlyPlaying())
        {
            agent.SetDestination(targetPos);
        }
    }
    private void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        //UnityEngine.Debug.Log("接触");
        Arriving_Checkpoint(other);

    }
    //目的地に到達したら目的地を変更
    void Arriving_Checkpoint(Collider other)
    {
        for (int i = 0; i < Checkpoint.Length; i++)
        {
            if (Checkpoint[i] == other.transform)
            {
                if (i >= Checkpoint.Length - 1)
                {
                    i = 0;
                }
                else
                {
                    i++;
                }
                target = Checkpoint[i];
                Target_RandomNumber(target);
                return;
            }
        }
    }
    void Target_RandomNumber(Transform target)
    {
        Vector3 insideUnitCircle = Random.insideUnitCircle * 10;
        float x = target.position.x + insideUnitCircle.x;
        float z = target.position.z + insideUnitCircle.y;
        targetPos.Set(x,target.position.y,z);
        //targetPos.Set(target.position.x, target.position.y, target.position.z);
    }
}