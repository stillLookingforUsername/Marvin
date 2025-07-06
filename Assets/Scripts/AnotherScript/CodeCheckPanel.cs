using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CodeCheckPanel : MonoBehaviour
{
    public TMP_Text codeDisplay;
    public GameObject codePanel;

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
        codeDisplay.text = _currentCode.PadRight(4, '_'); // e.g. "12_"
    }

    private void CodeCheck()
    {
        if (codeDisplay.text == _correctCode)
        {
            Debug.Log("Correct Code");

            //deactivate the panel
            codePanel.SetActive(false);

            // transition to next level
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(2);

        }
        else
        {
            Debug.Log("Wrong Code");

            //clear input
            codeDisplay.text = "";
        }
    }

}
