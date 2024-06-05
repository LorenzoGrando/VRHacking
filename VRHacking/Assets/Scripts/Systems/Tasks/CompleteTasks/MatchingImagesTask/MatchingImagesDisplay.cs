using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MatchingImagesDisplay : MonoBehaviour
{
    public event Action OnLeaveAnimationFinish;
    [SerializeField]
    private GameObject displayHolder;
    [SerializeField]
    private MatchingImagesTask taskManager;

    [SerializeField]
    MatchingImageButton[] buttons;

    private MatchingImageButton lastPressedButton;
    [SerializeField]
    private GameObject descriptionObject;
    [SerializeField]
    private GameObject selectionObject;
    [SerializeField]
    private AudioSource correctSound, wrongSound;
    
    public void InitiateDisplay(List<MatchingImagesTask.ImageType> orderedList, List<MatchingImagesTask.ImageType> shuffledList) {
        displayHolder.SetActive(true);

        descriptionObject.transform.DOScale(1, 0.5f);
        selectionObject.transform.localScale = Vector3.zero;
        GenerateButtons(shuffledList);
        lastPressedButton = null;
    }

    public void GenerateButtons(List<MatchingImagesTask.ImageType> imageList) {
        
        int numberOfButtons = imageList.Count;
        int buttonsArrayLenght = buttons.Length;
        List<int> shuffledIndexes = Enumerable.Range(0, buttonsArrayLenght).ToList();
        shuffledIndexes.Shuffle();
        //Start at a random point in elipse
        int randomStart = UnityEngine.Random.Range(0,buttonsArrayLenght);
        for(int i = 0; i < numberOfButtons; i++) {
            buttons[shuffledIndexes[i]].gameObject.SetActive(true);
            buttons[shuffledIndexes[i]].InitializeButton(imageList[i]);
        }
        
    }

    public void HideButtons() {
        descriptionObject.transform.DOScale(0, 0.75f);
        foreach(MatchingImageButton button in buttons) {
            button.HideButton();
        }

        DOVirtual.DelayedCall(0.95f, () => FireAnimCompletionEvent(false));
    }

    public void ResetDisplay() {
        foreach(MatchingImageButton button in buttons) {
            button.thisButtonType = MatchingImagesTask.ImageType.Null;
            button.gameObject.SetActive(false);
        }
    }

    public void FireAnimCompletionEvent(bool isEntry) {
        if(!isEntry) {
            OnLeaveAnimationFinish?.Invoke();
            displayHolder.SetActive(false);
        }
    }

    public void OnButtonPressed(MatchingImageButton button) {
        if(lastPressedButton == null) {
            lastPressedButton = button;
        }
        else if(lastPressedButton == button) {
            DeselectButtons();
            taskManager.ResetActiveType();
            return;
        }

        taskManager.TryActivateImage(button.thisButtonType);
    }

    public void SelectLastButton()
    {
        selectionObject.transform.localScale = Vector3.zero;
        selectionObject.transform.position = lastPressedButton.transform.position;
        selectionObject.transform.DOScale(1, 0.45f);
    }

    public void ActivateMatchingButtons(MatchingImagesTask.ImageType type) {
        int updatedButtons = 0;

        foreach(MatchingImageButton button in buttons) {
            if(button.thisButtonType == type) {
                button.HideButton();
                updatedButtons++;

                if(updatedButtons == 2)
                    break;
            }
        }

        correctSound.PlayOneShot(correctSound.clip);
    }

    public void DeselectButtons()
    {
        selectionObject.transform.DOScale(0, 0.25f);
        wrongSound.PlayOneShot(wrongSound.clip);

        lastPressedButton = null;
    }
}