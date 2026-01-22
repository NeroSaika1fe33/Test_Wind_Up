using UnityEngine;

public class ItemData : MonoBehaviour
{
    [SerializeField] string ItemID = string.Empty;  //アイテムのID
    [SerializeField] string InstallationLocation = string.Empty;    //アイテムがアタッチする場所

    //アイテムのIDを返す
    public string GetItem_ItemID { get { return ItemID; } }

    //アイテムがアタッチする場所を返す
    public string GetItem_InstallationLocation { get { return InstallationLocation; } }
}
