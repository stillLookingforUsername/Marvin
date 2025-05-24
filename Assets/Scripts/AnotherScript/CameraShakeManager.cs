using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;

    public float shakeForce = 2f;
    public CinemachineImpulseListener impulseListener;
    public CinemachineImpulseDefinition impulseDefination;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(shakeForce);
    }

    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        //apply Setting
        SetupScreenShakeSettings(profile,impulseSource);    
        //screenShake
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }
    public void SetupScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        //impulseSource.ImpulseDefinition.ImpulseDuration = 3;

        impulseDefination = impulseSource.ImpulseDefinition;


        //change the impulse source setting
        impulseDefination.ImpulseDuration = profile.impactDuration;
        impulseSource.DefaultVelocity = profile.defaultVelocity;    //refer 1-Page
        impulseDefination.CustomImpulseShape = profile.impulseCurve;

        //change the impulse listener settin
        impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
        impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
        impulseListener.ReactionSettings.Duration = profile.listenerDuration;
    }
}
