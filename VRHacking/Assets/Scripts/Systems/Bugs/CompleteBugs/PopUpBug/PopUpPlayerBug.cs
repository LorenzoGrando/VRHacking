using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PopUpPlayerBug : PlayerBug
{
    [SerializeField]
    private Transform bottomLeftAnchor, topRightAnchor;
    [SerializeField]
    private PopUpObject[] popUpObjects;
    [SerializeField]
    private float bugCompletionDuration;
    [SerializeField]
    private float popUpSpawnStutterTime;

    private Coroutine durationRoutine;
    private Coroutine spawnRoutine;

    private int numberActivePopUps;

    protected override void ResetBug()
    {
        return;
    }

    protected override void CompleteBug()
    {
        if(numberActivePopUps > 0) {
            foreach(PopUpObject popUpObject in popUpObjects) {
                popUpObject.CallDespawnPopup();
            }
        }
        else {
            prefabObject.SetActive(false);
        }
        base.CompleteBug();
    }

    public override bool CheckBugCompleted()
    {
        if(numberActivePopUps <= 0) {
            if(durationRoutine != null) {
                StopCoroutine(durationRoutine);
            }
            if(spawnRoutine != null) {
                StopCoroutine(spawnRoutine);
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
        spawnRoutine = StartCoroutine(routine: SpawnPopUps(numberActivePopUps));
        durationRoutine = StartCoroutine(routine:BugDurationRoutine(bugCompletionDuration));        
    }

    private void GenerateNumberOfPopups() {
        int baseGeneration = UnityEngine.Random.Range(2,5);

        int finalNumber = Mathf.RoundToInt(Mathf.Clamp(baseGeneration * gameSettingsData.difficulty, 2, 7));

        numberActivePopUps = finalNumber;
    }

    private IEnumerator SpawnPopUps(float amountToSpawn) {
        for(int i = 0; i < amountToSpawn; i++) {
            Vector3 position = GeneratePositionInBounds();
            position.z = popUpObjects[i].transform.position.z;
            popUpObjects[i].transform.position = position;
            popUpObjects[i].gameObject.SetActive(true);

            yield return new WaitForSeconds(popUpSpawnStutterTime + UnityEngine.Random.Range(-0.25f, 0.35f));
        }

        yield break;
    }

    private Vector2 GeneratePositionInBounds() {
        Vector2 rndPos = new Vector2();
        rndPos.x = UnityEngine.Random.Range(bottomLeftAnchor.position.x, topRightAnchor.position.x);
        rndPos.y = UnityEngine.Random.Range(bottomLeftAnchor.position.y, topRightAnchor.position.y);

        return rndPos;
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