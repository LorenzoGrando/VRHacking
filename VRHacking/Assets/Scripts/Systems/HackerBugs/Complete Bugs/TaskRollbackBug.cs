using UnityEngine;

public class TaskRollbackBug : HackerBug
{
    public override void OnBugUpload()
    {
        hackerManager.ModifyCompletedTasks(Mathf.RoundToInt(modifier));
        base.OnBugUpload();
    }

    public override void OnBugEnd()
    {
        return;
    }

    public override void OnCooldownEnd()
    {
        base.OnCooldownEnd();
    }
}