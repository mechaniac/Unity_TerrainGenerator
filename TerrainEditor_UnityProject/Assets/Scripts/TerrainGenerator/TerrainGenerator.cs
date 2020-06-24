using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    public GridTile tilePrefab;
    public TerrainChunk chunkPrefab;

    public int ChunkXSize;
    public int ChunkZSize;

    public int ChunkXCount;
    public int ChunkZCount;

    public int verticeCountX;
    public int verticeCountZ;

    public int verticeCountXPerChunk;
    public int verticeCountZPerChunk;

    Vector3[] allVertices;



    Vector3[] chunkVerticeList;
    TerrainChunk[] chunks;

    
    void Awake()
    {
        InitializeFields();

        allVertices = new Vector3[verticeCountX * verticeCountZ];
        LayoutInitialGrid();
        AdjustGridOffsets();
        InstantiateChunks();

      }
    private void Start()
    {

    }



    private void Update()
    {

    }
    private void OnDrawGizmos()
    {
        if (allVertices == null) return;

        Gizmos.color = Color.black;

        for (int i = 0; i < allVertices.Length; i++)
        {

            Gizmos.DrawSphere(allVertices[i], .06f);
        }

        if (chunkVerticeList == null) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < chunkVerticeList.Length; i++)
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


    void LayoutInitialGrid()    //vertices in 1/3 gridSize distance
    {
        for (int z = 0, i = 0; z < verticeCountZ; z++)
        {
            for (int x = 0; x < verticeCountX; x++, i++)
            {
                int _z = z % 3;
                int _x = x % 3;
                //Debug.Log($"vertice number: {i}");
                allVertices[i] = new Vector3((_x * (1f / 3f) + x / 3) * GridMetrics.gridTileOffset, 0, (_z * (1f / 3f) + z / 3) * GridMetrics.gridTileOffset);
            }
        }
        //mesh.vertices = vertices;
    }

    void InstantiateChunks()
    {
        chunks = new TerrainChunk[ChunkXCount * ChunkZCount];

        for (int z = 0, i = 0; z < ChunkZCount; z++)
        {
            for (int x = 0; x < ChunkXCount; x++, i++)
            {
                TerrainChunk c = chunks[i] = Instantiate(chunkPrefab);
                c.transform.parent = transform;
                c.CoordX = x;
                c.CoordZ = z;
                c.id = i;
                c.tG = this;

                c.SetChunkVertices(GetVerticesOfChunk(c));
                c.SetChunkTriangles();
                c.InstantiateGridTiles();
                //c.transform.position = new Vector3(x * ChunkXSize * GridMetrics.gridTileOffset, 0, z * ChunkZSize * GridMetrics.gridTileOffset);
            }
        }
    }




    Vector3[] GetVerticesOfChunk(TerrainChunk chunk)
    {
        int chunkCoordX = chunk.CoordX;
        int chunkCoordZ = chunk.CoordZ;
        int startVertex = chunkCoordX * (verticeCountXPerChunk - 1) + (chunkCoordZ * verticeCountX * (verticeCountZPerChunk - 1));

        Vector3[] chunkVertices = new Vector3[verticeCountXPerChunk * verticeCountZPerChunk];

        for (int z = 0, i = 0; z < verticeCountZPerChunk; z++)
        {
            for (int x = 0; x < verticeCountXPerChunk; x++, i++)
            {
                chunkVertices[i] = allVertices[startVertex + x + z * verticeCountX];
                //Debug.Log($"vertice number {i}, sits at {chunkVertices[i]}");
                //Debug.Log($"index of sourceVertice: {startVertex + x + z * verticeCountX} ");
            }
        }

        return chunkVertices;
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
                    allVertices[verticeCountX * z + x] += new Vector3(0, 0, o);
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
                    allVertices[x + verticeCountX * z] += new Vector3(o, 0, 0);
                }
            }
        }
        //mesh.vertices = allVertices;
    }

    void TestOffset(GridTile tile)
    {
        int startIndexFromX = tile.coordX * 3;
        int startIndexFromZ = tile.coordZ * 3 * verticeCountX;


        int s = startIndexFromZ + startIndexFromX;
        for (int z = s; z <= s + 3 * verticeCountX; z += verticeCountX)
        {
            for (int x = 0; x < 4; x++)
            {
                allVertices[z + x] += new Vector3(0, 1f, 0);

            }
        }


    }









}
