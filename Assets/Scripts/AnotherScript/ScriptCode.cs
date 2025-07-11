using UnityEngine;

public class ScriptCode : MonoBehaviour
{
    public GameObject codeDisplayPanel;
    public float duration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            codeDisplayPanel.SetActive(true);
            Invoke("HideCodeDisplay", duration);
            gameObject.SetActive(false);
        }
    }
    private void HideCodeDisplay()
    {
        codeDisplayPanel.SetActive(false);
    }


}
