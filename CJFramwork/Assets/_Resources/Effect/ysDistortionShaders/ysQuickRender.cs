using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class ysQuickRender : MonoBehaviour
{
    public List<Material> RenderMaterialList = new List<Material>();

    void OnEnable()
    {
        if (RenderMaterialList.Count>0)
        {
            return;
        }
        Renderer[] render = gameObject.GetComponents<Renderer>();
        for (int i = 0; i < render.Length; i++)
        {
            string name = render[i].sharedMaterial.shader.name.ToLower();
            if (name.Contains("ysshader") && name.Contains("distortion")&& !RenderMaterialList.Contains(render[i].sharedMaterial))
            {
                RenderMaterialList.Add(render[i].sharedMaterial);
            }
        }
        foreach (Transform item in transform)
        {
            Renderer[] mats = item.gameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < mats.Length; i++)
            {
                string name = mats[i].sharedMaterial.shader.name.ToLower();
                if (name.Contains("ysshader") && name.Contains("distortion") && !RenderMaterialList.Contains(mats[i].sharedMaterial))
                {
                    RenderMaterialList.Add(mats[i].sharedMaterial);
                }
            }
        }
    }
    void Start ()
    {
        RenderTexture rt = ysEffectCamera.instance.rendertexture;
        for (int i = 0; i < RenderMaterialList.Count; i++)
        {
            RenderMaterialList[i].SetTexture("_RenderTex", rt);
        }
    }
    void OnDestory()
    {

    }
}
