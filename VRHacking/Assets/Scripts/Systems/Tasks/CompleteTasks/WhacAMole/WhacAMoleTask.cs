using System.Collections;
using UnityEngine;

public class WhacAMoleTask : HackTask
{
     #region Generic Task Methods

    protected override void ResetTask()
    {
        InitializeValues();
        display.ResetDisplay();
    }

    public override void HideTask()
    {
        StopAllCoroutines();
        display.HideDisplay();
    }
    public override void StartTask(GameSettingsData settingsData)
    {
        this.gameSettingsData = settingsData;
        prefabObject.SetActive(true);
        ResetTask();
        display.InitiateDisplay();
        StartCoroutine(routine: MoleSpawnRoutine());
    }

    protected override bool CheckTaskCompleted()
    {
        if(currentMoleAmount >= targetMoleAmount) {
            return true;
        }

        return false;
    }

    protected override void CompleteTask()
    {
        //Other possible behaviour (anims, etc)
        StopAllCoroutines();
        //Will fire completion event
        display.AnimateButtons(false);
        display.OnLeaveAnimationFinish += () => prefabObject.SetActive(false);
        base.CompleteTask();
    }

    #endregion

    [SerializeField]
    private WhacAMoleDisplay display;
    [SerializeField]
    private float moleSpawnTime;
    [SerializeField]
    private float moleAvailableTime;
    [SerializeField]
    private int targetMoleAmount;
    private int currentMoleAmount;

    
    private void InitializeValues() {
        currentMoleAmount = 0;
    }

    private IEnumerator MoleSpawnRoutine() {
        while(currentMoleAmount < targetMoleAmount) {
            bool validButton = false;
            int rng = 0;
            do {
                rng = UnityEngine.Random.Range(0, 9);

                validButton = display.CheckButtonAvailability(rng);

                if(!validButton) {
                    rng++;
                    if(rng >= 9)
                        rng = 0;

                    validButton = display.CheckButtonAvailability(rng);
                }

            } while(!validButton);

            float availableTime = moleAvailableTime / 1 + gameSettingsData.difficulty;

            bool mineMole = false;
            if(enableMines) {
                int rnd = Random.Range(0,3);

                if(rnd == 0) {
                    mineMole = true;
                }
            }

            display.SpawnMole(rng, availableTime, mineMole);

            yield return new WaitForSeconds(moleSpawnTime / gameSettingsData.difficulty);
        }
    }

    public void OnMoleHit(bool isBugged) {
        if(isBugged) {
            CallGlitch();
            return;
        }

        currentMoleAmount++;

        Debug.Log("Moles hit: " + currentMoleAmount);

        if(CheckTaskCompleted()) {
            CompleteTask();
        }
    }
}