using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    [Header("Mobile UI Controls")]
    [SerializeField] private GameObject _mobileUICanvas;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Button _jumpButton;
    [SerializeField] private Button _dashButton;
    [SerializeField] private Button _wallGripButton;
    [SerializeField] private Button _upButton;
    
    [Header("UI Settings")]
    [SerializeField] private bool _showUIOnMobile = true;
    [SerializeField] private bool _showUIOnDesktop = false;
    [SerializeField] private bool _autoDetectPlatform = true;
    [SerializeField] private bool _autoFindUIElements = true;
    
    private void Awake()
    {
        // Singleton pattern - persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to scene loading events
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Auto-detect platform and show/hide UI accordingly
            if (_autoDetectPlatform)
            {
                bool isMobile = Application.isMobilePlatform;
                SetUIVisibility(isMobile ? _showUIOnMobile : _showUIOnDesktop);
            }
            
            SetupButtonListeners();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
        private void OnDestroy()
        {
            // Unsubscribe from scene loading events
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        /// <summary>
        /// Called when a new scene is loaded
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_autoFindUIElements)
            {
                FindUIElementsInScene();
                SetupButtonListeners();
            }
        }
        
        /// <summary>
        /// Automatically finds UI elements in the current scene
        /// </summary>
        private void FindUIElementsInScene()
        {
            // Find the mobile UI canvas
            if (_mobileUICanvas == null)
            {
                _mobileUICanvas = GameObject.Find("MobileUICanvas") ?? GameObject.Find("Canvas");
            }
            
            // Find buttons by name or tag
            if (_leftButton == null)
            {
                _leftButton = FindButtonByName("LeftButton") ?? FindButtonByName("Akey") ?? FindButtonByName("A");
            }
            
            if (_rightButton == null)
            {
                _rightButton = FindButtonByName("RightButton") ?? FindButtonByName("Dkey") ?? FindButtonByName("D");
            }
            
            if (_jumpButton == null)
            {
                _jumpButton = FindButtonByName("JumpButton") ?? FindButtonByName("SpaceButton") ?? FindButtonByName("Jump");
            }
            
            if (_dashButton == null)
            {
                _dashButton = FindButtonByName("DashButton") ?? FindButtonByName("DashKey") ?? FindButtonByName("Dash");
            }
            
            if (_wallGripButton == null)
            {
                _wallGripButton = FindButtonByName("WallGripButton") ?? FindButtonByName("Jkey") ?? FindButtonByName("J");
            }
            
            if (_upButton == null)
            {
                _upButton = FindButtonByName("UpButton") ?? FindButtonByName("Wkey") ?? FindButtonByName("W");
            }
            
            Debug.Log($"UIManager: Found UI elements in scene {SceneManager.GetActiveScene().name}. Canvas: {_mobileUICanvas != null}, Buttons: {(_leftButton != null ? "Left " : "")}{(_rightButton != null ? "Right " : "")}{(_jumpButton != null ? "Jump " : "")}{(_dashButton != null ? "Dash " : "")}{(_wallGripButton != null ? "WallGrip " : "")}{(_upButton != null ? "Up " : "")}");
        }
        
        /// <summary>
        /// Finds a button by name in the current scene
        /// </summary>
        private Button FindButtonByName(string buttonName)
        {
            // First try to find by exact name
            GameObject buttonObj = GameObject.Find(buttonName);
            if (buttonObj != null)
            {
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    Debug.Log($"UIManager: Found button '{buttonName}' by exact name");
                    return button;
                }
            }
            
            // If not found, search in all canvases
            //Canvas[] canvases = FindObjectsOfType<Canvas>();
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    if (button.gameObject.name == buttonName)
                    {
                        Debug.Log($"UIManager: Found button '{buttonName}' in canvas '{canvas.name}'");
                        return button;
                    }
                }
            }
            
            Debug.LogWarning($"UIManager: Button '{buttonName}' not found in scene");
            return null;
        }
    
    private void SetupButtonListeners()
    {
        Debug.Log("UIManager: Setting up button listeners...");
        
        // Check if InputManager is available
        if (InputManager.Instance == null)
        {
            Debug.LogError("UIManager: InputManager.Instance is null! Cannot setup button listeners.");
            return;
        }
        
        // Set up button press/release events for InputManager
        if (_leftButton != null)
        {
            var leftPointer = _leftButton.gameObject.GetComponent<UIButtonHandler>();
            if (leftPointer == null)
            {
                leftPointer = _leftButton.gameObject.AddComponent<UIButtonHandler>();
                Debug.Log("UIManager: Added UIButtonHandler to Left button");
            }
            leftPointer.Setup(() => InputManager.Instance?.OnMoveLeftDown(), () => InputManager.Instance?.OnMoveLeftUp());
        }
        else
        {
            Debug.LogWarning("UIManager: Left button not found!");
        }
        
        if (_rightButton != null)
        {
            var rightPointer = _rightButton.gameObject.GetComponent<UIButtonHandler>();
            if (rightPointer == null)
            {
                rightPointer = _rightButton.gameObject.AddComponent<UIButtonHandler>();
                Debug.Log("UIManager: Added UIButtonHandler to Right button");
            }
            rightPointer.Setup(() => InputManager.Instance?.OnMoveRightDown(), () => InputManager.Instance?.OnMoveRightUp());
        }
        else
        {
            Debug.LogWarning("UIManager: Right button not found!");
        }
        
        if (_jumpButton != null)
        {
            var jumpPointer = _jumpButton.gameObject.GetComponent<UIButtonHandler>();
            if (jumpPointer == null)
            {
                jumpPointer = _jumpButton.gameObject.AddComponent<UIButtonHandler>();
                Debug.Log("UIManager: Added UIButtonHandler to Jump button");
            }
            else
            {
                Debug.Log("UIManager: Jump button already has UIButtonHandler");
            }
            jumpPointer.Setup(() => InputManager.Instance?.OnJumpDown(), () => InputManager.Instance?.OnJumpUp());
        }
        else
        {
            Debug.LogWarning("UIManager: Jump button not found!");
        }
        
        if (_dashButton != null)
        {
            var dashPointer = _dashButton.gameObject.GetComponent<UIButtonHandler>();
            if (dashPointer == null)
            {
                dashPointer = _dashButton.gameObject.AddComponent<UIButtonHandler>();
                Debug.Log("UIManager: Added UIButtonHandler to Dash button");
            }
            dashPointer.Setup(() => InputManager.Instance?.OnDashDown(), () => InputManager.Instance?.OnDashUp());
        }
        else
        {
            Debug.LogWarning("UIManager: Dash button not found!");
        }
        
        if (_wallGripButton != null)
        {
            var wallGripPointer = _wallGripButton.gameObject.GetComponent<UIButtonHandler>();
            if (wallGripPointer == null)
            {
                wallGripPointer = _wallGripButton.gameObject.AddComponent<UIButtonHandler>();
                Debug.Log("UIManager: Added UIButtonHandler to WallGrip button");
            }
            wallGripPointer.Setup(() => InputManager.Instance?.OnWallGripDown(), () => InputManager.Instance?.OnWallGripUp());
        }
        else
        {
            Debug.LogWarning("UIManager: WallGrip button not found!");
        }
        
        if (_upButton != null)
        {
            var upPointer = _upButton.gameObject.GetComponent<UIButtonHandler>();
            if (upPointer == null)
            {
                upPointer = _upButton.gameObject.AddComponent<UIButtonHandler>();
                Debug.Log("UIManager: Added UIButtonHandler to Up button");
            }
            upPointer.Setup(() => InputManager.Instance?.OnMoveUpPressedDown(), () => InputManager.Instance?.OnMoveUpPressedUp());
        }
        else
        {
            Debug.LogWarning("UIManager: Up button not found!");
        }
        
        Debug.Log("UIManager: Button listeners setup complete!");
    }
    
    /// <summary>
    /// Shows or hides the mobile UI
    /// </summary>
    public void SetUIVisibility(bool visible)
    {
        if (_mobileUICanvas != null)
        {
            _mobileUICanvas.SetActive(visible);
        }
    }
    
    /// <summary>
    /// Toggle UI visibility
    /// </summary>
    public void ToggleUI()
    {
        if (_mobileUICanvas != null)
        {
            _mobileUICanvas.SetActive(!_mobileUICanvas.activeSelf);
        }
    }
    
    /// <summary>
    /// Check if UI is currently visible
    /// </summary>
    public bool IsUIVisible()
    {
        return _mobileUICanvas != null && _mobileUICanvas.activeSelf;
    }
    
    /// <summary>
    /// Manually refresh UI elements (for debugging)
    /// </summary>
    [ContextMenu("Refresh UI Elements")]
    public void RefreshUIElements()
    {
        FindUIElementsInScene();
        SetupButtonListeners();
    }
}