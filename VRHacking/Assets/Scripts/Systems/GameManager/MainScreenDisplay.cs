using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenDisplay : MonoBehaviour
{
    [Header("Holders")]
    [SerializeField]
    private GameObject startMenuHolder, startEndlessHolder, endlessCooldownHolder;
    [Header("Sliders")]
    [SerializeField]
    private Slider taskSlider;
    [SerializeField]
    private Slider cooldownSlider;
    [Header("Visualizers")]
    [SerializeField]
    private HackerBug[] hackerBugs;
    [SerializeField]
    private Image[] bugVisualizers;

    void OnEnable()
    {
        UpdatedVisualizers();
    }

    public void StartGameDisplay() {
        foreach(HackerBug hackerBug in hackerBugs) {
            hackerBug.OnCooldownStart += UpdatedVisualizers;
            hackerBug.OnCooldownEndTimer += UpdatedVisualizers;
        }

        ResetValues();
        endlessCooldownHolder.SetActive(false);
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

    public void UpdateCooldownSlider(float value) {
        cooldownSlider.value = value;
    }

    public void StartMenu() {
        startEndlessHolder.SetActive(false);

        startMenuHolder.SetActive(true);
    }

    public void ActivateMenuByMode(GameSettingsData.GameMode mode) {
        startMenuHolder.SetActive(false);

        switch(mode) {
            case GameSettingsData.GameMode.Campaign:

            break;
            case GameSettingsData.GameMode.Endless:
            startEndlessHolder.SetActive(true);

            //update data about the display
            break;
            case GameSettingsData.GameMode.Tutorial:

            break;
        }
    }

    public void OnEndDispute(GameSettingsData.GameMode mode, bool playerWon) {
        switch(mode) {
            case GameSettingsData.GameMode.Campaign:

            break;
            case GameSettingsData.GameMode.Endless:
                if(playerWon) {
                    endlessCooldownHolder.SetActive(true);
                }

                else {
                    startEndlessHolder.SetActive(true);
                }

            break;
            case GameSettingsData.GameMode.Tutorial:

            break;
        }
    }
}
