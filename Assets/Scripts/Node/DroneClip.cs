using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using XNode;
using Drone;

[NodeWidth(400)]
public class DroneClip : Node {
	[Input] public DroneClip fromNode;

	[Space(20)]
	public List<DroneCommand> droneCommand;

	[Space(20)]
	public float delayTime;

	[Space(20)]
	[Output] public DroneClip toNode;

	
	// Use this for initialization
	protected override void Init() {
		base.Init();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return this;
	}

	public DroneClip NextNode(){
		NodePort p = GetOutputPort("toNode");
		
		if(p.Connection != null)
			return p.Connection.node as DroneClip;
		
		return null;
	}
}