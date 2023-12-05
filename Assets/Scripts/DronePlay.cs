using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DronePlay : HimeLib.SingletonMono<DronePlay>
{
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
        foreach (var section in DroneStage.instance.droneSections)
        {
            
        }

        yield return null;
    }
    
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