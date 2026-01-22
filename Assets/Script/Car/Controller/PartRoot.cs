using Unity.VisualScripting;
using UnityEngine;

public class PartRoot : MonoBehaviour
{
    [SerializeField] Transform ZenmaiRoot = null;
    [SerializeField] Transform FrontRight_TireRoot = null;
    [SerializeField] Transform FrontLeft_TireRoot = null;
    [SerializeField] Transform BackRight_TireRoot = null;
    [SerializeField] Transform BackLeft_TireRoot = null;
    [SerializeField] Transform ItemRoot = null;
    void Start()
    {
        
    }
    public Transform GetZenmaiRoot { get { return ZenmaiRoot; } }
    public Transform GetFrontRight_TireRoot { get { return FrontRight_TireRoot; } }
    public Transform GetFrontLeft_TireRoot { get { return FrontLeft_TireRoot; } }
    public Transform GetBackRight_TireRoot { get { return BackRight_TireRoot; } }
    public Transform GetBackLeft_TireRoot { get { return BackLeft_TireRoot; } }
    public Transform GetItemRoot { get { return ItemRoot; } }


}
