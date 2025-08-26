using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CodeCheckPanel : MonoBehaviour
{
    public TMP_Text codeDisplay;
    public GameObject codePanel;
    [SerializeField] private Door _associatedDoor; // Reference to the door this panel is for

    private string _correctCode = "1122";
    private string _currentCode = "";

    public void AddDigit(string digit)
    {
        if (_currentCode.Length < 4)
        {
            _currentCode += digit;
            UpdateDisplay();
        }
    }
    
    public void SubmitCode()
    {
        CodeCheck();
    }

    public void ClearInput()
    {
        _currentCode = "";
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        codeDisplay.text = _currentCode.PadRight(4,'_'); // e.g. "12_"
    }

    private void CodeCheck()
    {
        if (codeDisplay.text == _correctCode)
        {
            Debug.Log("Correct Code");

            //deactivate the panel
            codePanel.SetActive(false);

            // Call the door's scene transition method instead of directly loading scene
            if (_associatedDoor != null)
            {
                _associatedDoor.OnCorrectCodeEntered();
            }
            else
            {
                Debug.LogWarning("CodeCheckPanel: No associated door assigned! Using fallback scene loading.");
                // Fallback to direct scene loading if no door is assigned
                SceneManager.LoadScene(2);
            }
        }
        else
        {
            Debug.Log("Wrong Code");

            //clear input
            ClearInput();
        }
    }
}