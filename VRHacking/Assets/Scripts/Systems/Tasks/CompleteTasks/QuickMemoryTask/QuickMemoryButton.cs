using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuickMemoryButton : PokeButtonUI
{
    private QuickMemoryTask task;
    private QuickMemoryTask.QuickMemoryData thisButtonData;
    [SerializeField]
    private Button thisButton;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Material mainMat, glitchedMat;
    [SerializeField]
    private AudioSource glitchedSource;
    [SerializeField]
    private Vector3 targetScale;
    public override void OnXRUIHover(UIHoverEventArgs enterArgs)
    {
        throw new System.NotImplementedException();
    }

    public override void OnXRUIHoverExit(UIHoverEventArgs exitArgs)
    {
        throw new System.NotImplementedException();
    }

    public void ResetStatus() {
        thisButtonData.numberInOrder = 0;
        thisButtonData.isBugged = false;
        thisButton.interactable = false;
    }

    public void HideButtonData() {
        textRef.text = " ";
    }

    public void InitializeButton(QuickMemoryTask.QuickMemoryData newData, float animDuration) {
        if(textRef == null) {
            textRef = GetComponentInChildren<TextMeshProUGUI>();
        }

        thisButtonData = newData;
        textRef.text = newData.numberInOrder.ToString();

        if(newData.isBugged) {
            image.material = glitchedMat;
            glitchedSource.PlayOneShot(glitchedSource.clip);
        }
        else
            image.material = mainMat;

        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, animDuration).SetEase(Ease.InQuad);
        UpdateButtonStatus(false);
    }

    public void UpdateButtonStatus(bool active) => thisButton.interactable = active;

    public override void OnButtonPressed() {
        if(task == null)
            task = FindObjectOfType<QuickMemoryTask>();

        if(task.TryActiveButton(thisButtonData)) {
            gameObject.SetActive(false);
        }
    }
}