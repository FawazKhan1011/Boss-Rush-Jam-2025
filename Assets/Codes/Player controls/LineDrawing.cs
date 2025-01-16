using System.Collections.Generic;
using UnityEngine;

public class LineDrawing : MonoBehaviour
{
    public LineRenderer lineRenderer; // Assign a Line Renderer in the inspector
    public Transform staffTip; // The tip of the staff for the line origin
    public Material lineMaterial; // Optional: Assign a glowing material for the line
    public float lineSegmentMinDistance = 0.1f; // Minimum distance for new points

    private List<Vector3> linePoints = new List<Vector3>();
    private bool isDrawing = false;

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
        }
    }

    void Update()
    {
        // Check for right mouse button input
        if (Input.GetMouseButtonDown(1))
        {
            StartLine();
        }

        if (Input.GetMouseButton(1))
        {
            UpdateLine();
        }

        if (Input.GetMouseButtonUp(1))
        {
            EndLine();
        }
    }

    void StartLine()
    {
        linePoints.Clear();
        lineRenderer.positionCount = 0;
        isDrawing = true;

        // Add the initial point at the staff tip
        Vector3 startPoint = staffTip.position;
        linePoints.Add(startPoint);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);
    }

    void UpdateLine()
    {
        if (isDrawing)
        {
            Vector3 currentPoint = staffTip.position;

            // Only add a new point if it's far enough from the last one
            if (linePoints.Count == 0 || Vector3.Distance(currentPoint, linePoints[linePoints.Count - 1]) > lineSegmentMinDistance)
            {
                linePoints.Add(currentPoint);
                lineRenderer.positionCount = linePoints.Count;
                lineRenderer.SetPosition(linePoints.Count - 1, currentPoint);
            }
        }
    }

    void EndLine()
    {
        isDrawing = false;

        // Perform logic when the line is completed
        Debug.Log("Line drawing ended. Total points: " + linePoints.Count);

        // Additional mechanics like checking for line closure or applying effects can be added here
    }
}
