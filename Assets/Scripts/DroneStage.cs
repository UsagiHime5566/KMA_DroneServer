using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using Sirenix.OdinInspector;

public class DroneStage : HimeLib.SingletonMono<DroneStage>
{
    public List<DroneSection> droneSections;
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
        
        [ShowIf("commandType", CommandType.Move)]
        public Vector3 targetPosition;

        [ShowIf("@this.commandType == CommandType.Up || this.commandType == CommandType.Down")][Range(20, 500)]
        public int Amount;
        //public float castTime;
    }

    public enum CommandType
    {
        Init = 1,
        Takeoff = 2,
        Land = 3,
        Move = 4,
        Up = 11,
        Down = 12,
    }
}