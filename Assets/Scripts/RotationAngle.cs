using UnityEngine;

public class RotationAngle : MonoBehaviour
{
    void Update()
    {
        // 获取物体当前的正前方向（z轴正向）
        Vector3 forwardDirection = transform.forward;
        //Debug.Log($"fd:{forwardDirection}");

        Vector3 xyDirect = new Vector3(forwardDirection.x, 0, forwardDirection.z);
        //Debug.Log($"fd:{xyDirect}");

        // 获取Z轴正向
        Vector3 zAxis = Vector3.forward;
        //Debug.Log($"z:{zAxis}");

        // 计算物体当前面向与Z轴正向之间的差距的角度
        float angle = Vector3.Angle(xyDirect, zAxis);

        // 输出角度
        //Debug.Log("角度差距: " + angle);

        // 如果需要知道方向差距，可以使用以下代码
        // Vector3 crossProduct = Vector3.Cross(forwardDirection, zAxis);
        // float direction = crossProduct.y < 0 ? -angle : angle;
        //Debug.Log("方向差距: " + direction);

        transform.Rotate(0, Mathf.Lerp(0, -angle, 0.2f), 0);
    }
}
