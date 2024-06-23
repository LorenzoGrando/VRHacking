using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIPinManager : MonoBehaviour
{
    public event Action OnNewPinAdded;
    [SerializeField]
    UILineRenderer lineRenderer;

    [SerializeField]
    public UIPinButton[] pins;
    [SerializeField]
    private Vector3 targetPinScale;

    [SerializeField]
    public List<GameObject> activePoints;

    public HackerBug activeBug;

    [SerializeField]
    private GameObject startMarker;

    [SerializeField]
    private GameObject activePinMarker;

    public void RenderLine(GameObject[] activeObjects) {
        Vector2[] points = new Vector2[activeObjects.Length];
        for(int i = 0; i < points.Length; i++) {
            points[i] = activeObjects[i].GetComponent<RectTransform>().anchoredPosition3D;
        }

        lineRenderer.SetParameters(points);
    }

    public void UpdateActivePins(GameObject[] points) {
        activePoints = new List<GameObject>();
        foreach(GameObject point in points) {
            activePoints.Add(point);
        }

        if(startMarker != null) {
            startMarker.SetActive(true);
            startMarker.transform.position = points[0].transform.position;
        }

        RenderLine(activePoints.ToArray());
    }

    public void AddNewPin(GameObject point) {
        if(!activePoints.Contains(point)) {
            activePoints.Add(point);

            RenderLine(activePoints.ToArray());
            if(!activePinMarker.activeSelf)
                activePinMarker.SetActive(true);

            activePinMarker.transform.position = activePoints[^1].transform.position;
            activePinMarker.transform.localScale = Vector3.zero;
            activePinMarker.transform.DOScale(targetPinScale * 1.125f, 0.25f);
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
        RenderLine(activePoints.ToArray());
        if(activePoints.Count > 0)
            activePinMarker.transform.position = activePoints[^1].transform.position;
        else
            activePinMarker.SetActive(false);
    }

    public void ClearLines() {
        activePoints = new List<GameObject>();
        activePinMarker.SetActive(false);
        RenderLine(activePoints.ToArray());
    }

    public void ActivateBug() {
        if(activeBug != null) {
            activeBug.BeginBugUpload();
            activeBug = null;
        }
    }

    public Sequence AnimatePins(bool isInit, bool animateLine) {
        Sequence sequence = DOTween.Sequence();
        float duration = 0.25f;

        if(isInit) {
            for(int i = 0; i < pins.Length; i++) {
                pins[i].transform.localScale = Vector3.zero;
                Tween tween = pins[i].transform.DOScale(targetPinScale, duration);
                duration += 0.075f;
                sequence.Insert(0, tween);
            }
            if(startMarker != null) {
                startMarker.transform.localScale = Vector3.zero;
                sequence.Append(startMarker.transform.DOScale(targetPinScale * 1.125f, 0.25f));
            }

            //linerenderer
            if(animateLine) {
                Vector2 values;
                values = isInit ? new Vector2(0,1) : new Vector2(1,0);
                Tween virtualFloat = DOVirtual.Float(values.x, values.y, 0.4f, t => {lineRenderer.SetThickness(t);});
                virtualFloat.SetEase(Ease.InCubic);
                sequence.Append(virtualFloat);
            }
            else {
                lineRenderer.SetExactThickness(0.15f);
            }
        }
        else {
            //linerenderer
            sequence.AppendInterval(0.085f);
            if(animateLine) {
                Vector2 values;
                values = isInit ? new Vector2(0,1) : new Vector2(1,0);
                Tween virtualFloat = DOVirtual.Float(values.x, values.y, 0.25f, t => {lineRenderer.SetThickness(t);});
                sequence.Append(virtualFloat);
            }
            else {
                lineRenderer.SetExactThickness(0.15f);
            }

            if(activePinMarker != null) {
                sequence.Insert(0 ,activePinMarker.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => activePinMarker.SetActive(false)));
            }
            if(startMarker != null) {
                sequence.Insert(0 ,startMarker.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => startMarker.SetActive(false)));
            }
            for(int i = 0; i < pins.Length; i++) {
                Tween tween = pins[i].transform.DOScale(Vector3.zero, duration);
                duration += 0.025f;
                sequence.Insert(0, tween);
            }
        }

        return sequence;
    }
}
