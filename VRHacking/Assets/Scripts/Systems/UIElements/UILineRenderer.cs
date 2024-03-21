using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UILineRenderer : Graphic
{
    private Vector2[] points;
    [SerializeField]
    private float targetThickness;
    private float thickness;
    [SerializeField]
    private bool centerOnCanvas;

    public void SetParameters(Vector2[] pointsArray) {
        points = pointsArray;
        SetVerticesDirty();
    }

    public void SetThickness(float interpolatorValue) {
        thickness = Mathf.Lerp(0, targetThickness, interpolatorValue);
        SetVerticesDirty();
    }

    public void SetExactThickness(float value) {
        thickness = value;
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        //cant make a line off of a single point
        if(points.Length < 2) {
            return;
        }

        for(int i = 0; i < points.Length - 1; i++) {
            CreateLine(points[i], points[i+1], vh);

            int index = i * 5;

            vh.AddTriangle(index, index+1, index+3);
            vh.AddTriangle(index+3, index+2, index);

            
            
            
        }
    }

    private void CreateLine(Vector3 point1, Vector3 point2, VertexHelper vh) {
        Vector3 offset = centerOnCanvas ? (rectTransform.sizeDelta / 2) : Vector2.zero;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        Quaternion point1Rotation = Quaternion.Euler(0,0, RotatePointTowardsObject(point1, point2) - 90);
        vertex.position = point1Rotation * new Vector3(-thickness/2, 0);
        vertex.position += point1 - offset;
        vh.AddVert(vertex);
        vertex.position = point1Rotation * new Vector3(thickness / 2, 0);
        vertex.position += point1 - offset;
        vh.AddVert(vertex);

        Quaternion point2Rotation = Quaternion.Euler(0, 0, RotatePointTowardsObject(point2, point1) + 90);
        vertex.position = point2Rotation * new Vector3(-thickness / 2, 0);
        vertex.position += point2 - offset;
        vh.AddVert(vertex);
        vertex.position = point2Rotation * new Vector3(thickness / 2, 0);
        vertex.position += point2 - offset;
        vh.AddVert(vertex);

        vertex.position = point2 - offset;
        vh.AddVert(vertex);
    }

    private float RotatePointTowardsObject(Vector2 point1, Vector2 point2) {
        return (float)(Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * (180 / Mathf.PI));
    }
}
