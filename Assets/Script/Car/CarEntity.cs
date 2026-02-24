using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarEntity : CarComponent
{
    public CarRaceController RaceController { get; private set; }

    public CarAbilityController AblityController { get; private set; }

    public PlayerStats PlayerStats { get; private set; }

    public CarSituation Situation { get; private set; }

    public Rigidbody Rigidbody { get; private set; }

    public CarCamera Camera { get; private set; }

    public CarController Controller { get; private set; }

    public CarInput Input { get; private set; }

    public InGameUI Hud { get; private set; }

    public QTEController QTEController { get; private set; }

    public PlayerPartsContainer PlayerPartsContainer { get; private set; }

    public CarAudio CarAudio { get; private set; }

    public PartBuffController PartBuffController { get; private set; }
    //マルチプレイのため
    //public static readonly List<Entity> Cars = new List<CarEntity>();

    //全部のコンポネント初期化
    virtual protected void Awake()
    {
        RaceController = GetComponent<CarRaceController>();
        AblityController = GetComponent<CarAbilityController>();
        PlayerStats = GetComponent<PlayerStats>();
        Situation = GetComponent<CarSituation>();
        Rigidbody = GetComponent<Rigidbody>();
        Camera = GetComponent<CarCamera>();
        Controller = GetComponent<CarController>();
        Input = GetComponent<CarInput>();
        QTEController = GetComponent<QTEController>();
        CarAudio = GetComponent<CarAudio>();
        PlayerPartsContainer = GetComponent<PlayerPartsContainer>();
        PartBuffController = GetComponent<PartBuffController>();
    }

    private void Start()
    {
        var components = GetComponentsInChildren<CarComponent>();
        foreach (var component in components) component.Init(this);
    }

    //このオブジェクト参照するUIを初期化
    public void InitInGameUI()
    {
        Hud = Instantiate(ResourceManager.Instance.hudPrefab);
        Hud.Init(this);
    }
}
