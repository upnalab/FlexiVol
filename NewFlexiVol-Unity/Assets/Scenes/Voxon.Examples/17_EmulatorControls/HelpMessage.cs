using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._17_EmulatorControls
{
    /// <summary>
    /// Help message to be displayed in simulator or touch screen when
    /// example 17 is used.
    /// </summary>
    public class HelpMessage : MonoBehaviour
    {
        /// <summary>
        /// Called on Start.
        /// Adds help information to Voxon Log
        /// </summary>
        void Start()
        {
            VXProcess.add_log_line("");
            VXProcess.add_log_line("Control the Emulator angle, rotation and zoom.");
            VXProcess.add_log_line("Angle: Up <W>   Down <S>");
            VXProcess.add_log_line("Rotate: Left <A>   Right <D>");
            VXProcess.add_log_line("Zoom: In <Q>   Out <E>");
        }
    }
}