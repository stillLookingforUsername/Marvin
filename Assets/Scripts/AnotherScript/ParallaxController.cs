using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private  float _startPos; //store the initial position 
    public GameObject cam;
    public float parallaxEffect; //The speed at which the BG shoudl move relative to the camera


    void Start()
    {
        //save the bg starting horizontal position for later reference
        _startPos = transform.position.x; //we can set it to y if we want to move vertically
    }

    void FixedUpdate()
    {
        //calculate distance BG move based on cam movement
        float distance = cam.transform.position.x * parallaxEffect; //0 = move with cam & 1 = doesn't move at all & 0.5 = move half way with cam

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);
    }

}
