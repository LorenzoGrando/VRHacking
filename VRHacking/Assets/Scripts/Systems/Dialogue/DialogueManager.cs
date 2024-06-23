using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public event Action OnEndDialogue;
    [SerializeField]
    private DialogueVisualizer hackerVisualizer, systemVisualizer;
    private DialogueVisualizer activeVisualizer;

    [Header("Display Data")]
    [SerializeField]
    private float intervalPreventNewMessages;
    private bool canGenerateMessage;
    private bool isExecuting;
    private Queue<string> messages;

    private Coroutine preventIntervalRoutine;

    [SerializeField]
    private DialogueAsset systemDialogue;
    private HackerData currentHacker;

    public void ResetDialogueData() {
        StopAllCoroutines();

        currentHacker = null;
        canGenerateMessage = true;
    }

    public void UpdateTargetVisualizer(bool isHacker) {
        if(isHacker) {
            activeVisualizer = hackerVisualizer;
        }
        else {
            activeVisualizer = systemVisualizer;
        }
    }

    public void UpdateHackerDialogue(HackerData hackerData) {
        currentHacker = hackerData;
    }

    public void OnRequestMessage(DialogueRequestData requestData) {
        if((canGenerateMessage && !isExecuting)|| requestData.isPriority) {
            messages = TryGenerateMessage(requestData);
            UpdateTargetVisualizer(requestData.type == DialogueAsset.DialogueType.Hacker);
            if(messages != null) {
                activeVisualizer.StartVisualization(currentHacker, () => BeginDisplaying());
                isExecuting = true;
            }
        }
    }

    private void BeginDisplaying() {
        CheckDisplayMessage();
        InitiateInterval();
    }

    private Queue<string> TryGenerateMessage(DialogueRequestData requestData) {
        Queue<string> returnMessages = new Queue<string>();
        switch (requestData.type) {
            case DialogueAsset.DialogueType.System:
                //TODO
                return null;

            case DialogueAsset.DialogueType.Hacker:
                string[] messages = currentHacker.hackerDialogue.RequestMessage(requestData);

                if(messages == null) {
                    Debug.Log("No messages for target source");
                    return null;
                }

                foreach(string message in messages) {
                    returnMessages.Enqueue(message);
                }

                return returnMessages;

            default:
                return null;
        }
    }

    private void CheckDisplayMessage() {
        if(messages.Count > 0) {
            activeVisualizer.DisplayMessage(messages.Dequeue(), () => CheckDisplayMessage());
        }
        else {
            activeVisualizer.EndVisualization();
            isExecuting = false;
            OnEndDialogue?.Invoke();
        }
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
}

public struct DialogueRequestData {
    public DialogueAsset.DialogueType type;
    public DialogueAsset.DialogueSource source;
    public bool isPriority;
}