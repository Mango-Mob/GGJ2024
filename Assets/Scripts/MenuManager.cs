using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsGroup;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider soundEffectVolume;
    [SerializeField] private Slider musicVolume;

    // Start is called before the first frame update
    void Start()
    {
        settingsGroup.SetActive(false);

        masterVolume.value = 0.5f;
        soundEffectVolume.value = 0.5f;
        musicVolume.value = 0.5f;
    }

    public void SetMasterVolume(float _value)
    {
        AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MASTER] = _value;
    }
    public void SetSoundEffectVolume(float _value)
    {
        AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.SOUND_EFFECT] = _value;
    }
    public void SetMusicVolume(float _value)
    {
        AudioManager.Instance.volumes[(int)AudioManager.VolumeChannel.MUSIC] = _value;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.UP, 0))
        {
            PlayGame();
        }
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RIGHT, 0))
        {
            ToggleSettings();
        }
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.DOWN, 0))
        {
            ExitGame();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void ToggleSettings()
    {
        settingsGroup.SetActive(!settingsGroup.activeInHierarchy);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
