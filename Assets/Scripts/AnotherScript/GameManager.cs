using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject player;
    private Vector3 _spawnPosition;
    private CinemachineCamera _cinemachineCamera;
    
    [Header("Scene Management")]
    [SerializeField] private bool _enableSceneManagement = true;
    [SerializeField] private float _sceneTransitionDelay = 1f;
    [SerializeField] private bool _showTransitionUI = true;
    [SerializeField] private GameObject _transitionUI;
    
    [Header("Debug")]
    [SerializeField] private bool _debugCameraRotation = false;
    [SerializeField] private bool _debugSceneManagement = false;
    
    // Scene progression tracking
    private int _currentSceneIndex;
    private bool _isTransitioning = false;
    
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
        
        // Initialize scene management
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Initialized scene management. Current scene index: {_currentSceneIndex}");
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

    #region Scene Management
    
    /// <summary>
    /// Called when a key reaches a door. Handles scene transition.
    /// </summary>
    /// <param name="doorName">Name of the door for debugging</param>
    public void OnKeyReachedDoor(string doorName = "Unknown")
    {
        if (!_enableSceneManagement || _isTransitioning)
        {
            if (_debugSceneManagement)
            {
                Debug.Log($"GameManager: Scene transition blocked. Enabled: {_enableSceneManagement}, Transitioning: {_isTransitioning}");
            }
            return;
        }
        
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Key reached door '{doorName}'. Starting scene transition...");
        }
        
        StartCoroutine(TransitionToNextScene());
    }
    
    /// <summary>
    /// Loads a specific scene by build index
    /// </summary>
    /// <param name="sceneIndex">Build index of the scene to load</param>
    public void LoadScene(int sceneIndex)
    {
        if (_isTransitioning) return;
        
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Loading scene with build index: {sceneIndex}");
        }
        
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }
    
    /// <summary>
    /// Loads the next scene in build order
    /// </summary>
    public void LoadNextScene()
    {
        int nextSceneIndex = _currentSceneIndex + 1;
        
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            if (_debugSceneManagement)
            {
                Debug.Log("GameManager: No more scenes in build order. Restarting from scene 0.");
            }
            nextSceneIndex = 0;
        }
        
        LoadScene(nextSceneIndex);
    }
    
    /// <summary>
    /// Restarts the current scene
    /// </summary>
    public void RestartCurrentScene()
    {
        LoadScene(_currentSceneIndex);
    }
    
    private IEnumerator TransitionToNextScene()
    {
        _isTransitioning = true;
        
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Starting transition from scene {_currentSceneIndex}");
        }
        
        // Show transition UI if enabled
        if (_showTransitionUI && _transitionUI != null)
        {
            _transitionUI.SetActive(true);
        }
        
        // Wait for transition delay
        yield return new WaitForSeconds(_sceneTransitionDelay);
        
        // Load next scene
        LoadNextScene();
    }
    
    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        _isTransitioning = true;
        
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Loading scene {sceneIndex}");
        }
        
        // Show transition UI if enabled
        if (_showTransitionUI && _transitionUI != null)
        {
            _transitionUI.SetActive(true);
        }
        
        // Wait for transition delay
        yield return new WaitForSeconds(_sceneTransitionDelay);
        
        // Load the scene
        SceneManager.LoadScene(sceneIndex);
        
        // Update current scene index
        _currentSceneIndex = sceneIndex;
        
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Scene {sceneIndex} loaded successfully");
        }
        
        _isTransitioning = false;
    }
    
    /// <summary>
    /// Gets the current scene index
    /// </summary>
    public int GetCurrentSceneIndex()
    {
        return _currentSceneIndex;
    }
    
    /// <summary>
    /// Checks if a scene transition is currently in progress
    /// </summary>
    public bool IsTransitioning()
    {
        return _isTransitioning;
    }
    
    #endregion

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