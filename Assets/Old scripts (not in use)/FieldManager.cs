using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

    public GameObject arrow;
    private GameObject[] charges;
    private static int xLength = 20;
    private static int zLength = 20;
    private static int yLength = 15;
    private static float xInitialPosition = 1f;
    private static float yInitialPosition = -1f;
    private static float zInitialPosition = 1f;

    public void UpdateElectricField()
    {
        DestroyField();
        CreateField();

    }

    private void CreateField()
    {
        charges = GameObject.FindGameObjectsWithTag("charge");

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    Vector3 arrowPosition = new Vector3(xInitialPosition + 1.5f * x,
                        yInitialPosition + 1.5f * y, zInitialPosition + 1.5f * z);

                    // Compute the x, y, z components of the E field here
                    double eX = GetElectricFieldComponent('x', arrowPosition);
                    double eY = GetElectricFieldComponent('y', arrowPosition);
                    double eZ = GetElectricFieldComponent('z', arrowPosition);

                    double sizeOfArrow = Math.Abs(eX) + Math.Abs(eY) + Math.Abs(eZ);

                    if (sizeOfArrow > 0.8 || sizeOfArrow < 0.02)
                    {
                        Debug.LogWarning("Electric field is too big or too small at this point. Arrow will not be displayed.");                        
                        continue;
                    }

                    GameObject arrowInstance = Instantiate(arrow);

                    arrowInstance.transform.position = arrowPosition;

                    // Compute size of the arrow
                    arrowInstance.transform.localScale = new Vector3((float)sizeOfArrow, 0.6f * (float)sizeOfArrow, 0.6f * (float)sizeOfArrow);

                    // Rotate the arrow                    
                    try
                    {
                        arrowInstance.transform.rotation = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), new Vector3((float)eX, (float)eY, (float)eZ));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Destroy(arrowInstance);
                        continue;
                    }

                    arrowInstance.GetComponent<ElectricFieldComponents>().eX = eX;
                    arrowInstance.GetComponent<ElectricFieldComponents>().eY = eY;
                    arrowInstance.GetComponent<ElectricFieldComponents>().eZ = eZ;

                }
            }
        }

    }

    private void DestroyField()
    {
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("arrow");
        foreach (GameObject arrowInstance in arrows)
        {
            Destroy(arrowInstance);
        }
    }

    private double GetElectricFieldComponent(char direction, Vector3 arrowPosition)
    {
        double eSum = 0.0;

        foreach (GameObject charge in charges)
        {
            double q = charge.GetComponent<ChargeProperties>().charge;

            double xA = arrowPosition.x;
            double yA = arrowPosition.y;
            double zA = arrowPosition.z;

            double xQ = charge.transform.position.x;
            double yQ = charge.transform.position.y;
            double zQ = charge.transform.position.z;

            if (Math.Abs(xA - xQ) < 0.0000001 &&
                Math.Abs(yA - yQ) < 0.0000001 &&
                Math.Abs(zA - zQ) < 0.0000001)
            {
                // Return a big number so that arrow is deleted afterwards for being too big.
                return 0.1 * Double.MaxValue;
            }

            double arrowPositionComponent = 0;
            double chargePositionComponent = 0;

            switch (direction)
            {
                case 'x':
                    arrowPositionComponent = xA;
                    chargePositionComponent = xQ;
                    break;
                case 'y':
                    arrowPositionComponent = yA;
                    chargePositionComponent = yQ;
                    break;
                case 'z':
                    arrowPositionComponent = zA;
                    chargePositionComponent = zQ;
                    break;

                default: break;
            }

            eSum += q * (arrowPositionComponent - chargePositionComponent) / Math.Pow(Math.Pow(xA - xQ, 2.0) + Math.Pow(yA - yQ, 2.0) + Math.Pow(zA - zQ, 2.0), 3.0 / 2.0);
        }

        return eSum;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
