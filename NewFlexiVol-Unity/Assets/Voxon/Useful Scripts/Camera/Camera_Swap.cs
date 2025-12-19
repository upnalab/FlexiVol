using UnityEngine;

namespace Voxon.Examples.Camera
{
    public class CameraSwap : MonoBehaviour {
        private int _cameraIndex;
        public VXCamera [] cameras;

        // Update is called once per frame
        void Update () {
            if(Voxon.Input.GetKeyDown("CameraSwapUp"))
            {
                _cameraIndex = Mathf.Min(++_cameraIndex, cameras.Length - 1);
                VXProcess.Instance.Camera = cameras[_cameraIndex];
            
            }

            if (Voxon.Input.GetKeyDown("CameraSwapDown"))
            {
                _cameraIndex = Mathf.Max(--_cameraIndex, 0);
                VXProcess.Instance.Camera = cameras[_cameraIndex];
            
            }
        }
    }
}
