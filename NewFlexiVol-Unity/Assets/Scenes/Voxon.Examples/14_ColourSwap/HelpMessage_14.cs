using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class HelpMessage_14 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VXProcess.add_log_line("");
        VXProcess.add_log_line("Cycle between color modes with the <SpaceBar>");
        VXProcess.add_log_line("Use the Voxon menu to confirm color mode changes.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
