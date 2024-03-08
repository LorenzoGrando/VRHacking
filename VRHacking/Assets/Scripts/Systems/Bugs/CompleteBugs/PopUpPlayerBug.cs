using UnityEngine;

public class PopUpPlayerBug : PlayerBug
{
    protected override void ResetBug()
    {
        throw new System.NotImplementedException();
    }

    protected override void CompleteBug()
    {
        base.CompleteBug();
    }

    public override bool CheckBugCompleted()
    {
        throw new System.NotImplementedException();
    }

    public override void StartBug()
    {
        throw new System.NotImplementedException();
    }
}