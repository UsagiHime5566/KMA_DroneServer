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
        while(true){
            if(!AutoMove){
                yield return null;
                continue;
            }

            var delta = (targetPoint.position - transform.position).normalized;
            Vector3 xyDirect = new Vector3(targetPoint.forward.x, 0, targetPoint.forward.z);
            float angle = Vector3.Angle(xyDirect, Vector3.forward);
            if(Mathf.Abs(angle) < 15){
                angle = 0;
            } else {
                angle = 0.5f * angle/Mathf.Abs(angle);
            }

            Debug.Log($"send: {delta.x},{delta.y},{delta.z},{angle}");
            Debug.Log(optitrackRigidBody);
            
            if(optitrackRigidBody)
                DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, delta.x, delta.y, delta.z, angle);

            yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
        }
    }

    void Update(){
        AutoMove = false;
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            AutoMove = true;
        }
    }
}
