using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenDisplay : MonoBehaviour
{
    [Header("Task Slider")]
    [SerializeField]
    private Slider taskSlider;
    [Header("Visualizers")]
    [SerializeField]
    private HackerBug[] hackerBugs;
    [SerializeField]
    private Image[] bugVisualizers;

    void OnEnable()
    {
        StartDisplay();
        UpdatedVisualizers();
    }

    public void StartDisplay() {
        foreach(HackerBug hackerBug in hackerBugs) {
            hackerBug.OnCooldownStart += UpdatedVisualizers;
            hackerBug.OnCooldownEndTimer += UpdatedVisualizers;
        }

        ResetValues();
    }

    private void ResetValues() {
        taskSlider.value = 0;
    }

    public void UpdatedVisualizers() {
        for(int i = 0; i < hackerBugs.Length; i++) {
            if(hackerBugs[i].cooldownTimer > 0) {
                bugVisualizers[i].color = Color.gray;
            }

            else {
                bugVisualizers[i].color = Color.green;
            }
        }
    }

    public void UpdateTaskSlider(float value) {
        taskSlider.value = value;
    }
}
