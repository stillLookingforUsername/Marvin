using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneTransitionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Text _loadingText;
    [SerializeField] private TMP_Text _loadingTextTMP;
    [SerializeField] private Slider _progressBar;
    
    [Header("Animation Settings")]
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.5f;
    [SerializeField] private string _loadingMessage = "Loading...";
    
    private void Awake()
    {
        // Get references if not assigned
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        
        if (_loadingText == null && _loadingTextTMP == null)
        {
            _loadingText = GetComponentInChildren<Text>();
            _loadingTextTMP = GetComponentInChildren<TMP_Text>();
        }
        
        if (_progressBar == null)
            _progressBar = GetComponentInChildren<Slider>();
        
        // Initially hide the UI
        gameObject.SetActive(false);
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }
    }
    
    /// <summary>
    /// Shows the transition UI with fade in animation
    /// </summary>
    public void ShowTransition()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }
    
    /// <summary>
    /// Hides the transition UI with fade out animation
    /// </summary>
    public void HideTransition()
    {
        StartCoroutine(FadeOut());
    }
    
    /// <summary>
    /// Updates the loading text
    /// </summary>
    /// <param name="text">New loading text</param>
    public void UpdateLoadingText(string text)
    {
        if (_loadingText != null)
            _loadingText.text = text;
        
        if (_loadingTextTMP != null)
            _loadingTextTMP.text = text;
    }
    
    /// <summary>
    /// Updates the progress bar
    /// </summary>
    /// <param name="progress">Progress value (0-1)</param>
    public void UpdateProgress(float progress)
    {
        if (_progressBar != null)
        {
            _progressBar.value = Mathf.Clamp01(progress);
        }
    }
    
    private System.Collections.IEnumerator FadeIn()
    {
        if (_canvasGroup == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < _fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeInDuration);
            yield return null;
        }
        
        _canvasGroup.alpha = 1f;
    }
    
    private System.Collections.IEnumerator FadeOut()
    {
        if (_canvasGroup == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < _fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeOutDuration);
            yield return null;
        }
        
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Shows a simple loading message
    /// </summary>
    public void ShowSimpleLoading()
    {
        UpdateLoadingText(_loadingMessage);
        ShowTransition();
    }
}