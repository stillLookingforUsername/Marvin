using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeProfile", menuName = "Scriptable Objects/ScreenShakeProfile")]
public class ScreenShakeProfile : ScriptableObject
{
    [Header("Impulse Source Setting")]
    public float impactDuration = 0.2f;
    public float impactForce = 1f;
    public Vector3 defaultVelocity = new Vector3(0.0f, 1f, 0.0f);
    public AnimationCurve impulseCurve;

    [Header("Impulse Listener Setting")]
    public float listenerAmplitude = 1f;
    public float listenerFrequency = 1f;
    public float listenerDuration = 1f;

}
