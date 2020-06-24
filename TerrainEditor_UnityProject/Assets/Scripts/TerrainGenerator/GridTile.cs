using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTile : MonoBehaviour
{
    Vector3[] vertices = new Vector3[16];
    Text tileText;

    public int id;
    public int coordX;
    public int coordZ;

    private void Awake()
    {
        tileText = GetComponentInChildren<Text>();
    }

    public void InitializeTile(int x, int z, int index)
    {
        
        tileText.text = $"{x} / {z}\n{index}";
        id = index;
        coordX = x;
        coordZ = z;
    }

   

}
