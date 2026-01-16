using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarWallHitDamage : CarComponent
{
    [Header("Detect Wall")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private LayerMask wallLayers = ~0; 

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Invincible")]
    [SerializeField] private float invincibleTime = 1.5f;
    private bool invincible;
    private float invTimer;

    [Header("Knockback")]
    [SerializeField] private float knockbackSpeed = 18f;
    [SerializeField] private float knockbackUp = 1.5f;
    [SerializeField] private float lockTime = 0.2f;

    private bool locked;
    private float lockTimer;

    Rigidbody rb;
    PlayerStats stats;
    CarController controller;
    CarAudio audio;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = car.PlayerStats;
        controller = car.Controller;
        audio = car.CarAudio;
    }

    void Update()
    {
        if (invincible)
        {
            invTimer -= Time.deltaTime;
            if (invTimer <= 0f) invincible = false;
        }

        if (locked)
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
            {
                locked = false;
                
                if (stats != null && stats.currentHP > 0)
                    controller?.SetCanControl(true);
            }
        }
    }

    bool IsWall(GameObject other)
    {
        if (other == null) return false;
        if (other.CompareTag(wallTag)) return true;

        int otherLayer = other.layer;
        return (wallLayers.value & (1 << otherLayer)) != 0;
    }

    void HitWall(GameObject wallObj)
    {
        if (invincible) return;

        
        if (stats != null) stats.TakeDamage(damage);

        
        invincible = true;
        invTimer = invincibleTime;

        
        Vector3 back = -transform.forward;
        back.y = 0f;
        back.Normalize();

        rb.linearVelocity = Vector3.zero;
        rb.linearVelocity = back * knockbackSpeed + Vector3.up * knockbackUp;

        
        controller?.SetCanControl(false);
        locked = true;
        lockTimer = lockTime;

        audio?.PlayCrash();
        Debug.Log("HIT WALL -> damage/knockback: " + wallObj.name);
    }

   
    void OnCollisionEnter(Collision collision)
    {
        if (!IsWall(collision.gameObject)) return;
        HitWall(collision.gameObject);
    }

   
    void OnTriggerEnter(Collider other)
    {
        if (!IsWall(other.gameObject)) return;
        HitWall(other.gameObject);
    }
}
