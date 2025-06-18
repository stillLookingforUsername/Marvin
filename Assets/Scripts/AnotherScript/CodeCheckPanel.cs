using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CodeCheckPanel : MonoBehaviour
{
    public TMP_InputField codeInput;
    public Button submitButotn;
    public GameObject codePanel;

    private string correctCode = "1122";

    private void Start()
    {
        submitButotn.onClick.AddListener(CodeCheck);
    }

    private void CodeCheck()
    {
        if (codeInput.text == correctCode)
        {
            Debug.Log("Correct Code");
            // transition to next level
            SceneManager.LoadScene(0);

            //deactivate the panel
            codePanel.SetActive(false);

        }
        else
        {
            Debug.Log("Wrong Code");

            //clear input
            codeInput.text = "";
        }
    }

}
