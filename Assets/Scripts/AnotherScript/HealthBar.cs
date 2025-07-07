using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f;

    private void Start()
    {
        // If playerHealth is not assigned, try to find it
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            // Subscribe to health changed event
            playerHealth.onHealthChanged.AddListener(UpdateHealthBar);
        }
        else
        {
            Debug.LogError("PlayerHealth not found!");
        }
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        fillImage.fillAmount = healthPercentage;

        // Interpolate color between low health and full health colors
        if (healthPercentage <= lowHealthThreshold)
        {
            fillImage.color = lowHealthColor;
        }
        else
        {
            float t = (healthPercentage - lowHealthThreshold) / (1 - lowHealthThreshold);
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, t);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
} 