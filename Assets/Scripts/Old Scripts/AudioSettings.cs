using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            LoadVolume();
        }
        else
        {
            BGMSlider.value = 0.75f;
            SFXSlider.value = 0.75f;
            SetBGMVolume();
            SetSFXVolume();
        }

        BGMSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    public void SetBGMVolume()
    {
        float volume = BGMSlider.value;

        if (volume <= 0.0001f)
        {
            myMixer.SetFloat("BGM", -80f);
        }
        else
        {
            myMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;

        if (volume <= 0.0001f)
        {
            myMixer.SetFloat("SFX", -80f);
        }
        else
        {
            myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetBGMVolume();
        SetSFXVolume();
    }
}