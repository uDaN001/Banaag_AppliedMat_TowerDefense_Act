using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierPath : MonoBehaviour
{
    public Transform[] controlPoints; // Assign 3 for Quadratic, 4 for Cubic

    [Header("In-Game LineRenderer Settings")]
    public float lineWidth = 0.1f;
    public bool updateAtRuntime = false; // Enable if control points move during gameplay

    [Header("Gizmo Visualization Settings")]
    public Color pathColor = Color.green;
    public Color handleColor = Color.yellow;
    [Range(10, 100)]
    public int resolution = 30; // Higher = smoother curve preview

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    void Start()
    {
        UpdateLineRendererPositions();
    }

    void Update()
    {
        if (updateAtRuntime)
        {
            UpdateLineRendererPositions();
        }
    }

    void OnValidate()
    {
        // Live update LineRenderer inside the Editor when tweaking values
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            SetupLineRenderer();
            UpdateLineRendererPositions();
        }
    }

    public void SetupLineRenderer()
    {
        if (lineRenderer == null) return;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;
        lineRenderer.useWorldSpace = true;
    }

    public void UpdateLineRendererPositions()
    {
        if (lineRenderer == null || !HasValidControlPoints()) return;

        lineRenderer.positionCount = resolution + 1;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            lineRenderer.SetPosition(i, GetPoint(t));
        }
    }

    private bool HasValidControlPoints()
    {
        if (controlPoints == null || controlPoints.Length < 3) return false;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (controlPoints[i] == null) return false;
        }
        return true;
    }

    public Vector3 GetPoint(float t)
    {
        t = Mathf.Clamp01(t);
        if (!HasValidControlPoints()) return Vector3.zero;

        if (controlPoints.Length == 3)
            return Bezier.QuadraticFast(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, t);

        if (controlPoints.Length == 4)
            return Bezier.CubicFast(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position, t);

        return Vector3.zero;
    }

    public Vector3 GetTangent(float t)
    {
        t = Mathf.Clamp01(t);
        if (!HasValidControlPoints()) return Vector3.zero;

        if (controlPoints.Length == 3)
            return Bezier.QuadraticTangent(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, t);

        if (controlPoints.Length == 4)
            return Bezier.CubicTangent(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position, t);

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (!HasValidControlPoints()) return;

        // 1. Draw handle lines connecting the control points
        Gizmos.color = handleColor;
        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
        }

        // 2. Draw small spheres at each control point position
        for (int i = 0; i < controlPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(controlPoints[i].position, 0.25f);
        }

        // 3. Render the smooth Bezier path
        Gizmos.color = pathColor;
        Vector3 previousPoint = GetPoint(0f);

        for (int i = 1; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            Vector3 currentPoint = GetPoint(t);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}