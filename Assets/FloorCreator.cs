using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreator : MonoBehaviour
{

    public GameObject tile;
    public static int width = 10;
    public static int height = 10;
    private static float xInitialPosition = 2f;
    private static float yInitialPosition = -0.9f;
    private static float zInitialPosition = 2f;

    private GameObject[,] grid = new GameObject[width, height];
    void Awake()
    {

        float shiftScale = 3.08f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject tileInstance = Instantiate(tile);
                tileInstance.transform.position = new Vector3(xInitialPosition + (x * shiftScale), yInitialPosition,
                    zInitialPosition + (z * shiftScale));
                grid[x, z] = tileInstance;
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
