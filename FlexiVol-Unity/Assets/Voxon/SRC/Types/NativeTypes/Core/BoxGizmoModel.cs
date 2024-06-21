using UnityEngine;

namespace Voxon
{
    public class BoxGizmoModel
    {
        public BoxModel Box;
        public Vector3[] Corners = new Vector3[6];

        public Vector3 Size;
        public Vector3 Center;

        #region constructor
        public BoxGizmoModel(BoxModel box)
        {
            Box = box;
            Update();
        }
        #endregion

        #region data_manipulation
        public void Update()
        {
            if (Box.Parent)
            {
                UpdateParent();
            }
            else
            {
                UpdateNoParent();
            }
        }

        private void UpdateParent()
        {
            Vector3 position = Box.Parent.transform.position;
            Vector3 tmp0 = Box.Position[0] + position;
            Vector3 tmp1 = Box.Parent.transform.rotation * (Box.Position[1] - Box.Position[0]);

            tmp1 = (tmp1 + Box.Position[0] + position);
            Vector3 localScale = Box.Parent.transform.localScale;
            tmp1.x *= localScale.x;
            tmp1.y *= localScale.y;
            tmp1.z *= localScale.z;

            Corners[0] = new Vector3(tmp0.x, tmp0.y, tmp1.z);
            Corners[1] = new Vector3(tmp0.x, tmp1.y, tmp0.z);
            Corners[2] = new Vector3(tmp1.x, tmp0.y, tmp0.z);
            Corners[3] = new Vector3(tmp1.x, tmp1.y, tmp0.z);
            Corners[4] = new Vector3(tmp1.x, tmp0.y, tmp1.z);
            Corners[5] = new Vector3(tmp0.x, tmp1.y, tmp1.z);

            Size = new Vector3(Mathf.Abs(Box.Position[1].x - Box.Position[0].x),
                                Mathf.Abs(Box.Position[1].y - Box.Position[0].y),
                                Mathf.Abs(Box.Position[1].z - Box.Position[0].z));

            Center = (tmp0 + tmp1) * 0.5f;
        }

        private void UpdateNoParent()
        {
            Corners[0] = new Vector3(Box.Position[0].x, Box.Position[0].y, Box.Position[0].z);
            Corners[1] = new Vector3(Box.Position[1].x, Box.Position[1].y, Box.Position[1].z);
            // corners[2] = new Vector3(box.Position[1].x, box.Position[0].y, box.Position[1].z);
            // corners[3] = new Vector3(box.Position[1].x, box.Position[1].y, box.Position[0].z);
            // corners[4] = new Vector3(box.Position[1].x, box.Position[0].y, box.Position[1].z);
            // corners[5] = new Vector3(box.Position[0].x, box.Position[1].y, box.Position[1].z);

            Size = new Vector3(Mathf.Abs(Box.Position[1].x - Box.Position[0].x),
                                            Mathf.Abs(Box.Position[1].y - Box.Position[0].y),
                                            Mathf.Abs(Box.Position[1].z - Box.Position[0].z));

            Center = (Box.Position[0] + Box.Position[1]) * 0.5f;
        }
        #endregion
    }
}