using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject player;
    private Vector3 _spawnPosition;
    private CinemachineCamera _cinemachineCamera;
    
    [Header("Debug")]
    [SerializeField] private bool _debugCameraRotation = false;
    
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
        //_spawnPosition = player.transform.position;
        
        // Find the Cinemachine camera (Cinemachine 3)
        _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        // Ensure rotation lock component exists on the Cinemachine camera root
        if (_cinemachineCamera != null)
        {
            var locker = _cinemachineCamera.gameObject.GetComponent<CameraRotationLocker>();
            if (locker == null)
            {
                locker = _cinemachineCamera.gameObject.AddComponent<CameraRotationLocker>();
            }
            locker.lockX = true;
            locker.lockY = true;
            locker.lockZ = true;
            locker.lockedEulerAngles = Vector3.zero;
        }

        if (_debugCameraRotation)
        {
            Debug.Log("GameManager: Found CinemachineCamera: " + (_cinemachineCamera != null));
        }
    }

    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;

    private void OnEnable()
    {
        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.onPlayerDeath.AddListener(RespawnPlayer);
        }
    }
    public void UpdateCheckPoint(Vector3 pos)
    {
        _spawnPosition = pos;
    }
    private void OnDisable()
    {
        if (player != null)
        {
            var healthh = player.GetComponent<PlayerHealth>();
            if (healthh != null)
            {
                healthh.onPlayerDeath.RemoveListener(RespawnPlayer);
            }
        }
    }
    private void RespawnPlayer()
    {
        StartCoroutine(RespawnAfterDelay(1f));
    }
    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.transform.position = _spawnPosition;
        player.transform.rotation = Quaternion.identity;
        
       
        var health = player.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.currentHealth = health.MaxHealth;
            health.onHealthChanged?.Invoke(1f);
        }
        player.SetActive(true);
    }

    
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