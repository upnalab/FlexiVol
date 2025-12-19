using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
/* This script allows an object to be moved, rotated and scale by the arrow keys
 * Move = Arrow
 * Rotate = WSAD
 * 
 */
public class MoveRotRaw : MonoBehaviour
{
    private bool _hidden;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {

        if (VXProcess.Instance.IsClosingVXProcess() == true || VXProcess.Runtime == null) return;

        Vector3 pos = transform.position;
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Left))
        {
            pos.x -= 0.1f;
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Right))
        {
            pos.x += 0.1f;
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Up))
        {
            pos.z += 0.1f;
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Down))
        {
            pos.z -= 0.1f;
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Q))
        {
            pos.y += 0.1f;
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Z))
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
        else if (Voxon.Input.GetKeyDown("Hide"))
        {
            tag = "Untagged";
            _hidden = !_hidden;
        }
        
        VXProcess.Instance.EulerAngles = worldRot;

        transform.position = pos;
    }

}
