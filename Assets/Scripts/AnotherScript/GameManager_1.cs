using TMPro;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;



    private void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            if (_menuCanvas != null)
            {
                bool isActive = _menuCanvas.activeSelf;
                _menuCanvas.SetActive(!isActive);
                Time.timeScale = _menuCanvas.activeSelf ? 0f : 1f;
            }
            else
            {
                Debug.Log("Menu Canvas is not assigned");
            }
        }
    }
}
