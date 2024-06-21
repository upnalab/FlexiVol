using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
/* Script to demonstrate how to write directly to the VoxieBox.dll.
 * 
 */ 

public class VoxonDirectDraw : MonoBehaviour, IDrawable
{

    point3d cursorPos = new point3d( 0.3f, 0.15f, -0.1f );
    int     cursorCol = 0xffffff;

    // Start is called before the first frame update
    void Start()
    {
        // Add this script to the VXProcess drawables...
        VXProcess.Drawables.Add(this);
    }

    public void Draw()
    {
        if (VXProcess.Instance.active == false || VXProcess.Runtime == null)
        {
            return;
        }

        point3d pp, rr, dd, ff;

        poltex[] vt = new poltex[4]; int[] mesh = new int[16]; int i = 0;
        vt[0].x = -0.4f; vt[0].y = -0.8f; vt[0].z = -0.2f; vt[0].col = 0xff0000;
        vt[1].x = -0.4f; vt[1].y = -0.4f; vt[1].z = +0.2f; vt[1].col = 0x00ff00;
        vt[2].x = +0.4f; vt[2].y = -0.8f; vt[2].z = +0.2f; vt[2].col = 0x0000ff;
        vt[3].x = +0.4f; vt[3].y = -0.4f; vt[3].z = -0.2f; vt[3].col = 0xff00ff;
        mesh[i++] = 0; mesh[i++] = 1; mesh[i++] = 2; mesh[i++] = -1; /*-1 = end of polygonal facet*/
        mesh[i++] = 1; mesh[i++] = 0; mesh[i++] = 3; mesh[i++] = -1;
        mesh[i++] = 2; mesh[i++] = 1; mesh[i++] = 3; mesh[i++] = -1;
        mesh[i++] = 0; mesh[i++] = 2; mesh[i++] = 3; mesh[i++] = -1;
        VXProcess.Runtime.DrawUntexturedMesh(vt, 4, mesh, i, 16 + 2, 0x404040); /* (for colour of vertices to work needd to have fill mode 16+ (0010000b) ) */

        VXProcess.Runtime.DrawBox(-0.9f, 0f, -0.4f, -0.5f, 0.4f, 0, 2, 0xff0000);

        pp.x = -0.4f; pp.y = 0; pp.z = -0.4f;
        rr.x = 0.4f; rr.y = 0; rr.z = 0;
        dd.x = 0; dd.y = 0.4f; dd.z = 0;
        ff.x = 0; ff.y = 0; ff.z = 0.4f;

        VXProcess.Runtime.DrawCube(ref pp, ref rr, ref dd, ref ff, 2, 0x00ff00);

        pp.x = +0.3f; pp.y = 0.2f; pp.z = -0.2f;
        rr.x = 0.4f; rr.y = 0; rr.z = 0;
        dd.x = 0; dd.y = 0.4f; dd.z = 0;
        ff.x = 0; ff.y = 0; ff.z = 0.4f;


        VXProcess.Runtime.DrawSphere(0.8f, 0.2f, -0.2f, .15f, 1, 0xffff00);

        VXProcess.Runtime.DrawLine(-0.8f, -0.2f, -0.4f, 0.8f, -0.2f, 0.4f, 0x00ffff);

        VXProcess.Runtime.DrawCone(-0.8f, 0.7f, 0.2f, 0.1f, 0.8f, 0.7f, -0.2f, 0.2f, 0, 0xff00ff);
  
        
        // Writing to the 2D display. 

        VXProcess.Runtime.Report("Flags", 400, 75);
        VXProcess.Runtime.Report("Keyboard", 30, 75);
        VXProcess.Runtime.LogToScreenExt(30, 300, 0x00ffff, -1, "Testing writing to the 2D screen");


        // Direct Input example -- bypassed The Input Manager

        cursorCol = 0xffffff;

        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Left))
        {
            cursorPos.x -= (1 * Time.deltaTime);
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Right))
        {
            cursorPos.x += (1 * Time.deltaTime);
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Up))
        {
            cursorPos.y -= (1 * Time.deltaTime);
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Arrow_Down))
        {
            cursorPos.y += (1 * Time.deltaTime);
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_W))
        {
            cursorPos.z -= (1 * Time.deltaTime);
        }
        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_S))
        {
            cursorPos.z += (1 * Time.deltaTime);
        }


        if (VXProcess.Runtime.GetKey((int)VX_KEYS.KB_Space_Bar))
        {
            cursorCol = 0x00ff00;
        }
        if (VXProcess.Runtime.GetKeyDown((int)VX_KEYS.KB_Space_Bar))
        {
            cursorCol = 0x0000ff;
        }
        if (VXProcess.Runtime.GetKeyDownTime((int)VX_KEYS.KB_Space_Bar) > 2)
        {
            cursorCol = 0xff00ff;
        }
        if (VXProcess.Runtime.GetKeyUp((int)VX_KEYS.KB_Space_Bar))
        {
            cursorCol = 0xffff00;
        }

        VXProcess.Runtime.DrawSphere(ref cursorPos, (float)Math.Cos(Time.time) * 0.1f, 0, cursorCol);
        VXProcess.Runtime.DrawSphere(ref cursorPos, 0.05f, 1, cursorCol);



    }


}

