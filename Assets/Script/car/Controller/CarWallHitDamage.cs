using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CarWallHitDamage : CarComponent
{
    [Header("Wall")]
    [SerializeField] private string wallTag = "Wall";

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Invincible")]
    [SerializeField] private float invincibleTime = 2f;

    [Header("Knockback")]
    [SerializeField] private float wallKnockbackSpeed = 20f;
    [SerializeField] private float wallKnockbackUp = 2f;

    [Header("Control Lock")]
    [SerializeField] private float wallKnockbackLockTime = 0.2f;

    private bool isInvincible;
    private float invincibleTimer;

    private bool isKnockbackLock;
    private float knockbackLockTimer;

    private bool lockedByMe; 

    private Rigidbody Rb => car.Rigidbody;
    private PlayerStats Stats => car.PlayerStats;
    private CarController Controller => car.Controller;
    private InGameUI Hud => car.Hud;
    private CarAudio Audio => car.CarAudio;

    public bool IsInvincible => isInvincible;
    public bool IsKnockbackLocked => isKnockbackLock;

    void Update()
    {
        TickInvincible(Time.deltaTime);
        TickKnockbackLock(Time.deltaTime);
    }

    private void TickInvincible(float dt)
    {
        if (!isInvincible) return;

        invincibleTimer -= dt;
        if (invincibleTimer <= 0f)
        {
            isInvincible = false;
            
        }
    }

    private void TickKnockbackLock(float dt)
    {
        if (!isKnockbackLock) return;

        knockbackLockTimer += dt;

        
        Audio?.PlayCrash();

        if (knockbackLockTimer >= wallKnockbackLockTime)
        {
            isKnockbackLock = false;

            
            if (lockedByMe && ShouldRestoreControl())
            {
                Controller?.SetCanControl(true);
            }

            lockedByMe = false;
        }
    }

    private bool ShouldRestoreControl()
    {
      
        if (Stats != null && Stats.currentHP <= 0) return false;

        if (Hud != null && Hud.QTEPanel != null && Hud.QTEPanel.activeInHierarchy) return false;

        return true;
    }

    private void SetInvincible()
    {
        isInvincible = true;
        invincibleTimer = invincibleTime;
    }

    private void ApplyDamage()
    {
        if (Stats == null) return;

        Stats.TakeDamage(damage);
        SetInvincible();
    }

    private void LockControlBriefly()
    {
        if (Controller == null) return;

      
        lockedByMe = true;
        Controller.SetCanControl(false);

        isKnockbackLock = true;
        knockbackLockTimer = 0f;
    }

    private void ApplyKnockback()
    {
       
        Vector3 knockDir = -transform.forward;
        knockDir.y = 0f;
        knockDir.Normalize();

        Rb.linearVelocity = Vector3.zero;
        Rb.linearVelocity = knockDir * wallKnockbackSpeed + Vector3.up * wallKnockbackUp;
    }

    private bool IsFrontHit(Collision collision)
    {
     
        if (collision.contactCount <= 0) return false;
        ContactPoint contact = collision.GetContact(0);
        Vector3 localHitPoint = transform.InverseTransformPoint(contact.point);
        return localHitPoint.z > 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(wallTag)) return;

       
        if (isInvincible) return;

      
        if (!IsFrontHit(collision)) return;

       
        ApplyDamage();
        ApplyKnockback();
        LockControlBriefly();
    }
}
