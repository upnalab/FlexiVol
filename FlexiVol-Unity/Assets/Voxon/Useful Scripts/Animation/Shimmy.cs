using UnityEngine;

/** Shimmy a script to rotate and/or shimmy an object 
* Matthew Vecchio for VOXON
* v 1.0
* 
*/
namespace Voxon.Examples.Animation
{
    public class Shimmy : MonoBehaviour {

        [Tooltip("The speed of rotation 100 is default. Value are 0 - infinity")]
        public float speed = 100; 
        public enum Direction { NONE, LEFT, RIGHT, UP, DOWN, FORWARD, BACKWARD,  }

        [Tooltip("The direction you wish the object to rotate")]
        public Direction directionSelect;
        [Tooltip("The 2nd direction you wish the object to rotate")]
        public Direction directionSelectSub;

        [Tooltip("Enable this to have the object reverse its direction after a set point")]
        public bool directionSwitch;

        [Tooltip("The first point to switch the direction of the object. Value is between 0 and 1")]
        public float changeThreshold1 = 0.5f;
        [Tooltip("The second point to switch the direction of the object. Value is between 0 and 1")]
        public float changeThreshold2 = 0.5f;


        // Update is called once per frame
        void Update() {

            if (directionSwitch)
            {
                switch (directionSelect)
                {

                    case Direction.RIGHT:
                        if (transform.rotation.x > changeThreshold1)
                        {

                            directionSelect = Direction.LEFT;
                        }
                        break;
                    case Direction.LEFT:
                        if (transform.rotation.x < -changeThreshold2)
                        {

                            directionSelect = Direction.RIGHT;
                        }
                        break;
                    case Direction.UP:
                
                        if (transform.rotation.y > changeThreshold1)
                        {
                       
                            directionSelect = Direction.DOWN;
                        }
                        break;
                    case Direction.DOWN:
                  
                        if (transform.rotation.y  < -changeThreshold2)
                        {
                      
                            directionSelect = Direction.UP;
                        }
                        break;
                    case Direction.FORWARD:
                        if (transform.rotation.z > changeThreshold1)
                        {

                            directionSelect = Direction.BACKWARD;
                        }
                        break;
                    case Direction.BACKWARD:
                        if (transform.rotation.z < -changeThreshold2)
                        {

                            directionSelect = Direction.FORWARD;
                        }
                        break;

                }

                switch (directionSelectSub)
                {

                    case Direction.RIGHT:
                        if (transform.rotation.x > changeThreshold1)
                        {

                            directionSelectSub = Direction.LEFT;
                        }
                        break;
                    case Direction.LEFT:
                        if (transform.rotation.x < -changeThreshold2)
                        {

                            directionSelectSub = Direction.RIGHT;
                        }
                        break;
                    case Direction.UP:

                        if (transform.rotation.y > changeThreshold1)
                        {

                            directionSelectSub = Direction.DOWN;
                        }
                        break;
                    case Direction.DOWN:

                        if (transform.rotation.y < -changeThreshold2)
                        {

                            directionSelectSub = Direction.UP;
                        }
                        break;
                    case Direction.FORWARD:
                        if (transform.rotation.z > changeThreshold1)
                        {

                            directionSelectSub = Direction.BACKWARD;
                        }
                        break;
                    case Direction.BACKWARD:
                        if (transform.rotation.z < -changeThreshold2)
                        {

                            directionSelectSub = Direction.FORWARD;
                        }
                        break;

                }
            }


            switch (directionSelect) {

                case Direction.RIGHT:
                    transform.Rotate(speed * Time.deltaTime * Vector3.right);
                    break;
                case Direction.LEFT:
                    transform.Rotate(speed * Time.deltaTime * Vector3.left);
                    break;
                case Direction.UP:
                    transform.Rotate(speed * Time.deltaTime * Vector3.up);
                    break;
                case Direction.DOWN:
                    transform.Rotate(speed * Time.deltaTime * Vector3.down);
                    break;
                case Direction.FORWARD:
                    transform.Rotate(speed * Time.deltaTime * Vector3.forward);
                    break;
                case Direction.BACKWARD:
                    transform.Rotate(speed * Time.deltaTime * Vector3.back);
                    break;

            }

            switch (directionSelectSub)
            {

                case Direction.RIGHT:
                    transform.Rotate(speed * Time.deltaTime * Vector3.right);
                    break;
                case Direction.LEFT:
                    transform.Rotate(speed * Time.deltaTime * Vector3.left);
                    break;
                case Direction.UP:
                    transform.Rotate(speed * Time.deltaTime * Vector3.up);
                    break;
                case Direction.DOWN:
                    transform.Rotate(speed * Time.deltaTime * Vector3.down);
                    break;
                case Direction.FORWARD:
                    transform.Rotate(speed * Time.deltaTime * Vector3.forward);
                    break;
                case Direction.BACKWARD:
                    transform.Rotate(speed * Time.deltaTime * Vector3.back);
                    break;

            }
        }
    }
}
