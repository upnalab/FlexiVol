
using UnityEngine;

namespace Voxon
{
    /// <summary>  
    ///  Voxon.Input is a Unity input replacement. Utilises Keybindings as set in Capture Volume
    ///  </summary>
    ///  <remarks>
    ///  Unity.Input does not allow input via -Batchmode (required for VX1), thus requiring the use of Voxon.Input
    ///  For single player simply replace Input with Voxon.Input and ensure binding strings are available in Input Controller (found on Capture Volume)
    ///  For multiplayer games; use GetXY(BindingName, PlayerNumber). Players are numbered 0-3.
    /// </remarks>  
    public static class Input
    {
        // Keyboard Input
        public static bool GetKey(string keyName)
        {
            if (VXProcess.Runtime == null) return false ;
            return VXProcess.Runtime.GetKey((int)InputController.GetKey(keyName));
        }

        public static bool GetKeyUp(string keyName)
        {
            if (VXProcess.Runtime == null) return false;
            return VXProcess.Runtime.GetKeyUp((int)InputController.GetKey(keyName));
        }

        public static bool GetKeyDown(string keyName)
        {
            if (VXProcess.Runtime == null) return false;
            return VXProcess.Runtime.GetKeyDown((int)InputController.GetKey(keyName));
        }

        // Player 1 Default Input

        // Multiplayer Input
        public static bool GetButton(string buttonName, int player = 0)
        {
            return VXProcess.Runtime.GetButton((int)InputController.GetButton(buttonName, player), player);
        }

        public static bool GetButtonDown(string buttonName, int player = 0)
        {
            return VXProcess.Runtime.GetButtonDown((int)InputController.GetButton(buttonName, player), player);
        }

        public static bool GetButtonUp(string buttonName, int player = 0)
        {
            return VXProcess.Runtime.GetButtonUp((int)InputController.GetButton(buttonName, player), player);
        }

        public static float GetAxis(string axisName, int player = 0)
        {
            return VXProcess.Runtime.GetAxis((int)InputController.GetAxis(axisName, player), player);
        }

        public static bool GetMouseButtonDown(string buttonName)
        {
            return VXProcess.Runtime.GetMouseButtonDown((int)InputController.GetMouseButton(buttonName));
        }

        public static bool GetMouseButton(string buttonName)
        {
            return VXProcess.Runtime.GetMouseButton((int)InputController.GetMouseButton(buttonName));
        }

        public static MousePosition GetMousePos()
        {
            return new MousePosition(VXProcess.Runtime.GetMousePosition());
        }

        public static Vector3 GetSpaceNavPos()
        {
            var fpos = VXProcess.Runtime.GetSpaceNavPosition();
            return new Vector3(fpos[0]/350,-fpos[2]/350, fpos[1]/350);
        }
        
        public static Vector3 GetSpaceNavRot()
        {
            var frot = VXProcess.Runtime.GetSpaceNavRotation();
            return new Vector3(frot[0]/35,-frot[2]/35, frot[1]/35);
        }

        public static bool GetSpaceNavButton(string buttonName)
        {
            return VXProcess.Runtime.GetSpaceNavButton((int) InputController.GetSpaceNavButton(buttonName));
        }
        
    }
}