using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayTrackPlaylist : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private string m_PlaylistName;

    private void Start() {
        AudioManager.Instance.PlayPlaylist(m_PlaylistName);
    }
}

