using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New SettingsSO", menuName = "Settings/Create SettingsSO")]
public class SettingsSO : ScriptableObject {
    [Header("Audio Settings")]
    public float DefaultVolume = 0.7f;
}

