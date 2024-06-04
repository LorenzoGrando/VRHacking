using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class MatchingImageButton : PokeButtonUI
{
    [SerializeField]
    private MatchingImagesDisplay taskManager;
    public MatchingImagesTask.ImageType thisButtonType;
    [SerializeField]
    private GameObject[] sprites;
    [SerializeField]
    private AudioSource selectSound;

    public void InitializeButton(MatchingImagesTask.ImageType imageType) {
        thisButtonType = imageType;
        UpdateAppearance();
        ScaleButton(true);
    }

    public void HideButton() {
        ScaleButton(false);
    }

    private void UpdateAppearance() {
        foreach(GameObject spriteObject in sprites) {
            spriteObject.SetActive(false);
        }

        sprites[(int)thisButtonType - 1].SetActive(true);
    }

    private void ScaleButton(bool isEntry) {
        Vector3 targetScale = Vector3.zero;
        if(isEntry) {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            targetScale = new Vector3(0.1f, 0.09f, 0.1f);
        }

        Tween t = transform.DOScale(targetScale, 0.85f);
        if(!isEntry)
            t.OnComplete(() => OnHideButton());
    }

    private void OnHideButton() {
        foreach(GameObject spriteObject in sprites) {
            spriteObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public override void OnButtonPressed()
    {
        selectSound.PlayOneShot(selectSound.clip);
        taskManager.OnButtonPressed(this);
    }

    public override void OnXRUIHover(UIHoverEventArgs enterArgs)
    {
        //
    }

    public override void OnXRUIHoverExit(UIHoverEventArgs exitArgs)
    {
        //
    }
}