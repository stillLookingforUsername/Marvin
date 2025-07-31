using System;
using UnityEngine;

public enum SoundType
{
    COIN,
    JUMP,
    WALLCLIMB,
    RUNNING,
    WALLGRIB
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    //[SerializeField] private AudioClip[] soundList;

    //use it to play random sound frm a list of sounds (eg: coinSound1,coinSound2,etc)
    [SerializeField] private SoundList[] soundList;

    private AudioSource audioSource;

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));  // 124 - returns a string array (string[]) containing the names of all the constants (fields) in the enum SoundTypreturns a string array[] containing the names
        Array.Resize(ref soundList, names.Length);   //125

        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);

        //instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

}
[Serializable]
public struct SoundList
{
    public AudioClip[] sounds { get{ return audioClip; }}
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] audioClip;
}
