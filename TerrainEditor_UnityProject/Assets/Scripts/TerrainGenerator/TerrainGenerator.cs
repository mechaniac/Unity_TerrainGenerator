using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    public GridTile tilePrefab;
    public TerrainChunk chunkPrefab;

    int verticeCountX;
    int verticeCountZ;

    public int ChunkXSize;
    public int ChunkZSize;

    public int ChunkXCount;
    public int ChunkZCount;

    int verticeCountXPerChunk;
    int verticeCountZPerChunk;

    Vector3[] vertices;
    int[] triangles;

    GridTile[] tiles;
    Vector3[] chunkVerticeList;
    TerrainChunk[] chunks;

    Mesh mesh;
    void Awake()
    {
        InitializeFields();
        chunks = new TerrainChunk[ChunkXCount * ChunkZCount];
        vertices = new Vector3[verticeCountX * verticeCountZ];
        
        
        InstantiateChunks();
        LayoutInitialGrid();
        Mesh myChunkMesh = chunks[3].GetComponent<MeshFilter>().mesh;
        chunkVerticeList = GetVerticesOfChunk((1, 1), chunks[3]);
        Debug.Log($"vertices in chunk: {chunkVerticeList.Length}");

        
        myChunkMesh.vertices = chunkVerticeList;
        SetChunkTriangles(myChunkMesh);
        myChunkMesh.RecalculateNormals();
    }

    private void Update()
    {

    }
    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        Gizmos.color = Color.black;

        for (int i = 0; i < vertices.Length; i++)
        {
            
            Gizmos.DrawSphere(vertices[i], .06f);
        }

        Gizmos.color = Color.red;
        for(int i = 0; i<chunkVerticeList.Length; i++)
        {
            Gizmos.DrawSphere(chunkVerticeList[i], .07f); 
        }
    }

    void InitializeFields()
    {
        verticeCountX = ChunkXSize * ChunkXCount + 1 + ChunkXSize * ChunkXCount * 2;
        verticeCountZ = ChunkZSize * ChunkZCount + 1 + ChunkZSize * ChunkZCount * 2;

        verticeCountXPerChunk = ChunkXSize + 1 + 2 * ChunkXSize;
        verticeCountZPerChunk = ChunkZSize + 1 + 2 * ChunkZSize;

        Debug.Log($"verticeCountX: {verticeCountX}, verticeCOuntZ: {verticeCountZ}");
        Debug.Log($"verticeCountXPerChunk: {verticeCountXPerChunk}, verticeCOuntZPerChunk: {verticeCountZPerChunk}");
    }

    void InstantiateChunks()
    {
        for (int z = 0, i = 0; z < ChunkZCount; z++)
        {
            for (int x = 0; x < ChunkXCount; x++, i++)
            {
                chunks[i] = Instantiate(chunkPrefab);
                chunks[i].transform.parent = transform;
                //chunks[i].transform.position = new Vector3(x * ChunkXSize * GridMetrics.gridTileOffset, 0, z * ChunkZSize * GridMetrics.gridTileOffset);
            }
        }
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

    int SetVertexCount()  //vertices of Grid with 2 separations.
    {

        return verticeCountX * verticeCountZ;
    }

    Vector3[] GetVerticesOfChunk(ValueTuple<int, int> chunkCoordinates, TerrainChunk chunkMesh)
    {
        int chunkCoordX = chunkCoordinates.Item1;
        int chunkCoordZ = chunkCoordinates.Item2;
        int startVertex = chunkCoordX * verticeCountX *(verticeCountZPerChunk-1) + chunkCoordZ * (verticeCountXPerChunk-1);

        Vector3[] chunkVertices = new Vector3[(ChunkXSize + 1 + 2 * ChunkXSize) * (ChunkZSize + 1 + 2 * ChunkZSize)];

        Transform t = chunkMesh.transform;
        Debug.Log($"trasnd: {t.position}");
        Vector3 test = new Vector3(1, 2, 4);
        Debug.Log($"trasnd: {t.InverseTransformPoint(test)}");

        for (int z = 0, i =0; z < verticeCountZPerChunk; z++)
        {
            for (int x = 0; x < verticeCountXPerChunk; x++, i++)
            {
                chunkVertices[i] =  vertices[startVertex + x + z * verticeCountX];
            }
        }

        return chunkVertices;
    }

    void SetChunkTriangles(Mesh chunkMesh)
    {

        triangles = new int[6 * verticeCountXPerChunk * verticeCountZPerChunk];

        for (int z = 0, ti = 0; z < verticeCountZPerChunk - 1; z++)
        {
            for (int x = 0; x < verticeCountXPerChunk - 1; x++, ti += 6)
            {
                //Debug.Log($"chunky");
                int offsetFRomZ = z * verticeCountXPerChunk;
                triangles[ti] = offsetFRomZ + x;
                triangles[ti + 1] = triangles[ti + 4] = offsetFRomZ + x + verticeCountXPerChunk;
                triangles[ti + 2] = triangles[ti + 3] = offsetFRomZ + x + 1;
                triangles[ti + 5] = offsetFRomZ + x + verticeCountXPerChunk + 1;
            }
        }
        chunkMesh.triangles = triangles;
        chunkMesh.RecalculateNormals();
    }

    void LayoutInitialGrid()    //vertices in 1/3 gridSize distance
    {
        for (int z = 0, i = 0; z < verticeCountZ; z++)
        {
            for (int x = 0; x < verticeCountX; x++, i++)
            {
                int _z = z % 3;
                int _x = x % 3;
                //Debug.Log($"vertice number: {i}");
                vertices[i] = new Vector3((_x * (1f / 3f) + x / 3) * GridMetrics.gridTileOffset, 0, (_z * (1f / 3f) + z / 3) * GridMetrics.gridTileOffset);
            }
        }
        //mesh.vertices = vertices;
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
        triangles = new int[6 * verticeCountXPerChunk * verticeCountZPerChunk];

        for (int z = 0, ti = 0; z < verticeCountZPerChunk - 1; z++)
        {
            for (int x = 0; x < verticeCountXPerChunk - 1; x++, ti += 6)
            {
                int offsetFRomZ = z * verticeCountXPerChunk;
                triangles[ti] = offsetFRomZ + x;
                triangles[ti + 1] = triangles[ti + 4] = offsetFRomZ + x + verticeCountXPerChunk;
                triangles[ti + 2] = triangles[ti + 3] = offsetFRomZ + x + 1;
                triangles[ti + 5] = offsetFRomZ + x + verticeCountXPerChunk + 1;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }



}
