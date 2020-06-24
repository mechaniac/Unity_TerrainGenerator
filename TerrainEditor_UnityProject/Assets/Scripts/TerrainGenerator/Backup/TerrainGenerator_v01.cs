using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class TerrainGenerator_v01 : MonoBehaviour
{
    public GridTile tilePrefab;

    public int ChunkXSize;
    public int ChunkZSize;

    public int ChunkXCount;
    public int ChunkZCount;

    int verticeCountX;
    int verticeCountZ;

    Vector3[] vertices;
    int[] triangles;

    GridTile[] tiles;

    Mesh mesh;
    void Awake()
    {
        tiles = new GridTile[ChunkXSize * ChunkZSize];

        mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.name = "Terrain Mesh";

        vertices = new Vector3[CalculateVertexCount()];
        GenerateTiles();

        LayoutInitialGrid();
        AdjustGridOffsets();
        //TestOffset((1 ,2));
        GenerateTriangles();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void Update()
    {

    }

    void GenerateTiles()
    {
        for (int z = 0, i = 0; z < ChunkZSize; z++)
        {
            for (int x = 0; x < ChunkXSize; x++, i++)
            {
                GridTile t = tiles[i] = Instantiate(tilePrefab);
                float o = GridMetrics.gridTileOffset;
                t.transform.position = new Vector3(x * o + o / 2f, 0, z * o + o / 2f);
                t.transform.parent = transform;
                t.InitializeTile(x, z, i);

            }
        }
    }

    int CalculateVertexCount()  //vertices of Grid with 2 separations.
    {
        verticeCountX = ChunkXSize + 1 + ChunkXSize * 2;
        verticeCountZ = ChunkZSize + 1 + ChunkZSize * 2;
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

                vertices[i] = new Vector3((_x * (1f / 3f) + x / 3) * GridMetrics.gridTileOffset, 0, (_z * (1f / 3f) + z / 3) * GridMetrics.gridTileOffset);
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
                    vertices[verticeCountX * z + x] += new Vector3(0, 0, o);
                }
            }

        }

        for (int x = 0, _x = 0; x < verticeCountX; x++)
        {
            if (x % 3 != 0)
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

    void TestOffset(ValueTuple<int, int> tileCoordinates)
    {
        int startIndexFromX = tileCoordinates.Item1 * 3;
        int startIndexFromZ = tileCoordinates.Item2 * 3 * verticeCountX;


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

    void RaiseTile(Vector2 tileCoordinates)
    {

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


}
