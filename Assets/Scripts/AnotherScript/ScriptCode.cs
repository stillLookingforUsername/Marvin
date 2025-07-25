using UnityEngine;

public class ScriptCode : MonoBehaviour
{
    public GameObject codeDisplayPanel;
    public float duration = 2f;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
            codeDisplayPanel.SetActive(true);
            Invoke("HideCodeDisplay", duration);
            //gameObject.SetActive(false);
            Invoke("DeactivateSelf", audioSource.clip.length);
        }
    }
    private void HideCodeDisplay()
    {
        codeDisplayPanel.SetActive(false);
    }
    private void DeactivateSelf()
    {
        gameObject.SetActive(false);
    }


}
