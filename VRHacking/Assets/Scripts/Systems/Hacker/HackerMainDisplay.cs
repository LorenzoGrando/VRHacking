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
    }

    public void UpdateMainSlider(float value) {
        hackerTasksCompletedSlider.value = value;
    }

    public void UpdateHackerData(HackerData data) {
        hackerImage.sprite = data.icon;
        hackerNameText.text = data.callsign;
        hackerBehaviourText.text = Enum.GetName(typeof(HackerData.HackerBehaviour), (int)data.behaviour); 
    }

    public void InitiateTask(RectTransform[] referencePoints) {
        buttonsObject.SetActive(false);
        mainTaskDisplayObject.SetActive(true);

        referenceAreaPins.UpdateActivePins(referencePoints);
        copyAreaPins.ClearLines();
    }

    public void FinishTask() {

    }
}