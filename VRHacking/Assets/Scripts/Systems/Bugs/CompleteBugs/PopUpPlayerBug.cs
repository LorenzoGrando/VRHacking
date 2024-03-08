using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PopUpPlayerBug : PlayerBug
{
    [SerializeField]
    private GameObject[] popUpObjects;
    [SerializeField]
    private float duration;

    private Coroutine durationRoutine;

    private int numberActivePopUps;

    protected override void ResetBug()
    {
        //reset positions and values
        return;
    }

    protected override void CompleteBug()
    {
        prefabObject.SetActive(false);
        base.CompleteBug();
    }

    public override bool CheckBugCompleted()
    {
        if(numberActivePopUps <= 0) {
            if(durationRoutine != null) {
                StopCoroutine(durationRoutine);
            }
            return true;
        }

        else {
            return false;
        }
    }

    public override void StartBug(GameSettings.GameSettingsData data)
    {
        ResetBug();
        gameSettingsData = data;


        //do all pre-start operations
        GenerateNumberOfPopups();


        prefabObject.SetActive(true);
        durationRoutine = StartCoroutine(routine:BugDurationRoutine(duration));        
    }

    private void GenerateNumberOfPopups() {
        int baseGeneration = UnityEngine.Random.Range(2,5);

        int finalNumber = Mathf.RoundToInt(Mathf.Clamp(baseGeneration * gameSettingsData.difficulty, 2, 7));

        numberActivePopUps = finalNumber;
    }

    private void SpawnPopUps(float amountToSpawn) {
        //TODO
    }

    private IEnumerator BugDurationRoutine(float duration) {
        yield return new WaitForSeconds(duration);

        CompleteBug();

        yield break;
    }

    public void OnPopUpDestroyed() {
        numberActivePopUps--;

        if(CheckBugCompleted()) {
            CompleteBug();
        }
    }
}