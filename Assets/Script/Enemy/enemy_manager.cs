using Unity.VisualScripting;
using UnityEngine;

public class enemy_manager : MonoBehaviour
{
    [SerializeField]
    bool CurrentlyPlaying = false;
    public void RaceStart()
    {
        CurrentlyPlaying = true;
       
    }
    public void RaceFinish()
    {
        CurrentlyPlaying = false;
    }
    public bool GetCurrentlyPlaying()
    {
        return CurrentlyPlaying;
    }
}
