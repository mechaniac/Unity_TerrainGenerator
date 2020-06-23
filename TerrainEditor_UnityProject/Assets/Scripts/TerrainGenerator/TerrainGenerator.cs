using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter),typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    public GridTile tilePrefab;

    public int xSize;
    public int zSize;

    public int verticeCountX;
    public int verticeCountZ;

    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.name = "Terrain Mesh";
        
        vertices = new Vector3[CalculateVertexCount()];
        GenerateTiles();

        LayoutInitialGrid();
        AdjustGridOffsets();
        TestOffset(new Vector2(1, 1));
        GenerateTriangles();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void Update()
    {

    }

    void GenerateTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                GridTile t = Instantiate(tilePrefab);
                t.transform.position = new Vector3(x * GridMetrics.gridTileOffset, 0, z * GridMetrics.gridTileOffset);
                t.transform.parent = transform;
                t.InitializeTile();

            }
        }
    }

    int CalculateVertexCount()  //vertices of Grid with 2 separations.
    {
        verticeCountX = xSize + 1 + xSize * 2;
        verticeCountZ = zSize + 1 + zSize * 2;
        return verticeCountX * verticeCountZ;
    }

    void LayoutInitialGrid()    //vertices in 1/3 gridSize distance
    {
        for (int z = 0, i = 0; z < verticeCountZ; z++)
        {
            for (int x = 0; x < verticeCountX; x++, i++)
            {
                int _z = z % 3;
                int _x = x % 3;

                vertices[i] = new Vector3((_x * (1f / 3f) + x / 3) *GridMetrics.gridTileOffset, 0, (_z * (1f / 3f) + z / 3)*GridMetrics.gridTileOffset);
            }
        }
        mesh.vertices = vertices;
    }

    void AdjustGridOffsets()    //incorporate innerOffset
    {
        for (int z = 0, _z = 0; z < verticeCountZ; z++)
        {
            if (z % 3 != 0)
            {
                _z++;
                float o = GridMetrics.innerOffset;
                if (_z % 2 != 0) o = -o;
                //Debug.Log($"_z = {_z}, z = {z}, _z%2 = {_z%2}");
                for (int x = 0; x < verticeCountX; x++)
                {
                    vertices[verticeCountX * z +  x] += new Vector3(0, 0, o);
                }
            }

        }

        for (int x = 0, _x = 0; x < verticeCountX; x++)
        {
            if(x %3 != 0)
            {
                _x++;
                float o = GridMetrics.innerOffset;
                if (_x % 2 != 0) o = -o;

                for (int z = 0; z < verticeCountZ; z++)
                {
                    vertices[x + verticeCountX * z] += new Vector3(o, 0, 0);
                }
            }
        }
        mesh.vertices = vertices;
    }

    void TestOffset(Vector2 tileCoordinates)
    {
        int startIndexFromZ = (int)tileCoordinates.y * 3 * verticeCountX;
        int startIndexFromX = (int)tileCoordinates.x * 3;


        int s = startIndexFromZ + startIndexFromX;
        for (int z = s; z <= s + 3 * verticeCountX; z += verticeCountX)
        {
            for (int x = 0; x < 4; x++)
            {
                vertices[z + x] += new Vector3(0, 1f, 0);

            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void GenerateTriangles()
    {
        triangles = new int[6 * verticeCountX * verticeCountZ];

        for (int z = 0, ti = 0; z < verticeCountZ - 1; z++)
        {
            for (int x = 0; x < verticeCountX - 1; x++, ti += 6)
            {
                int offsetFRomZ = z * verticeCountX;
                triangles[ti] = offsetFRomZ + x;
                triangles[ti + 1] = triangles[ti + 4] = offsetFRomZ + x + verticeCountX;
                triangles[ti + 2] = triangles[ti + 3] = offsetFRomZ + x + 1;
                triangles[ti + 5] = offsetFRomZ + x + verticeCountX + 1;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        Gizmos.color = Color.black;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .06f);
        }
    }

}
