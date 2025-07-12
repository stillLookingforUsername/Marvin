using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public Upgrade[] upgrades;
    
    private int coins;

    //References
    public TextMeshProUGUI coinText;
    public GameObject shopUI;
    public Transform shopContent;
    public GameObject itemPrefab;
    public PlayerHealth playerHealth;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //No to destroy the shop manager when the scene is changed
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        // Load coins from PlayerPrefs (saved high score)
        LoadCoins();

        foreach(Upgrade upgrade in upgrades)
        {
            GameObject item = Instantiate(itemPrefab, shopContent);

            upgrade.itemRef = item;

            foreach(Transform child in item.transform)
            {
                if(child.gameObject.name == "Quantity")
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = upgrade.quantity.ToString();
                }
                else if(child.gameObject.name == "Cost")
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = upgrade.cost.ToString();
                }
                else if(child.gameObject.name == "Name")
                {
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = upgrade.name;
                }
                else if(child.gameObject.name == "Image")
                {
                    child.gameObject.GetComponent<Image>().sprite = upgrade.image;
                }
            }
            item.GetComponent<Button>().onClick.AddListener(() => Buy(upgrade));
        }

        // Update UI with initial coin count
        UpdateCoinText();
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("HighScore", coins);
        PlayerPrefs.Save();

        // If Score instance exists, update its display
        if (Score.Instance != null)
        {
            Score.Instance.UpdateHighScoreFromShop(coins);
        }
    }

    public void Buy(Upgrade upgrade)
    {
        if(coins >= upgrade.cost)
        {
            coins -= upgrade.cost;
            upgrade.quantity++;
            upgrade.itemRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgrade.quantity.ToString();

            // Save coins after purchase
            SaveCoins();
            // Update UI
            UpdateCoinText();

            ApplyUpgrade(upgrade);
        }
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins.ToString();
        }
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth reference is missing in ShopManager!");
            return;
        }

        switch(upgrade.name)
        {
            case "Health":
                playerHealth.Heal(10f);
                break;
            default:
                Debug.Log("Upgrade not found");
                break;

                /*
            case "Speed":
                playerMovement.speed += 1;
                break;
            case "Jump":
            */
        }
    }

    public void ToggleShop()
    {
        // Refresh coin count when shop is opened
        if (!shopUI.activeSelf)
        {
            LoadCoins();
            UpdateCoinText();
        }
        shopUI.SetActive(!shopUI.activeSelf);
    }
}

[System.Serializable]
public class Upgrade
{
    public string name;
    public int cost;
    public Sprite image;
    [HideInInspector] public int quantity;
    [HideInInspector] public GameObject itemRef;
}
