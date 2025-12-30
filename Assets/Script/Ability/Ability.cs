using UnityEngine;

public class Ability : MonoBehaviour,IAbility
{
    public string Name { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public bool canUse()
    {
        throw new System.NotImplementedException();
    }

    public void useAbility()
    {
        throw new System.NotImplementedException();
    }

    public string GetAbilityName()
    {
        return Name;
    }

}
