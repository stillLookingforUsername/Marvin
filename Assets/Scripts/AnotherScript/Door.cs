using UnityEngine;
//using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _locked;
    private Animator _anim;
    [SerializeField] private GameObject _player;
    public GameObject codeUIPanel;

    [Header("Scene Management")]
    [SerializeField] private bool _useGameManagerSceneTransition = true;
    [SerializeField] private int _targetSceneIndex = -1;    //-1 means next scene, otherwise spedific scene
    [SerializeField] private bool _requireCodePanel = false;
    [SerializeField] private string _doorName = "Door"; //for debugging

    private bool _keyReached = false;
    private bool _sceneTransitionTriggered = false;


    private void Start()
    {
        _locked = true;
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        /*
        float distance = Vector2.Distance(_player.transform.position, transform.position);
        if (!_locked && distance < 0.5f)
        {
            Debug.Log("door touched");
            SceneManager.LoadScene(0);
        }
        */
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            //open the door
            _anim.SetTrigger("openAnimatorParam");
            _locked = false;
            _keyReached = true;

            //Show code panel if required
            if (_requireCodePanel && codeUIPanel != null)
            {
                codeUIPanel.SetActive(true);
            }

            //otherwise, trigger scene transition immediately

            else if (_useGameManagerSceneTransition && !_sceneTransitionTriggered)
            {
                _sceneTransitionTriggered = true;
                TriggerSceneTransition();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            //close the door
            _anim.SetTrigger("closeAnimatorParam");
            _locked = true;
            _keyReached = false;
            _sceneTransitionTriggered = false;
        }
    }

    /// <summary>
    /// Called by the code panel when correct code is entered
    /// </summary>
    public void OnCorrectCodeEntered()
    {
        if (_keyReached && _useGameManagerSceneTransition && !_sceneTransitionTriggered)
        {
            _sceneTransitionTriggered = true;
            TriggerSceneTransition();
        }
    }
    
    /// <summary>
    /// Triggers the scene transition through GameManager
    /// </summary>
    private void TriggerSceneTransition()
    {
        if (GameManager.instance != null)
        {
            if (_targetSceneIndex >= 0)
            {
                // Load specific scene
                GameManager.instance.LoadScene(_targetSceneIndex);
            }
            else
            {
                // Load next scene
                GameManager.instance.OnKeyReachedDoor(_doorName);
            }
        }
        else
        {
            Debug.LogError("GameManager instance not found! Scene transition failed.");
        }
    }
    
    /// <summary>
    /// Public method to manually trigger scene transition (for testing)
    /// </summary>
    [ContextMenu("Test Scene Transition")]
    public void TestSceneTransition()
    {
        TriggerSceneTransition();
    }
}
