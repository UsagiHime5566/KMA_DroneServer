using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using OscJack;
using Sirenix.OdinInspector;

public class DroneSetup : HimeLib.SingletonMono<DroneSetup>
{
    public int targetPort;
    public List<DroneConfig> droneConfigs;
    public List<int> Batterys;

    [Title("初始化常數")]
    public float basePointUp = 0.3f;
    public float tweenSpeed = 0.5f;


    [Title("追蹤參數")]
    [InfoBox("傳送指令速度", InfoMessageType.None)]
    public float commandSendFrequency = 0.2f;

    [InfoBox("多少距離內視為已抵達", InfoMessageType.None)]
    public float DroneReachDistance = 0.05f;

    [InfoBox("飛行速度", InfoMessageType.None)]
    public float DroneFlyAmountAmp = 0.8f;

    [InfoBox("可反向飛行速度(接近目標時)", InfoMessageType.None)]
    public float DroneFlyInversAmountAmp = -0.5f;

    [InfoBox("方向偏離多少時, 要迴轉", InfoMessageType.None)]
    public float DroneReRotateTrig = 15;

    [InfoBox("迴轉速度", InfoMessageType.None)]
    public float DroneReRotateSpeed = 0.2f;

    [Title("測試用指令")]
    public string OSCPrefixAddress = "/cmd";
    public string TestOscCommad = "command";

    void Start()
    {
        StartConnectedOSCClient();
    }

    public void StartConnectedOSCClient(){
        foreach (var conf in droneConfigs)
        {
            conf._client = new OscClient(conf._ip, targetPort);
        }
    }

    [Button("傳送測試")]
    public void BrocastTestOSC(){
        foreach (var conf in droneConfigs)
        {
            conf._client.Send(OSCPrefixAddress, TestOscCommad); // Second element
        }
    }

    [Button("快速起飛", ButtonSizes.Large)]
    public void BrocastTakeoff(){
        foreach (var conf in droneConfigs)
        {
            conf._client.Send(OSCPrefixAddress, TelloCommands.takeoff); // Second element
        }
    }

    [Button("快速降落", ButtonSizes.Large)]
    public void BrocastLand(){
        foreach (var conf in droneConfigs)
        {
            conf._client.Send(OSCPrefixAddress, TelloCommands.land); // Second element
        }
    }

    public void BrocastCustom(string msg){
        foreach (var conf in droneConfigs)
        {
            conf._client.Send(OSCPrefixAddress, msg); // Second element
        }
    }

    public void PrepareAndLand(int deviceIndex){
        droneConfigs[deviceIndex]._object.PrepareLanding();
    }

    public void CommandLandImmediate(int deviceIndex){
        droneConfigs[deviceIndex]._client.Send(OSCPrefixAddress, TelloCommands.land);
    }

    public int FindMyIndex(DroneObject obj){
        for (int i = 0; i < droneConfigs.Count; i++)
        {
            if(droneConfigs[i]._object == obj){
                return i;
            }
        }
        return -1;
    }

    //用於追蹤指令
    public void CommandDroneMoveToPos(int deviceIndex, float x, float y, float z, float r){

        //                              旋轉,上下,左右,前後
        //Tello.controllerState.setAxis(lx,  ly,  rx, ry);
        
        string oscCommand = $"{TelloCommands.axis} {r} {y} {x} {z}";
        droneConfigs[deviceIndex-1]._client.Send(OSCPrefixAddress, oscCommand);
    }

    //用於簡單指令, 起飛降落
    public void CommandDroneOsc(int index, string oscCommand){
        droneConfigs[index]._client.Send(OSCPrefixAddress, oscCommand);
    }

    public void CommandStation(int index, string oscCommand){

    }

    public void TurnAutoMove(bool turn){
        foreach (var item in droneConfigs)
        {
            item._object.AutoMove = turn;
        }
    }
}

namespace Drone
{
    [System.Serializable]
    public class DroneConfig
    {
        public string _ip;
        public OscClient _client;
        public DroneObject _object;
    }
}