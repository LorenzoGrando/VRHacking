using UnityEngine;

public class UIBugUploadButton : MonoBehaviour
{
    [SerializeField]
    private HackerMainDisplay hackerMainDisplay;

    [SerializeField]
    private RectTransform[] thisPinConfig;

    public void OnClick() {
        hackerMainDisplay.InitiateTask(thisPinConfig);
    }
}