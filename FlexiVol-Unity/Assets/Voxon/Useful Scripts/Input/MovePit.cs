using UnityEngine;

namespace Voxon.Examples.Input
{
    public class MovePit : MonoBehaviour {
        private bool _hidden;

        // Update is called once per frame
        void Update () {
            Vector3 pos = transform.position;
            if (Voxon.Input.GetKey("Cam_Left"))
            {
                pos.x -= 0.1f;
            }
            if (Voxon.Input.GetKey("Cam_Right"))
            {
                pos.x += 0.1f;
            }
            if (Voxon.Input.GetKey("Cam_Forward"))
            {
                pos.z += 0.1f;
            }
            if (Voxon.Input.GetKey("Cam_Backward"))
            {
                pos.z -= 0.1f;
            }   
            if (Voxon.Input.GetKey("Cam_Up"))
            {
                pos.y += 0.1f;
            }
            if (Voxon.Input.GetKey("Cam_Down"))
            {
                pos.y -= 0.1f;
            }

            Vector3 worldRot = VXProcess.Instance.EulerAngles;
            if (Voxon.Input.GetKey("RotLeft"))
            {
                worldRot.y += 1f;
            }
            if (Voxon.Input.GetKey("RotRight"))
            {
                worldRot.y -= 1f;
            }

			if (Voxon.Input.GetKeyDown("Hide") && !_hidden)
            {
                VXProcess.add_log_line("Hiding: " + _hidden.ToString());
                tag = "VoxieHide";
                _hidden = !_hidden;
            }
            else if(Voxon.Input.GetKeyDown("Hide"))
            {
                tag = "Untagged";
                _hidden = !_hidden;
            }

            VXProcess.Instance.EulerAngles = worldRot;

            transform.position = pos;
        }
    }
}
