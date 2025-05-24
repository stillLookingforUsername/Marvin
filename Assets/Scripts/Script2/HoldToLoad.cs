using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class HoldToLoad : MonoBehaviour
{
    public LoadLevelCanvas data;
    public Image fillCircle;

    private float _holdTimer = 0;
    private bool _isHolding = false;


    public static event Action OnHoldComplete;

    private void Update()
    {
        if (_isHolding)
        {
            _holdTimer += Time.deltaTime;
            fillCircle.fillAmount = _holdTimer / data.holdDuration;

            if (_holdTimer > data.holdDuration)
            {
                OnHoldComplete?.Invoke();
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isHolding = true;
        }
        else if(context.canceled)
        {
            ResetHold();
        }
    }
    private void ResetHold()
    {
        _isHolding = false;
        _holdTimer = 0;
        fillCircle.fillAmount = 0;
    }
}
