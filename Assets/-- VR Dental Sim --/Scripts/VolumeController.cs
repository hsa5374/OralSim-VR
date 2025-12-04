using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the UI Slider
    public Toggle turnToggle;

    private const string VolumeKey = "GameVolume"; // Key for PlayerPrefs

    void OnEnable()
    {
        // Load the saved volume from PlayerPrefs, default to 1 (max volume) if not found
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);




        // Initialize the toggle state based on the PlayerPrefs value
        int currentValue = PlayerPrefs.GetInt("IsSnap", 1); // Default to 0 if the key doesn't exist
        turnToggle.isOn = currentValue == 1;

        // Add a listener to the toggle's onValueChanged event
        turnToggle.onValueChanged.AddListener(OnToggleValueChanged);


        // Set the slider value to the saved volume
        volumeSlider.value = savedVolume;

        // Apply the volume to the AudioListener
        AudioListener.volume = savedVolume;

        // Add a listener to the slider to update the volume when the slider is moved
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float volume)
    {
        // Update the volume of the game
        //AudioListener.volume = volume;

        // Save the new volume to PlayerPrefs
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save(); // Save the changes to disk
    }


    private void OnToggleValueChanged(bool isOn)
    {
        // Set PlayerPrefs value based on the toggle state
        int newValue = isOn ? 1 : 0;
        PlayerPrefs.SetInt("IsSnap", newValue);

        // Save the PlayerPrefs
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs value set to: " + newValue);
    }
}