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

    [Header("Death & Respawn Settings")]
    [SerializeField] private float _deathDelay = 1f;
    [SerializeField] private bool _reloadSceneOnDeath = true;
    [SerializeField] private bool _enableDebugDeathLogs = true;
    
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
        SceneManager.sceneLoaded += OnSceneLoaded;
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

        // Unsubscribe from scene loaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from scene loaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneIndex = scene.buildIndex;
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: Scene loaded - {scene.name} (index: {_currentSceneIndex})");
        }
    }
    /*
    private void RespawnPlayer()
    {
        StartCoroutine(RespawnAfterDelay(1f));
    }
    */
    private void RespawnPlayer()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (_enableDebugDeathLogs)
        {
            Debug.Log($"GameManager: Player died in scene {currentScene} ({sceneName})");
        }
        
        // Check if we should reload the scene or use checkpoint respawn
        if (_reloadSceneOnDeath && ShouldReloadSceneOnDeath(currentScene))
        {
            if (_enableDebugDeathLogs)
            {
                Debug.Log($"GameManager: Reloading scene {sceneName} due to player death");
            }
            StartCoroutine(ReloadSceneAfterDelay(_deathDelay));
        }
        else
        {
            if (_enableDebugDeathLogs)
            {
                Debug.Log($"GameManager: Using checkpoint respawn in scene {sceneName}");
            }
            StartCoroutine(RespawnAfterDelay(_deathDelay));
        }
    }
    
    /// <summary>
    /// Determines if the scene should be reloaded when player dies
    /// </summary>
    private bool ShouldReloadSceneOnDeath(int sceneIndex)
    {
        // Reload scene for Level-1 (index 1) and Level-2 (index 2)
        // Keep checkpoint respawn for Home (index 0) and GameOverScene (index 3)
        return sceneIndex == 1 || sceneIndex == 2;
    }
    
    /// <summary>
    /// Reloads the current scene after a delay
    /// </summary>
    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        if (_enableDebugDeathLogs)
        {
            Debug.Log($"GameManager: Reloading scene in {delay} seconds...");
        }
        
        // Show death UI or effects here if you have them
        // You can add death screen, fade out, etc.
        
        yield return new WaitForSeconds(delay);
        
        // Reload the current scene
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
        
        if (_enableDebugDeathLogs)
        {
            Debug.Log($"GameManager: Scene {currentScene} reloaded successfully");
        }
    }
    
    /// <summary>
    /// Original respawn method for checkpoint-based respawning
    /// </summary>
    
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

        if(_enableDebugDeathLogs)
        {
            Debug.Log("GameManager: Player respawn at checkpoint");
        }
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

        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: LoadNextScene called. current: {_currentSceneIndex}");
        }

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
    /// Loads Level-2 specifically (for Level-1 to Level-2 transition)
    /// </summary>
    public void LoadLevel2()
    {
        if (_debugSceneManagement)
        {
            Debug.Log("GameManager: Loading Level-2 specifically");
        }
        LoadScene(2); // Level-2 scene index
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
        if (_debugSceneManagement)
        {
            Debug.Log($"GameManager: About to call LoadNextScene from TransitionToNextScene. Current scene index: {_currentSceneIndex}");
        }
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
            //Debug.Log($"GameManager: Scene {sceneIndex} loaded successfully");
            Debug.Log($"GameManager: Scene {sceneIndex} loaded successfully. Updated current scene index to: {_currentSceneIndex}");
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
    /// <summary>
    /// Test method to manually trigger player death (for debugging)
    /// </summary>
    [ContextMenu("Test Player Death")]
    public void TestPlayerDeath()
    {
        Debug.Log("GameManager: TestPlayerDeath called");
        RespawnPlayer();
    }
    
    /// <summary>
    /// Test method to manually reload current scene (for debugging)
    /// </summary>
    [ContextMenu("Test Reload Current Scene")]
    public void TestReloadCurrentScene()
    {
        Debug.Log("GameManager: TestReloadCurrentScene called");
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
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