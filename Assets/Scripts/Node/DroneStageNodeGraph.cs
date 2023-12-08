using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class DroneStageNodeGraph : NodeGraph { 
    public DroneClip startNode;
	public DroneClip GetStartNode(){
        return startNode;
    }
}