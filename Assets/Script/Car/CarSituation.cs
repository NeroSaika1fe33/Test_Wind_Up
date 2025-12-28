using System;
using System.Linq.Expressions;
using UnityEngine;

public class CarSituation : CarComponent
{
    public enum Steat
    {
        None,
        Waiting,
        Driving,
        Goal,
        Destroyed,
    }

    Steat steat = Steat.None;
    private void Start()
    {
    
    }
    public Steat Get_Steat()
    {
        return steat;
    }
    //ƒS[ƒ‹ó‘Ô‚É‚·‚é
    public void steat_goal()
    {
        steat = Steat.Goal;
    }
    //‘Ò‹@ó‘Ô‚É‚·‚é
    public void steat_Waiting()
    {
        steat = Steat.Waiting;
    }
    //‘–só‘Ô‚É‚·‚é
    public void Driving()
    {
        steat = Steat.Driving;
    }
    //”j‘¹ó‘Ô‚É‚·‚é
    public void steat_Destroyed()
    {
        steat = Steat.Destroyed;
    }
    void Update()
    {

        switch (steat)
        {
            case Steat.Goal:
                {
                   // UnityEngine.Debug.Log("Goal");
                }
                break;
            case Steat.Destroyed:
                {
                   // UnityEngine.Debug.Log("Destroyed");
                }
                break;
            default:
                break;
        }
    }
}
