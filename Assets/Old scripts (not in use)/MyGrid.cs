using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{

    public GameObject plane;
    public static int width = 10;
    public static int height = 10;
    private static float xInitialPosition = -8f;
    private static float yInitialPosition = -0.98f;
    private static float zInitialPosition = -8f;

    private GameObject[,] grid = new GameObject[width, height];
    void Awake()
    {        

        float shiftScale = 4.05f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject gridPlane = (GameObject)Instantiate(plane);
                gridPlane.transform.position = new Vector3(xInitialPosition + (x*shiftScale), yInitialPosition,
                    zInitialPosition + (z*shiftScale));
                grid[x, z] = gridPlane;
            }
        }

    }

    void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 10, 150, 100), " Delete grid[3,3]"))
        //    Destroy(grid[3, 3]);
    }

    void Start()
    {

    }


    void Update()
    {

    }
}
