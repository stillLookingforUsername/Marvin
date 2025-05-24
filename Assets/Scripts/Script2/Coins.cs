using System;
using UnityEngine;

public class Coins : MonoBehaviour, InterfaceCollector
{
    public static event Action<int> OnCoinCollect;
    public int worth = 5;
    public void Collect()
    {
        OnCoinCollect.Invoke(worth);
        Destroy(gameObject);
    }
}
