using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using OscJack;

public class DroneSetup : MonoBehaviour
{
    public int targetPort;
    public List<DroneConfig> droneConfigs;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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


    public class DroneOscCommand
    {
        public static string command = "/command";
    }

    public class StationOscCommand
    {
        public static string PushOn1 = "/1on";
        public static string PushOff1 = "/1off";
        public static string PushOn2 = "/2on";
        public static string PushOff2 = "/2off";
        
    }
}