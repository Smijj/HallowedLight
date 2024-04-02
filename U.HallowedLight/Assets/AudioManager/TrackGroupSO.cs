using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new TrackGroupSO", menuName = "AudioModule/New TrackGroupSO")]
public class TrackGroupSO : TrackSO
{
    [Header("SFX Group Settings (Ignore Above Settings): ")]
    [SerializeField] private List<TrackSO> m_TrackGroup = new List<TrackSO>();
    public List<TrackSO> TrackGroup => m_TrackGroup;

    private int m_Index = 0;

    public override void PlayOneShot() {
        if (!AudioManager.Instance) return;
        if (!m_Source) {
            InitializeSource();
        }

        // Get Next SFX from TrackSFXs
        TrackSO track = GetNextTrack();
        m_Source.PlayOneShot(track.Clip);



        // Play that SFX
        //this.CancelAnyTweens();
        //this.FadeOut(m_TrackFadeOutTime, PlayShot);
    }

    private void PlayShot() {
        // Get Next SFX from TrackSFXs
        TrackSO track = GetNextTrack();

        m_Source.volume = this.m_Volume;
        m_Source.PlayOneShot(track.Clip);
        //this.FadeIn(this.m_TrackFadeInTime);
    }

    private TrackSO GetNextTrack() {
        if (m_TrackGroup.Count == 0) return null;
        if (m_TrackGroup.Count == 1) return m_TrackGroup[0];

        if (m_Index >= m_TrackGroup.Count) m_Index = 0;

        TrackSO nextTrack = m_TrackGroup[m_Index];
        m_Index++;

        return nextTrack;
    }
}

