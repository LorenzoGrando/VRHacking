using UnityEngine;

public class MinesPlayerBug : PlayerBug
{
    [SerializeField]
    private TaskManager taskManager;
    public override bool CheckBugCompleted()
    {
        return true;
    }

    public override void StartBug(GameSettingsData data)
    {
        CallMines();
    }

    protected override void ResetBug()
    {
        //Nothing to reset
    }

    private void CallMines() {
        if(taskManager == null) {
            FindObjectOfType<TaskManager>();
        }

        taskManager.enableMines = true;
        CompleteBug();
    }
}