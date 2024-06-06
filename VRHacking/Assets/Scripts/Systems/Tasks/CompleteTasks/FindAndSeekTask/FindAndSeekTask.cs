using System;
using UnityEngine;

public class FindAndSeekTask : HackTask
{
    #region Generic Task Methods
    public override void HideTask()
    {
        display.HideDisplay();
    }

    public override void StartTask(GameSettingsData settingsData)
    {
        this.gameSettingsData = settingsData;
        ResetTask();
        InitializeValues();
        display.InitiateDisplay();
        executing = true;
    }

    protected override bool CheckTaskCompleted()
    {
        return raycaster.HitTargetIcon();
    }

    protected override void ResetTask()
    {
        raycaster.UpdateTargetIcon(TargetIcon.Null);
        raycaster.UpdateFireStatus(false);
        display.ResetDisplay();
        executing = false;
    }

    protected override void CompleteTask()
    {
        raycaster.UpdateFireStatus(false);
        executing = false;
        base.CompleteTask();
    }

    #endregion

    [SerializeField]
    private FindAndSeekDisplay display;
    [SerializeField]
    private FindAndSeekRaycaster raycaster;

    public enum TargetIcon {
        Null, Target1, Target2, Target3, Target4
    };

    private bool executing;

    private void InitializeValues() {
        int rng = UnityEngine.Random.Range(1, Enum.GetValues(typeof(TargetIcon)).Length);
        raycaster.UpdateTargetIcon((TargetIcon)rng);
        raycaster.UpdateFireStatus(true);
    }

    private void Update() {
        if(executing) {
            if(CheckTaskCompleted()) {
                display.OnScalingAnimComplete += AwaitScalingAnim;
                executing = false;
                display.HideDisplay();
            }
        }
    }

    private void AwaitScalingAnim() {
        display.OnScalingAnimComplete -= AwaitScalingAnim;
        CompleteTask();
    }
}