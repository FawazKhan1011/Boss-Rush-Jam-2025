using System.Collections.Generic;
using UnityEngine;

public class LineDrawing : MonoBehaviour
{
    public LineRenderer lineRenderer; // Assign a Line Renderer in the inspector
    public Transform staffTip; // The tip of the staff for the line origin
    public Material lineMaterial; // Optional: Assign a glowing material for the line
    public float lineSegmentMinDistance = 0.1f; // Minimum distance for new points
    public float shrinkSpeed = 5f; // Speed at which the line shrinks
    public Transform boss; // Assign the boss object in the inspector for collision check
    public float collisionCheckInterval = 0.1f; // Interval in seconds for checking collisions

    private List<Vector3> linePoints = new List<Vector3>();
    private bool isDrawing = false;
    private bool isShrinking = false;
    private Vector3 centerPoint;
    private float lastCollisionCheckTime = 0f; // Track time for collision checks

    public Boss bossScript;

    // New variable: the radius used to check for collisions along the line.
    // Adjust as needed for your game’s scale.
    public float collisionRadius = 0.1f;

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

        // Handle shrinking logic
        if (isShrinking)
        {
            ShrinkLine();
        }
    }

    void StartLine()
    {
        FindAnyObjectByType<AudioManager>().Play("zap");
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

    public void EndLine()
    {
        FindAnyObjectByType<AudioManager>().Stop("zap");
        isDrawing = false;

        // Calculate the center of the line
        centerPoint = CalculateCenter();
        isShrinking = true;
    }

    Vector3 CalculateCenter()
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 point in linePoints)
        {
            sum += point;
        }
        return sum / linePoints.Count;
    }

    void ShrinkLine()
    {
        for (int i = 0; i < linePoints.Count; i++)
        {
            // Move each point towards the center
            linePoints[i] = Vector3.MoveTowards(linePoints[i], centerPoint, shrinkSpeed * Time.deltaTime);
        }

        lineRenderer.positionCount = linePoints.Count;
        for (int i = 0; i < linePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, linePoints[i]);
        }

        // Check for collisions at an interval to reduce frequency
        if (Time.time - lastCollisionCheckTime >= collisionCheckInterval)
        {
            lastCollisionCheckTime = Time.time;
            CheckCollisions();
        }

        // Check if all points have reached the center
        if (linePoints.TrueForAll(point => Vector3.Distance(point, centerPoint) < 0.1f))
        {
            isShrinking = false;
            lineRenderer.positionCount = 0; // Clear the line
        }
    }

    void CheckCollisions()
    {
        // Use Physics.OverlapSphere to check if any line point is overlapping the boss collider
        foreach (Vector3 point in linePoints)
        {
            // Get all colliders overlapping this point within collisionRadius
            Collider[] hitColliders = Physics.OverlapSphere(point, collisionRadius);
            foreach (Collider col in hitColliders)
            {
                if (boss != null && col.gameObject == boss.gameObject)
                {
                    // The line is actually hitting the boss: stop shrinking, deal damage, and clear the line.
                    DealDamage();
                    ClearLine();
                    return;
                }
            }
        }
    }

    void DealDamage()
    {
        Debug.Log("Boss hit! Damage dealt.");
        if (boss != null)
        {
            bossScript.TakeDamage(1);
        }
    }

    void ClearLine()
    {
        // Clear the line immediately after dealing damage
        lineRenderer.positionCount = 0;
        linePoints.Clear();
        isShrinking = false;
    }

    public void InstantClearLine()
    {
        FindAnyObjectByType<AudioManager>().Stop("zap");
        if (isDrawing)
        {
            FindAnyObjectByType<AudioManager>().Play("glass1");
        }
        lineRenderer.positionCount = 0; // Remove the line visually
        linePoints.Clear(); // Clear all the points
        isDrawing = false; // Stop drawing
        isShrinking = false; // Stop shrinking
    }
}
