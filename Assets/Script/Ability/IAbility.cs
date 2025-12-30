using UnityEngine;

public interface IAbility
{
    string Name { get; set; }
    bool canUse();
    void useAbility();
}
