using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public abstract class PokeButtonUI : MonoBehaviour
{
    protected TextMeshProUGUI textRef;

    public abstract void OnButtonPressed();

    public abstract void OnXRUIHover(UIHoverEventArgs enterArgs);

    public abstract void OnXRUIHoverExit(UIHoverEventArgs exitArgs);
}