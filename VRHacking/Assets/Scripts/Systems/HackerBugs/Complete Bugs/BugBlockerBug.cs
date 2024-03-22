using UnityEngine;

public class BugBlockerBug : HackerBug
{
    public override void OnBugUpload()
    {
        hackerManager.TriggerBlockNextBug();
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