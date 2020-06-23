using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTiles : MonoBehaviour
{
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { HandleInput(); }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit))
        {
            Debug.Log($"hit: {hit.triangleIndex}");     //TriangleIndex
        }
    }
}
