using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FindAndSeekDisplay : MonoBehaviour
{
    public event Action OnScalingAnimComplete;
    [SerializeField]
    private GameObject displayHolder, iconHolder;
    [SerializeField]
    private UIRestrainer imageRestrainer;
    [SerializeField]
    private GameObject iconPrefab;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    private List<FindAndSeekIcon> icons;
    [SerializeField] private Transform bottomLeftAnchor, upperRightAnchor;
    private Vector2 imageCenter;
    [SerializeField]
    private Vector2 minIconDistance;
    public void HideDisplay()
    {
        foreach(FindAndSeekIcon icon in icons) {
            icon.gameObject.SetActive(false);
        }

        descriptionText.transform.DOScale(0, 0.65f).OnComplete(() => OnFinishAnim(false));
    }

    public void ResetDisplay()
    {
        foreach(FindAndSeekIcon icon in icons) {
            icon.gameObject.SetActive(false);
        }
        CalculateImageCenter();
        iconHolder.transform.position = imageCenter;
    }

    void Update() {
        if(iconHolder.activeSelf) {
            imageRestrainer.TryRestrain(true);
        }
    }

    public void InitiateDisplay() {
        displayHolder.gameObject.SetActive(true);
        iconHolder.transform.localPosition = Vector3.zero;
        int iconsLength = Enum.GetValues(typeof(FindAndSeekTask.TargetIcon)).Length;
        if(icons == null ||  iconsLength - 1 != icons.Count) {
            icons = new List<FindAndSeekIcon>();
            GenerateIcons(iconsLength - 1);
        }

        for(int i = 0; i < icons.Count; i++) {
            icons[i].gameObject.SetActive(true);
        }
        ScaleIcons(true);
        descriptionText.transform.DOScale(1, 0.45f);
    }

    private void CalculateImageCenter() {
        imageCenter.x = Mathf.Lerp(bottomLeftAnchor.position.x, upperRightAnchor.position.x, 0.5f);
        imageCenter.y = Mathf.Lerp(bottomLeftAnchor.position.y, upperRightAnchor.position.y, 0.5f);
    }

    private void GenerateIcons(int amount) {
        if(icons != null || icons.Count >= 0) {
            foreach(var icon in icons) {
                Destroy(icon);
            }
            icons.Clear();
        }

        for(int i = 0; i < amount; i++) {
            FindAndSeekIcon icon = Instantiate(iconPrefab).GetComponent<FindAndSeekIcon>();
            icon.transform.parent = iconHolder.transform;
            icon.UpdateAppearance((FindAndSeekTask.TargetIcon) i+1);
            icon.transform.localPosition = GenerateIconPosition();
            icon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private Vector2 GenerateIconPosition() {
        float xLength = upperRightAnchor.position.x - bottomLeftAnchor.position.x;
        float yLength = upperRightAnchor.position.y - bottomLeftAnchor.position.y;

        //primary SDF
        Vector2 distanceRangeX = new Vector2(imageCenter.x - xLength/2, imageCenter.x - minIconDistance.x);
        Vector2 distanceRangeY = new Vector2(imageCenter.y - yLength/2, imageCenter.y - minIconDistance.y);
        
        float x = UnityEngine.Random.Range(distanceRangeX.x, distanceRangeX.y);
        float y = UnityEngine.Random.Range(distanceRangeY.x, distanceRangeY.y);

        bool flipX = UnityEngine.Random.Range(0,2) == 0;
        bool flipY = UnityEngine.Random.Range(0,2) == 0;

        Vector2 finalPos = new Vector2(flipX ? Mathf.Abs(x) : x, flipY ? Mathf.Abs(y) : y);

        return finalPos;
    }

    private void ScaleIcons(bool isEntry) {
        float targetScale = isEntry ? 1 : 0;
        for(int i = 0; i < icons.Count; i++) {
            if(!isEntry)
                icons[i].transform.localScale = Vector3.zero;
            icons[i].transform.DOScale(targetScale, 0.5f);
        }
    }

    public void OnFinishAnim(bool isEntry) {
        OnScalingAnimComplete?.Invoke();

        if(!isEntry) {
            displayHolder.SetActive(false);
        }
    }
}