using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    Vector3[] vertices = new Vector3[16];

    private void Start()
    {
        
    }


    public void InitializeTile()
    {
        float oneThird = (1f / 3f);

        for (int x = 0, i = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++, i++)
            {
                vertices[i] = new Vector3(transform.position.x + x * oneThird, 0, transform.position.z + z * oneThird);
            }
        }
    }

    public void GenerateTriangles()
    {

    }

}
