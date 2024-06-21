using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;


/*
 *  Easy way to show Voxie Text on the display.
 * 
 * 
 * 
 * 
 */

namespace Voxon
{
    public class VoxieText : MonoBehaviour, IDrawable
    {

        point3d _pr, _pd, _pp;

        [FormerlySerializedAs("_pr")]
        [Tooltip("right vector - length is size of single character")]
        Vector3 pr = new Vector3(0.0f, 0.0f, 0.0f);
        [FormerlySerializedAs("_pd")]
        [Tooltip("down vector - length is height of character")]
        Vector3 pd = new Vector3(0.0f, 0.0f, 0.0f);
        [FormerlySerializedAs("_pp")]
        [Tooltip("top-left-up corner of first character")]
        public Vector3 voxiePos = new Vector3(0f, 0f, 0.0f);
        [FormerlySerializedAs("_color")]
        int color = 0xffffff;
        int orgColor = 0xffffff;

        public float fontSize = 1;
        [Tooltip("Set starting horizontal angle in degrees")]
        public float horizontalAngle = 0;
        [Tooltip("Set starting  vertical angle in degrees")]
        public float verticalAngle = 0;
        [Tooltip("Set a starting tilt in degrees ")]
        public float tilt = 0;


        [Tooltip("Red color value 0 - 255")]
        public int rValue = 255;
        [Tooltip("Green color value 0 - 255")]
        public int gValue = 0;
        [Tooltip("Blue color value 0 - 255")]
        public int bValue = 255;


        [Tooltip("The Text's height default is 0.2f")]
        public float textHeight = 0.2f;

        [Tooltip("The Text's width default is 0.1f")]
        public float textWidth = 0.1f;

        [Tooltip("Set a value to rotate the text horizontal 0 = off")]
        public float rotateHorizontal = 0f;

        [Tooltip("Set a value to rotate the text vertical 0 = off")]
        public float rotateVertical = 0f;

        [Tooltip("Set a value to rotate the twist 0 = off")]
        public float rotateTilt = 0f;


        [Tooltip("Fade out text after the fade out time. (requires Update Per Frame)")]
        public bool fadeText = false;
        [Tooltip("Fade out text after X number of seconds")]
        public double fadeOutTime = 3;


        private static readonly System.Text.Encoding Enc = System.Text.Encoding.ASCII;

        public string text = "";

        public bool updatePerFrame = false;
        public bool relativePos = false;

        double setStringTime = 0;

        private byte[] _ts;
        // Use this for initialization
        public void Start()
        {

            
            SetString(text);
            UpdateLocation();
            UpdateTransforms();
            VXProcess.Drawables.Add(this);
            orgColor = color;
        }

        public void SetString(string newString)
        {
            text = newString;
            // Get Char Values for String
            byte[] tmp = Enc.GetBytes(newString);
            _ts = new byte[tmp.Length + 1];
            tmp.CopyTo(_ts, 0);
            // Append 0 to end string
            _ts[tmp.Length] = 0;
            setStringTime = Time.timeAsDouble + fadeOutTime;
            color = orgColor;
        }
        // tweens a colour to a destination colour... good for fade outs or tweens 
        public int tweenCol(int colour, int speed, int destColour)
        {

            int b, g, r;
            int bd, gd, rd;

            b = (colour & 0xFF);
            g = (colour >> 8) & 0xFF;
            r = (colour >> 16) & 0xFF;
            bd = (destColour & 0xFF);
            gd = (destColour >> 8) & 0xFF;
            rd = (destColour >> 16) & 0xFF;

            if (b > bd) b -= speed;
            else if (b < bd) b += speed;
            if (r > rd) r -= speed;
            else if (r < rd) r += speed;
            if (g > gd) g -= speed;
            else if (g < gd) g += speed;

            if (r < 0x00) r = 0x00;
            if (r > 0xFF) r = 0xFF;
            if (g < 0x00) g = 0x00;
            if (g > 0xFF) g = 0xFF;
            if (b < 0x00) b = 0x00;
            if (b > 0xFF) b = 0xFF;

            return (r << 16) | (g << 8) | (b);

        }


        void UpdateString()
        {
            // Get Char Values for String
            byte[] tmp = Enc.GetBytes(text);
            _ts = new byte[tmp.Length + 1];
            tmp.CopyTo(_ts, 0);
            // Append 0 to end string
            _ts[tmp.Length] = 0;
        }

        public void SetColor(int rValue, int gValue, int bValue)
        {
            color = (rValue << 16) | (gValue << 8) | (bValue);
            orgColor = color;
        }


        public void UpdateLocation()
        {

            if (VXProcess.Instance.Camera)
            {
                Matrix4x4 matrix = VXProcess.Instance.Camera.GetMatrix() * transform.localToWorldMatrix;
                //Matrix4x4 matrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance.Camera.transform.worldToLocalMatrix * transform.localToWorldMatrix;

                Vector3 pos = matrix * transform.position;

                _pp = pos.ToPoint3d();
            }
        }



        /// <summary>  
        ///  Draw the drawable mesh; Uses Capture Volume's transform to determine if play space has changed
        ///  Animated meshes are set to redraw every frame while statics only redrawn on them or the volume
        ///  changing transform.
        ///  </summary>
        public void Draw()
        {
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;

            if (!gameObject.activeInHierarchy || CompareTag("VoxieHide"))
            {
                Debug.Log($"{gameObject.name}: Skipping");
                return;
            }

            if (updatePerFrame)
            {

                horizontalAngle += (rotateHorizontal * Time.deltaTime);
                verticalAngle += (rotateVertical * Time.deltaTime);
                tilt += (rotateTilt * Time.deltaTime);

                if (Math.Abs(horizontalAngle) > 360) horizontalAngle %= 360;
                if (Math.Abs(verticalAngle) > 360) verticalAngle %= 360;
                if (Math.Abs(tilt) > 360) tilt %= 360;

     
                UpdateString();

                UpdateTransforms();

                if (fadeText && fadeOutTime < Time.timeAsDouble && color != 0x000000) 
                {
                    color = tweenCol(color, 5, 0x000000);

                } else if  (fadeText == false)
                {
                    color = (rValue << 16) | (gValue << 8) | (bValue);
                }

            } else
            {
                color = (rValue << 16) | (gValue << 8) | (bValue);
            }

      

            VXProcess.Runtime.DrawLetters(ref _pp, ref _pr, ref _pd, color, _ts);
        }

        public void UpdateTransforms()
        {

            double f = 0, ch = 0, sh = 0, cv = 0, sv = 0;

            f = horizontalAngle * (Math.PI / 180f); 
            ch = Math.Cos(f); sh = Math.Sin(f);
            f = verticalAngle * (Math.PI / 180f); 
            cv = Math.Cos(f); sv = Math.Sin(f);
            f = tilt * (Math.PI / 180f);


            _pr.x = (float)ch;
            _pr.y = (float)sh;
            _pr.z = 0f;

            _pd.x = (float)-sh * (float)cv;
            _pd.y = (float)ch * (float)cv;
            _pd.z = (float)-sv;

            float g, c, s;

            c = (float)Math.Cos(f);
            s = (float)Math.Sin(f);
            g = _pr.x;
            _pr.x = g * c + _pd.x * s;
            _pd.x = _pd.x * c - g * s;
            g = _pr.y;
            _pr.y = g * c + _pd.y * s;
            _pd.y = _pd.y * c - g * s;
            g = _pr.z;
            _pr.z = g * c + _pd.z * s;
            _pd.z = _pd.z * c - g * s;

            f = (fontSize * textWidth) * .5f; _pr.x *= (float)f; _pr.y *= (float)f; _pr.z *= (float)f;
            f = (fontSize * textHeight) * .5f; _pd.x *= (float)f; _pd.y *= (float)f; _pd.z *= (float)f;
     
            if (relativePos)
            {
                _pp.x = voxiePos.x + this.gameObject.transform.position.x;
                _pp.y = voxiePos.y + this.gameObject.transform.position.y;  // -z
                _pp.z = voxiePos.z + this.gameObject.transform.position.z; // -y
            }
            else
            {
                _pp.x = voxiePos.x;
                _pp.y = voxiePos.y;
                _pp.z = voxiePos.z;
            }

        }



        private void OnDestroy()
        {
            try
            {
                _ts = new byte[1];
                _ts[0] = 0;

                // Remove ourselves from Draw cycle
                VXProcess.Drawables.Remove(this);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Error while Destroying {gameObject.name}   {e}");
#else
				
					ExceptionHandler.Except($"Error while Destroying {gameObject.name}", e);
				
#endif
            }
        }
    }
}

