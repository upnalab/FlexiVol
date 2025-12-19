using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    public class VXTextComponent : MonoBehaviour, IDrawable
    {
        point3d _pr, _pd, _pp;

        [FormerlySerializedAs("_pr")] [Tooltip("right vector - length is size of single character")]
        public Vector3 pr = new Vector3(0.1f, 0.0f, 0.0f);
        [FormerlySerializedAs("_pd")] [Tooltip("down vector - length is height of character")]
        public Vector3 pd = new Vector3(0.0f, 0.5f, 0.0f);
        [FormerlySerializedAs("_pp")] [Tooltip("top-left-up corner of first character")]
        public Vector3 pp = new Vector3(-0.75f, 0.5f, 0.0f);
        [FormerlySerializedAs("_color")] [Tooltip("text colour")]
        public int color = 0xffffff;


        private static readonly System.Text.Encoding Enc = System.Text.Encoding.ASCII;

        public string text = "";

		public bool forceUpdatePerFrame = false;

		private byte[] _ts;
        // Use this for initialization
        public void Start()
        {
            _pr.x = pr.x;
            _pr.y = pr.y;
            _pr.z = pr.z;

            _pd.x = pd.x;
            _pd.y = pd.y;
            _pd.z = pd.z;

            _pp.x = pp.x;
            _pp.y = pp.y;
            _pp.z = pp.z;

            SetString(text);
            UpdateLocation();

            VXProcess.Drawables.Add(this);
        }

        public void SetString(string newString)
        {
            text = newString;
            // Get Char Values for String
            byte[] tmp = Enc.GetBytes(newString);
            _ts = new byte[tmp.Length+1];
            tmp.CopyTo(_ts, 0);
            // Append 0 to end string
            _ts[tmp.Length] = 0;
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

        public void SetCharWidth(point3d width)
        {
            _pd = width;
        }

        public void SetCharWidth(Vector3 width)
        {
            SetCharWidth(width.ToPoint3d());
        }

        public void SetCharHeight(point3d height)
        {
            _pr = height;
        }

        public void SetCharHeight(Vector3 height)
        {
            SetCharHeight(height.ToPoint3d());
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

			if (forceUpdatePerFrame)
			{
				UpdateString();
			}

			VXProcess.Runtime.DrawLetters(ref _pp, ref _pr, ref _pd, color, _ts);
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