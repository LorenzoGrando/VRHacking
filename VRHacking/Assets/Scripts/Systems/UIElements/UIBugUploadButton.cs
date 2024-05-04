using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit.UI;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class UIBugUploadButton : PokeButtonUI
{
    [Header("Functionality")]
    [SerializeField]
    private HackerMainDisplay hackerMainDisplay;

    [SerializeField]
    private GameObject[] thisPinConfig;
    [SerializeField]
    private HackerBug thisButtonBug;
    [SerializeField]
    private GameObject buttonNameText;

    [Header("Cooldown")]
    [SerializeField]
    private GameObject cooldownHolder;
    [SerializeField]
    private TextMeshProUGUI cooldownText;

    private Button thisButton;
    private bool isOnCooldown;

    [Header("Animation")]
    [SerializeField]
    private Vector3 targetLocalScale;

    [Header("Description")]
    [SerializeField]
    private GameObject descritionHolder;
    [SerializeField]
    private TextMeshProUGUI descriptionTextObject;
    private Tween descriptionScaleTween;
    private Tween descriptionPositionTween;
    [SerializeField]
    private Vector3 targetDescriptionScale;
    [SerializeField]
    private Vector3 targetPosition;
    [SerializeField]
    private float animDuration;
    

    private void OnEnable()
    {
        thisButtonBug.OnCooldownStart += StartCooldown;
        thisButtonBug.OnCooldownEndTimer += EndCooldown;

        if(thisButtonBug.cooldownTimer > 0) {
            StartCooldown();
        }

        else {
            EndCooldown();
        }
    }

    private void OnDisable()
    {
        thisButtonBug.OnCooldownStart -= StartCooldown;
        thisButtonBug.OnCooldownEndTimer -= EndCooldown;
    }

    void Update()
    {
        if(isOnCooldown) {
            int timeInSeconds = Mathf.RoundToInt(thisButtonBug.cooldownTimer);
            cooldownText.text = $"{timeInSeconds}s";
        }
    }

    public override void OnButtonPressed()
    {
        hackerMainDisplay.InitiateTask(thisPinConfig, thisButtonBug);
    }

    public override void OnXRUIHover(UIHoverEventArgs enterArgs)
    {
        Debug.Log("Hovered");
        descritionHolder.SetActive(true);
        descriptionTextObject.text = thisButtonBug.description;

        descritionHolder.transform.localPosition = Vector3.zero;
        descritionHolder.transform.localScale = Vector3.zero;

        ClearDescriptionTweens();

        descriptionScaleTween = descritionHolder.transform.DOScale(targetDescriptionScale, animDuration).SetEase(Ease.OutQuart);
        descriptionPositionTween = descritionHolder.transform.DOLocalMove(targetPosition, animDuration / 2).SetEase(Ease.OutQuart);
    }
    public override void OnXRUIHoverExit(UIHoverEventArgs exitArgs)
    {
        ClearDescriptionTweens();

        descriptionScaleTween = descritionHolder.transform.DOScale(Vector3.zero, animDuration  / 2).OnComplete(() => descritionHolder.SetActive(false)).SetEase(Ease.InQuart);
        descriptionPositionTween = descritionHolder.transform.DOLocalMove(Vector3.zero + new Vector3(0, 1.5f, 0), animDuration / 2).SetEase(Ease.InQuart);
    }

    public void StartCooldown() {
        if(thisButton == null) {
            thisButton = GetComponent<Button>();
        }
        isOnCooldown = true;
        thisButton.interactable = false;
        buttonNameText.SetActive(false);
        cooldownHolder.SetActive(true);
    }

    public void EndCooldown() {
        if(thisButton == null) {
            thisButton = GetComponent<Button>();
        }
        isOnCooldown = false;
        thisButton.interactable = true;
        thisButtonBug.cooldownTimer = 0;
        cooldownHolder.SetActive(false);
        buttonNameText.SetActive(true);
    }

    private void ClearDescriptionTweens() {
        if(descriptionScaleTween != null) {
            descriptionScaleTween.Kill();
            descriptionScaleTween = null;
        }

        if(descriptionPositionTween != null) {
            descriptionPositionTween.Kill();
            descriptionPositionTween = null;
        }
    }

    public Tween AnimateButton(bool isInit, float duration) {
        Tween returnTween;
        if(isInit) {
            returnTween = transform.DOScale(Vector3.zero, duration);
        }
        else {
            transform.localScale = Vector3.zero;
            returnTween = transform.DOScale(targetLocalScale, duration);
        }

        return returnTween;
    }
}