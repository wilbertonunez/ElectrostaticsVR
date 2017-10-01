using UnityEngine;

using System.Collections;
using System;

[RequireComponent(typeof(Collider))]
public class ChargeCreator : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material gazedAtMaterial;
    public GameObject chargedSphere;

    void Start()
    {
        HighlightTile(false);
    }

    public void HighlightTile(bool gazedAt)
    {
        if (inactiveMaterial != null && gazedAtMaterial != null)
        {
            GetComponent<Renderer>().material = gazedAt ? gazedAtMaterial : inactiveMaterial;
            return;
        }
        GetComponent<Renderer>().material.color = gazedAt ? Color.gray : Color.black;
    }

    public void CreateCharge()
    {
        GameObject chargedSphereInstance = Instantiate(chargedSphere);
        chargedSphereInstance.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);

        ChargeModifier.ActivateChangeIndicator();
    }

    public void MoveChargeUp()
    {
        GameObject[] charges = GameObject.FindGameObjectsWithTag("charge");

        foreach(GameObject charge in charges)
        {
            if (Math.Abs(charge.transform.position.x - gameObject.transform.position.x) < 0.001 && Math.Abs(charge.transform.position.z - gameObject.transform.position.z) < 0.001)
            {
                if (charge.transform.position.y < 40)
                {
                    charge.transform.position = new Vector3(charge.transform.position.x, charge.transform.position.y + 1f, charge.transform.position.z);
                }

                ChargeModifier.ActivateChangeIndicator();

                // Return because, for now, there will only be at most one charge above the tile
                return;
            }
        }

        // If didn't return inside the loop, that means there is no charge above this tile. Create one.
        CreateCharge();
    }
}
