using UnityEngine;

public enum NextCornerData 
{
    Left,
    Right,
}
public class NextCorner : MonoBehaviour
{
    [SerializeField]
    NextCornerData nextCornerData;
    public NextCornerData GetNextCorner { get { return nextCornerData; }}
}
