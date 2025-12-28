using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomLists : MonoBehaviour
{
    PartsContainer setParts = null;
    GameObject Car = null;
    public string scenename = "InGame";
    public string[] CustomList = new string[3];

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneloaded;
        Debug.Log("Start");
    }

    void OnSceneloaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == scenename)
        {
            var ob = GameObject.Find("PartsDataManager");
            Car = GameObject.Find("Car");
            setParts = Car.GetComponent<PartsContainer>();
            if (setParts != null) { Debug.Log("SetParts”­Œ©"); }
            Debug.Log(PartsDataManager.Instance.Get_PartsID(CustomList[0]));
            Debug.Log(PartsDataManager.Instance.Get_PartsID(CustomList[1]));
            Debug.Log(PartsDataManager.Instance.Get_PartsID(CustomList[2]));
            setParts.InitialSettingsParts(
                PartsDataManager.Instance.Get_PartsID(CustomList[0]), 
                PartsDataManager.Instance.Get_PartsID(CustomList[1]), 
                PartsDataManager.Instance.Get_PartsID(CustomList[2]));
        }
    }

    void OnSceneUnloaded(Scene current)
    {
        if (current.name == "Result")
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {

    }
    public void DataStorage(string Body, string Wheel, string Mainspring)
    {
        CustomList[0] = Body;
        CustomList[1] = Mainspring;
        CustomList[2] = Wheel;
    }
    public string[] GetData()
    {
        return CustomList;
    }

}
