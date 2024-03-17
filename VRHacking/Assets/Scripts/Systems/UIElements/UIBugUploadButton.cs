using UnityEngine;

public class UIBugUploadButton : MonoBehaviour
{
    [SerializeField]
    private HackerMainDisplay hackerMainDisplay;

    [SerializeField]
    private GameObject[] thisPinConfig;
    [SerializeField]
    private HackerBug thisButtonBug;

    public void OnClick() {
        hackerMainDisplay.InitiateTask(thisPinConfig, thisButtonBug);
    }
}