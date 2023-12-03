using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;

public class DroneStage : HimeLib.SingletonMono<DroneStage>
{
    public List<DroneSection> droneSections;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

namespace Drone
{
    [Serializable]
    public class DroneSection
    {
        public List<DroneCommand> droneCommand;
        public float delayTime;
    }

    [Serializable]
    public class DroneCommand
    {
        [Range(0, 7)]
        public int droneIndex;
        public CommandType commandType;
        public Vector3 targetPosition;
        public float castTime;
    }

    public enum CommandType
    {
        Fly = 1,
        Move = 2,
        Land = 3,
    }
}