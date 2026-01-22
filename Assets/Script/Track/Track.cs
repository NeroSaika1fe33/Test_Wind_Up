using UnityEngine;

//トラックオブジェクトを実装するためのクラス
public class Track : MonoBehaviour
{
    public CameraTrack[] introTracks;
    public Checkpoint[] checkpoints;
    public Transform[] startLinePos;
    public int TrackID = 0;

    private void Awake()
    {
        GameManager.Instance.SetCurrentTrack(this);
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
