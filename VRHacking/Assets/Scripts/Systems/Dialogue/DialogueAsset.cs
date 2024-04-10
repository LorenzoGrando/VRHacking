using UnityEngine;

[CreateAssetMenu(menuName = "VRHacking/ScriptableObjects/DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    #region Enums
    public enum DialogueType {
        System,
        Hacker
    }

    public enum DialogueSource {
        System,
        Greeting,
        TaskThreshold,
        PlayerGlitched,
        HackerBugUploaded,
        EfficientTask,
        SlowTask,
        PlayerWon,
        PlayerLost
    }
    #endregion

   [SerializeField]
   internal SerializableArray[] greetingMessages, taskThresholdMessages, playerGlitchedMessages, hackerBugUploadMessages; 
   [SerializeField]
   internal SerializableArray[] efficentTaskMessages, slowTaskMessages, playerWonMessages, playerLostMessages;
   
    public string[] RequestMessage(DialogueRequestData requestData) {
        SerializableArray[] targetArray = null;

        switch (requestData.source) {
            case DialogueSource.System:
                //TODO
            break;

            case DialogueSource.Greeting:
                targetArray = greetingMessages;
            break;

            case DialogueSource.TaskThreshold:
                targetArray = taskThresholdMessages;
            break;

            case DialogueSource.PlayerGlitched:
                targetArray = playerGlitchedMessages;
            break;

            case DialogueSource.HackerBugUploaded:
                targetArray = hackerBugUploadMessages;
            break;

            case DialogueSource.EfficientTask:
                targetArray = efficentTaskMessages;
            break;

            case DialogueSource.SlowTask:
                targetArray = slowTaskMessages;
            break;

            case DialogueSource.PlayerWon:
                targetArray = playerWonMessages;
            break;

            case DialogueSource.PlayerLost:
                targetArray = playerLostMessages;
            break;
        }

        int indexOfMessages = 0;

        if(targetArray == null || targetArray.Length == 0) {
            return null;
        }
        else if(targetArray.Length > 1) {
            indexOfMessages = Random.Range(0, targetArray.Length);
        }

        return targetArray[indexOfMessages].messages;
    }
    
}
[System.Serializable]
internal class SerializableArray {
    public string[] messages;
}