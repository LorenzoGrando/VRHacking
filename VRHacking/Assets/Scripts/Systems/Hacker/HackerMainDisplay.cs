using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackerMainDisplay : MonoBehaviour
{
    #region System Structs

    public struct SliderData {
        public float hackerBugUploadValue;
        public float hackerNextTaskValue;
    }

    #endregion
    [Header("Canvas")]
    [SerializeField]
    private GameObject displayHolder;

    [Header("Task Area")]
    //Main Task Area
    [SerializeField]
    private GameObject mainTaskDisplayObject;
    [SerializeField]
    private UIPinManager copyAreaPins;
    [SerializeField]
    private UIPinManager referenceAreaPins;

    [Header("Hacker Bug Upload")]
    [SerializeField]
    private Slider hackerBugUploadSlider;

    [Header("Main Hacker Info")]
    [SerializeField]
    private Slider hackerTasksCompletedSlider;
    [SerializeField]
    private Slider hackerNextTaskSlider;


    [Header("Player Bug Uploading")]
    //Bug Upload Buttons
    [SerializeField]
    private GameObject buttonsObject;
    [SerializeField]
    private GameObject playerBugUploadObject;
    [SerializeField]
    private Slider playerBugUploadSlider;

    [Header("Hacker Cosmetics")]
    [SerializeField]
    private TextMeshProUGUI hackerNameText;
    [SerializeField]
    private TextMeshProUGUI hackerBehaviourText;
    [SerializeField]
    private Image hackerImage;

    private HackerBug executingBug;

    public void InitiateCanvas() {
        ResetValues();
        displayHolder.SetActive(true);
    }
    
    private void ResetValues() {
        hackerBugUploadSlider.value = 0;
        hackerNextTaskSlider.value = 0;
        hackerTasksCompletedSlider.value = 0;
        playerBugUploadObject.gameObject.SetActive(false);
        buttonsObject.SetActive(true);
        mainTaskDisplayObject.SetActive(false);
    }
    public void UpdateContinuousSliders(SliderData currentSliderData) {
        hackerBugUploadSlider.value = currentSliderData.hackerBugUploadValue;
        hackerNextTaskSlider.value = currentSliderData.hackerNextTaskValue;

        if(executingBug != null) {
            playerBugUploadSlider.value = executingBug.progress;
            if(playerBugUploadSlider.value >= 0.95) {
                FinishUpload();
            }
        }
    }

    public void UpdateMainSlider(float value) {
        hackerTasksCompletedSlider.value = value;
    }

    public void UpdateHackerData(HackerData data) {
        hackerImage.sprite = data.icon;
        hackerNameText.text = data.callsign;
        hackerBehaviourText.text = Enum.GetName(typeof(HackerData.HackerBehaviour), (int)data.behaviour); 
    }

    public void InitiateTask(GameObject[] referencePoints, HackerBug bug) {
        buttonsObject.SetActive(false);
        mainTaskDisplayObject.SetActive(true);

        referenceAreaPins.UpdateActivePins(referencePoints);
        copyAreaPins.OnNewPinAdded += CheckPinCompletion;
        copyAreaPins.ClearLines();
        copyAreaPins.activeBug = bug;
    }

    private void CheckPinCompletion() {
        if(copyAreaPins.activePoints.Count == referenceAreaPins.activePoints.Count) {
            bool hasCompleted = true;
            for(int i =0; i < copyAreaPins.activePoints.Count; i++) {
                if(copyAreaPins.activePoints[i].name != referenceAreaPins.activePoints[i].name) {
                    hasCompleted = false;
                    break;
                }
            }

            if(hasCompleted) {
                FinishTask();
            }
        }
    }

    public void FinishTask() {
        Debug.Log("Started uploading");
        copyAreaPins.OnNewPinAdded -= CheckPinCompletion;
        executingBug = copyAreaPins.activeBug;
        copyAreaPins.ActivateBug();

        buttonsObject.SetActive(false);
        mainTaskDisplayObject.SetActive(false);
        playerBugUploadObject.SetActive(true);
        playerBugUploadSlider.value = 0;
    }

    private void FinishUpload() {
        Debug.Log("Finished upload");
        executingBug = null;
        buttonsObject.SetActive(true);
        playerBugUploadObject.SetActive(false);
    }
}