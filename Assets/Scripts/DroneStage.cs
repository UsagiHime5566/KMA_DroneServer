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
        public StageCommandType commandType;
        
        [ShowIf("commandType", StageCommandType.M移動至)]
        public Vector3 targetPosition;
    }

    public enum StageCommandType
    {
        初始化 = 1,
        起飛 = 2,
        降落 = 3,
        M移動至 = 4,
        B移至起點 = 5,
        停滯 = 10,
    }

    [Serializable]
    public class DroneCommandParams
    {
        public Vector3 vec3Value;
    }
}