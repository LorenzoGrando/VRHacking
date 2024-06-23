using UnityEngine;
using DG.Tweening;

public class DissolvePlayerBug : PlayerBug
{
    [SerializeField]
    private float bugCompletionDuration;
    [SerializeField]
    private GameObject[] anchorSpots;
    [SerializeField]
    private GameObject monitorObject, hackerObject;
    [SerializeField]
    private AudioSource source;
    private int currentIndex = 0;
    private Sequence durationSequence;
    private EnvironmentManager environmentManager;
    [SerializeField]
    private Vector3 targetMonitorScale;
    [SerializeField]
    private Vector3 targetHackerScale;
    public override bool CheckBugCompleted()
    {
        return true;
    }

    public override void StartBug(GameSettingsData data)
    {
        StartDissolveSequence();
    }

    protected override void CompleteBug()
    {
        
        base.CompleteBug();
    }

    protected override void ResetBug()
    {
        
        
    }

    void Start()
    {
        environmentManager = FindObjectOfType<EnvironmentManager>();
    }
 
    private int GenerateRandomPosition() {
        int index = UnityEngine.Random.Range(0,anchorSpots.Length);
        if(index == currentIndex) {
            index++;
        }
 
        if(index >= anchorSpots.Length) {
            index = 0;
        }
 
        currentIndex = index;
 
        return index;
    }

    private void StartDissolveSequence() {
        durationSequence = DOTween.Sequence();
        durationSequence.AppendCallback(() => AnimTransition(true));
        durationSequence.AppendInterval(bugCompletionDuration/2);
        durationSequence.AppendCallback(() => ChangeWorldPos());
        durationSequence.AppendInterval(bugCompletionDuration/5f);
        durationSequence.AppendCallback(() => environmentManager.DissolveWorld(bugCompletionDuration/2, false));
        durationSequence.AppendCallback(() => AnimTransition(false));
        durationSequence.AppendInterval(bugCompletionDuration/1.5f).OnComplete(() => CompleteBug());
        source.Play();
    }

    private void ChangeWorldPos() {
        GameObject targetPos = anchorSpots[GenerateRandomPosition()];
        prefabObject.transform.parent = targetPos.transform;
        prefabObject.transform.localPosition = Vector3.zero;
        prefabObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void AnimTransition(bool isDissolve) {
        if(isDissolve) {
            environmentManager.DissolveWorld(bugCompletionDuration/2, true);
            monitorObject.transform.DOScale(Vector3.zero, bugCompletionDuration/4);
            hackerObject.transform.DOScale(Vector3.zero, bugCompletionDuration/4);
        }
        else {
            DOVirtual.Float(0, 1, bugCompletionDuration/2, (x) => Foo(x)).OnComplete(() => source.Stop());
            monitorObject.transform.DOScale(targetMonitorScale, bugCompletionDuration/4);
            hackerObject.transform.DOScale(targetHackerScale, bugCompletionDuration/4);
        }
    }

    private void Foo(float x){}
}