using UnityEngine;

public class Part : IPart
{
    public string partsName { get; set; }
    public float maxSpeed { get; set; }
    public float acceleration { get; set; }
    public float weight { get; set; }
    public string partsType { get; set; }

    public string abilityName { get; set; }

    public string GetPartsName()
    {
        return partsName;
    }

    public float PlusMaxSpeed(float _maxSpeed)
    {
        return _maxSpeed + maxSpeed;
    }
    public float PlusAcceleration(float _acceleration)
    {
        return _acceleration + acceleration;
    }
    public float PlusWeight(float _weight)
    {
        return _weight + weight;
    }


}
