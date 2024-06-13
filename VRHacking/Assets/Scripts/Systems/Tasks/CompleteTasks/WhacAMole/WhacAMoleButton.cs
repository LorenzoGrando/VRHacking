using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class WhacAMoleButton : PokeButtonUI
{
    [SerializeField]
    private WhacAMoleTask task;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image moleImage;
    [SerializeField]
    private GameObject moleObject;
    [SerializeField]
    private Material mainMat, glitchedMat;
    [SerializeField]
    private AudioSource mainSource, glitchedSource;

    public bool isActive;
    public bool mined;
    [SerializeField]
    private Sprite[] sprites;
    private Coroutine delayRoutine;
    private Tween activeTween;

    public void OnDisable() {
        if(delayRoutine != null) {
            StopCoroutine(delayRoutine);
            delayRoutine = null;
        }
        if(activeTween != null) {
            activeTween.Kill();
            activeTween = null;
        }
    }
    public void ResetStatus() {
        mined = false;
        isActive = false;
        button.interactable = false;
        moleObject.transform.localScale = Vector3.zero;
    }

    public void UpdateAppearance() {
        if(mined) {
            moleImage.material = glitchedMat;
            Color32 color = new Color32(255, 0, 0, 255);
            moleImage.color = color; 
            moleImage.overrideSprite = sprites[1];
            glitchedSource.PlayOneShot(glitchedSource.clip);
        }
        else {
            moleImage.material = mainMat;
            Color32 color = new Color32(255, 255, 255, 255);
            moleImage.color = color; 
            moleImage.overrideSprite = sprites[0];
        }
    }

    public void AnimateMoleButton(float activeTime) {
        isActive = true;
        
        activeTween = moleObject.transform.DOScale(1, activeTime/6).OnComplete(() => delayRoutine = StartCoroutine(routine: DelayedEndAnimation(activeTime)));
    }

    public override void OnButtonPressed()
    {
        if(delayRoutine != null) {
            StopCoroutine(delayRoutine);
            delayRoutine = null;
        }
        if(activeTween != null) {
            activeTween.Kill();
            activeTween = null;
        }
        activeTween = moleObject.transform.DOScale(0, 0.15f).OnComplete(() => isActive = false);
        button.interactable = false;

        task.OnMoleHit(mined);
        if(mined)
            mined = false;
        else
            mainSource.PlayOneShot(mainSource.clip);
    }

    public override void OnXRUIHover(UIHoverEventArgs enterArgs)
    {
        //
    }

    public override void OnXRUIHoverExit(UIHoverEventArgs exitArgs)
    {
        //
    }

    private IEnumerator DelayedEndAnimation(float duration) {
        button.interactable = true;
        activeTween = null;
        
        yield return new WaitForSeconds(duration);

        button.interactable = false;
        activeTween = moleObject.transform.DOScale(0, duration/6).OnComplete(() => isActive = false);

        yield break;
    }
}