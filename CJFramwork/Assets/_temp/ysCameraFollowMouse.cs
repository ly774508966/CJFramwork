using UnityEngine;
using System.Collections;

public class ysCameraFollowMouse : MonoBehaviour {

    //方向灵敏度  
    public float sensitivityX = 0.000000001F;
    public float sensitivityY = 0.0000001F;

    //上下最大视角(Y视角)  
    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    Vector2 recordMousePos;
    Vector2 currentMousePos;
    Vector2 deltaMousePos;
    void Update()
    {

        recordMousePos = currentMousePos;
        currentMousePos = Input.mousePosition;
        // deltaMousePos = new Vector2(Mathf.Abs(currentMousePos.x - recordMousePos.x), Mathf.Abs(currentMousePos.y - recordMousePos.y));
        deltaMousePos = currentMousePos - recordMousePos;
        //根据鼠标移动的快慢(增量), 获得相机左右旋转的角度(处理X)  
        float rotationX = transform.localEulerAngles.y + deltaMousePos.x * sensitivityX*Time.deltaTime;

        //根据鼠标移动的快慢(增量), 获得相机上下旋转的角度(处理Y)  
        rotationY += deltaMousePos.y * sensitivityY * Time.deltaTime;
        //角度限制. rotationY小于min,返回min. 大于max,返回max. 否则返回value   
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        //总体设置一下相机角度  
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

}
