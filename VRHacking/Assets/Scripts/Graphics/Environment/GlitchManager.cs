using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public HandController[] controllers;
    public GameObject textHolderObject;
    public GameObject displayHolder;
    public Material mainMonitorMat;
    public Texture2D[] monitorTextures;
    public float glitchDuration;
    private EnvironmentManager environmentManager;
    [SerializeField]
    private Material mat;
    private const string typoChars = "abcdefghijklmnopqrstuvwxyz;ABCDEFGHIJKLMNOPQRSTUVWXYZ^.,";
    private char[] chars;
    private List<GlitchableText> targetTexts;
    private Coroutine glitchRoutine;
    private float initialTime;

    private struct GlitchableText {
        public TextMeshProUGUI textObject;
        public string originalText;
    }

    void Start()
    {
        chars = typoChars.ToCharArray();
        targetTexts= new List<GlitchableText>();
        environmentManager = FindObjectOfType<EnvironmentManager>();
    }

    private IEnumerator ExecuteGlitch(float duration) {
        while(Time.time < initialTime + duration) {
            foreach(var text in targetTexts) {
                int rng = UnityEngine.Random.Range(0, 3);
                if(rng > 0) {
                    text.textObject.text = GlitchSingleText(text.originalText);
                }
            }

            yield return null;
        }

        EndGlitches();
        glitchRoutine = null;
        yield break;
    }

    private string GlitchSingleText(string originalString) {
        char[] brokenString = originalString.ToCharArray();

        for(int i = 0; i < brokenString.Length; i++) {
            if(brokenString[i] != ' ') {
                brokenString[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            }
        }

        return brokenString.ToString();
    }

    public void CallGlitch() {
        targetTexts.Clear();
        TextMeshProUGUI[] texts = textHolderObject.GetComponentsInChildren<TextMeshProUGUI>();

        foreach(TextMeshProUGUI text in texts) {
            GlitchableText t = new GlitchableText() {
                originalText = text.text,
                textObject = text
            };

            targetTexts.Add(t);
        }

        displayHolder.SetActive(true);
        mat.EnableKeyword("_GLITCH_ON");
        mainMonitorMat.SetTexture("_BaseTex", monitorTextures[1]);
        environmentManager.ChangeWorldState(true);
        glitchRoutine = StartCoroutine(routine: ExecuteGlitch(glitchDuration));
        foreach(HandController controller in controllers) {
            controller.ChangeInteractorState(false);
        }
    }

    public void EndGlitches() {
        foreach(GlitchableText t in targetTexts) {
            t.textObject.text = t.originalText;
        }

        mainMonitorMat.SetTexture("_BaseTex", monitorTextures[0]);
        mat.DisableKeyword("_GLITCH_ON");
        displayHolder.SetActive(false);
        foreach(HandController controller in controllers) {
            controller.ChangeInteractorState(true);
        }
    }
}