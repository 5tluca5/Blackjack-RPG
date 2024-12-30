using UnityEngine;

[ExecuteAlways] // Runs in the editor for live updates
public class CurvedHorizontalLayout3D : MonoBehaviour
{
    [Header("Layout Settings")]
    public float spacing = 1.0f; // Space between objects
    public float curve = 1.0f; // Curve of the layout (higher = more curve)
    public bool curveOnYAxis = true; // Curve along the Y-axis or Z-axis

    void Update()
    {
        AlignChildrenWithCurve();
    }

    private void AlignChildrenWithCurve()
    {
        if (transform.childCount == 0) return;

        float totalWidth = 0f;
        Transform[] children = new Transform[transform.childCount];

        // Collect children and calculate total width
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
            totalWidth += children[i].localScale.x + (i < transform.childCount - 1 ? spacing : 0);
        }

        // Calculate starting position
        float startX = -totalWidth / 2f;

        // Align children with curve
        float currentX = startX;

        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            float halfWidth = child.localScale.x / 2f;

            // Calculate horizontal position
            float xPosition = currentX + halfWidth;

            // Apply curve
            float curvedOffset = Mathf.Sin((i / (float)(children.Length - 1)) * Mathf.PI) * curve;

            // Determine axis for curve (Y-axis or Z-axis)
            float yPosition = child.localPosition.y;
            float zPosition = !curveOnYAxis ? curvedOffset : child.localPosition.z;

            // Set new position
            child.localPosition = new Vector3(xPosition, yPosition, zPosition);

            // Update X position for next child
            currentX += child.localScale.x + spacing;
        }
    }
}
