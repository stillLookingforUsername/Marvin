using UnityEngine;

public class ScriptCode : MonoBehaviour
{
    public GameObject codeDisplayPanel;
    public float duration = 2f;
    private AudioSource audioSource;

    [Header("Code Configuration")]
    [SerializeField] private LevelCodeConfig _config;
    [SerializeField] private string _doorID = "";
    [SerializeField] private string _fallbackCode = "1122";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateCodeDisplay();
    }

    private void UpdateCodeDisplay()
    {
        if (codeDisplayPanel == null) return;
        string codeToShow = _fallbackCode;

        //try to get code from config
        if (_config != null)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (_config.TryGetCode(sceneName, _doorID, out string configCode, out int length))
            {
                codeToShow = configCode;
                Debug.Log($"using code '{codeToShow}' from config '{sceneName}',door {_doorID}'");
            }
            else
            {
                Debug.LogWarning("No config found");
            }
        }
        //Update the display panel with correct code
        UpdateDisplayPanel(codeToShow);
    }

    private void UpdateDisplayPanel(string code)
    {
        //Find TMPro components in the CodeDisplayPanel and update them
        TMPro.TextMeshProUGUI[] textComponents = codeDisplayPanel.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach(var textComponent in textComponents)
        {
            //look for text that might contain the code(like 1122)
            if(textComponent.text.Contains("1122") || textComponent.text.Contains("Code"))
            {
                textComponent.text = textComponent.text.Replace("1122", code);
                textComponent.text = textComponent.text.Replace("Code: 1122", $"Code: {code}");
                Debug.Log($"Updated display text to : {textComponent.text}");
            }
        }
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
