using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Drone;

public class DronePlay : HimeLib.SingletonMono<DronePlay>
{
    public DroneStageNodeGraph droneStageNodeGraph;
    public Button BTN_StartButton;
    public Text TXT_Log;
    public int maxQueueNum = 35;
    Queue<string> QueueLog;
    
    void DebugQueueLog(string msg){
        QueueLog.Enqueue(msg);
        if(QueueLog.Count > maxQueueNum)
            QueueLog.Dequeue();

        string result = "";
        foreach (var s in QueueLog)
        {
            result = s + "\n" + result;
        }
        TXT_Log.text = result;
    }
    void Start()
    {
        QueueLog = new Queue<string>();
        BTN_StartButton.onClick.AddListener(PlayDroneStage);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            PlayDroneStage();
        }
    }

    public void PlayDroneStage(){
        DebugQueueLog("Stage Start!");
        StartCoroutine(StartDronePlay());
    }

    IEnumerator StartDronePlay(){
        DroneClip startNode = droneStageNodeGraph.GetStartNode();

        float startTime = Time.time;

        yield return ExecuteNode(startNode);

        yield return null;

        float playTime = Time.time - startTime;
        Debug.Log($"Drone Play Finished. ({playTime})");
    }

    IEnumerator ExecuteNode(DroneClip clip){

        foreach (var cmd in clip.droneCommand)
        {
            ExecuteCommand(cmd.droneIndex, cmd.commandType, new DroneCommandParams(){intValue = cmd.Amount, vec3Value = cmd.targetPosition});
        }

        yield return new WaitForSeconds(clip.delayTime);

        Debug.Log($"Clip '{clip.name}' Finished.");
        var nextNode = clip.NextNode();
        if(nextNode != null)
            yield return ExecuteNode(nextNode);
    }
    
    void ExecuteCommand(int deviceIndex, CommandType commandType, DroneCommandParams para){
        string output = "";

        if(commandType == CommandType.Init){
            output = TelloCommands.command;
        }
        if(commandType == CommandType.Takeoff){
            output = TelloCommands.takeoff;
        }
        if(commandType == CommandType.Land){
            output = TelloCommands.land;
        }
        if(commandType == CommandType.Stop){
            output = TelloCommands.stop;
        }
        if(commandType == CommandType.Up){
            output = TelloCommands.up + " " + para.intValue;
        }
        if(commandType == CommandType.Down){
            output = TelloCommands.down + " " + para.intValue;
        }
        if(commandType == CommandType.Cw){
            output = TelloCommands.cw + " " + para.intValue;
        }
        if(commandType == CommandType.Ccw){
            output = TelloCommands.ccw + " " + para.intValue;
        }
        DroneSetup.instance.CommandOSCOut(deviceIndex, output);
        DebugQueueLog($"Execute: ({deviceIndex}) {output}");
    }
}




public class TelloCommands
{
    public static string takeoff = "takeoff";
    public static string land = "land";
    public static string command = "command";
    public static string stop = "stop";
    public static string rc = "rc";             //with 4 param
    public static string up = "up";             //with 1 param
    public static string down = "down";         //with 1 param
    public static string forward = "forward";  //with 1 param
    public static string back = "back";        //with 1 param
    public static string left = "left";        //with 1 param
    public static string right = "right";      //with 1 param
    public static string cw = "cw";             //with 1 param
    public static string ccw = "ccw";           //with 1 param
    public static string speed = "speed";      //with 1 param
    public static string[] noParamCommand = new string[] { takeoff, land, command, stop };
    public static string[] withParamCommand = new string[] { rc, up, down, forward, back, left, right, cw, ccw };
}

public class ArduinoCommands
{
    public static string on1 = "1on";
    public static string on2 = "2on";
    public static string off1 = "1off";
    public static string off2 = "2off";
    public static string con = "con";
    public static string coff = "coff";
    public static string close = "close";
    public static string[] noParamCommand = new string[] { on1, on2, off1, off2, con, coff, close };
}