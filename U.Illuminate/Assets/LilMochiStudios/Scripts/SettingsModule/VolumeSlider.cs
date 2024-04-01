using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string volumeParameter = "MasterVolume";
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle volumeToggle;
    [SerializeField] private float multiplier = 40f;
    private bool disableToggleEvent;
    private bool disableSliderEvent;


    private void Reset() {
        if (!volumeSlider) TryGetComponent<Slider>(out volumeSlider);
        if (!volumeToggle) TryGetComponent<Toggle>(out volumeToggle);
    }
    private void OnEnable() {
        SettingsHandler.OnSettingsResetToDefault += OnSettingsUpdated;
        volumeSlider.onValueChanged.AddListener(val => HandleSliderValueChanged(val));
        volumeToggle.onValueChanged.AddListener(val => HandleToggleValueChanged(val));
        volumeSlider.value = GetVolume();   // Load the volume from the save data
    }
    private void OnDisable() {
        SettingsHandler.OnSettingsResetToDefault -= OnSettingsUpdated;
        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeToggle.onValueChanged.RemoveAllListeners();
        SaveVolume();   // Save volume when player closes page (for redundancy)
    }

    private void OnSettingsUpdated() {
        if (disableSliderEvent) return;

        if (!disableToggleEvent) volumeSlider.value = GetVolume();   // Only set the slider.value if the OnSettingsUpdate didnt come from the SaveVolume in the HandleSliderValueChanged function
        mixer.SetFloat(volumeParameter, (Mathf.Log10(GetVolume()) * multiplier) + 5);   // Set volume based on the saved volume
    }

    private void HandleSliderValueChanged(float _value) {
        mixer.SetFloat(volumeParameter, (Mathf.Log10(_value) * multiplier) + 5);

        if (disableSliderEvent) return; // Stops this HandleSliderValueChanged function from setting toggle stuff when the toggle caused the change

        disableToggleEvent = true;
        SaveVolume();
        volumeToggle.isOn = volumeSlider.value <= volumeSlider.minValue;
        disableToggleEvent = false;
    }
    private void HandleToggleValueChanged(bool _disableSound) {
        // Stops the HandleToggleValueChanged function from running while the volume is actively updating, this prevents the toggle changed code from
        // running if the volume slider is set to 0. This is because the toggle changed code would try to change the volume slider value.
        if (disableToggleEvent) return; 

        disableSliderEvent = true;
        if (_disableSound) {
            SaveVolume();   // Saves the volume (which uses the slider.value) before setting the slider.value to 0 so that, when the toggle is unchecked it will set slider.value to what it was before.
            volumeSlider.value = volumeSlider.minValue;
        } else {
            volumeSlider.value = GetVolume();
        }
        disableSliderEvent = false;
    }

    private void SaveVolume() {
        SettingsHandler.SetAudioValue(volumeParameter, volumeSlider.value);
    }
    private float GetVolume() {
        return SettingsHandler.GetAudioValue(volumeParameter);
    }
}
