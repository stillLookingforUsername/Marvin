using UnityEngine;

public class LayerMaskHelper : MonoBehaviour
{
    //Return true if the gameObject's layer is contained within the layermask's layers.
    //<param name = "gameObject"> The gameObject we are comparing against the LayerMask.
    //<param name = "layerMask"> The LayerMask we are checking if the gameObject is within.

    public static bool ObjectIsInLayerMask(GameObject gameObject, LayerMask layerMask)
    {
        if ((layerMask.value & (1 << gameObject.layer)) > 0)
        {
            return true;
        }
        return false;
    }

    public static LayerMask CreateLayerMask(params int[] layers)
    {
        LayerMask layerMask = 0;
        foreach (int layer in layers)
        {
            layerMask |= (1 << layer);  //This adds that layer to the mask using bitwise OR (|=).
        }
        return layerMask;
    }

}
