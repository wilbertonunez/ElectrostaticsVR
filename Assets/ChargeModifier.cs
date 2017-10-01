using UnityEngine;

using System.Collections;

[RequireComponent(typeof(Collider))]
public class ChargeModifier : MonoBehaviour
{
    private Vector3 startingPosition;
    private Material inactiveMaterial;
    public Material negativeChargeMaterial;
    public Material positiveChargeMaterial;
    public Material gazedAtMaterial;

    void Start()
    {
        startingPosition = transform.localPosition;
        if (GetComponent<ChargeProperties>().charge > 0)
            inactiveMaterial = positiveChargeMaterial;
        else
            inactiveMaterial = negativeChargeMaterial;

        SetGazedAt(false);
    }

    public void SetGazedAt(bool gazedAt)
    {
        if (inactiveMaterial != null && gazedAtMaterial != null)
        {
            GetComponent<Renderer>().material = gazedAt ? gazedAtMaterial : inactiveMaterial;
            return;
        }
        GetComponent<Renderer>().material.color = gazedAt ? Color.green : Color.gray;
    }

    public void Reset()
    {
        transform.localPosition = startingPosition;
    }

    public void Recenter()
    {
#if !UNITY_EDITOR
    GvrCardboardHelpers.Recenter();
#else
        GvrEditorEmulator emulator = FindObjectOfType<GvrEditorEmulator>();
        if (emulator == null)
        {
            return;
        }
        emulator.Recenter();
#endif  // !UNITY_EDITOR
    }

    public void DestroyCharge()
    {
        Destroy(gameObject);
        ActivateChangeIndicator();
    }

    public void FlipChargeValue()
    {
        gameObject.GetComponent<ChargeProperties>().charge = -gameObject.GetComponent<ChargeProperties>().charge;

        Start();
        ActivateChangeIndicator();
    }

    public static void ActivateChangeIndicator()
    {
        GameObject[] changeIndicator = GameObject.FindGameObjectsWithTag("changefalse");
        if (changeIndicator.Length > 0)
            changeIndicator[0].tag = "changetrue";
    }
}
