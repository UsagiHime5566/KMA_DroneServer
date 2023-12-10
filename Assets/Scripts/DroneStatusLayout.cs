using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneStatusLayout : MonoBehaviour
{
    public Text TXT_Index;
    public Text TXT_Battery;
    public Text TXT_Stats;
    public int sectionIndex = -1;

    private void OnValidate() {
        TXT_Index.text = sectionIndex.ToString();
    }

    public void RecieveBatteryInfo(string msg){
        TXT_Battery.text = $"Battery: {msg}";

        int.TryParse(msg, out int battery);
        int deviceIndex = Mathf.Clamp(sectionIndex, 0, DroneSetup.instance.Batterys.Count);
        DroneSetup.instance.Batterys[deviceIndex] = battery;
    }

    public void RecieveStatInfo(string msg){
        TXT_Stats.text = msg;
    }
}
