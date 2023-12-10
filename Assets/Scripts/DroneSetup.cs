using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using OscJack;

public class DroneSetup : HimeLib.SingletonMono<DroneSetup>
{
    public int targetPort;
    public List<DroneConfig> droneConfigs;
    public List<int> Batterys;
    public float commandSendFrequency = 0.2f;
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

    [Sirenix.OdinInspector.Button]
    public void BrocastTestOSC(){
        foreach (var conf in droneConfigs)
        {
            conf._client.Send(OSCPrefixAddress, TestOscCommad); // Second element
        }
    }

    public void CommandDroneMoveToPos(int deviceIndex, float x, float y, float z, float r){
        
        //Tello.controllerState.setAxis(lx, ly, rx, ry);
        //旋轉,上下,左右,前後
        //Tello.controllerState.setAxis(r, y, x, z);

        string oscCommand = $"axis {x} {y} {z} {r}";
        droneConfigs[deviceIndex]._client.Send(OSCPrefixAddress, oscCommand);
    }

    public void CommandOSCOut(int index, string oscCommand){
        droneConfigs[index]._client.Send(OSCPrefixAddress, oscCommand);
    }

    public void CommandStation(int index, string oscCommand){

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