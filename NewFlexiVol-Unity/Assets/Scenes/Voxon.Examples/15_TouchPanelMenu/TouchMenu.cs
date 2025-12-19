using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = UnityEngine.Random;

 
/* Voxon x Unity - Menu Intergration example. By Matthew Vecchio for Voxon 28th of April 
 * 
 * Example scene / script to make your own menu tab on the VX touch screen. 
 * 
 * 
 * 
 */ 


namespace Voxon.Examples._15_TouchPanelMenu
{
    /// <summary>
    /// Example class on how to generate a touchmenu for the Voxon Runtime
    /// 
    /// </summary>
    public class TouchMenu : MonoBehaviour
    {
        /// <summary>
        /// Has the menu been initialised
        /// </summary>
        private bool _initialised = false;
        static public GameObject MenuListener; 
        static public float MovementSpeed = 20;
        static TouchMenuListener touchMenuListener;
        static VoxieText voxieText; 


        // for File picker
        //   static strinmg pickedFilePath
        static string unityWorkingDirectory = Directory.GetCurrentDirectory();
        static string pickedFilePath = "";
        static string textToWrite = "";
  
        /* Menu Values */
        /// <summary>
        /// As each menu item needs to have an unique id (which consists of a intger / whole number)
        /// it can be easier to store them as an ENUM...
        /// 
        /// </summary>
        public enum MENU_NAMES
        {
            //Generic names
            MOVE_MODEL_FORWARD, MOVE_MODEL_LEFT, MOVE_MODEL_RIGHT, MOVE_MODEL_BACKWARD,  FLIP_MODEL,
            SELECT_MODEL_1, SELECT_MODEL_2, SELECT_MODEL_3, SELECT_MODEL_4, ADJUST_MODEL_HEIGHT_POSITION, ADJUST_MODEL_SCALE,
            MENU_PICKFILE_NAME, MENU_TEXTBOX, MENU_TEXT,
            EDIT_STR, EDIT_STR_DO, SHOW_TEXT,

        };

        /// <summary>
        /// Run on Update.
        /// Includes an initialiser for Menu generation as VXProcess needs to have an 
        /// active instance before a menu can be generated.
        /// Included are examples on generating 1 or more buttons, button positioning,
        /// sliders (vertical and horizontal), text input and adding text to menus.
        /// </summary>
        void Update()
        {
            if (!_initialised && VXProcess.Instance.active)
            {
                _initialised = true;
  
                //  OPTIONAL : binding to another
                //  bind the touchMenuListener so we can use its functions... OR you can do this within this script
                MenuListener = GameObject.FindGameObjectWithTag("GameController");
                touchMenuListener = MenuListener.GetComponent<TouchMenuListener>();

                // So Menu File Picker works by changing the current working directory which Unity does not like so after we run the File Picker we need to change it back
                // here is where we get the current working directory
                unityWorkingDirectory = Directory.GetCurrentDirectory();

                // Voxie text game script writes directly the Voxon Screen.
                voxieText = GetComponent<VoxieText>();


                // if you want to add your own menu. You need to run the MenuReset function and pass in a custom function pointer that will manage your content.
                VXProcess.Runtime.MenuReset(Custom_Menu_Update, IntPtr.Zero); // resets the menu tab and sends through the 'Custom_Menu_Update'
                // now on update the Voxiebox.dll will run the Custom_Menu_Update function to monitor the input. 



                // add a new tab
                // function to create a new menu tab on the touch screen. This one will be called 'Unity App' 
                // its X position will be 350 pixels and Y position will be 0. It will be 600 pixels wide and 500 pixels high. 
                VXProcess.Runtime.MenuAddTab("Unity App", 350, 0, 600, 500);  

                // adding text -- can share ID with other menu elements that don't need to be updated. (text can be updated via the handle and updateItem function)
                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXT, "Unity Custom Menu", 200, 20, 0, 0, 0); // adding menu text


                // if you aren't updating the text on a menu no need to give it a unique ID...
                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXT, "Move Model\nSingle Use Buttons", 10, 60, 0, 0, 0); // adding menu text

                // define a single button (MENU_BUTTON+3) - ensure that the last param is set to MENUPMENU_BUTTON_TYPEOSITION.SINGLE
                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.MOVE_MODEL_FORWARD, "^", 110, 110, 75, 75, 0x808000, (int)MENU_BUTTON_TYPE.SINGLE);

                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.MOVE_MODEL_LEFT, "<", 20, 185, 75, 75, 0x808000, (int)MENU_BUTTON_TYPE.SINGLE);

                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.MOVE_MODEL_RIGHT, ">", 200, 185, 75, 75, 0x808000, (int)MENU_BUTTON_TYPE.SINGLE);

                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.MOVE_MODEL_BACKWARD, "V", 110, 260, 75, 75, 0x808000, (int)MENU_BUTTON_TYPE.SINGLE);


                // Adding Text Example
                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXT, "Select Model\n(Linked Buttons)", 320, 60, 0, 0, 0); // adding menu text
               
                // if you want to link a bunch buttons together - so that one button stays down and not have multiple instances
                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.SELECT_MODEL_1, "1", 300, 125, 75, 75, 0x800080, (int)MENU_BUTTON_TYPE.FIRST);  // first button to link together

                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.SELECT_MODEL_2, "2", 400, 125, 75, 75, 0x800080, (int)MENU_BUTTON_TYPE.MIDDLE); // middle buttons can be multiple of this

                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.SELECT_MODEL_3, "3", 500, 125, 75, 75, 0x800080, (int)MENU_BUTTON_TYPE.END);    // end button to end the linked buttons

                // How to update a button's state

                // As the first button in the selected models should be pressed as its selected by default we update that button instance with it being pressed
                // passing in the third param 'down' as a 1 to make the button be pressed. 0 is off. Use the ID as the handle, You can update the text, the state and the current value
                VXProcess.Runtime.MenuUpdateItem((int)MENU_NAMES.SELECT_MODEL_1, null, 1, 0);


                // Adding Text Example
                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXT, "Flip Model", 275, 280, 0, 0, 0); // adding menu text

                // A Toggle button use '\r' to separate values each entry has its only (v) value which gets passed to the menu_update function
                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.FLIP_MODEL, "0'\r90'\r180'\r270'", 275, 300, 120, 75, 0x804040, (int)MENU_BUTTON_TYPE.TOGGLE);

                // Horizontal slider - returns a double value (v) to the user function
                // sider value settings are the last four parameters : starting value, lowest value, highest value, minimal adjustment, major adjustment
                VXProcess.Runtime.MenuAddHorizontalSlider((int)MENU_NAMES.ADJUST_MODEL_SCALE, "Adjust Scale", 85, 400, 240, 64, 0xFFFF80, 1f, 0.5f, 5f, 0.1f, 0.3f);

                // Vertical slider - returns a double value (v) to the user function
                // sider value settings are the last four parameters : starting value, lowest value, highest value, minimal adjustment, major adjustment
                VXProcess.Runtime.MenuAddVerticleSlider((int)MENU_NAMES.ADJUST_MODEL_HEIGHT_POSITION , "Adjust Height", 425, 275, 64, 150, 0xFFFF80, 0.0f, 1, -2.0f, 0.1f, 0.3f);


                // Add a second tab...
                // its X position will be 350 pixels and Y position will be 20. It will be 600 pixels wide and 500 pixels high. 
                VXProcess.Runtime.MenuAddTab("Xtra", 350,  0, 600, 500);


                // File Picker button works by changing the current working directory. The string it returns will be just the file name. 
                // use the '\r' value to apply a filetype filter. In this example we are showing only .txt files. 
                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.MENU_PICKFILE_NAME, "Load in .txt\r*.txt", 20, 10, 560, 64, 0x908070, (int)MENU_BUTTON_TYPE.FILE_PICKER);

                // Adding some text with a unique ID so we can update the text later with a string we pass to it. 
                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXTBOX, "Load in text from a .txt file.\nThe first three lines of the file.\nWill appear here.", 10, 80, 0, 0, 0); // adding menu text


                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXT, "Edit Menu without follow up button.\n(press the 'GO' button to update)", 50, 240, 0, 0, 0); // adding menu text

                // Edit is like Edit Do does the next action when you press enter in this case will hit the Go button
                VXProcess.Runtime.MenuAddEdit((int)MENU_NAMES.EDIT_STR, "Write on to the VX1 display", 50, 290, 400, 50, 0x808080, false);


                VXProcess.Runtime.MenuAddText((int)MENU_NAMES.MENU_TEXT, "Edit Menu with follow up button.\n(press enter or 'GO' to update)", 50, 360, 0, 0, 0); // adding menu text

                // Edit is like Edit Do but its callback is associated with the button next do it when 'Enter' is pressed  
                VXProcess.Runtime.MenuAddEdit((int)MENU_NAMES.EDIT_STR_DO, "See this Text on the VX1", 50, 410, 400, 50, 0x808080, true);

                // A button which is linked to Edit Do
                VXProcess.Runtime.MenuAddButton((int)MENU_NAMES.SHOW_TEXT, "GO", 460, 410, 90, 50, 0x08FF80, (int)MENU_BUTTON_TYPE.SINGLE);

            } 


        }

        /// <summary>  
        /// Switches action based on key enumeration
        /// </summary>
        ///  Everytime the custom menu is interacted with this function is called. 
        ///  This call back function allows you to parse and data and make changes to your  program
        /// 
        /// <param name="id">ID of action. Used those in <see cref="MENU_NAMES"/>.</param>
        /// <param name="st">String value of action</param>
        /// <param name="v">double value</param>
        /// <param name="how">integer value</param>
        /// <param name="userdata">User Data as defined in menu reset</param>
        /// <returns></returns>
        static int Custom_Menu_Update(int id, string st, double v, int how, IntPtr userdata)
        {
           

            switch (id) // id is the button's id so our ENUM names help to understand what is going on....
            {
                case (int)MENU_NAMES.MOVE_MODEL_FORWARD:
                    touchMenuListener.MoveModel(0, 0, 1, MovementSpeed);
                    break;
                case (int)MENU_NAMES.MOVE_MODEL_BACKWARD:
                    touchMenuListener.MoveModel(0, 0, -1, MovementSpeed);
                    break;
                case (int)MENU_NAMES.MOVE_MODEL_LEFT:
                    touchMenuListener.MoveModel(-1, 0, 0, MovementSpeed);
                    break;
                case (int)MENU_NAMES.MOVE_MODEL_RIGHT:
                    touchMenuListener.MoveModel(1, 0, 0, MovementSpeed);
                    break;


                case (int)MENU_NAMES.SELECT_MODEL_1:
                    touchMenuListener.SelectModelByIndex(0);
                    break;
                case (int)MENU_NAMES.SELECT_MODEL_2:
                    touchMenuListener.SelectModelByIndex(1);
                    break;
                case (int)MENU_NAMES.SELECT_MODEL_3:
                    touchMenuListener.SelectModelByIndex(2);
                    break;

                case (int)MENU_NAMES.FLIP_MODEL: // toggle button
                    switch((int)v) // v value changes depending on toggle button position use a switch to differ { 0 - 1st, 90' = 2nd, 180' = 3rd, 270' = 4th)  
                    {
                        case 0:
                            touchMenuListener.RotateModel(90);
                            break;
                        case 1:
                            touchMenuListener.RotateModel(90);
                            break;
                        case 2:
                            touchMenuListener.RotateModel(90);
                            break;
                        case 3:
                            touchMenuListener.RotateModel(90);
                            break;
                    }
                    break;

                case (int)MENU_NAMES.ADJUST_MODEL_SCALE:
                    touchMenuListener.SetScale((float)v);
                    break;
                case (int)MENU_NAMES.ADJUST_MODEL_HEIGHT_POSITION:
                    touchMenuListener.SetModelHeight((float)v);
                    break;


                case (int)MENU_NAMES.MENU_PICKFILE_NAME:
                   
                    // st = the returned value from the menu function... will only be "FILENAME.X" not the full path.
                    // Voxon's File picker changes the current working directory and returns only the file name. So to make a full path w 
                    // combine the the filename and the current working directory.
                    pickedFilePath = Path.Combine(Directory.GetCurrentDirectory(), st);
                    Directory.SetCurrentDirectory(unityWorkingDirectory); // !Important - set the current working directory back as Unity will crash otherwise.
                    VXProcess.add_log_line("File Picker Path: " + st);
                    ReadInFile();
                    break;

                case (int)MENU_NAMES.EDIT_STR:
                    textToWrite = st;
                    break;
                case (int)MENU_NAMES.EDIT_STR_DO:
                    textToWrite = st;
                    break;
                case (int)MENU_NAMES.SHOW_TEXT:           
                    voxieText.SetString(textToWrite);
                    break;


            }
            return (1);
        }

        static void ReadInFile()
        {
            String line;
            int lineCount = 0;

            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(pickedFilePath);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file

                // just read in the first 3 lines
                while (line != null && lineCount < 3)
                {
                    //Read the next line
                    line += sr.ReadLine();
                    line += "\n";
                    lineCount++;
                }
             
                //close the file
                sr.Close();
              
            }
            catch (Exception e)
            {
                VXProcess.add_log_line("Exception: " + e.Message);
              
                return;
            }
          
            VXProcess.Runtime.MenuUpdateItem((int)MENU_NAMES.MENU_TEXTBOX, line, 0, 0);
        }
      
    }
}