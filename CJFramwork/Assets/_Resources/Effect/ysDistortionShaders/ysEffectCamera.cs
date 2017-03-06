using UnityEngine;
using System.Collections;

public class ysEffectCamera {

    static ysEffectCamera s_instance = null;
    private RenderTexture m_rendertexture;
    public GameObject EffectCameraPrefab;
    ysEffectCamera() { Init(); }
    public static ysEffectCamera GetInstance()
    {
        if (s_instance != null)
        {
            return s_instance;
        }
        else
        {
            s_instance = new ysEffectCamera();
            return  s_instance;
        }
    }
    public static ysEffectCamera instance
    {
        get {return GetInstance();}
    }

    public RenderTexture rendertexture
    {
        get
        {
            return m_rendertexture;
        }
    }

    void Init()
    {
        s_instance = this;
        m_rendertexture = Resources.Load("ysDistortionRenderTexture") as RenderTexture;
       
        ResetCamera();
    }

    public void TurnOn()
    {
        if (s_instance == null)
        {
            Init();
        }
        if (EffectCameraPrefab != null)
        {
            EffectCameraPrefab.SetActive(true);
        }

    }
    public void TurnOff()
    {
        if (s_instance == null)
        {
            return;
        }
        if (EffectCameraPrefab != null)
        {
            EffectCameraPrefab.SetActive(false);
        }
    }

    public void SetMaskLayer(int id)
    {
        if (EffectCameraPrefab == null)
        {
            return;
        }
        Camera camera = EffectCameraPrefab.GetComponent<Camera>();
        camera.cullingMask = ~(1<< id);
    }
    public void ResetCamera()
    {
        if (EffectCameraPrefab ==null)
        {
            EffectCameraPrefab = GameObject.Instantiate(Resources.Load("ysDistortionCamera") as GameObject);
        }
        EffectCameraPrefab.transform.SetParent(Camera.main.transform,false);
        Camera camera = EffectCameraPrefab.GetComponent<Camera>();
        camera.fieldOfView = Camera.main.fieldOfView;
    }

    public void OnFree()
    {

    }
}
