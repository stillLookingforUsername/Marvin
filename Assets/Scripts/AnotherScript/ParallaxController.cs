using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private  float _startPos, length; //store the initial position 
    public GameObject cam;
    public float parallaxEffect; //The speed at which the BG shoudl move relative to the camera


    void Start()
    {
        //save the bg starting horizontal position for later reference
        _startPos = transform.position.x; //we can set it to y if we want to move vertically
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        //calculate distance BG move based on cam movement
        float distance = cam.transform.position.x * parallaxEffect; //0 = move with cam & 1 = doesn't move at all & 0.5 = move half way with cam
        float movement = cam.transform.position.x * (1 - parallaxEffect);   //how far the camera has moved

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);

        if (movement > _startPos + length)
        {
            _startPos += length;
        }
        else if (movement < _startPos - length)
        {
            _startPos -= length;
        }

    }

}
