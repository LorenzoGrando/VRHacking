using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class UIBugUploadButton : MonoBehaviour
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

    [Header("Description")]
    [SerializeField]
    private GameObject descritionHolder;
    [SerializeField]
    private TextMeshProUGUI descriptionTextObject;
    private Tween scaleTween;
    private Tween positionTween;
    [SerializeField]
    private Vector3 targetLocalScale;
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

    public void OnClick() {
        hackerMainDisplay.InitiateTask(thisPinConfig, thisButtonBug);
    }

    public void OnHover(HoverEnterEventArgs hoverEnterEventArgs) {
        descritionHolder.SetActive(true);
        descriptionTextObject.text = thisButtonBug.description;

        descritionHolder.transform.localPosition = Vector3.zero;
        descritionHolder.transform.localScale = Vector3.zero;

        ClearTweens();

        scaleTween = descritionHolder.transform.DOScale(targetLocalScale, animDuration).SetEase(Ease.OutQuart);
        positionTween = descritionHolder.transform.DOLocalMove(targetPosition, animDuration / 2).SetEase(Ease.OutQuart);
    }

    public void OnUnhover(HoverExitEventArgs hoverExitEventArgs) {
        ClearTweens();

        scaleTween = descritionHolder.transform.DOScale(Vector3.zero, animDuration  / 2).OnComplete(() => descritionHolder.SetActive(false)).SetEase(Ease.InQuart);
        positionTween = descritionHolder.transform.DOLocalMove(Vector3.zero + new Vector3(0, 1.5f, 0), animDuration / 2).SetEase(Ease.InQuart);
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
        cooldownHolder.SetActive(false);
        buttonNameText.SetActive(true);
    }

    private void ClearTweens() {
        if(scaleTween != null) {
            scaleTween.Kill();
            scaleTween = null;
        }

        if(positionTween != null) {
            positionTween.Kill();
            positionTween = null;
        }
    }
}