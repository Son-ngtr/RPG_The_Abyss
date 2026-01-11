using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Audio/AudioDataBaseSO")]
public class AudioDataBaseSO : ScriptableObject
{
    public List<AudioClipData> playerAudio;
    public List<AudioClipData> uiAudio;


    private Dictionary<string, AudioClipData> clipCollection;


    private void OnEnable()
    {
        clipCollection = new Dictionary<string, AudioClipData>();

        AddToCollection(playerAudio);
        AddToCollection(uiAudio);
    }

    public AudioClipData Get(string groupName)
    {
        return clipCollection.TryGetValue(groupName, out var audioClipData) ? audioClipData : null;
    }


    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach (var audioClipData in listToAdd)
        {
            if (audioClipData != null && clipCollection.ContainsKey(audioClipData.audioName) == false)
            {
                clipCollection.Add(audioClipData.audioName, audioClipData);
            }
        }
    }
}


[Serializable]
public class AudioClipData
{
    public string audioName;
    public List<AudioClip> clips = new List<AudioClip>();

    [Range(0f, 1f)] 
    public float volume = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips.Count == 0 || clips == null)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, clips.Count);
        return clips[randomIndex];
    }
}
