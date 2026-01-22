using UnityEngine;


public enum Object_Type
{
    Item,
    Trap,
    obstacles,
    Player,
}
public class ObjectType : MonoBehaviour
{
    [SerializeField]Object_Type Object_type;
    public Object_Type GetObjectType { get { return Object_type; } }
    public void SetObjectType(Object_Type object_type) {  Object_type = object_type; }
}
