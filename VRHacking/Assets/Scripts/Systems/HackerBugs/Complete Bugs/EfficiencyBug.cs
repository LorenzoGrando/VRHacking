using UnityEngine;

public class EfficiencyBug : HackerBug
{
    public override void OnBugUpload()
    {
        hackerManager.SetTaskCompletionModifier(modifier);
        base.OnBugUpload();
    }

    public override void OnBugEnd()
    {
        hackerManager.SetTaskCompletionModifier(1);
    }

    public override void OnCooldownEnd()
    {
        base.OnCooldownEnd();
    }

}