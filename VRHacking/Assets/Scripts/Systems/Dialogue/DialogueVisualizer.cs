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
    private TextMeshProUGUI callsignBox;
    [SerializeField]
    private Image imageBox;
    [SerializeField]
    private RectMask2D imageMask;
    [SerializeField]
    private AudioSource source;
    private Tween rotationTween;

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
        scalingSequence.AppendInterval(0.5f);
        scalingSequence.AppendCallback(() => UpdateVisualizerToHacker());
        //Padding is a vector4, going: Left, Right, Top, Bottom
        scalingSequence.Append(DOVirtual.Float(1,0, 0.45f, (x) => imageMask.padding = new Vector4(0,0,0,x)));
        scalingSequence.Insert(5, DOVirtual.Int(0, defaultHackerData.description.Length - 1,  0.35f, (x) => textBox.text = defaultHackerData.description.Remove(defaultHackerData.description.Length - (1 + x))));
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
        sequence.OnComplete(() => source.Stop());
        sequence.Play();
    }

    private void ResetToDefault() {
        textBox.text = defaultHackerData.description;
        callsignBox.text = "";
        imageBox.sprite = defaultHackerData.icon;
        if(rotationTween != null) {
            rotationTween.Kill();
            rotationTween = null;
        }
        rotationTween = imageBox.gameObject.transform.parent.transform.DORotateQuaternion(Quaternion.Euler(0,0,-360), 1).SetLoops(20, LoopType.Restart);

        dialogueHolder.transform.localScale = Vector3.zero;
        imageBox.transform.localScale = Vector3.zero;
        textBox.transform.localScale = Vector3.zero;
    }

    private void UpdateVisualizerToHacker() {
        //Makes mask cover entire sprite, hiding it
        Debug.Log("Called");
        if(rotationTween != null) {
            rotationTween.Kill();
            rotationTween = null;
        }
        imageBox.transform.parent.transform.rotation = Quaternion.Euler(0,0,0);
        imageBox.sprite = currentHackerData.icon;
        callsignBox.text = currentHackerData.callsign;
        imageMask.padding = new Vector4(0,0,0,1);
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

        source.Play();

        while (currentIndex < orderedChars.Count) {
            
            if(currentTypos < maxTypos) {
                float typoThreshold = currentHackerData.behaviour == HackerData.HackerBehaviour.Trickster ? 0.1f : 0.005f;
                float rng = UnityEngine.Random.Range(1.0f,4.0f);
    
                int wholePart = (int)Math.Truncate(rng);
                if(rng - wholePart <= typoThreshold) {
                    currentTypos++;
                    yield return StartCoroutine(routine: TypoRoutine(displayMessage.ToCharArray(), wholePart));
                }
            }
            displayMessage += orderedChars[currentIndex];
            textBox.text = displayMessage;
            currentIndex++;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        while(currentIndex > 0) {
            displayMessage.Remove(currentIndex - 1);
            currentIndex--;
            textBox.text = displayMessage;
            yield return new WaitForSeconds(typewriterSpeed - typewriterSpeed/4);
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

        while(currentIndex < initialIndex + typoSize) {
            int randomCharIndex = UnityEngine.Random.Range(0, typoChars.Length);
            displayMessage += typoChars[randomCharIndex];
            textBox.text = displayMessage;
            currentIndex++;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        while(currentIndex > initialIndex) {
            displayMessage.Remove(Mathf.Max(0,currentIndex - 1));
            currentIndex--;
            textBox.text = displayMessage;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        yield break;
    }
}