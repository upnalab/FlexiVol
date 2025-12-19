using UnityEngine;
using Voxon;

/**
 *  Voxie Cam Tuner
 *  used to move and adjust camera around to help tune it to a screen
 *  by Matthew Vecchio for VOXON
 *  
 *  Changelog
 *  
 *  last updated 18/12/2018
 *  + Added JSON file support
 *  + Added speed up and down of controls
 * 
 * */
public class VoxieCamTuner : MonoBehaviour {

    [Tooltip("The settings for the JSON file you can set a custom file name")]
    public VoxieCamTunerSettings settings = new VoxieCamTunerSettings();

    Vector3 originalPos, originalScale;
    Quaternion originalRot;
    [Tooltip("The speed of camera movement '8' is default")]
    public float speed = 8;
    float currentSpeed;
    public bool loadSettingsFromFile = true, saveOnChange = true;

    const float SPACENAV_SPEED_FACTOR = 0.0028571428571429f;
    const float SPACENAV_ROTATION_FACTOR = 0.028571428571429f;
        
    /*
      
      

Voxie Cam Tuner V 1.0 19/12/2018 
"Voxie Canned Tuna"
by Matthew Vecchio for Voxon
        
An attachable script for the Voxon camera which allows the user to be able to move and stretch the camera while the software is running on a VX1.
Can save and load the settings to a JSON file

public variables (editable within the Unity Editor)

Speed - the speed of the camera movements default is 8
Filename (look under settings) - the filename for the JSON file to save and load
Load Settings From File - enable this to load the settings from the scene starts 

Default key bindings (Make sure you map these in the VOXON input manager)

String Name         Default Key         Function

Cam_Reset_Pos       B               :   Reset the camera's position to last loaded file or default
Cam_Reset_Rot       N               :   Reset the camera's rotation to last loaded file or default
Cam_Reset_Scale     M               :   Reset the camera's scale to last loaded file or default
Cam_Foward          Arrow Up        :   Move the camera forward
Cam_Backward        Arrow Down      :   Move the camera backward
Cam_Up              Right Shift     :   Move the camera up
Cam_Down            Right Ctrl      :   move the camera down
Cam_Left            Arrow Left      :   Move the camera left
Cam_Right           Arrow Right     :   Move the camera right
Cam_Scale           Z               :   Modify scale hold down this key while pressing the camera directions to change the rotation.
Cam_Rotation        X               :   Modify rotations hold down this key while pressing the camera directions to change the rotation.
Cam_Print           P               :   Press this button to display the Y,X,Z infomation to the debug menu
Cam_SpeedSlow       Left Ctrl       :   Hold down this key to slow down the camera movement
Cam_SpeedFast       Left ALT        :   Hold down this key to speed up the camera movement
Cam_Save            Left ALT + S    :   Saves the camera settings to a JSON file
Cam_Load            Left ALT + L    :   Loads the camera settings from JSON file
Cam_Control         Left ALT        :   Hold down this button to load and save to file
     
         
         */

    // Use this for initialization
    void Start () {
        if (loadSettingsFromFile) loadSettings();

        originalPos = transform.position;
        originalRot = transform.rotation;
        originalScale = transform.localScale;
        currentSpeed = speed;
    }
	
	// Update is called once per frame
	void Update () {

        // save settings
        if (Voxon.Input.GetKey("Cam_Control") && Voxon.Input.GetKeyDown("Cam_Save"))
        {
            saveSettings();
            Debug.Log("Settings Saved");
           


        }
        // load settings 
        if (Voxon.Input.GetKey("Cam_Control") && Voxon.Input.GetKeyDown("Cam_Load"))
        {

            loadSettings();
            Debug.Log("Settings Loaded");
         


        }


        // speed settings

        if (Voxon.Input.GetKey("Cam_SpeedFast") )
        {
            // Debug.Log("Cam Fast ON");
            currentSpeed = speed * 3;
       
        }
        else if (Voxon.Input.GetKey("Cam_SpeedSlow"))
          {
            Debug.Log("Cam Slow ON");
            currentSpeed = speed / 3;
        }
        else if (currentSpeed != speed)  
        {
             Debug.Log("Cam Speed reset");
             currentSpeed = speed;
        }


        if (Voxon.Input.GetKey("Cam_Print"))
        {
            camPrint();
        }

        
        if (Voxon.Input.GetKey("Cam_Scale"))
        {

            // scale

            if (Voxon.Input.GetKey("Cam_Left"))
            {
                transform.localScale = new Vector3(transform.localScale.x + 0.2f, transform.localScale.y, transform.localScale.z);

            }
            if (Voxon.Input.GetKey("Cam_Right"))
            {
                transform.localScale = new Vector3(transform.localScale.x - 0.2f, transform.localScale.y, transform.localScale.z);

            }
            if (Voxon.Input.GetKey("Cam_Up"))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 0.2f, transform.localScale.z);

            }
            if (Voxon.Input.GetKey("Cam_Down"))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - 0.2f, transform.localScale.z);
            }
            if (Voxon.Input.GetKey("Cam_Forward"))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + 0.2f);

            }
            if (Voxon.Input.GetKey("Cam_Backward"))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - 0.2f);

            }
        }
        
        if (Voxon.Input.GetKey("Cam_Rotation"))
        {
            // rotation

            if (Voxon.Input.GetKey("Cam_Left"))
            {
                transform.Rotate(Vector3.forward * (currentSpeed * 2) * Time.deltaTime);

            }
            if (Voxon.Input.GetKey("Cam_Right"))
            {
                transform.Rotate(Vector3.back * (currentSpeed * 2) * Time.deltaTime);

            }
            if (Voxon.Input.GetKey("Cam_Forward"))
            {
                transform.Rotate(Vector3.left * (currentSpeed * 2) * Time.deltaTime);

            }
            if (Voxon.Input.GetKey("Cam_Backward"))
            {
                transform.Rotate(Vector3.right * (currentSpeed * 2) * Time.deltaTime);

            }
            if (Voxon.Input.GetKey("Cam_Up"))
            {
                transform.Rotate(Vector3.up * (currentSpeed * 2) * Time.deltaTime);

            }
            if (Voxon.Input.GetKey("Cam_Down"))
            {
                transform.Rotate(Vector3.down * (currentSpeed * 2) * Time.deltaTime);

            }

        }
        else
        {

            // directions

            if (Voxon.Input.GetKey("Cam_Forward"))
            {
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            }

            if (Voxon.Input.GetKey("Cam_Backward"))
            {
                transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
            }

            if (Voxon.Input.GetKey("Cam_Left"))
            {
                transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

            }

            if (Voxon.Input.GetKey("Cam_Right"))
            {
                transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
            }

            if (Voxon.Input.GetKey("Cam_Up"))
            {
                transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);

            }

            if (Voxon.Input.GetKey("Cam_Down"))
            {
                transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);
            }
        }

        // Space Navigator
        if (Voxon.Input.GetSpaceNavButton("LeftButton"))
        {
            VXProcess.Instance.Camera.transform.localScale *= (1.05f);
        }
        
        if(Voxon.Input.GetSpaceNavButton("RightButton"))
        {
            VXProcess.Instance.Camera.transform.localScale *= (0.95f);
        }
        
        var position = VXProcess.Runtime.GetSpaceNavPosition();
        var rotation = VXProcess.Runtime.GetSpaceNavRotation();
        
        if (rotation != null)
        {
            // Rotation [Roll, Pitch, Yaw]
            // v3rot (pitch, yaw, roll)
            var v3rot = new Vector3(rotation[1]/70,-rotation[2]/70,rotation[0]/70);
            // var v3rot = new Vector3(rotation[1]/70,rotation[0]/70,-rotation[2]/70);
            transform.Rotate(v3rot);    
        }
        
        var v3pos = transform.position;
        if (position != null)
        {
            v3pos.x -= currentSpeed*(position[0]/350.0f);
            v3pos.y += currentSpeed*(position[2]/350.0f);
            v3pos.z += currentSpeed*(position[1]/350.0f);
            VXProcess.Instance.Camera.transform.position = v3pos;
        }
        
        // reset

        if (Voxon.Input.GetKey("Cam_Reset"))
        {
            VXProcess.add_log_line("**** Voxon Camera Position Reset ****");
            transform.SetPositionAndRotation(originalPos, originalRot);
            transform.localScale = originalScale;
        }

        if (Voxon.Input.GetSpaceNavButton("LeftButton") && Voxon.Input.GetSpaceNavButton("RightButton"))
        {
            VXProcess.add_log_line("**** Voxon Camera Position Reset ****");
            VXProcess.Instance.Camera.transform.position = originalPos;
            VXProcess.Instance.Camera.transform.rotation = originalRot;
            VXProcess.Instance.Camera.transform.localScale = originalScale;
        }

    }

    [ExecuteInEditMode]
    private void Awake()
    {
       // loadSettings();
    }

    void OnValidate()
    {
       if(saveOnChange) saveSettings();
    }

    void loadSettings()
    {
        if (settings.filename == "")
        {
            settings.setFilename(transform.root.gameObject.name);
        }
        settings.load();
        transform.SetPositionAndRotation(new Vector3(settings.posX, settings.posY, settings.posZ), new Quaternion(settings.rotX, settings.rotY, settings.rotZ, settings.rotW));
        transform.localScale = new Vector3(settings.scaX, settings.scaY, settings.scaZ);
        originalPos = transform.position;
        originalRot = transform.rotation;
        originalScale = transform.localScale;
        VXProcess.add_log_line("**** Voxon Camera Settings Loaded ****");
    }

    void saveSettings()
    {
        if (settings.filename == "")
        {
            settings.setFilename(transform.root.gameObject.name);
        }

        Vector3 scale = transform.localScale;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        
        settings.posX = pos.x;
        settings.posY = pos.y;
        settings.posZ = pos.z;
        settings.rotX = rot.x;
        settings.rotY = rot.y;
        settings.rotZ = rot.z;
        settings.rotW = rot.w;
        settings.scaX = scale.x;
        settings.scaY = scale.y;
        settings.scaZ = scale.z;


        settings.save();
        VXProcess.add_log_line("**** Voxon Camera Settings Saved ****");
        camPrint();
    }

    void camPrint()
    {
        VXProcess.add_log_line("**** Voxon Camera Settings ****");
        VXProcess.add_log_line("Pos X:" + transform.position.x + " Y: " + transform.position.y + " Z: " + transform.position.z);
        VXProcess.add_log_line("Rot X:" + transform.rotation.x + " Y: " + transform.rotation.y + " Z: " + transform.rotation.z + " W: " + transform.rotation.w);
        VXProcess.add_log_line("Sca X:" + transform.localScale.x + " Y: " + transform.localScale.y + " Z: " + transform.localScale.z);

    }


}
