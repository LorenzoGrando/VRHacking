using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickMemoryDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject displayHolder;

    [SerializeField]
    private QuickMemoryButton[] characterButtons;

    [SerializeField]
    private Transform bottomLeftAnchor, topRightAnchor;

    private Vector2[] validPositions;
    private List<int> positionIndexes;

    public void ResetDisplay() {
        foreach(QuickMemoryButton button in characterButtons) {
            button.ResetStatus();
            button.gameObject.SetActive(false);
        }
    }

    public void InitializeDisplay(List<QuickMemoryTask.QuickMemoryData> data) {
        if(validPositions == null) {
            GeneratePositions();
        }
        StartCoroutine(routine:DisplayButtonsRoutine(data));
    }

    private void GeneratePositions() {
        float xLenght = topRightAnchor.transform.localPosition.x - bottomLeftAnchor.transform.localPosition.x;
        float yLenght = topRightAnchor.transform.localPosition.y - bottomLeftAnchor.transform.localPosition.y;

        float stepSizeX = xLenght/4;
        float stepSizeY = yLenght/4;
        validPositions = new Vector2[16];
        int index = 0;

        positionIndexes = new List<int>();

        for(int x = 0; x < 4; x++) {
            for(int y = 0; y < 4; y++) {
                validPositions[index] = new Vector2(x * stepSizeX, y * stepSizeY);
                positionIndexes.Add(index);
                index++;
            } 
        }
    }

    public Vector2 GenerateButtonPos(int index) {
        return validPositions[index];
    }

    private IEnumerator DisplayButtonsRoutine(List<QuickMemoryTask.QuickMemoryData> data) {
        positionIndexes.Shuffle();
        for(int i = 0; i < data.Count; i++) {
            characterButtons[i].gameObject.SetActive(true);
            characterButtons[i].transform.localPosition = GenerateButtonPos(positionIndexes[i]);
            characterButtons[i].InitializeButton(data[i], 0.25f);

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(2.5f);

        foreach(QuickMemoryButton button in characterButtons) {
            button.HideButtonData();
            button.UpdateButtonStatus(true);
        }
    }
}