using Unity.VisualScripting;
using UnityEngine;

//able to call it from anywhere very easyly it exist as global and it has only one instance of it 
public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource _soundFXObj;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            return;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform,float intensity)
    {
        //spawn object
        AudioSource audioSource = Instantiate(_soundFXObj, spawnTransform.position, Quaternion.identity);
        //assign the audio clip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = intensity;

        //play the sound
        audioSource.Play();

        //get the length of the sound FX clip
        float clipLength = audioSource.clip.length;

        //finally destroy this gameObject after certain amount of time
        Destroy(audioSource.gameObject, clipLength);
    }
}
