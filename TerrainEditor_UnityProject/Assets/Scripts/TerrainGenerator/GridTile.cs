using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTile : MonoBehaviour
{
    Vector3[] vertices = new Vector3[16];
    Text tileText;

    private void Awake()
    {
        tileText = GetComponentInChildren<Text>();
    }
    private void Start()
    {
        
        
    }


    public void InitializeTile(int x, int z, int index)
    {
        
        tileText.text = $"{x} / {z}\n{index}";
    }

    public void GenerateTriangles()
    {

    }

}
