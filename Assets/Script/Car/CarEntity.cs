using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarEntity : CarComponent
{

    public CarCamera Camera { get; private set; }

    public CarController Controller { get; private set; }

    public CarInput Input { get; private set; }

    public CarRaceController RaceController { get; private set; }

    public InGameUI Hud { get; private set; }

    public CarAbilityController AblityController { get; private set; }

    public PlayerStats PlayerStats { get; private set; }

    public QTEController QTEController { get; private set; }

    public CarSituation Situation {  get; private set; }

    public Rigidbody Rigidbody { get; private set; }


    //マルチプレイのため
    //public static readonly List<CarEntity> Cars = new List<CarEntity>();

    //全部のコンポネント初期化
    void Awake()
    {
        Camera = GetComponent<CarCamera>();
        Controller = GetComponent<CarController>();
        Input = GetComponent<CarInput>();
        RaceController = GetComponent<CarRaceController>();
        AblityController = GetComponent<CarAbilityController>();
        PlayerStats = GetComponent<PlayerStats>();
        QTEController = GetComponent<QTEController>();
        Situation = GetComponent<CarSituation>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        var components = GetComponentsInChildren<CarComponent>();
        foreach (var component in components) component.Init(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitInGameUI()
    {
        Hud = Instantiate(ResourceManager.Instance.hudPrefab);
        Hud.Init(this);
    }
}
