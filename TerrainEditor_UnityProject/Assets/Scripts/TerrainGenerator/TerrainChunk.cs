using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class TerrainChunk : MonoBehaviour
{
    public int id;
    private Mesh chunkMesh;


    public int CoordX;
    public int CoordZ;

    public TerrainGenerator tG;

    GridTile[] tiles;


    private void Awake()
    {
        chunkMesh = GetComponent<MeshFilter>().mesh = new Mesh();
        chunkMesh.name = "ChunkMesh " + CoordX + " " + CoordZ;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;


    }

    void Start()
    {
        Debug.Log($"chunkPrefab: " + tG.tilePrefab.name);
    }
    public void SetChunkVertices(Vector3[] vertices)
    {
        chunkMesh.vertices = vertices;

    }

    public void SetChunkTriangles()
    {
        int xCount = tG.verticeCountXPerChunk;
        int zCount = tG.verticeCountZPerChunk;

        int[] triangles = new int[6 * xCount * zCount];

        for (int z = 0, ti = 0; z < zCount - 1; z++)
        {
            for (int x = 0; x < xCount - 1; x++, ti += 6)
            {
                //Debug.Log($"chunky");
                int offsetFRomZ = z * xCount;
                triangles[ti] = offsetFRomZ + x;
                triangles[ti + 1] = triangles[ti + 4] = offsetFRomZ + x + xCount;
                triangles[ti + 2] = triangles[ti + 3] = offsetFRomZ + x + 1;
                triangles[ti + 5] = offsetFRomZ + x + xCount + 1;
            }
        }
        chunkMesh.triangles = triangles;
        chunkMesh.RecalculateNormals();
    }

    public void InstantiateGridTiles()
    {
        tiles = new GridTile[tG.ChunkXSize  * tG.ChunkZSize];

        float chunkXOffset = tG.ChunkXSize * CoordX * GridMetrics.gridTileOffset;
        float chunkZOffset = tG.ChunkZSize * CoordZ * GridMetrics.gridTileOffset;

        for (int z = 0, i = 0; z <  tG.ChunkZSize; z++)
        {
            for (int x = 0; x < tG.ChunkXSize; x++, i++)
            {
                GridTile t = tiles[i] = Instantiate(tG.tilePrefab);
                t.transform.position = new Vector3(
                    x * GridMetrics.gridTileOffset + GridMetrics.gridTileOffset / 2f + chunkXOffset,
                    0,
                    z * GridMetrics.gridTileOffset + GridMetrics.gridTileOffset / 2f + chunkZOffset);
                t.transform.parent = transform;
                t.InitializeTile(x + tG.ChunkXSize * CoordX, z + tG.ChunkZSize * CoordZ, i);
            }
        }
    }
}
