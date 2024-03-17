using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIPinManager : MonoBehaviour
{
    public event Action OnNewPinAdded;
    [SerializeField]
    UILineRenderer lineRenderer;

    [SerializeField]
    public List<GameObject> activePoints;

    public HackerBug activeBug;

    [SerializeField]
    private GameObject startMarker;

    void OnEnable()
    {
        RenderLine();
    }

    void OnValidate()
    {
        RenderLine();
    }

    public void RenderLine() {
        Vector2[] points = new Vector2[activePoints.Count];
        for(int i = 0; i < points.Length; i++) {
            points[i] = activePoints[i].GetComponent<RectTransform>().anchoredPosition3D;
        }

        lineRenderer.SetParameters(points);
    }

    public void UpdateActivePins(GameObject[] points) {
        activePoints = new List<GameObject>();
        foreach(GameObject point in points) {
            activePoints.Add(point);
        }
        if(startMarker != null)
            startMarker.transform.position = activePoints[0].transform.position;

        RenderLine();
    }

    public void AddNewPin(GameObject point) {
        if(!activePoints.Contains(point)) {
            activePoints.Add(point);

            RenderLine();
            OnNewPinAdded?.Invoke();
        }
    }

    public void RemovePin(GameObject point) {
        int targetIndex = 0;

        for(int i =0; i < activePoints.Count; i++) {
            if(activePoints[i].name == point.name) {
                targetIndex = i;
                break;
            }
        }

        for(int r = targetIndex; r < activePoints.Count; r++) {
            UIPinButton pinButton = activePoints[r].GetComponent<UIPinButton>();
            pinButton.ChangePinStatus(false);
        }

        activePoints.RemoveRange(targetIndex, activePoints.Count - targetIndex);
        RenderLine();
    }

    public void ClearLines() {
        activePoints = new List<GameObject>();
        
        RenderLine();
    }

    public void ActivateBug() {
        activeBug.BeginBugUpload();
        activeBug = null;
    }
}
