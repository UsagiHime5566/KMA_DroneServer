using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Drone;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

public class DronePlay : HimeLib.SingletonMono<DronePlay>
{
    public DroneStageNodeGraph droneStageNodeGraph;
    public Button BTN_StartButton;
    public Button BTN_Stop;
    public Text TXT_Log;
    public Text TXT_Layout;
    public List<Toggle> EnabledDrone;
    public int maxQueueNum = 35;

    [Title("動態參數")]
    [InfoBox("目前是第幾排飛機的場", InfoMessageType.None)]
    public int currentDroneLayout = 0;
    Queue<string> QueueLog;

    bool isPlaying = false;
    
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
        BTN_Stop.onClick.AddListener(StopDroneImmediate);

        for (int i = 0; i < EnabledDrone.Count; i++)
        {
            var val = i;
            EnabledDrone[val].onValueChanged.AddListener(x => {
                SystemConfig.Instance.SaveData($"enable{val}", x);
            });
            var eb = SystemConfig.Instance.GetData<bool>($"enable{val}", true);
            EnabledDrone[val].isOn = eb;
        }
    }

    public void OscRecieveDroneCommand(string x){
        Debug.Log($"OSC come : {x}");

        if(x == "GO"){
            PlayDroneStage();
        }
    } 

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            PlayDroneStage();
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            StopDroneImmediate();
        }
    }

    public void StopDroneImmediate(){
        isPlaying = false;
        DroneSetup.instance.TurnAutoMove(false);
        StartCoroutine(DoStop());

        IEnumerator DoStop(){
            DroneSetup.instance.BrocastCustom($"axis 0 0 0 0");
            yield return new WaitForSeconds(1);
            DroneSetup.instance.BrocastTakeoff();
            yield return null;
        }
    }

    public void PlayDroneStage(){
        DebugQueueLog("Stage Start!");
        TXT_Layout.text = currentDroneLayout.ToString();
        StartCoroutine(StartDronePlay(currentDroneLayout));
        currentDroneLayout = (currentDroneLayout + 1) % 2;
    }

    

    IEnumerator StartDronePlay(int clipLayout){
        DroneClip startNode = droneStageNodeGraph.GetStartNode();

        float startTime = Time.time;
        DroneSetup.instance.TurnAutoMove(true);

        DebugQueueLog($"---- Start Stage ----");
        isPlaying = true;

        DroneSetup.instance.ClearTarget();

        //DroneSetup.instance.BrocastTakeoff();
        //yield return new WaitForSeconds(5);
        
        // for (int i = 0; i < 62; i++)
        // {
        //     if(DroneSetup.instance.CheckMissing(0, 3) == true){
        //         DroneSetup.instance.BrocastDown();
        //         yield return new WaitForSeconds(2);
        //     } else {
        //         break;
        //     }
        // }

        yield return ExecuteNode(startNode, clipLayout);

        yield return null;

        float playTime = Time.time - startTime;
        Debug.Log($"Drone Play Finished. ({playTime})");
        DebugQueueLog($"Drone Play Finished. ({playTime})");
    }

    IEnumerator ExecuteNode(DroneClip clip, int clipLayout){

        DebugQueueLog($"---- Clip {clip.name} ----");

        if(isPlaying == false){
            yield break;
        }

        foreach (var cmd in clip.droneCommand)
        {
            //droneIndex 是劇本裡的index, 範圍是 0~3
            int targetDeviceIndex = cmd.droneIndex + clipLayout * 4;
            if(EnabledDrone[targetDeviceIndex].isOn){
                ExecuteCommand(cmd.droneIndex + clipLayout * 4, cmd.commandType, new DroneCommandParams(){vec3Value = cmd.targetPosition});
            }
        }

        yield return new WaitForSeconds(clip.delayTime);

        Debug.Log($"Clip '{clip.name}' Finished.");
        var nextNode = clip.NextNode();
        if(nextNode != null)
            yield return ExecuteNode(nextNode, clipLayout);
    }
    
    void ExecuteCommand(int deviceIndex, StageCommandType commandType, DroneCommandParams para){
        string output = "";

        switch (commandType)
        {
            case StageCommandType.初始化:
                output = TelloCommands.command;
                break;
            case StageCommandType.起飛:
                output = TelloCommands.takeoff;
                break;
            case StageCommandType.降落:
                DroneSetup.instance.PrepareAndLand(deviceIndex);
                DebugQueueLog($"Execute: ({deviceIndex}) {output}");
                return;
            case StageCommandType.停滯:
                output = TelloCommands.stay;
                break;
            case StageCommandType.推桿1on:
                output = ArduinoCommands.on1;
                break;
            case StageCommandType.推桿2on:
                output = ArduinoCommands.on2;
                break;
            case StageCommandType.推桿1off:
                output = ArduinoCommands.off1;
                break;
            case StageCommandType.推桿2off:
                output = ArduinoCommands.off2;
                break;
            case StageCommandType.開關:
                output = ArduinoCommands.turnpower;
                break;
            case StageCommandType.開機PowerOn:
                output = ArduinoCommands.poweron;
                break;
            case StageCommandType.關機PowerOff:
                output = ArduinoCommands.poweroff;
                break;
            case StageCommandType.充電on:
                output = ArduinoCommands.con;
                break;
            case StageCommandType.充電off:
                output = ArduinoCommands.coff;
                break;
            case StageCommandType.重開中繼程式:
                output = ComputerCommands.restart;
                break;
            case StageCommandType.B移至起點:
                //不送移動指令是因為, 他有自動追蹤功能, 所以從飛機自身去送飛行指令
                var v = DroneSetup.instance.droneConfigs[deviceIndex]._object.CreateTempBottomLocal();
                DebugQueueLog($"Execute: ({deviceIndex}) move (0 0 0) (local)");
                return;
            case StageCommandType.M移動至:
                //不送移動指令是因為, 他有自動追蹤功能, 所以從飛機自身去送飛行指令
                DroneSetup.instance.droneConfigs[deviceIndex]._object.CreateTempTargetPointLocal(para.vec3Value);
                DebugQueueLog($"Execute: ({deviceIndex}) move {para.vec3Value} (local)");
                return;
        }

        DroneSetup.instance.CommandDroneOsc(deviceIndex, output);
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

    //SDK Command
    public static string axis = "axis";
    public static string stay = "stay";
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
    public static string turnpower = "turnpower";
    public static string poweron = "poweron";
    public static string poweroff = "poweroff";
    public static string[] noParamCommand = new string[] { on1, on2, off1, off2, con, coff, turnpower, poweron, poweroff };
}

public class ComputerCommands
{
    public static string restart = "restart";
    public static string[] noParamCommand = new string[] { restart };
}