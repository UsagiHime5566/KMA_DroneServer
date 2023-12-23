using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class DroneObject : MonoBehaviour
{
    [Header("Runtime setting for initialize")]
    public Transform BasePoint;

    [Header("暫存變數")]
    public Transform _TempCreatePoint;
    public bool AutoMove = false;

    [Space(30)]
    [Header("追蹤目標")]
    public Transform TargetPoint;

    public bool isPrepareLand = false;
    public Vector3 lastVector = Vector3.zero;
    
    public int trackingMissingCount = 0;
    public int trackingMissingThreshold = 20;
    

    //Local param
    OptitrackRigidBody optitrackRigidBody;

    void Awake(){
        optitrackRigidBody = GetComponent<OptitrackRigidBody>();
    }

    void Start()
    {
        StartCoroutine(GoTarget());
    }

    public void ResetStartPoint(){
        UpdateBasePoint();
    }

    IEnumerator LateBasePoint(){
        yield return null;
        UpdateBasePoint();
    }

    public void UpdateBasePoint(){
        if(BasePoint != null){
            Destroy(BasePoint.gameObject);
        }
        Vector3 b_point = transform.position + new Vector3(0, DroneSetup.instance.basePointUp, 0);
        BasePoint = Instantiate(PrefabData.instance.DroneBasePoint, b_point, Quaternion.identity);
        BasePoint.name += "_" + optitrackRigidBody.RigidBodyId;

        isPrepareLand = false;
    }

    public void CreateTempTargetPointLocal(Vector3 v3){
        CreateTempTargetPointLocal(v3.x, v3.y, v3.z);
    }

    public void CreateTempTargetPointLocal(float x, float y, float z){
        //上一個目標位置, 提供動畫讓畫面更好閱讀
        Vector3 lastPoint = TargetPoint ? TargetPoint.position : BasePoint.position;

        if(_TempCreatePoint){
            Destroy(_TempCreatePoint.gameObject);
        }

        Vector3 b_point = BasePoint.transform.position + new Vector3(x, y, z);
        _TempCreatePoint = Instantiate(PrefabData.instance.DroneGoalPoint, lastPoint, Quaternion.identity);
        _TempCreatePoint.name += "_" + optitrackRigidBody.RigidBodyId;

        _TempCreatePoint.DOMove(b_point, DroneSetup.instance.tweenSpeed).OnComplete(() => {
            TargetPoint = _TempCreatePoint;
        });

        isPrepareLand = false;
    }

    public Vector3 CreateTempBottomLocal(){
        CreateTempTargetPointLocal(0, 0, 0);
        return BasePoint.transform.position;
    }

    public void ClearTarget(){
        TargetPoint = null;
    }

    public void PrepareLanding(){
        isPrepareLand = true;
    }

    public bool TrackIsMissing(){
        return trackingMissingCount > trackingMissingThreshold;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            AutoMove = !AutoMove;
        }

        if(Vector3.Distance(lastVector, transform.position) == 0){
            trackingMissingCount++;
        } else {
            lastVector = transform.position;
            trackingMissingCount = 0;
        }
    }

    IEnumerator GoTarget(){
        yield return null;

        while(true){
            
            //沒有開啟追蹤狀態時, 不送狀態
            if(!AutoMove && isPrepareLand == false){
                yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                continue;
            }

            if(TrackIsMissing()){
                DroneSetup.instance.CommandLandImmediate(DroneSetup.instance.FindMyIndex(this));
                yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                DroneSetup.instance.CommandLandImmediate(DroneSetup.instance.FindMyIndex(this));
                yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                isPrepareLand = false;
                
                continue;
            }

            //查看目前方向
            float angleDifference = ExternFunc.CalculateAngleDifferenceZ(transform.rotation.eulerAngles.y);
            {
                //Debug.Log($"fly return angle {angleDifference}");

                //方向錯誤時, 校正優先!
                if(Mathf.Abs(angleDifference) > DroneSetup.instance.DroneReRotateTrig){
                    angleDifference = -DroneSetup.instance.DroneReRotateSpeed * (angleDifference/Mathf.Abs(angleDifference));
                    DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, 0, 0, 0, angleDifference);
                    yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                    continue;
                }
            }

            if(isPrepareLand){
                TargetPoint.position = BasePoint.position + new Vector3(0, DroneSetup.instance.landPointup, 0);
                Debug.Log($"({optitrackRigidBody.RigidBodyId}) 嘗試降落中:" + Vector3.Distance(TargetPoint.position, transform.position));
                if(Vector3.Distance(TargetPoint.position, transform.position) < DroneSetup.instance.DroneReachDistance){
                    
                    Debug.Log($"即將降落");

                    DroneSetup.instance.CommandLandImmediate(DroneSetup.instance.FindMyIndex(this));
                    yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                    DroneSetup.instance.CommandLandImmediate(DroneSetup.instance.FindMyIndex(this));
                    
                    isPrepareLand = false;
                    AutoMove = false;

                    continue;
                }
            }

            //追蹤狀態開啟↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
            if(TargetPoint == null){
                DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, 0, 0, 0, 0);
                yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                continue;
            }

            //抵達目標時, 傳送停滯
            if(Vector3.Distance(TargetPoint.position, transform.position) < DroneSetup.instance.DroneReachDistance){
                DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, 0, 0, 0, 0);
                yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
                continue;
            }

            //尚未抵達目標, 開始計算該行走的方向
            {
                var delta = (TargetPoint.position - transform.position).normalized;
                var amp = DroneSetup.instance.DroneFlyAmountAmp;

                //接近目標時, 反向減速
                if(Vector3.Distance(TargetPoint.position, transform.position) < DroneSetup.instance.DroneReachDistance * 2){
                    delta *= DroneSetup.instance.DroneFlyInversAmountAmp;
                }
                var d_delta = new Vector3(ExternFunc.CutFloat(delta.x * amp), ExternFunc.CutFloat(delta.y * amp), ExternFunc.CutFloat(delta.z * amp));

                //傳送追蹤量
                DroneSetup.instance.CommandDroneMoveToPos(optitrackRigidBody.RigidBodyId, d_delta.x, d_delta.y, d_delta.z, 0);
            }

            yield return new WaitForSeconds(DroneSetup.instance.commandSendFrequency);
        }
    }


    // IEnumerator EmuMove(){
    //     while(isEmul){
            
    //         var delta = TargetPoint.position - transform.position;

    //         transform.Translate(Vector3.Lerp(Vector3.zero, delta, 0.2f));
            
    //         yield return new WaitForSeconds(0.5f);
    //     }
    //     yield return null;
    // }
}
