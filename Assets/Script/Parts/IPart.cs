using UnityEngine;

public interface IPart
{
    string partsName { get; set; }
    float maxSpeed { get; set; }
    float acceleration { get; set; }
    float weight { get; set; }
    string partsType {  get; set; }

    string abilityName { get; set; }

}
