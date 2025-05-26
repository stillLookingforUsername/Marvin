using UnityEngine;

public class GrassInterationController : MonoBehaviour
{
    [Range(0f, 20f)] public float externalInfluenceStrength = 10f;
    public float easeInTime = 0.15f;
    public float easeOutTime = 0.15f;
    public float velocityThreshold = 5f;

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    //function where we pass the material & player's velocity
    //& set the externalInfluence of the material base on the player's velocity
    public void InfluenceGrass(Material mat,float XVelocity)
    {
        mat.SetFloat(_externalInfluence, XVelocity);
    }
}
