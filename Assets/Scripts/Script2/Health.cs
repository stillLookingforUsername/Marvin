using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public Image heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private List<Image> listOfHearts = new List<Image>();

    public void SetMaxHearts(int maxHearts)
    {
        foreach (Image heart in listOfHearts)
        {
            Destroy(heart);
        }
        listOfHearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, transform);
            newHeart.sprite = fullHeartSprite;
            newHeart.color = Color.red;
            listOfHearts.Add(newHeart);
        }

    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < listOfHearts.Count; i++)
        {
            if (i < currentHealth)
            {
                listOfHearts[i].sprite = fullHeartSprite;
                listOfHearts[i].color = Color.red;
            }
            else
            {
                listOfHearts[i].sprite = emptyHeartSprite;
                listOfHearts[i].color = Color.white;
            }
        }
    }
}
