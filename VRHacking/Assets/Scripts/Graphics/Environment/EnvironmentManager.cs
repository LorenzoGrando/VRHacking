using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    private bool isGlitched = false;
    [SerializeField]
    private Color baseColor, glitchColor;
    [Header("Skybox")]
    [SerializeField]
    private Material skyboxMat;
    [SerializeField]
    private float skyboxRotationSpeed = 2.5f;
    private float internalRotationSpeed;
    private float internalRotationValue = 0;
    [Header("Ground")]
    [SerializeField]
    private Material groundMat;
    [SerializeField]
    private float groundDisplaceSpeed = 0.85f;

    [Header("Dissolves")]
    [SerializeField]
    private GameObject[] dissolveObjects;
    private Material[] dissolveMats;
    [SerializeField]
    public float maxDissolveHeight;

    [Header("Glitch")]
    [SerializeField]
    private float timeToGlitch;
    [SerializeField]
    private float glitchedSkyboxSpeed;
    [SerializeField]
    private float glitchedGroundSpeed;

    void OnEnable()
    {
        SetupMats();
        ResetToDefault();
    }

    private void Update() {
        RotateSkybox();
    }

    private void SetupMats() {
        if(skyboxMat == null) {
            skyboxMat = RenderSettings.skybox;
        }
        if(dissolveMats == null && dissolveObjects != null) {
            dissolveMats = new Material[dissolveObjects.Length];
            for(int i = 0; i < dissolveObjects.Length; i++) {
                dissolveMats[i] = dissolveObjects[i].GetComponent<MeshRenderer>().material;
            }
        }
    }

    private void ResetToDefault() {
        UpdateGroundDisplacement(groundDisplaceSpeed);
        UpdateMats(baseColor);
        skyboxMat.SetFloat("_Rotation", skyboxRotationSpeed);
        SetDissolveMats(maxDissolveHeight);
    }

    public void ChangeWorldState(bool isGlitch) {
        if(isGlitch) {
            DOVirtual.Float(skyboxRotationSpeed, glitchedSkyboxSpeed, 0.25f, (x) => internalRotationSpeed = x);
            DOVirtual.Float(groundDisplaceSpeed, glitchedGroundSpeed, 0.25f, (x) => UpdateGroundDisplacement(x));

            DOVirtual.Color(baseColor, glitchColor, 0.25f, (x) => UpdateMats(x)).OnComplete(() => ChangeWorldState(isGlitched = !isGlitched));
        }
        else {
            DOVirtual.Float(glitchedSkyboxSpeed, skyboxRotationSpeed, timeToGlitch, (x) => internalRotationSpeed = x);
            DOVirtual.Float(glitchedGroundSpeed, groundDisplaceSpeed, timeToGlitch, (x) => UpdateGroundDisplacement(x));
            
            DOVirtual.Color(glitchColor, baseColor, timeToGlitch, (x) => UpdateMats(x));
        }
    }

    private void UpdateMats(Color color) {
        skyboxMat.SetColor("_Tint", color);
        groundMat.color = color;
        foreach(Material mat in dissolveMats) {
            mat.color = color;
        }
    }

    private void UpdateGroundDisplacement(float value) {
        groundMat.SetFloat("_ScrollSpeed", value);
    }

    private void RotateSkybox() {
        if (skyboxMat != null) {
            internalRotationValue += internalRotationSpeed * Time.deltaTime;
            if(internalRotationValue > 360) {
                internalRotationValue -= 360;
            }

            skyboxMat.SetFloat("_Rotation", internalRotationValue);
        }
    }

    private void SetDissolveMats(float value) {
        foreach(Material mat in dissolveMats) {
            mat.SetFloat("_CutoffHeight", value);
        }
    }

    public void DissolveWorld(float duration, bool isInit) {
        if(isInit) {
            DOVirtual.Float(maxDissolveHeight, -maxDissolveHeight/2, duration, (x) => SetDissolveMats(x));
        }
        else {
            DOVirtual.Float(-maxDissolveHeight/2, maxDissolveHeight, duration, (x) => SetDissolveMats(x));
        }
    }
 }
