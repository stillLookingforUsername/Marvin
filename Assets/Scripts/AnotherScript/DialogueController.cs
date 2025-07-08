using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField,Range(0.1f,5f)] private float _typingSpeed = 0.5f;


    private Queue<string> _currentParagraphs = new Queue<string>();

    private string p;

    private Coroutine _typingCoroutine;

    private bool _isTyping;
    private bool conversationEnded;

    private const string HTML_ALPHA = "<color=#00000000>";
    [SerializeField,Range(0.1f,5f)] private const float _MAX_TYPING_SPEED = 0.1f;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        //if there is nothing in the queue,
        if(_currentParagraphs.Count == 0)
        {
            if(!conversationEnded)
            {
                //start conversation 
                StartConversation(dialogueText);
            }
            else if(conversationEnded && !_isTyping)
            {
                //end conversation
                EndConversation();
                return;
            }
        }
        //if there is something in the queue
        if(!_isTyping)
        {
            p = _currentParagraphs.Dequeue();
            _typingCoroutine = StartCoroutine(TypeDialogueText(p));
        }

        //conversation is being typed out
        else
        {
            FinishParagraph();
        }


        //update conversationEnded bool
        if(_currentParagraphs.Count == 0)
        {
            conversationEnded = true;
        }

    }
    private void StartConversation(DialogueText dialogueText)
    {
        //active gameObject
        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        //update the name
        NPCNameText.text = dialogueText.speakerName;

        //add paragraphs to the queue
        for(int i=0;i<dialogueText.paragraphs.Length; i++)
        {
            _currentParagraphs.Enqueue(dialogueText.paragraphs[i]);
        }


    }
    private void EndConversation()
    {
        //clear the queue

        //return bool to false
        conversationEnded = false;

        //deactive gameObject
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    private IEnumerator TypeDialogueText(string p)
    {
        _isTyping = true; 

        NPCDialogueText.text = "";
        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;
        foreach(char c in p.ToCharArray())
        {
            alphaIndex++;
            NPCDialogueText.text = originalText;

            displayedText = NPCDialogueText.text.Insert(alphaIndex,HTML_ALPHA);
            NPCDialogueText.text = displayedText;
            yield return new WaitForSeconds(_MAX_TYPING_SPEED/_typingSpeed);
        }

        _isTyping = false;
    }

    private void FinishParagraph()
    {
        //stop the coroutine
        StopCoroutine(_typingCoroutine);

        //finish displaying the text
        NPCDialogueText.text = p;

        //update isTyping bool
        _isTyping = false;
    }
}
