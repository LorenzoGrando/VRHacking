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
    }

    public void InitiateTask(GameObject[] referencePoints, HackerBug bug) {
        referenceAreaPinManager.UpdateActivePins(referencePoints);
        copyAreaPinManager.OnNewPinAdded += CheckPinCompletion;
        copyAreaPinManager.ClearLines();
        copyAreaPinManager.activeBug = bug;

        mainTaskDisplayObject.SetActive(true);
        DisplayTaskSequence(isInit: true);
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

    private void DisplayTaskSequence(bool isInit) {
        Sequence sequence = DOTween.Sequence();

        if(isInit) {
            float buttonAnimDuration = 0.5f;
            for(int i = 0; i < buttons.Length; i++) {
                Tween tween = buttons[i].AnimateButton(isInit, buttonAnimDuration).SetEase(Ease.InBack);

                buttonAnimDuration += 0.15f;
                if(i + 1 == buttons.Length) {
                    tween.OnComplete(() => buttonsObject.SetActive(false));
                    sequence.Append(tween);
                }
            }

            
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