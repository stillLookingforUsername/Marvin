using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMasterVolume(float level)
    {
        //_audioMixer.SetFloat("masterVolume", level);
        _audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }
    public void SetSoundFXVolume(float level)
    {
        _audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
    }
    public void SetBGMusicVolume(float level)
    {
        _audioMixer.SetFloat("bgMusicVolume", Mathf.Log10(level) * 20f);
    }
}

