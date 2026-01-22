using UnityEngine;

//トラックオブジェクトを実装するためのクラス
public class Track : MonoBehaviour
{
    public CameraTrack[] introTracks;
    public Checkpoint[] checkpoints;
    public Transform[] startLinePos;
    public Transform[] ItemPos;
    public int TrackID = 0;
    public GameObject[] Item;


    private void Awake()
    {
        GameManager.Instance.SetCurrentTrack(this);
        InitCheckpoints();
        InitItemPos();
    }

    private void InitCheckpoints()
    {
        for(int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].index = i;
        }
    }

    private void InitItemPos()
    {
        for(int i = 0;i<ItemPos.Length;i++)
        {
            var item=Instantiate(Item[i],ItemPos[i]);
        }
    }
}
