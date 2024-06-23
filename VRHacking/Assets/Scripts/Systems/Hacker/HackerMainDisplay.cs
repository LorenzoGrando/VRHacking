using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HackerMainDisplay : MonoBehaviour
{
    #region System Structs

    public struct SliderData {
        public float hackerBugUploadValue;
        public float hackerNextTaskValue;
    }

    #endregion
    public event Action<DialogueRequestData> OnMessageTrigger;
    private delegate void OnCompleteSequenceCallback();

    [Header("Canvas")]
    [SerializeField]
    private GameObject displayHolder;

    [Header("Task Area")]
    //Main Task Area
    [SerializeField]
    private GameObject mainTaskDisplayObject;
    [SerializeField]
    private UIPinManager copyAreaPinManager;
    [SerializeField]
    private UIPinManager referenceAreaPinManager;

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
    private UIBugUploadButton[] buttons;
    [SerializeField]
    private GameObject bugDescriptionHolder;
    [SerializeField]
    private TextMeshProUGUI bugDescriptionNameText, bugDescriptionText;
    
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
    [SerializeField]
    private Image backgroundHackerImage;

    private HackerBug executingBug;

    private GameObject[] cachedPins;
    private GameObject cachedStarterPin;
    private HackerBug cachedBug;

    public void InitiateCanvas() {
        ResetValues();
        for(int i = 0; i < buttons.Length; i++) {
            float buttonAnimDuration = 0.5f;
            buttons[i].AnimateButton(false, buttonAnimDuration).SetEase(Ease.OutBack);

            buttonAnimDuration += 0.1f;
        } 
        displayHolder.SetActive(true);
    }

    public void DisableCanvas() {
        foreach(UIBugUploadButton button in buttons) {
            button.EndCooldown();
        }
        displayHolder.SetActive(false);
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
            if(playerBugUploadSlider.value >= 0.98) {
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
        backgroundHackerImage.sprite = data.icon;
    }

    public void CallBugDescription(GameObject[] referencePoints, GameObject startPoint, HackerBug bug, string name, string description) {
        Sequence sequence = DOTween.Sequence();
        float buttonAnimDuration = 0.5f;
        for(int i = 0; i < buttons.Length; i++) {
            Tween tween = buttons[i].AnimateButton(true, buttonAnimDuration).SetEase(Ease.InBack);

            buttonAnimDuration += 0.15f;
            if(i + 1 == buttons.Length) {
                tween.OnComplete(() => buttonsObject.SetActive(false));
                sequence.Append(tween);
            }
        }
        bugDescriptionNameText.text = name;
        bugDescriptionText.text = description;
        cachedPins = referencePoints;
        cachedStarterPin = startPoint;
        cachedBug = bug;
        sequence.AppendCallback(() => bugDescriptionHolder.SetActive(true));
        sequence.Append(bugDescriptionHolder.transform.DOScale(1, 0.45f));
        sequence.Play();
    }

    public void InitiateTask() {
        if(cachedPins == null || cachedBug == null)
            return;
        referenceAreaPinManager.UpdateActivePins(cachedPins);
        copyAreaPinManager.OnNewPinAdded += CheckPinCompletion;
        copyAreaPinManager.ClearLines();
        copyAreaPinManager.activeBug = cachedBug;

        mainTaskDisplayObject.SetActive(true);
        DisplayTaskSequence(true, () => copyAreaPinManager.AddNewPin(cachedStarterPin));
    }

    private void CheckPinCompletion() {
        if(copyAreaPinManager.activePoints.Count == referenceAreaPinManager.activePoints.Count) {
            bool hasCompleted = true;
            for(int i =0; i < copyAreaPinManager.activePoints.Count; i++) {
                if(copyAreaPinManager.activePoints[i].name != referenceAreaPinManager.activePoints[i].name) {
                    hasCompleted = false;
                    break;
                }
            }

            if(!hasCompleted) {
                int regularIndex = 0;
                for(int i = copyAreaPinManager.activePoints.Count - 1; i >= 0; i--) {
                    if(copyAreaPinManager.activePoints[regularIndex].name != referenceAreaPinManager.activePoints[i].name) {
                        hasCompleted = false;
                        break;
                    }
                    else {
                        hasCompleted = true;
                    }
                    regularIndex++;
                }
            }

            if(hasCompleted) {
                FinishTask();
            }
        }
    }

    public void FinishTask() {
        Debug.Log("Started uploading");
        copyAreaPinManager.OnNewPinAdded -= CheckPinCompletion;
        executingBug = copyAreaPinManager.activeBug;
        playerBugUploadSlider.value = 0;
        DisplaySlider(true);
    }

    private void FinishUpload() {
        Debug.Log("Finished upload");
        executingBug = null;
        DisplaySlider(false);

        DialogueRequestData requestData = new DialogueRequestData {
            type = DialogueAsset.DialogueType.Hacker,
            source = DialogueAsset.DialogueSource.HackerBugUploaded
        };
        OnMessageTrigger?.Invoke(requestData);
    }

    private void DisplayTaskSequence(bool isInit, OnCompleteSequenceCallback callback = null) {
        Sequence sequence = DOTween.Sequence();

        if(isInit) {
            sequence.Append(bugDescriptionHolder.transform.DOScale(0, 0.45f).OnComplete(() => bugDescriptionHolder.gameObject.SetActive(false)));   

            
            sequence.Append(copyAreaPinManager.AnimatePins(isInit, false));

            sequence.Append(referenceAreaPinManager.AnimatePins(isInit, true));
        }

        else {
            buttonsObject.SetActive(true);
            float buttonAnimDuration = 0.65f;

            for(int i = 0; i < buttons.Length; i++) {
                Tween tween = buttons[i].AnimateButton(isInit, buttonAnimDuration).SetEase(Ease.OutBack);

                buttonAnimDuration += 0.1f;
                if(i + 1 == buttons.Length) {
                    sequence.Append(tween);
                }
            } 
        }

        if(callback != null) {
            callback?.Invoke();
        }

        sequence.Play();
    }

    private void DisplaySlider(bool isInit) {
        Sequence sequence = DOTween.Sequence();
        if(isInit) {
            sequence.Append(copyAreaPinManager.AnimatePins(!isInit, true));
            sequence.Insert(0, referenceAreaPinManager.AnimatePins(!isInit, true).OnComplete(() => mainTaskDisplayObject.SetActive(false)));
            sequence.AppendInterval(0.05f);
            playerBugUploadObject.transform.localScale = Vector3.zero;
            playerBugUploadObject.SetActive(true);
            sequence.Append(playerBugUploadObject.transform.DOScale(Vector3.one, 0.35f));
            sequence.OnComplete(() => copyAreaPinManager.ActivateBug());
        }
        else {
            sequence.Append(playerBugUploadObject.transform.DOScale(Vector3.zero, 0.325f).SetEase(Ease.InBack).OnComplete(() => playerBugUploadObject.SetActive(false)));
            sequence.AppendInterval(0.05f).OnComplete(() => DisplayTaskSequence(false));
        }
        sequence.Play();
    }
}