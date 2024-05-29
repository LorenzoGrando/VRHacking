using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GlitchDisplay : MonoBehaviour
{
    private delegate void Callback();
    [SerializeField]
    private GameObject holderObject;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private string targetText;
    [SerializeField]
    private TextMeshProUGUI textObject;
    [SerializeField]
    private TextMeshProUGUI countdownObject;
    private float duration;

    void OnDisappear() {
        StopAllCoroutines();
        Sequence endSequence = DOTween.Sequence();
        endSequence.Append(textObject.transform.DOScale(Vector3.zero, 0.05f));
        endSequence.Insert(0, countdownObject.transform.DOScale(Vector3.zero, 0.05f));
        endSequence.Append(backgroundImage.transform.DOScale(Vector3.zero, 0.15f));
        
        endSequence.OnComplete(() => holderObject.SetActive(false));
        endSequence.Play();
    }

    public void CallDisplay(float duration) {
        this.duration = duration;
        holderObject.SetActive(true);

        textObject.text = "";
        countdownObject.text = "";
        backgroundImage.transform.localScale = Vector3.zero;
        textObject.transform.localScale = Vector3.zero;
        countdownObject.transform.localScale = Vector3.zero;

        Sequence startSequence = DOTween.Sequence();
        startSequence.Append(backgroundImage.transform.DOScale(Vector3.one, 0.25f));
        startSequence.Append(textObject.transform.DOScale(Vector3.one/100, 0.10f));
        startSequence.Insert(1, countdownObject.transform.DOScale(Vector3.one/100, 0.10f));

        startSequence.OnComplete(() => StartCoroutine(TypewriterRoutine(targetText, () => StartCoroutine(CountdownRoutine(duration, () => OnDisappear())))));
        startSequence.Play();
    }

    private IEnumerator TypewriterRoutine(string message, Callback OnCompleteCallback = null) {
        int currentIndex = 0;
        List<char> orderedChars = new List<char>(message.ToCharArray());
        string displayMessage = "";

        while (currentIndex < orderedChars.Count) {
            displayMessage += orderedChars[currentIndex];
            textObject.text = displayMessage;
            currentIndex++;
            yield return new WaitForSeconds(0.15f);
        }

        while(currentIndex > 0) {
            displayMessage.Remove(currentIndex - 1);
            currentIndex--;
            textObject.text = displayMessage;
            yield return new WaitForSeconds(0.15f);
        }

        if(OnCompleteCallback != null) {
            OnCompleteCallback.Invoke();
        }
        yield break;
    }

    private IEnumerator CountdownRoutine(float duration, Callback OnCompleteCallback = null) {
        float timeElapsed = 0;

        while(timeElapsed < duration) {
            int displayInt = Mathf.CeilToInt(duration - timeElapsed);
            timeElapsed += Time.deltaTime;
            countdownObject.text = displayInt.ToString();
            yield return null;
        }

        if(OnCompleteCallback != null) {
            OnCompleteCallback.Invoke();
        }

        yield break;
    }
}
