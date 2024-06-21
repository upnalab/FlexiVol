using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class HelpMessage_15 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VXProcess.add_log_line("");
        VXProcess.add_log_line("See [Test Menu] option.");
        VXProcess.add_log_line("This button demonstrates the use of sliders, buttons and editor boxes");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
