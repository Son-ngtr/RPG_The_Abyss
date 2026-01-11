using UnityEngine;

public class Entity_SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX NAMES")]
    [SerializeField] private string attackHit;
    [SerializeField] private string attackMiss;

    [Space]
    [SerializeField] private float soundDistance = 15f;
    [SerializeField] private bool showGizmo = true;


    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }


    public void PlayAttackHitSFX()
    {
        AudioManager.instance.PlaySFX(attackHit, audioSource, soundDistance);
    }

    public void PlayAttackMissSFX()
    {
        AudioManager.instance.PlaySFX(attackMiss, audioSource, soundDistance);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundDistance);
    }
}
