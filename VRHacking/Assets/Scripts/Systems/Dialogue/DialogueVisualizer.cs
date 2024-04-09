using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueVisualizer : MonoBehaviour
{
    public delegate void Callback();
    [Header("Functionality")]
    [SerializeField]
    private HackerData defaultHackerData;
    private HackerData currentHackerData;
    [SerializeField]
    private float scalingDuration;
    [SerializeField]
    private float typewriterSpeed;
    private const string typoChars = "abcdefghijklmnopqrstuvwxyz;ABCDEFGHIJKLMNOPQRSTUVWXYZ^.,";

    [Header("Display References")]
    [SerializeField]
    private GameObject dialogueHolder;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private Image imageBox;
    [SerializeField]
    private RectMask2D imageMask;

    private void OnEnable()
    {
        ResetToDefault();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void StartVisualization(HackerData hackerData, Callback callback) {
        StopAllCoroutines();
        ResetToDefault();
        currentHackerData = hackerData;

        //Do scaling
        float brokenDuration = scalingDuration/3;
        Sequence scalingSequence = DOTween.Sequence();
        scalingSequence.Append(dialogueHolder.transform.DOScale(Vector3.one, brokenDuration));
        scalingSequence.Append(imageBox.transform.DOScale(Vector3.one, brokenDuration));
        scalingSequence.Insert(1, textBox.transform.DOScale(Vector3.one, brokenDuration));
        scalingSequence.AppendInterval(0.5f).OnComplete(() => UpdateVisualizerToHacker());
        //Padding is a vector4, going: Left, Right, Top, Bottom
        scalingSequence.Append(DOVirtual.Float(1,0, 0.45f, (x) => imageMask.padding = new Vector4(0,0,0,x)));
        scalingSequence.Insert(4, DOVirtual.Int(0, defaultHackerData.description.Length - 1,  0.35f, (x) => textBox.text = defaultHackerData.description.Remove(defaultHackerData.description.Length - (1 + x))));
        //Calback message function
        scalingSequence.OnComplete(() => callback.Invoke());

        scalingSequence.Play();
    }

    public void EndVisualization() {
        //Inside components scale down faster than structure
        Sequence sequence = DOTween.Sequence();
        sequence.Append(imageBox.transform.DOScale(Vector3.zero, scalingDuration/2));
        sequence.Insert(0, textBox.transform.DOScale(Vector3.zero, scalingDuration/2));
        sequence.Insert(0, dialogueHolder.transform.DOScale(Vector3.zero, scalingDuration));

        sequence.Play();
    }

    private void ResetToDefault() {
        textBox.text = defaultHackerData.description;
        imageBox.sprite = defaultHackerData.icon;

        dialogueHolder.transform.localScale = Vector3.zero;
        imageBox.transform.localScale = Vector3.zero;
        textBox.transform.localScale = Vector3.zero;
    }

    private void UpdateVisualizerToHacker() {
        //Makes mask cover entire sprite, hiding it
        imageMask.padding = new Vector4(0,0,0,1);
        imageBox.sprite = currentHackerData.icon;
    }

    public void DisplayMessage(string message, Callback OnCompleteCallback = null) {
        StartCoroutine(routine: TypewriterRoutine(message, OnCompleteCallback));
    }

    private IEnumerator TypewriterRoutine(string message, Callback OnCompleteCallback = null) {
        int currentIndex = 0;
        List<char> orderedChars = new List<char>(message.ToCharArray());
        string displayMessage = "";

        int maxTypos = 3;
        int currentTypos = 0;

        while (currentIndex < orderedChars.Count) {
            
            if(currentTypos < maxTypos) {
                float typoThreshold = currentHackerData.behaviour == HackerData.HackerBehaviour.Trickster ? 0.15f : 0.05f;
                float rng = UnityEngine.Random.Range(1.0f,4.0f);
    
                int wholePart = (int)Math.Truncate(rng);

                Debug.Log("Full: " + rng + ", Whole: " + wholePart);
                if(rng - wholePart <= typoThreshold) {
                    currentTypos++;
                    //yield return StartCoroutine(routine: TypoRoutine(displayMessage.ToCharArray(), wholePart));
                }
            }
            
            

            displayMessage += orderedChars[currentIndex];
            textBox.text = displayMessage;
            currentIndex++;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        while(currentIndex < 0) {
            displayMessage.Remove(currentIndex - 1);
            currentIndex--;
            yield return new WaitForSeconds(typewriterSpeed/2);
        }

        if(OnCompleteCallback != null) {
            OnCompleteCallback.Invoke();
        }
        yield break;
    }

    private IEnumerator TypoRoutine(char[] currentMessage, int typoSize) {
        int initialIndex = currentMessage.Length - 1;
        int currentIndex = initialIndex;
        string displayMessage = "";

        for(int i = 0; i < initialIndex; i++) {
            displayMessage += currentMessage[i];
        }

        while(initialIndex < initialIndex + typoSize) {
            int randomCharIndex = UnityEngine.Random.Range(0, typoChars.Length);
            displayMessage += typoChars[randomCharIndex];
            textBox.text = displayMessage;
            currentIndex ++;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        while(currentIndex > initialIndex) {
            displayMessage.Remove(currentIndex);
            currentIndex--;
            textBox.text = displayMessage;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        yield break;
    }
}