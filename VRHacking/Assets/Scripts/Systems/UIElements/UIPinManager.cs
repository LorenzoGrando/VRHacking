using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIPinManager : MonoBehaviour
{
    [SerializeField]
    UILineRenderer lineRenderer;

    [SerializeField]
    private List<RectTransform> activePoints;

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
            points[i] = activePoints[i].anchoredPosition3D;
        }

        lineRenderer.SetParameters(points);
    }

    public void UpdateActivePins(RectTransform[] points) {
        activePoints = new List<RectTransform>();
        foreach(RectTransform point in points) {
            activePoints.Add(point);
        }

        RenderLine();
    }

    public void AddNewPin(RectTransform point) {
        activePoints.Add(point);

        RenderLine();
    }

    public void RemovePin(RectTransform point) {
        int targetIndex = activePoints.IndexOf(point);

        activePoints.RemoveRange(targetIndex, activePoints.Count - targetIndex);
        RenderLine();
    }

    public void ClearLines() {
        activePoints = new List<RectTransform>();
        
        RenderLine();
    }
}
