using UnityEngine;

[CreateAssetMenu(fileName = "GameType", menuName = "Scriptable Objects/GameType")]
public class GameType : ScriptableObject
{
    public string modeName;
    public int lapCount;
    public bool hasPickupsParts;
}
