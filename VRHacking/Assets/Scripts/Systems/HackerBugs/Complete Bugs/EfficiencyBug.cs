using UnityEngine;

public class EfficiencyBug : HackerBug
{
    public override void OnBugUpload()
    {
        Debug.Log("Bug Uploaded");
        hackerManager.SetTaskCompletionModifier(modifier);
        StartCoroutine(routine: ExecuteBugTimer());
    }

    public override void OnBugEnd()
    {
        hackerManager.SetTaskCompletionModifier(1);
    }

}