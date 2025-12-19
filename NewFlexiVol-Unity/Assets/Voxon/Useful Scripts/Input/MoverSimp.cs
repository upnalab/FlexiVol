using UnityEngine;

namespace Voxon.Examples.Input
{
    public class MoverSimp : MonoBehaviour {
        private bool _hidden;

        public float moveSpeed = 1f;

        
        void Start()
        {

        }


        // Update is called once per frame
        void Update () {


            Vector3 pos = transform.position;
            if (Voxon.Input.GetKey("Left"))
            {
                pos.x -= moveSpeed * Time.deltaTime;
                ;
            }
            if (Voxon.Input.GetKey("Right"))
            {
                pos.x += moveSpeed * Time.deltaTime;
                ;
            }
            if (Voxon.Input.GetKey("Forward"))
            {
                pos.z += moveSpeed * Time.deltaTime;
                ;
            }
            if (Voxon.Input.GetKey("Back"))
            {
                pos.z -= moveSpeed * Time.deltaTime;
                ;
            }   
            if (Voxon.Input.GetKey("Up"))
            {
                pos.y -= moveSpeed * Time.deltaTime;
                ;
            }
            if (Voxon.Input.GetKey("Down"))
            {
                pos.y += moveSpeed * Time.deltaTime;
                ;
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
