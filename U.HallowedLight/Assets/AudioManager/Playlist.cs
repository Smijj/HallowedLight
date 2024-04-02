using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Playlist
{
    public string Name;
    public AudioMixerGroup Mixer;
    public bool PlayInRandomOrder = true;
    public TrackSO[] Tracks;

    /// <summary>
    /// Initializes all the SoundSO's in the m_Music array, which will create audio sources for each of them.
    /// </summary>
    /// <param name="sourceParent"></param>
    public void InitializeMusic(GameObject sourceParent = null) {
        if (!AudioManager.Instance) return;

        if (sourceParent == null) {
            sourceParent = AudioManager.Instance.gameObject;
        }

        foreach (var soundSO in Tracks) {
            soundSO.InitializeSource(sourceParent, Mixer);
        }
    }
}
