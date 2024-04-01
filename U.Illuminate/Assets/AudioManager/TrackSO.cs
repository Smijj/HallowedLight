using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "new TrackSO", menuName = "AudioModule/New TrackSO")]
public class TrackSO : ScriptableObject {

    public string DisplayName => m_DisplayName;
    [SerializeField] private string m_DisplayName = "Song Name";

    public AudioClip Clip => m_Clip;
    [SerializeField] private AudioClip m_Clip;

    [Range(0, 1), SerializeField] protected float m_Volume = 1;
    [Range(0f, 3f), SerializeField] private float m_Pitch = 1;
    [SerializeField] private bool m_Loop = false;
    [SerializeField] protected float m_TrackFadeInTime = 0.5f;
    [SerializeField] protected float m_TrackFadeOutTime = 1f;


    public AudioSource Source { get => m_Source; set => m_Source = value; }
    protected AudioSource m_Source;
    public float CurrentTime { get => m_Source ? m_Source.time : 0; private set { } }
    public bool IsPlaying { get => m_Source ? m_Source.isPlaying : false; }

    private LTDescr m_FadeIn;
    private LTDescr m_FadeOut;
    private System.Action FadeOutAction;
    private float m_PausedTime = 0;


    #region Public Methods

    public void Play() {
        if (StartSound() == false) return;
        m_PausedTime = 0;

#if LI_Debug
        Debug.Log($"{name} Played.");
#endif
    }

    public void Stop() {
        if (StopSound() == false) return;
        m_PausedTime = 0;

#if LI_Debug
        Debug.Log($"{name} Stopped.");
#endif
    }

    public void Resume() {
        if (StartSound() == false) return;
        m_Source.time = m_PausedTime;

#if LI_Debug
        Debug.Log($"{name} Resumed.");
#endif
    }

    public void Pause() {
        if (StopSound() == false) return;
        m_PausedTime = m_Source.time;

#if LI_Debug
        Debug.Log($"{name} Paused.");
#endif
    }

    public virtual void PlayOneShot() {
        if (!m_Clip || !AudioManager.Instance) return;
        if (!m_Source) {
            InitializeSource();
        }

        m_Source.PlayOneShot(m_Clip);
    }

    public void InitializeSource(GameObject sourceParent = null, AudioMixerGroup mixer = null) {
        if (m_Source) return;

        if (sourceParent == null) {
            sourceParent = AudioManager.Instance.gameObject;
            m_Source = sourceParent.AddComponent<AudioSource>();
        } 
        else {
            m_Source = sourceParent.AddComponent<AudioSource>();
        }
        m_Source.clip = m_Clip;
        m_Source.volume = m_Volume;
        m_Source.pitch = m_Pitch;
        m_Source.loop = m_Loop;
        m_Source.outputAudioMixerGroup = mixer;
        m_Source.playOnAwake = false;
    }

    #endregion


    #region Private Methods

    protected bool StartSound() {
        if (!m_Clip || !AudioManager.Instance) return false;
        if (!m_Source) {
            InitializeSource();
        }

        // If the clip was fading out when Play was called then cancel the FadeOut.
        CancelAnyTweens();
        m_Source.Play();
        FadeIn(m_TrackFadeInTime);

        return true;
    }

    protected bool StopSound() {
        if (!m_Clip || !m_Source || !AudioManager.Instance) return false;

        // If the clip was fading in when Stop was called then cancel the FadeIn.m_TrackFadeInTime
        CancelAnyTweens();
        FadeOut(m_TrackFadeOutTime);

        return true;
    }


    protected void FadeIn(float duration = 0.5f) {
        if (duration.Equals(0)) {
            m_FadeIn = null;
            return;
        }

        m_Source.volume = 0;
        m_FadeIn = LeanTween.value(m_Source.gameObject, x => m_Source.volume = x, 0, m_Volume, duration)
            .setOnComplete(() => m_FadeIn = null);
    }

    protected void FadeOut(float duration = 1f, System.Action callback = null) {
        if (duration.Equals(0)) {
            m_Source.Stop();
            callback?.Invoke();
            m_FadeOut = null;
            return;
        }

        float currentVolume = m_Source.volume;
        FadeOutAction = callback;

        m_FadeOut = LeanTween.value(m_Source.gameObject, x => m_Source.volume = x, currentVolume, 0, duration)
            .setOnComplete(() => {
                m_Source.Stop();
                callback?.Invoke(); 
                FadeOutAction = null;
                m_FadeOut = null;
            });
    }


    protected void CancelAnyTweens() {
        if (m_FadeIn != null) {
            LeanTween.cancel(m_Source.gameObject, m_FadeIn.id);
            m_FadeIn = null;
#if LI_Debug
            Debug.Log($"{name} FadeIn cancelled.");
#endif
        }

        if (m_FadeOut != null) {
            LeanTween.cancel(m_Source.gameObject, m_FadeOut.id);
            m_FadeOut = null;
            FadeOutAction?.Invoke();
            FadeOutAction = null;
#if LI_Debug
            Debug.Log($"{name} FadeOut cancelled.");
#endif
        }
    }

    #endregion
}

