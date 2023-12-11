using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drone;
using Sirenix.OdinInspector;

public class DroneStage : HimeLib.SingletonMono<DroneStage>
{
    [ListDrawerSettings(ShowIndexLabels = true)]
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

        [ShowIf("@this.commandType == CommandType.Up || this.commandType == CommandType.Down || this.commandType == CommandType.Cw || this.commandType == CommandType.Ccw")][Range(20, 500)]
        public int Amount;
        //public float castTime;
    }

    public enum CommandType
    {
        初始化 = 1,
        起飛 = 2,
        降落 = 3,
        Move = 4,
        Bottom = 5,
        LocalMove = 6,
        Stop = 10,
        Up = 11,
        Down = 12,
        Cw = 21,
        Ccw = 22,
    }

    [Serializable]
    public class DroneCommandParams
    {
        public int intValue;
        public Vector3 vec3Value;
    }
}