using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class HelpMessage_17 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VXProcess.add_log_line("");
        VXProcess.add_log_line("Control the Emulator angle, rotation and zoom.");
        VXProcess.add_log_line("Angle: Up <W>   Down <S>");
        VXProcess.add_log_line("Rotate: Left <A>   Right <D>");
        VXProcess.add_log_line("Zoom: In <Q>   Out <E>");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
