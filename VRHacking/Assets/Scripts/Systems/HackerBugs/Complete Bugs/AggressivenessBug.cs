using UnityEngine;

public class AggressivenessBug : HackerBug
{
    public override void OnBugUpload()
    {
        hackerManager.SetBugUploadModifier(modifier);
        base.OnBugUpload();
    }

    public override void OnBugEnd()
    {
        hackerManager.SetBugUploadModifier(1);
    }

    public override void OnCooldownEnd()
    {
        base.OnCooldownEnd();
    }
}