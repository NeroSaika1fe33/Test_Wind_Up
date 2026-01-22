using System.Collections;
using UnityEngine;

public class CarWheelSparkVFX : CarComponent
{
    [Header("Refs")]
    [SerializeField] private CarDriftModule drift;
    [SerializeField] private Transform[] wheelPoints;        // 4Ç¬é‘ó÷à íu
    [SerializeField] private GameObject chargeSparkPrefab;     // PS_WheelSpark prefab
    [SerializeField] private GameObject boostSparkPrefab;

    [Header("Timing")]
    [SerializeField] private float playSeconds = 0.25f;

    private ParticleSystem[] chargeSpawned;
    private ParticleSystem[] boostSpawned;
    
    void Awake()
    {
        if (drift == null) drift = GetComponent<CarDriftModule>();
    }

    void OnEnable()
    {
        if (drift == null) return;
        drift.DriftChargeStarted += OnChargeStarted;
        drift.DriftBoostStarted += OnBoostStarted;

    }

    void OnDisable()
    {
        if (drift == null) return;
        drift.DriftChargeStarted -= OnChargeStarted;
        drift.DriftBoostStarted -= OnBoostStarted;

    }

    void Start()
    {
        // 4Ç¬ÇÃsparkê∂ê¨ 4 å¬ó±éqÅiinstantiateÇîÇØÇÈ
        if (wheelPoints == null || wheelPoints.Length == 0) return;

        chargeSpawned = new ParticleSystem[wheelPoints.Length];
        boostSpawned = new ParticleSystem[wheelPoints.Length];

        for (int i = 0; i < wheelPoints.Length; i++)
        {
            var wp = wheelPoints[i];
            if (wp == null) continue;

            if (chargeSparkPrefab != null)
            {
                var go = Instantiate(chargeSparkPrefab, wp.position, wp.rotation, wp);
                chargeSpawned[i] = go.GetComponentInChildren<ParticleSystem>(true);
                chargeSpawned[i]?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (boostSparkPrefab != null)
            {
                var go = Instantiate(boostSparkPrefab, wp.position, wp.rotation, wp);
                boostSpawned[i] = go.GetComponentInChildren<ParticleSystem>(true);
                boostSpawned[i]?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private void PlaySparks(ParticleSystem[] list)
    {
        if (list == null || wheelPoints == null) return;

        for (int i = 0; i < list.Length; i++)
        {
            var ps = list[i];
            if (ps == null) continue;
            if (i >= wheelPoints.Length || wheelPoints[i] == null) continue;

            ps.transform.position = wheelPoints[i].position;
            ps.transform.rotation = wheelPoints[i].rotation;
            ps.Play(true);
        }


    }
    private void OnChargeStarted()
    {
        PlaySparks(chargeSpawned);   
    }

    private void OnBoostStarted()
    {
        PlaySparks(boostSpawned);    
    }
}
