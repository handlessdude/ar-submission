using UnityEngine;

public class DrawColliderBorders : MonoBehaviour
{
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (boxCollider != null)
        {
            DrawBoxColliderBorders();
        }
    }

    private void DrawBoxColliderBorders()
    {
        // Get the center and size of the box collider
        Vector3 center = transform.position + boxCollider.center;
        Vector3 size = boxCollider.size;

        // Half-extents for drawing lines
        Vector3 halfSize = size / 2;

        // Draw the 12 edges of the box
        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, -halfSize.y, halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(halfSize.x, halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, halfSize.y, halfSize.z), Color.green);

        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(halfSize.x, -halfSize.y, halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, halfSize.y, -halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(-halfSize.x, halfSize.y, halfSize.z), center + new Vector3(halfSize.x, halfSize.y, halfSize.z), Color.green);

        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(-halfSize.x, halfSize.y, halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z), center + new Vector3(halfSize.x, halfSize.y, -halfSize.z), Color.green);
        Debug.DrawLine(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z), center + new Vector3(halfSize.x, halfSize.y, halfSize.z), Color.green);
    }
}
