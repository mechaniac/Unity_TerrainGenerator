using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGrid_old : MonoBehaviour {
    public int sizeX, sizeZ;

    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
        
    }

    private void Update()
    {
        
    }


    void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Terrain Grid Mesh";

        vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        //Create vertices, Z axis first
        for (int z = 0, i = 0; z <= sizeZ; z++)
        {
            for (int x = 0; x <= sizeX; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, z);
                uv[i] = new Vector2((float)x / sizeX, (float)z / sizeZ);
            }
        }

        //Create triangles, Z axis first
        int[] triangles = new int[sizeX * sizeZ * 6];
        for (int z = 0, ti = 0, vi = 0; z < sizeZ; z++, vi++)
        {
            for (int x = 0; x < sizeX; x++, vi++, ti += 6)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + sizeX + 1;
                triangles[ti + 5] = vi + sizeX + 2;
            }
            
        }


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) { return; }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
