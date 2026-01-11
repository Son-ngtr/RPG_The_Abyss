using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX NAMES")]
    [SerializeField] private string attackHit;
    [SerializeField] private string attackMiss;


    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }


    public void PlayAttackHitSFX()
    {
        AudioManager.instance.PlaySFX(attackHit, audioSource);
    }

    public void PlayAttackMissSFX()
    {
        AudioManager.instance.PlaySFX(attackMiss, audioSource);
    }
}
