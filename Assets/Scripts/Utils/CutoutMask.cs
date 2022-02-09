using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CutoutMask : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", 3);
            return material;
        }
    }
}