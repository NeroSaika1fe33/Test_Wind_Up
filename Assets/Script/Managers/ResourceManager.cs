using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance => Singleton<ResourceManager>.Instance;

    public InGameUI hudPrefab;
    public GameType[] gameType;
    public GameObject[] carPrefab;
    public Texture2D[] texture2Ds;
    public GameObject CameraPrefab;
    public GameObject[] TrackPrefab;
    public TrackDefinition[] TrackDefs;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
