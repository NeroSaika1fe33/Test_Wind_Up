using UnityEngine;

public class PlayerSaveData
{
    public string name;
    public int id;
    public string[] CustomizeList = new string[3];
    public float maxSpeed;
    public float acceleration;
    public float weight;
    public string abilityName;
    public string result;
    public PlayerSaveData(string _name, int _id, string[] _CustomizeList, float _maxSpeed, float _acceleration, float _weight, string abilityName, string result)
    {
        this.name = _name;
        this.id = _id;
        CustomizeList = _CustomizeList;
        this.maxSpeed = _maxSpeed;
        this.acceleration = _acceleration;
        this.weight = _weight;
        this.abilityName = abilityName;
        this.result = result;
    }
}
