using UnityEngine;

public class EnemyEntity : EnemyComponent
{
    public EnemyPartsContainer EnemyPartsContainer { get; private set; }
    public EnemyController EnemyController { get; private set; }

    public Rigidbody Rigidbody { get; private set; }

    public EnemyStats EnemyStats { get; private set; }
    //マルチプレイのため
    //public static readonly List<EnemyEntity> Cars = new List<CarEntity>();

    //全部のコンポネント初期化
    void Awake()
    {
        EnemyPartsContainer = GetComponent<EnemyPartsContainer>();
        Rigidbody = GetComponent<Rigidbody>();
        EnemyController = GetComponent<EnemyController>();
        EnemyStats = GetComponent<EnemyStats>();
    }

    void Start()
    {
        var components = GetComponentsInChildren<EnemyComponent>();
        foreach (var component in components) component.Init(this);
    }
}
