using UnityEngine;

public class AudioRangeControler : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform player;

    [SerializeField] private float minDistanceToHearSound = 12f;
    private float maxVolume;
    [SerializeField] private bool showGizmo = true;


    private void Start()
    {
        player = Player.instance.transform;
        audioSource = GetComponent<AudioSource>();

        maxVolume = audioSource.volume;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));

        float targetVolume = Mathf.Lerp(0, maxVolume, t * t);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * 3f);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false)
        {
            return;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minDistanceToHearSound);
    }
}
