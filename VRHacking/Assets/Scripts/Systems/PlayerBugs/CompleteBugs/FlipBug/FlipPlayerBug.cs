using UnityEngine;
using DG.Tweening;

public class FlipPlayerBug : PlayerBug
{
    [SerializeField]
    private GameObject canvasObject;
    [SerializeField]
    private float flipDuration;
    [SerializeField]
    private float flipSpeed;

    private Sequence sequence;

    public override bool CheckBugCompleted()
    {
        return true;
    }

    public override void StartBug(GameSettingsData data)
    {
        StartFlipSequence();
    }

    protected override void ResetBug()
    {   
        if(sequence != null) {
            sequence.Kill(false);
            sequence = null;
        }

        canvasObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void StartFlipSequence() {
        sequence = DOTween.Sequence();
        sequence.Append(canvasObject.transform.DOLocalRotate(new Vector3(0,0,-180), flipSpeed, RotateMode.Fast));
        sequence.AppendInterval(flipDuration);
        sequence.Append(canvasObject.transform.DOLocalRotate(Vector3.zero, flipSpeed, RotateMode.FastBeyond360).OnComplete(() => CompleteBug()));
        sequence.Play();
    }
}