using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float _starPos, length;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        _starPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(_starPos + distance ,transform.position.y, transform.position.z);

        if (movement > _starPos + length)
        {
            _starPos += length;
        }
        else if (movement < _starPos - length)
        {
            _starPos -= length;
        }
    }
}
