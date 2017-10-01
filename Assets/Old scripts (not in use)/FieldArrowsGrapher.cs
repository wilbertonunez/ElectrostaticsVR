using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldArrowsGrapher : MonoBehaviour
{
    [Range(2, 5)]
    public int numberOfArrows;
    private int currentNumberOfArrows;
    public GameObject arrow;
    private const float SHIFT_LENGTH = 1f;
    private GameObject[][] arrowsTemp;

    void Start()
    {        
        CreateArrows();
    }

    private void CreateArrows()
    {

        currentNumberOfArrows = numberOfArrows;

        arrowsTemp = new GameObject[8][];

        arrowsTemp[0] = CreateSomeArrows(SHIFT_LENGTH, 'x', 1, 45, -90, 0);
        arrowsTemp[1] = CreateSomeArrows(-SHIFT_LENGTH, 'x', -1, 135, -90, 0);
        arrowsTemp[2] = CreateSomeArrows(-SHIFT_LENGTH, 'x', 1, 225, -90, 0);
        arrowsTemp[3] = CreateSomeArrows(SHIFT_LENGTH, 'x', -1, -45, -90, 0);

        arrowsTemp[4] = CreateSomeArrows(SHIFT_LENGTH, 'z', 1, 45, 180, 0);
        arrowsTemp[5] = CreateSomeArrows(-SHIFT_LENGTH, 'z', -1, 135, 180, 0);
        arrowsTemp[6] = CreateSomeArrows(-SHIFT_LENGTH, 'z', 1, 225, 180, 0);
        arrowsTemp[7] = CreateSomeArrows(SHIFT_LENGTH, 'z', -1, -45, 180, 0);
        

    }

    private GameObject[] CreateSomeArrows(float initialShift, char axis, float slope,
        float xAngle, float yAngle, float zAngle)
    {
        GameObject[] someArrows = new GameObject[numberOfArrows];
        float[] distanceToOriginSquared = new float[numberOfArrows];

        for (int i = 0; i < numberOfArrows; i++)
        {

            someArrows[i] = Instantiate(arrow);
            
            //someArrows[i].transform.startColor = new Color(Math.Abs(variable), 0f, 0f, 0.5f);
            
            float variable;
            float scale = 0.2f;

            if (i == 0)
            {
                // Position of the first arrow.
                variable = initialShift;
                
            } else if (i == 1)
            {
                // Position of second arrow.
                variable = 1.5f*initialShift/Math.Abs(initialShift) + initialShift;

            } else
            {
                float previousValue1 = 0;
                float previousValue2 = 0;

                if (axis == 'x')
                {
                    previousValue1 = someArrows[i-1].transform.position.x;
                    previousValue2 = someArrows[i-2].transform.position.x;

                } else if (axis == 'z')
                {
                    previousValue1 = someArrows[i-1].transform.position.z;
                    previousValue2 = someArrows[i-2].transform.position.z;
                }

                // Recurrence relation (CHANGE IT):
                // v_{i} = v_{i-1} + 0.8(v_{i-1} - v_{i-2})
                variable = previousValue1 + 0.8f * (previousValue1 - previousValue2);

                //variable = Math.Pow(Math.Pow(previousValue1 - previousValue1, 3.0) * distanceToOriginSquared[i-1] / Math.Pow(),1.0/3.0) + previousValue1;
            } 

            if (axis == 'x')
            {                
                someArrows[i].transform.position = new Vector3(variable, slope * variable, 0f);
            }
            else if (axis == 'z')
            {
                someArrows[i].transform.position = new Vector3(0f, slope * variable, variable);
            }

            distanceToOriginSquared[i] = variable*variable + slope*variable*slope*variable;

            if (i > 0)
            {
                // S_{i}^3 = S_{i-1}^3 * r_{i-1}^2 / r_{i}^2

                scale = (float)Math.Pow((float)Math.Pow((double)someArrows[i-1].transform.localScale.x,3) * distanceToOriginSquared[i-1] / distanceToOriginSquared[i], 1.0/3.0);
            }

            someArrows[i].transform.localScale = new Vector3(scale, scale, 4*scale);
            someArrows[i].transform.Rotate(xAngle, yAngle, zAngle);
        }

        return someArrows;
    }

    void Update()
    {
        if (currentNumberOfArrows != numberOfArrows || arrowsTemp == null
            || arrowsTemp[0] == null || arrowsTemp[1] == null || arrowsTemp[2] == null
            || arrowsTemp[3] == null)
        {
            CreateArrows();
        }        
    }
 
}
