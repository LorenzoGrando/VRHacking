using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.UI;
using DG.Tweening;

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
    private AudioSource glitchedSource;

    public bool isActive;
    public bool mined;
    [SerializeField]
    private Sprite[] sprites;


    public void ResetStatus() {
        mined = false;
        isActive = false;
        button.enabled = false;
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
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(moleObject.transform.DOScale(1, activeTime/3).OnComplete(() => button.enabled = true));
        sequence.AppendInterval(activeTime);
        sequence.AppendCallback(() => button.enabled = false);
        sequence.Append(moleObject.transform.DOScale(0, activeTime/3).OnComplete(() => isActive = false));
    }

    public override void OnButtonPressed()
    {
        task.OnMoleHit(mined);
        if(mined)
            mined = false;
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