using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using OscJack;

public class DroneSetup : HimeLib.SingletonMono<DroneSetup>
{
    public int targetPort;
    public List<DroneConfig> droneConfigs;

    public string TestOscCommad1 = "1on";
    public string TestOscCommad2 = "1on";

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
            conf._client.Send(TestOscCommad1, TestOscCommad2); // Second element
        }
    }

    public void CommandDronePosition(int index, Vector3 pos){

    }

    public void CommandDrone(int index, string oscCommand, int leftRight = 0, int fowardBackward = 0, int upDown = 0, int yaw = 0){

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