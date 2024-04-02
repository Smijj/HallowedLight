using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class AudioManager : MonoBehaviour {
    #region Singleton
    public static AudioManager Instance;
    private void CheckSingleton() {
        if (!Instance) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Music Settings")]
    [SerializeField] private Playlist[] m_MusicPlaylists;
    [Header("SFX Settings")]
    [SerializeField] private AudioMixerGroup m_SfxMixer;
    [SerializeField] private TrackSO[] m_SFX;
    [Header("Ambient Settings")]
    [SerializeField] private AudioMixerGroup m_AmbientMixer;
    [SerializeField] private TrackSO[] m_Ambient;

    [Header("Active Music Queue's")]
    [SerializeField] private bool m_SubTrackIsPlaying = false;
    [SerializeField] private List<TrackSO> m_MainQueue = new List<TrackSO>();
    [SerializeField] private List<TrackSO> m_AmbientTracks = new List<TrackSO>();

    [Header("Debug")]
    [SerializeField] private TrackSO m_CurrentTrack;
    [SerializeField] private Playlist m_CurrentPlaylist;


    #region Unity Methods

    private void Awake() {
        CheckSingleton();
    }

    private void Update() {
        HandleTrackFinished();
    }

//#if UNITY_EDITOR
//    [MenuItem("LarrikinInteractive/Find AudioManager")]
//    public static void FindAudioManager() {
//        // Load object
//        UnityEngine.Object obj = Resources.Load("Prefabs/Systems/AudioManager");

//        // Select the object in the project folder
//        Selection.activeObject = obj;

//        // Also flash the folder yellow to highlight it
//        EditorGUIUtility.PingObject(obj);
//    }
//#endif

#endregion


    #region Public Methods

    /// <summary>
    /// Pauses currently playing track
    /// </summary>
    public void Pause() {
        if (!m_CurrentTrack) return;
        m_CurrentTrack.Pause();
    }

    /// <summary>
    /// Resumes current track
    /// </summary>
    public void Resume() {
        if (!m_CurrentTrack) return;
        m_CurrentTrack.Resume();
    }

    /// <summary>
    /// Goes to the next song in the track queue, if a subtrack is currently playing it will just resume the track queue
    /// </summary>
    public void SkipTrack() {
        if (m_MainQueue.Count.Equals(0)) return;

        if (m_SubTrackIsPlaying) {
            m_SubTrackIsPlaying = false;
            ResumeMainQueue();
        } else {
            IncrementMainQueue();
        }
    }

    /// <summary>
    /// Stops the currently playing track and clears the current track and track queue
    /// </summary>
    public void StopMusic() {
        if (m_CurrentTrack) {
            m_CurrentTrack.Stop();
            m_CurrentTrack = null;
        }
        m_CurrentPlaylist = null;
        m_SubTrackIsPlaying = false;
    }

    /// <summary>
    /// Adds a playlist to the music/track queue. (Overwrites anything that was originally in the queue)
    /// </summary>
    /// <param name="name"></param>
    public void PlayPlaylist(string name) {
        if (m_MusicPlaylists.Length.Equals(0)) {
            Debug.Log($"There are no Playlists.");
            return;
        }

        // Get the playlist and check if it exists
        Playlist playlist = GetPlaylist(name);
        if (playlist == null) return;

        // check if the playlist is the same as the one already playing
        // if it is then no need to init the music and setup the current playlist
        if (!playlist.Equals(m_CurrentPlaylist)) {
            // if it isnt then init the new playlist's music. (and maybe Uninit the music of the previous playlist?) 
            //m_CurrentPlaylist.UnInitializeMusic(); 

            playlist.InitializeMusic(gameObject);
            m_CurrentPlaylist = playlist;
            m_SubTrackIsPlaying = false;
            QueueTracks(playlist);
        }
    }

    /// <summary>
    /// Pauses the main queue and plays the provided track. The main queue of music will resume once this track is finsihed playing.
    /// </summary>
    /// <param name="name"></param>
    public void PlayTrack(string name) {
        if (m_MainQueue.Count.Equals(0)) return;  // Not sure why this is here so ill leave it commented until it causes an issue ;) *it caused an issue*

        // Get the track and check if it exists
        TrackSO track = GetTrack(name);
        if (track == null) return;

        if (!m_SubTrackIsPlaying) PauseMainQueue();
        m_SubTrackIsPlaying = true;
        Play(track);
    }

    /// <summary>
    /// Plays a sound effect as a oneshot
    /// </summary>
    /// <param name="name"></param>
    public void PlaySFX(string name) {
        TrackSO sfx = GetSFX(name); 
        if (sfx == null) return;

        sfx.InitializeSource(gameObject, m_SfxMixer);
        sfx.PlayOneShot();
    }

    /// <summary>
    /// Plays a track. You can play multiple ambient tracks at once.
    /// </summary>
    /// <param name="name"></param>
    public void PlayAmbient(string name) {
        TrackSO ambient = GetAmbient(name);
        if (ambient == null) return;

        ambient.InitializeSource(gameObject, m_AmbientMixer);
        ambient.Play();
        if (!m_AmbientTracks.Contains(ambient)) {
            m_AmbientTracks.Add(ambient);
        }
    }
    /// <summary>
    /// Stops an ambient track.
    /// </summary>
    /// <param name="name"></param>
    public void StopAmbient(string name) {
        TrackSO ambient = GetAmbient(name);
        if (ambient == null) return;

        ambient.Stop();
        if (m_AmbientTracks.Contains(ambient)) {
            m_AmbientTracks.Remove(ambient);
        }
    }
    /// <summary>
    /// Pauses all playing ambient tracks
    /// </summary>
    public void PauseAllAmbient() {
        if (m_AmbientTracks.Count.Equals(0)) return;
        foreach (var track in m_AmbientTracks) {
            track.Pause();
        }
    }
    /// <summary>
    /// Resumes all paused ambient tracks
    /// </summary>
    public void ResumeAllAmbient() {
        if (m_AmbientTracks.Count.Equals(0)) return;
        foreach (var track in m_AmbientTracks) {
            track.Resume();
        }
    }
    /// <summary>
    /// Stops all ambient tracks
    /// </summary>
    public void StopAllAmbient() {
        if (m_AmbientTracks.Count.Equals(0)) return;
        foreach (var track in m_AmbientTracks) {
            track.Stop();
        }
        m_AmbientTracks.Clear();
    }


    #endregion


    #region Private Methods

    /// <summary>
    /// Plays the music attached to a SoundSO
    /// </summary>
    /// <param name="track"></param>
    /// <param name="resume">If true will Resume the track at where it was pause if it was paused, otherwise will Play the track</param>
    private void Play(TrackSO track, bool resume = false) {
        if (!track) return;

        if (m_CurrentTrack && !m_SubTrackIsPlaying) {
            m_CurrentTrack.Stop();
        }

        m_CurrentTrack = track;
        if (!resume) track.Play();
        else track.Resume();
    }

    private void HandleTrackFinished() {
        if (m_CurrentTrack == null || !m_CurrentTrack.Source) return;

        if (m_CurrentTrack.Source.time > m_CurrentTrack.Clip.length - 1) {
            Debug.Log($"{m_CurrentTrack.name} song is over");
            if (m_SubTrackIsPlaying) {
                ResumeMainQueue();
                m_SubTrackIsPlaying = false;
            } 
            else {
                IncrementMainQueue();
            }
        }
    }

    private void PauseMainQueue() {
        if (m_MainQueue.Count.Equals(0)) return;
        if (m_CurrentTrack) {
            m_CurrentTrack.Pause();
        }
    }
    private void ResumeMainQueue() {
        if (m_MainQueue.Count.Equals(0)) return;
        if (m_MainQueue[0]) {
            Play(m_MainQueue[0], true);
        }
    }

    /// <summary>
    /// Removes the first index of the MainQueue and Plays the next song in the queue. 
    /// If the queue is finished it will requeue it using the CurrentPlaylist.
    /// </summary>
    private void IncrementMainQueue() {
        m_MainQueue.RemoveAt(0);

        // Play the next song in the MainQueue
        if (m_MainQueue.Count > 0) {
            Play(m_MainQueue[0]);
        }
        // If there is no more music in the queue, requeue the playlist.
        else {
            QueueTracks(m_CurrentPlaylist);
        }
    }

    /// <summary>
    /// Adds a Playlist to the MainQueue
    /// </summary>
    /// <param name="playlist"></param>
    private void QueueTracks(Playlist playlist) {
        if (playlist == null) return;

        m_MainQueue = SamplePlaylist(playlist);
        Play(m_MainQueue[0]);
    }

    /// <summary>
    /// Returns a List of all the SoundSO's in a Playlist. 
    /// Will randomize the order of the List depending on the Playlist's settings.
    /// </summary>
    /// <param name="playlist"></param>
    /// <returns></returns>
    private List<TrackSO> SamplePlaylist(Playlist playlist) {
        if (playlist == null) return null;

        List<TrackSO> sounds = new List<TrackSO>(playlist.Tracks);

        if (playlist.PlayInRandomOrder) {
            System.Random rng = new System.Random();
            return sounds.OrderBy(a => rng.Next()).ToList();
        }

        return sounds;
    }

    /// <summary>
    /// Gets the playlist by comparing the name provided against the names of all the playlists. NOT case sensitive - forces all strings to lowercase.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private Playlist GetPlaylist(string name) {
        if (m_MusicPlaylists.Length.Equals(0)) {
            Debug.Log($"There are no Playlists.");
            return null;
        }

        // Check if the playlist exists
        Playlist playlist = Array.Find(m_MusicPlaylists, _playlist => _playlist.Name.ToLower().Equals(name.ToLower()));
        if (playlist != null) return playlist;
        return null;
    }

    /// <summary>
    /// Gets a track by comparing the name provided against the names of all the tracks in all the playlists. NOT case sensitive - forces all strings to lowercase.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private TrackSO GetTrack(string name) {
        if (string.IsNullOrEmpty(name)) return null;

        foreach (var playlist in m_MusicPlaylists) {
            TrackSO track = GetTrackFromArray(playlist.Tracks, name);
            if (track != null) return track;
        }

        return null;
    }
    private TrackSO GetSFX(string name) {
        if (string.IsNullOrEmpty(name)) return null;

        return GetTrackFromArray(m_SFX, name);
    }
    private TrackSO GetAmbient(string name) {
        if (string.IsNullOrEmpty(name)) return null;

        return GetTrackFromArray(m_Ambient, name);
    }

    private TrackSO GetTrackFromArray(TrackSO[] array, string trackName) {
        if (array == null || array.Length.Equals(0) || string.IsNullOrEmpty(trackName)) return null;

        //TrackSO track = Array.Find(array, _track => _track != null && _track.name.ToLower().Equals(trackName.ToLower()));
        TrackSO track = null;
        foreach (var trackSO in array) {
            if (trackSO == null) continue;
            if (trackSO.name.ToLower().Equals(trackName.ToLower())) {
                track = trackSO;
            }
        }

        if (track != null) return track;

        Debug.Log($"Unable to find the track '{trackName}' in {array}.");

        return null;
    }

    #endregion

}

