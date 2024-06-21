using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Voxon.Examples._15_TouchPanelMenu
{
    public class TouchMenuListener : MonoBehaviour
    {
        Transform  currentModel;
      
        int currentChildIndex = 0;
        int maxIndex = 0;
     


        // Start is called before the first frame update
        void Start()
        {
            currentChildIndex = 0;
            maxIndex = transform.childCount;
            currentModel = transform.GetChild(currentChildIndex);
            HideModels();
        }

        // Update is called once per frame
        void Update()
        {
          



        }

        public void SelectNextModel()
        {
            currentChildIndex++;
            if (currentChildIndex >= maxIndex) currentChildIndex = 0;
            currentModel = transform.GetChild(currentChildIndex);
            HideModels();
        }

        public void SelectPrevModel()
        {
            currentChildIndex--;
            if (currentChildIndex < 0) currentChildIndex = maxIndex - 1;
            currentModel = transform.GetChild(currentChildIndex);
            HideModels();
        }

        public void SelectModelByIndex(int index)
        {
            if (index < 0 || index >= maxIndex) index = 0;
            currentChildIndex = index;
            currentModel = transform.GetChild(currentChildIndex);
            HideModels();
        }


        #region Transform Model Functions
        public void MoveModel(float XAmount, float YAmount, float ZAmount, float movementSpeed)
        {
            transform.position = transform.position + new Vector3(XAmount * movementSpeed * Time.deltaTime, YAmount * movementSpeed * Time.deltaTime, ZAmount * movementSpeed * Time.deltaTime);
        }

        public void SetModelHeight( float Height)
        {
            transform.position = new Vector3(transform.position.x, Height, transform.position.z);
        }
   
        public void SetScale(float scaleAmount)
        {
            transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
       
        }

        public void RotateModel(int Degrees)
        {
            transform.Rotate(0, Degrees, 0, Space.Self);

        }
        #endregion


        // This function hides and shows the selected model. tagging a Mesh as 'VoxieHide' will hide it from rending on the Voxon display.
        // Here we parse through the child (and sub child) transform and to correctly tag them.
        void HideModels()
        {
            Transform childModel;
            Transform subChildModel;
            int i = 0;
            int j = 0;
            for  ( i = 0; i < maxIndex; i++)
            {
                childModel = transform.GetChild(i);

                if (i == currentChildIndex)
                {
                    currentModel.tag = "Untagged";

                    for (j = 0; j < childModel.childCount; j++) { 
                        subChildModel = childModel.GetChild(j);
                        subChildModel.tag = "Untagged";
                    }

                }
                else
                {
                    childModel.tag = "VoxieHide";

                    for (j = 0; j < childModel.childCount; j++)
                    {
                        subChildModel = childModel.GetChild(j);
                        subChildModel.tag = "VoxieHide";
                    }
                }  
            }
        }

    


    }
}
