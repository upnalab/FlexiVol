using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._17_EmulatorControls
{
    /// <summary>
    /// MonoBehaviour that adds controls for the Voxon Runtime Simulators 
    /// 'camera' orientation and zoom level.
    /// </summary>
    public class EmulatorControls : MonoBehaviour
    {
        /// <summary>
        /// Vertical angle of display in radians. Range [0,pi/2]
        /// </summary>
        public float VerticleAngle;
        /// <summary>
        /// Horizontal angle of display in radians. Range [-pi,pi]
        /// </summary>
        public float HorizontalAngle;
        /// <summary>
        /// Distance of display. Range [400,4000]
        /// </summary>
        public float Distance;


        // Update is called once per frame
        /// <summary>
        /// Called per frame.
        /// Update horizontal rotation via "Right" and "Left" keys
        /// Update vertical rotation via "Up" and "Down" keys
        /// Update distance via "In" and "Out" keys
        /// </summary>
        void Update()
        {

            int horizontalDisplacement = (Voxon.Input.GetKey("Right") ? -1 : 0) +
                                            (Voxon.Input.GetKey("Left") ? 1 : 0);
            if (horizontalDisplacement != 0)
            {
                HorizontalAngle = VXProcess.Runtime.GetEmulatorHorizontalAngle();
                HorizontalAngle += horizontalDisplacement * Time.deltaTime;
                HorizontalAngle = VXProcess.Runtime.SetEmulatorHorizontalAngle(HorizontalAngle);
            }

            int verticalDisplacement = (Voxon.Input.GetKey("Down") ? -1 : 0) +
                                          (Voxon.Input.GetKey("Up") ? 1 : 0);
            if (verticalDisplacement != 0)
            {
                VerticleAngle = VXProcess.Runtime.GetEmulatorVerticalAngle();
                VerticleAngle += verticalDisplacement * Time.deltaTime;
                VerticleAngle = VXProcess.Runtime.SetEmulatorVerticalAngle(VerticleAngle);
            }

            int distanceDisplacement = (Voxon.Input.GetKey("In") ? -1 : 0) +
                                      (Voxon.Input.GetKey("Out") ? 1 : 0);

            if (distanceDisplacement != 0)
            {
                Distance = VXProcess.Runtime.GetEmulatorDistance();
                Distance += distanceDisplacement * Time.deltaTime * 1000;
                Distance = VXProcess.Runtime.SetEmulatorDistance(Distance);
            }
        }
    }
}