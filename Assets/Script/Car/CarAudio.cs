using UnityEngine;

public class CarAudio : CarComponent
{
    [Header("Audio Sources")]
    public AudioSource oneShotSource;   //�Ԃ���A�u�X�g�Ȃǈ��
    public AudioSource accelloopSource;      // accel loop�p
    public AudioSource driftloopSource;// DRIFT loop�p
    public AudioSource oneShotChargeSource;

    [Header("Clips")]
    public AudioClip crashClip;
    public AudioClip driftChargeLoop;
    public AudioClip boostClip;
    public AudioClip AccelLoop;
    public AudioClip ZenmaiLoop;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;//Inspector slider
    public float crashCooldown = 0.15f;

    float lastCrashTime = -999f;

    void Awake()
    {
        //日本語学校明確に散策だだあ
        if (oneShotSource == null)
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
            oneShotSource.loop = false;
            oneShotSource.spatialBlend = 0f; // 0:2D 1:3d 
        }

        if (accelloopSource == null)
        {
            accelloopSource = gameObject.AddComponent<AudioSource>();
            accelloopSource.playOnAwake = false;
            accelloopSource.loop = true;
            accelloopSource.spatialBlend = 0f; // 0:2D 1:3d 
        }

        if (driftloopSource == null)
        {
            driftloopSource = gameObject.AddComponent<AudioSource>();
            driftloopSource.playOnAwake = false;
            driftloopSource.loop = true;
            driftloopSource.spatialBlend = 0f;
        }

        if (oneShotChargeSource == null)
        {
            oneShotChargeSource = gameObject.AddComponent<AudioSource>();
            oneShotChargeSource.playOnAwake = false;
            oneShotChargeSource.loop = false;
            oneShotChargeSource.spatialBlend = 0f;
        }
    }

    void StopAllLoops()
    {
        if (accelloopSource != null) accelloopSource.Stop();
        if (oneShotSource != null) oneShotSource.Stop();
        if (driftloopSource != null) driftloopSource.Stop();
    }

    void PlayOneShot(AudioClip clip, float volumeMul = 1f, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        if (clip == null || oneShotSource == null) return;
        oneShotSource.pitch = Random.Range(pitchMin, pitchMax); // �����������_�� <1:�������A�ቹ�@���P�F�����A����
        oneShotSource.PlayOneShot(clip, volume * volumeMul);
        oneShotSource.pitch = 1f;
    }

    public void PlayCrash()
    {
        if (Time.time - lastCrashTime < crashCooldown) return;
        lastCrashTime = Time.time;
        PlayOneShot(crashClip,0.6f, 0.9f, 1.05f);
    }

    public void StartDriftCharge()
    {
        if (driftChargeLoop == null || driftloopSource == null) return;
        if (driftloopSource.isPlaying && driftloopSource.clip == driftChargeLoop) return;

        driftloopSource.clip = driftChargeLoop;
        driftloopSource.volume = volume * 0.8f;
        driftloopSource.Play();
    } 

    public void StopDriftCharge()
    {
        if (driftloopSource != null && driftloopSource.isPlaying && driftloopSource.clip == driftChargeLoop)
            driftloopSource.Stop();
    }

    public void PlayBoost()
    {
        PlayOneShot(boostClip, 0.95f, 1.15f);
    }

    public void StartAccel()
    {
        if(AccelLoop == null || accelloopSource == null) return;
        if(accelloopSource.isPlaying && accelloopSource.clip == AccelLoop) return;

        accelloopSource.clip = AccelLoop;
        accelloopSource.volume = volume * 0.8f;
        accelloopSource.Play();
    }

    public void StopAccel()
    {
        if (accelloopSource != null && accelloopSource.isPlaying && accelloopSource.clip == AccelLoop)
            accelloopSource.Stop();
    }

    public void StartZenmai()
    {
        if (ZenmaiLoop == null || accelloopSource == null) return;
        if (accelloopSource.isPlaying && accelloopSource.clip == ZenmaiLoop) return;

        accelloopSource.clip = ZenmaiLoop;
        accelloopSource.Play();
    }

    public void StopZenmai()
    {
        if (accelloopSource != null && accelloopSource.isPlaying && accelloopSource.clip == ZenmaiLoop)
            accelloopSource.Stop();
    }
}