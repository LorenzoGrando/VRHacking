using System;
using UnityEngine;
using DG.Tweening;

public class FallingObjectsDisplay : MonoBehaviour
{
    public event Action OnScalingAnimComplete;
    [SerializeField] private GameObject displayHolder;
    [SerializeField] private GameObject[] collectors;
    [SerializeField] private GameObject descriptionText;
    [SerializeField] private float animSpeed;

    public void InitiateDisplay()
    {
        descriptionText.transform.localScale = Vector3.zero;
        for(int i = 0; i < collectors.Length; i++) {
            collectors[i].transform.localScale = Vector3.zero;
        }

        displayHolder.SetActive(true);
        ScaleDisplay(true);
    }

    private void ScaleDisplay(bool isEntry) {
        Sequence sequence = DOTween.Sequence();
        if(isEntry) {
            sequence.Append(descriptionText.transform.DOScale(1, animSpeed/2));
            for(int i = 0; i < collectors.Length; i++) {
                sequence.Append(collectors[i].transform.DOScale(1, animSpeed/3));
            }
        }

        else {
            sequence.Append(descriptionText.transform.DOScale(0, animSpeed/3));
            for(int i = 0; i < collectors.Length; i++) {
                sequence.Append(collectors[i].transform.DOScale(0, animSpeed/4));
            }
        }

        sequence.OnComplete(() => OnScalingAnimComplete?.Invoke());
        sequence.Play();
    }

    public void HideDisplay(bool anim) {
        if(!anim) {
            displayHolder.SetActive(false);
        }
        else {
            ScaleDisplay(false);
        }
    }
}