using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDataBaseSO audioDataBaseSO;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Space]
    private AudioClip lastMusicPlayed;
    private Transform player;
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    [SerializeField] private bool bgmShoudPlay;

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

    private void Update()
    {
        if (bgmShoudPlay 
            && bgmSource.isPlaying == false 
            && string.IsNullOrEmpty(currentBgmGroupName) == false)
        {
            NextBGM(currentBgmGroupName);
        }

        if (bgmSource.isPlaying && bgmShoudPlay == false)
        {
            StopBGM();
        }
    }

    public void StartBGM(string musicGroup)
    {
        bgmShoudPlay = true;

        if (musicGroup == currentBgmGroupName)
        {
            return;
        }

        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)
    {
        bgmShoudPlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
        {
            StopCoroutine(currentBgmCo);
        }

        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    public void StopBGM()
    {
        bgmShoudPlay = false;
        StartCoroutine(FadeVolumeCo(bgmSource, 0f, 2f));

        if (currentBgmCo != null)
        {
            StopCoroutine(currentBgmCo);
        }
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDataBaseSO.Get(musicGroup);
        AudioClip nextMusic = data.GetRandomClip();

        if (nextMusic == null || data.clips.Count == 0)
        {
            Debug.LogWarning($"No music clips found for group: {musicGroup}");
            yield break;
        }

        while (nextMusic == lastMusicPlayed && data.clips.Count > 1)
        {
            nextMusic = data.GetRandomClip();
        }

        if (bgmSource.isPlaying)
        {
            yield return FadeVolumeCo(bgmSource, 0f, 2f);
        }

        lastMusicPlayed = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.volume = 0f;
        bgmSource.Play();

        StartCoroutine(FadeVolumeCo(bgmSource, data.maxVolume, 2f));
    }

    private IEnumerator FadeVolumeCo(AudioSource sfxSource, float targetVolume, float duration)
    {
        float startVolume = sfxSource.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            sfxSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        sfxSource.volume = targetVolume;
    }

    public void PlaySFX(string audioName, AudioSource sfxSource, float minDistanceToHearSound = 5f)
    {
        if (player == null)
            player = Player.instance.transform;

        var data = audioDataBaseSO.Get(audioName);

        if (data == null)
        {
            Debug.LogWarning($"AudioClipData with name {audioName} not found.");
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null)
            return;

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
