using UnityEngine;
using System.Collections;

public class ysReflectionEffectCamera : MonoBehaviour
{
    static ysReflectionEffectCamera s_instance = null;
    [SerializeField]private RenderTexture m_rendertexture;
    public GameObject EffectCameraPrefab;
    public GameObject planeWithMesh;
     bool isInit = false;
    Vector3 planeNormal = Vector3.up;
    Mesh planeMesh;
 
    public static ysReflectionEffectCamera GetInstance()
    {
        if (s_instance != null)
        {
            return s_instance;
        }
        else
        {
            s_instance = new ysReflectionEffectCamera();
            return s_instance;
        }
    }
    public static ysReflectionEffectCamera instance
    {
        get { return GetInstance(); }
    }

    public RenderTexture rendertexture
    {
        get
        {
            return m_rendertexture;
        }
    }
    void Awake()
    {
        if ( !isInit)
        {
            Init();
        }

    }
    void Update()
    {
        if (Camera.main!=null&&planeWithMesh!=null)
        {
            ResetCamera();
            //transform.position = new Vector3(transform.position.x, planeWithMesh.transform.position.y - Camera.main.transform.position.y, transform.position.z);
        }
    }

    void Init()
    {
        isInit = true;
        s_instance = this;
        m_rendertexture = Resources.Load("ysReflectionRenderTexture") as RenderTexture;
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
        camera.cullingMask = ~(1 << id);
    }
    public void SetReflectPlane(GameObject plane)
    {
        planeWithMesh = plane;
        if (planeWithMesh.GetComponent<Mesh>() == null)
        {
            Debug.LogError("未找到反射平面");
        }
        else
        {
            planeMesh = planeWithMesh.GetComponent<Mesh>();
        }
    }

    public void ResetCamera()
    {
        if (EffectCameraPrefab == null)
        {
            EffectCameraPrefab = GameObject.Instantiate(Resources.Load("ysReflectionCamera") as GameObject);
        }

        EffectCameraPrefab.transform.position = new Vector3
        (
            Camera.main.transform.position.x,
            planeWithMesh.transform.position.y -Camera.main.transform.position.y,
            Camera.main.transform.position.z
        );
        EffectCameraPrefab.transform.rotation = Quaternion.Euler(
            Camera.main.transform.rotation.eulerAngles.x+180f,
             Camera.main.transform.rotation.eulerAngles.y+180f,
              Camera.main.transform.rotation.eulerAngles.z //+360f
            );

        EffectCameraPrefab.transform.SetParent(Camera.main.transform, true);
        Camera camera = EffectCameraPrefab.GetComponent<Camera>();
        camera.fieldOfView = Camera.main.fieldOfView;

    }

    public void OnFree()
    {

    }
}


