using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDataBaseSO audioDataBaseSO;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private Transform player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(string audioName, AudioSource sfxSource, float minDistanceToHearSound = 5f)
    {
        if (player == null)
        {
            player = Player.instance.transform;
        }

        var data = audioDataBaseSO.Get(audioName);

        if (data == null)
        {
            Debug.LogWarning($"AudioClipData with name {audioName} not found.");
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null)
        {
            return;
        }

        float maxVolume = data.maxVolume;
        float distanceToPlayer = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - (distanceToPlayer / minDistanceToHearSound)); // 0 when out of range, 1 when at the source

        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t); // Volume decreases with distance
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGlobalSFX(string soundName)
    {
        var data = audioDataBaseSO.Get(soundName);
        if (data == null)
        {
            Debug.LogWarning($"AudioClipData with name {soundName} not found.");
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null)
        {
            return;
        }

        Debug.Log($"Playing global SFX: {soundName}");
        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }
}
