using UnityEngine;
using TMPro;

public abstract class PokeButtonUI : MonoBehaviour
{
    protected TextMeshProUGUI textRef;

    public abstract void OnButtonPressed();
}