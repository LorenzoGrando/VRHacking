using UnityEngine;

public class HackerBugButton : PokeButtonUI
{
    [SerializeField]
    private HackerMainDisplay display;

    void Start()
    {
        if(display == null)
            display = FindObjectOfType<HackerMainDisplay>();
    }

    public override void OnButtonPressed()
    {
        throw new System.NotImplementedException();
    }
}