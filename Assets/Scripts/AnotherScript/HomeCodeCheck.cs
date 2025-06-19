using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeCodeCheck : MonoBehaviour
{
    public TMP_Text codeDisplay;
    private string _currentCode = "";
    private string code = "1122";

    public void AddDigit(string digit)
    {
        if (_currentCode.Length >= 4) return;
        _currentCode += digit;
        UpdateDisplay();
    }

    public void ClearCode()
    {
        _currentCode = "";
        UpdateDisplay();
    }

    public void SubmitCode()
    {
        Debug.Log("Submitted Code : " + _currentCode);

        //other logic
        CodeCheck();

    }
    private void CodeCheck()
    {
        if (_currentCode == code)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("wrong code");
        }
    }


    public void UpdateDisplay()
    {
        codeDisplay.text = _currentCode;
    }
}
