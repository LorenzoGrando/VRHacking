using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class UIBugUploadButton : MonoBehaviour
{
    [Header("Functionality")]
    [SerializeField]
    private HackerMainDisplay hackerMainDisplay;

    [SerializeField]
    private GameObject[] thisPinConfig;
    [SerializeField]
    private HackerBug thisButtonBug;
    [SerializeField]
    private GameObject buttonNameText;

    [Header("Cooldown")]
    [SerializeField]
    private GameObject cooldownHolder;
    [SerializeField]
    private TextMeshProUGUI cooldownText;

    private Button thisButton;
    private bool isOnCooldown;

    private void OnEnable()
    {
        thisButtonBug.OnCooldownStart += StartCooldown;
        thisButtonBug.OnCooldownEndTimer += EndCooldown;

        if(thisButtonBug.cooldownTimer > 0) {
            StartCooldown();
        }

        else {
            EndCooldown();
        }
    }

    private void OnDisable()
    {
        thisButtonBug.OnCooldownStart -= StartCooldown;
        thisButtonBug.OnCooldownEndTimer -= EndCooldown;
    }

    void Update()
    {
        if(isOnCooldown) {
            int timeInSeconds = Mathf.RoundToInt(thisButtonBug.cooldownTimer);
            cooldownText.text = $"{timeInSeconds}s";
        }
    }

    public void OnClick() {
        hackerMainDisplay.InitiateTask(thisPinConfig, thisButtonBug);
    }

    public void StartCooldown() {
        if(thisButton == null) {
            thisButton = GetComponent<Button>();
        }
        isOnCooldown = true;
        thisButton.interactable = false;
        buttonNameText.SetActive(false);
        cooldownHolder.SetActive(true);
    }

    public void EndCooldown() {
        if(thisButton == null) {
            thisButton = GetComponent<Button>();
        }
        isOnCooldown = false;
        thisButton.interactable = true;
        cooldownHolder.SetActive(false);
        buttonNameText.SetActive(true);
    }
}