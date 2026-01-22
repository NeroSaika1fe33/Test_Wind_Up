using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TrackDefinition", menuName = "Scriptable Objects/TrackDefinition")]
public class TrackDefinition : ScriptableObject
{
    public string trackName;
    public Sprite trackIcon;
    public int trackID;
}
