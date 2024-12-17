using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RemoveCubeSide : MonoBehaviour
{
    [Tooltip("Choose which face to remove: 0=Front, 1=Back, 2=Left, 3=Right, 4=Top, 5=Bottom")]
    private int faceToRemove = 1; // Default to the front face

    void Awake()
    {
        UpdateMesh();
    }

    void OnValidate()
    {
        UpdateMesh();
    }

    void UpdateMesh()
    {
        // Standard cube vertex indices per face:
        int[][] cubeFaces = {
            new int[] { 2, 3, 7, 6 }, // Front (+Z)
            new int[] { 0, 1, 5, 4 }, // Back (-Z)
            new int[] { 0, 3, 7, 4 }, // Left (-X)
            new int[] { 1, 2, 6, 5 }, // Right (+X)
            new int[] { 4, 5, 6, 7 }, // Top (+Y)
            new int[] { 0, 1, 2, 3 }  // Bottom (-Y)
        };

        // Get the cube's mesh
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Mesh mesh = meshFilter.sharedMesh;

        // Original cube data
        Vector3[] originalVertices = mesh.vertices;
        int[] originalTriangles = mesh.triangles;

        // Get the face to remove
        int[] faceVertices = cubeFaces[Mathf.Clamp(faceToRemove, 0, cubeFaces.Length - 1)];

        // Filter triangles to exclude the chosen face
        var newTriangles = new System.Collections.Generic.List<int>();
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            if (System.Array.Exists(faceVertices, v => v == originalTriangles[i]) &&
                System.Array.Exists(faceVertices, v => v == originalTriangles[i + 1]) &&
                System.Array.Exists(faceVertices, v => v == originalTriangles[i + 2]))
            {
                // Skip triangles belonging to the face
                continue;
            }

            // Keep all other triangles
            newTriangles.Add(originalTriangles[i]);
            newTriangles.Add(originalTriangles[i + 1]);
            newTriangles.Add(originalTriangles[i + 2]);
        }

        // Update the mesh
        Mesh newMesh = new Mesh();
        newMesh.vertices = originalVertices;
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals(); // Fix lighting issues

        // Assign the modified mesh
        meshFilter.mesh = newMesh;
    }
}
