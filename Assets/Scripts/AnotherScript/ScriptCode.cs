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
        //if(collision.gameObject.CompareTag("Player"))
        if(collision.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            //Check if codeDisplayPanel is assigned
            if (codeDisplayPanel == null)
            {
                Debug.Log("CodeDisplay is not assigned");
                return;
            }
            //play audio if available
            if(audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
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
