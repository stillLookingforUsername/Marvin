using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            //return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public int coins = 300;

    public Upgrade[] upgrades;

    //References
    public Text coinText;
    public GameObject shopUI;
    public Transform ShopContent;
    public GameObject itemPrefab;
    public PlayerMovement player;



    public void ToggleShop()
    {
        shopUI.SetActive(!shopUI.activeSelf);
    }

    private void OnGUI()
    {
        coinText.text = "Coins: " + coins.ToString();
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
