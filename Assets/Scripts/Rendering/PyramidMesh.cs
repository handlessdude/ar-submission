using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PyramidMesh : MonoBehaviour
{
    void Awake()
    {
        GenerateMesh();
    }

    void OnValidate()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        // Define vertices with the pivot at the base center
        float halfBase = 0.5f; // Half of the base length
        float height = 1.0f;   // Height of the pyramid

        Vector3[] vertices = {
            new Vector3(-halfBase, 0, -halfBase), // Base - Bottom Left (0)
            new Vector3(halfBase, 0, -halfBase),  // Base - Bottom Right (1)
            new Vector3(halfBase, 0, halfBase),   // Base - Top Right (2)
            new Vector3(-halfBase, 0, halfBase),  // Base - Top Left (3)
            new Vector3(0, height, 0)            // Apex (4)
        };

        int[] triangles = {
            // Base (two triangles)
            0, 2, 1,
            0, 3, 2,

            // Sides
            0, 1, 4, // Side 1
            1, 2, 4, // Side 2
            2, 3, 4, // Side 3
            3, 0, 4  // Side 4
        };

        // Assign mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}