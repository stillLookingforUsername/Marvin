using UnityEngine;
//using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _locked;
    private Animator _anim;
    [SerializeField] private GameObject _player;
    public GameObject codeUIPanel;

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
            codeUIPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            //close the door
            _anim.SetTrigger("closeAnimatorParam");
            _locked = true;
        }
    }
}
