using UnityEngine;

public class FindAndSeekIcon : MonoBehaviour
{
    [SerializeField]
    private GameObject[] sprites;
    public FindAndSeekTask.TargetIcon thisIconType;
    public void UpdateAppearance(FindAndSeekTask.TargetIcon targetAppearance) {
        foreach(GameObject spriteObject in sprites) {
            spriteObject.gameObject.SetActive(false);
        }

        sprites[(int)targetAppearance - 1].SetActive(true);
        thisIconType = targetAppearance;
    }
}