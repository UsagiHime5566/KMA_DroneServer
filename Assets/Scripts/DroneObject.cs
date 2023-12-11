using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneObject : MonoBehaviour
{
    public Transform targetPoint;
    OptitrackRigidBody optitrackRigidBody;
    public bool isEmul;

    public bool AutoMove = false;

    void Awake(){
        optitrackRigidBody = GetComponent<OptitrackRigidBody>();
    }

    void Start()
    {
        StartCoroutine(EmuMove());
        StartCoroutine(GoTarget());
    }

    IEnumerator EmuMove(){
        while(isEmul){
            
            var delta = targetPoint.position - transform.position;

            transform.Translate(Vector3.Lerp(Vector3.zero, delta, 0.2f));
            
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    IEnumerator GoTarget(){
        yield return null;
        while(true){
            if(!AutoMove){
                if(optitrackRigidBody){
                    DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, 0, 0, 0, 0);
                    yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                }
                yield return null;
                continue;
            }

            var delta = (targetPoint.position - transform.position).normalized;

            Vector3 eulerRotation = transform.rotation.eulerAngles;
            // 获取方位角（以Z轴正向为基准）
            float azimuthAngle = eulerRotation.y;
            // 计算与Z轴正向的差值
            float angleDifference = CalculateAngleDifference(azimuthAngle);

            Debug.Log($"fly angle {angleDifference}");
            if(Mathf.Abs(angleDifference) < 15){
                angleDifference = 0;
            } else {
                angleDifference = -0.5f * (angleDifference/Mathf.Abs(angleDifference));
            }


            //Debug.Log($"fly angle {angle}");

            //Debug.Log($"send: {delta.x},{delta.y},{delta.z},{angle}");
            //Debug.Log(optitrackRigidBody);
            
            if(optitrackRigidBody)
                DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, delta.x, delta.y, delta.z, angleDifference);

            yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
        }
    }

    float CalculateAngleDifference(float azimuthAngle)
    {
        // 以Z轴正向为基准
        float referenceAngle = 0f;

        // 计算差值（以180为周期）
        float angleDifference = (azimuthAngle - referenceAngle + 180f) % 360f - 180f;

        return angleDifference;
    }

    void Update(){
        AutoMove = false;
        if(Input.GetKey(KeyCode.LeftControl)){
            AutoMove = true;
        }
    }
}
