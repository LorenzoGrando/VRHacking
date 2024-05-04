using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(UIRestrainer)), RequireComponent(typeof(BoxCollider))]
public class PopUpObject : MonoBehaviour
{
    private UIRestrainer restrainer;
    private BoxCollider myCol;
    [SerializeField]
    private PopUpPlayerBug bugManager;
    [SerializeField]
    private Vector3 targetScale;
    [SerializeField]
    private float graceDurationOnSpawn;
    [SerializeField]
    private string[] popupMessages;
    [SerializeField]
    private TextMeshProUGUI textObject;

    private Tween scaleTween;
    private bool preventMultiCall = false;

    private void OnEnable()
    {
        if(restrainer == null)
            restrainer = GetComponent<UIRestrainer>();
        if(myCol == null)
            myCol = GetComponent<BoxCollider>();
        
        restrainer.OnHitBounds += HitBoundsCall;


        UpdatePopUpMessage();
        OnSpawnPopUp();
    }

    void OnDisable()
    {
        restrainer.OnHitBounds -= HitBoundsCall;
    }

    private void OnSpawnPopUp() {
        myCol.enabled = false;
        preventMultiCall = false;
        transform.localScale = Vector3.zero;
        scaleTween = transform.DOScale(targetScale, graceDurationOnSpawn).SetEase(Ease.InOutQuad).OnComplete(()  => OnAnimComplete(true));
    }

    public void CallDespawnPopup() {
        if(!preventMultiCall) {
            scaleTween = transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InCubic).OnComplete(() => DespawnPopup());
            preventMultiCall = true;
        }
    }

    private void DespawnPopup() {
        bugManager.OnPopUpDestroyed();
        this.gameObject.SetActive(false);
    }

    private void OnAnimComplete(bool colStatus) {
        myCol.enabled = colStatus;
    }

    private void UpdatePopUpMessage() {
        int rng = UnityEngine.Random.Range(0, popupMessages.Length);

        textObject.text = popupMessages[rng];
    }

    private void HitBoundsCall() {
        CallDespawnPopup();
    }
}
