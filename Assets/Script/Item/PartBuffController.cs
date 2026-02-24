using System.Collections;
using UnityEngine;


// 管理「 item part +  buff」

public class PartBuffController : CarComponent
{
    [System.Serializable]
    public class BoostPartConfig
    {
        public ItemType itemType = ItemType.Boost;
        public string itemPartPrefabName = "Item_Boost";   // Resources の prefab 名
        [Min(0.1f)] public float durationMin = 5f;
        [Min(0.1f)] public float durationMax = 10f;

        [Header("Buff Multipliers")]
        public float maxSpeedMultiplier = 1.25f;          // 最高速 +25%
        public float accelerationMultiplier = 1.15f;      // 加速度 +15%
    }

    [Header("Configs")]
    [SerializeField] private BoostPartConfig boostConfig = new BoostPartConfig();

    private Coroutine runningRoutine;
    private CarLocomotion locomotion;
    private PlayerPartsContainer partsContainer;

    private void Start()
    {
        locomotion = GetComponent<CarLocomotion>();
        partsContainer = car != null ? car.PlayerPartsContainer : GetComponent<PlayerPartsContainer>();
    }

    
    // 呼ぶ： itemTypeによってbuffを起動
  
    public void ActivateByItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Boost:
                ActivateBoostRandomDuration();
                break;

            
            case ItemType.test1:
                Debug.Log("[TemporaryPartBuffController] test1 実装してない");
                break;

            case ItemType.test2:
                Debug.Log("[TemporaryPartBuffController] test2 実装してない");
                break;

            default:
                break;
        }
    }

    
    //  Boost（5~10s random）
   
    public void ActivateBoostRandomDuration()
    {
        float duration = Random.Range(boostConfig.durationMin, boostConfig.durationMax);
        ActivateTemporaryPartBuff(
            boostConfig.itemPartPrefabName,
            duration,
            boostConfig.maxSpeedMultiplier,
            boostConfig.accelerationMultiplier
        );
    }

  
    // 起動「 part + buff」
    
    public void ActivateTemporaryPartBuff(string partPrefabName, float duration, float maxSpeedMul, float accelMul)
    {
        // アイテムを持ってる場合，先に解除する
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
            ForceClearNow();
        }

        runningRoutine = StartCoroutine(Co_TemporaryPartBuff(partPrefabName, duration, maxSpeedMul, accelMul));
    }

    private IEnumerator Co_TemporaryPartBuff(string partPrefabName, float duration, float maxSpeedMul, float accelMul)
    {
        // パーツ外見を示す
        if (partsContainer != null && !string.IsNullOrEmpty(partPrefabName))
        {
            partsContainer.UpdateItemParts(partPrefabName);
        }

        // buffを使う
        if (locomotion != null)
        {
            locomotion.ApplyItemBuff(maxSpeedMul, accelMul);
        }

        Debug.Log($"[ItemPartBuffController] Start item buff: {partPrefabName}, duration={duration:F1}s, maxSpeed x{maxSpeedMul}, accel x{accelMul}");

        // count
        yield return new WaitForSeconds(duration);

        // clear
        ForceClearNow();

        runningRoutine = null;
    }

   
    // 即解除（時間切り / 被る場合）
   
    public void ForceClearNow()
    {
        if (locomotion != null)
        {
            locomotion.ClearItemBuff();
        }

        if (partsContainer != null)
        {
            partsContainer.ClearItemParts();
        }

        Debug.Log("[TemporaryPartBuffController] ForceClearNow");
    }
}