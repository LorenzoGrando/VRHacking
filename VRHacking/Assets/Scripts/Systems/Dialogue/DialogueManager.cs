using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Display Data")]
    [SerializeField]
    private float intervalBetweenDisplayMessages;
    [SerializeField]
    private float intervalPreventNewMessages;
    private bool canGenerateMessage;

    private Coroutine preventIntervalRoutine;
    private Coroutine inbetweenIntervalRoutine;

    [SerializeField]
    private DialogueAsset systemDialogue;
    private DialogueAsset currentHackerDialogue;
    
    void Start()
    {
        ResetDialogueData();
        currentHackerDialogue = systemDialogue;
        DialogueRequestData testRequest = new DialogueRequestData {
            type = DialogueAsset.DialogueType.Hacker,
            source = DialogueAsset.DialogueSource.Greeting
        };

        OnRequestMessage(testRequest);
    }
    public void ResetDialogueData() {
        StopAllCoroutines();

        currentHackerDialogue = null;
        canGenerateMessage = true;
    }
    public void UpdateHackerDialogue(DialogueAsset hackerDialogue) {
        currentHackerDialogue = hackerDialogue;
    }

    public void OnRequestMessage(DialogueRequestData requestData) {
        if(canGenerateMessage || requestData.isPriority) {
            string[] messages = TryGenerateMessage(requestData);
            if(messages != null) {
                DisplayMessages(messages);
                InitiateInterval();
            }
        }
    }

    private string[] TryGenerateMessage(DialogueRequestData requestData) {
        switch (requestData.type) {
            case DialogueAsset.DialogueType.System:
                //TODO
                return null;

            case DialogueAsset.DialogueType.Hacker:
                string[] messages = currentHackerDialogue.RequestMessage(requestData);

                if(messages == null) {
                    Debug.Log("No messages for target source");
                    return null;
                }

                return messages;

            default:
                return null;
        }
    }

    private void DisplayMessages(string[] messages) {
        if(messages.Length == 1) {
            DisplaySingleMessage(messages[0]);
        }


        inbetweenIntervalRoutine = StartCoroutine(routine: StaggerMessageDialogue(messages));
    }

    private void DisplaySingleMessage(string message) {
        Debug.Log(message);
    }
    
    private void InitiateInterval() {
        if(preventIntervalRoutine != null) {
            StopCoroutine(preventIntervalRoutine);
        }

        preventIntervalRoutine = StartCoroutine(routine: ExecuteMessageInterval());
    }

    private IEnumerator ExecuteMessageInterval() {
        canGenerateMessage = false;
        yield return new WaitForSeconds(intervalPreventNewMessages);

        canGenerateMessage = true;
        yield break;
    }

    private IEnumerator StaggerMessageDialogue(string[] messages) {
        int currentIndex = 0;

        while(currentIndex < messages.Length) {
            DisplaySingleMessage(messages[currentIndex]);
            currentIndex++;

            yield return new WaitForSeconds(intervalBetweenDisplayMessages);
        }

        yield break;
    }
}

public struct DialogueRequestData {
    public DialogueAsset.DialogueType type;
    public DialogueAsset.DialogueSource source;
    public bool isPriority;
}