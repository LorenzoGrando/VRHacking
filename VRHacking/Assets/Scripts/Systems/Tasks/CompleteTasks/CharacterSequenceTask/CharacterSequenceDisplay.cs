using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CharacterSequenceDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI mainSequenceTextRef;
    private const string defaultColor = "#959595";
    private const string correctColor = "#56E554";

    [SerializeField]
    CharacterSequenceButton[] characterButtons;

    private List<CharacterSequenceTask.CharacterSequenceData> orderedData;
    
    public void InitiateDisplay(List<CharacterSequenceTask.CharacterSequenceData> orderedList, List<CharacterSequenceTask.CharacterSequenceData> shuffledList) {
        gameObject.SetActive(true);
        orderedData = orderedList;

        GenerateButtons(shuffledList);

        UpdateDisplay(-1);
    }

    public void UpdateDisplay(int orderValue) {
        string correctChars = "";
        string remainingChars = "";

        for(int i = 0; i < orderedData.Count; i++) {
            if(i <= orderValue) {
                correctChars += orderedData[i].characters;
            }
            else {
                remainingChars += orderedData[i].characters;
            }
        }

        mainSequenceTextRef.text = $"<color={correctColor}>{correctChars}</color><color={defaultColor}>{remainingChars}</color>";
    }

    public void GenerateButtons(List<CharacterSequenceTask.CharacterSequenceData> characterList) {
        int numberOfButtons = characterList.Count;
        int buttonsArrayLenght = characterButtons.Length;
        //Start at a random point in elipse
        int randomStart = UnityEngine.Random.Range(0,buttonsArrayLenght);
        for(int i = 0; i < numberOfButtons; i++) {
            characterButtons[randomStart].gameObject.SetActive(true);
            characterButtons[randomStart].InitializeButton(characterList[i]);

            randomStart++;

            //Restart from beginning
            if(randomStart == buttonsArrayLenght) {
                randomStart = 0;
            }
        }
    }

    public void ResetDisplay() {
        foreach(CharacterSequenceButton button in characterButtons) {
            button.gameObject.SetActive(false);
        }
    }
}
