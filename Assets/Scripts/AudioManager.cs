using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDataBaseSO audioDataBaseSO;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

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

    public void PlaySFX(string audioName, AudioSource sfxSource)
    {
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

        sfxSource.clip = clip;
        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = data.volume;
        sfxSource.PlayOneShot(clip);
    }
}
