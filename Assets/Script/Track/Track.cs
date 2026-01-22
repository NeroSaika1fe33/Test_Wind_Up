using UnityEngine;

//トラックオブジェクトを実装するためのクラス
public class Track : MonoBehaviour
{
    public CameraTrack[] introTracks;
    public Checkpoint[] checkpoints;
    public Transform[] startLinePos;

    private void Awake()
    {
        InitCheckpoints();
    }

    private void InitCheckpoints()
    {
        for(int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].index = i;
        }
    }
}
