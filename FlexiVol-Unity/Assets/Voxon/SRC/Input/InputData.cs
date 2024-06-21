using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    /// <summary>  
    ///  Used to maintain keybindings, as well as handle saved and loaded bindings
    ///  </summary>
    [Serializable]
    public class InputData
    {
        [FormerlySerializedAs("Keyboard")] [SerializeField]
        private KeyBindings keyboard;
        [FormerlySerializedAs("Mouse")] [SerializeField]
        private MouseBindings mouse;
        [SerializeField]
        private SpaceNavBindings spaceNav;
        [FormerlySerializedAs("J1Buttons")] [SerializeField]
        private ButtonBindings j1Buttons;
        [FormerlySerializedAs("J1Axis")] [SerializeField]
        private AxisBindings j1Axis;
        [FormerlySerializedAs("J2Buttons")] [SerializeField]
        private ButtonBindings j2Buttons;
        [FormerlySerializedAs("J2Axis")] [SerializeField]
        private AxisBindings j2Axis;
        [FormerlySerializedAs("J3Buttons")] [SerializeField]
        private ButtonBindings j3Buttons;
        [FormerlySerializedAs("J3Axis")] [SerializeField]
        private AxisBindings j3Axis;
        [FormerlySerializedAs("J4Buttons")] [SerializeField]
        private ButtonBindings j4Buttons;
        [FormerlySerializedAs("J4Axis")] [SerializeField]
        private AxisBindings j4Axis;

        // Use this for initialization
        public InputData()
        {
            keyboard = new KeyBindings();
            mouse = new MouseBindings();
            spaceNav = new SpaceNavBindings();
            j1Buttons = new ButtonBindings();
            j1Axis = new AxisBindings();
            j2Buttons = new ButtonBindings();
            j2Axis = new AxisBindings();
            j3Buttons = new ButtonBindings();
            j3Axis = new AxisBindings();
            j4Buttons = new ButtonBindings();
            j4Axis = new AxisBindings();
        }

        public void From_IC()
        {

            KeyBindCopy(ref InputController.Instance.keyboard, ref keyboard);
            MouseBindCopy(InputController.Instance.mouse, mouse);
            SpaceNavBindCopy(InputController.Instance.spacenav, spaceNav);

            ButBindCopy(InputController.Instance.j1Buttons, j1Buttons);
            ButBindCopy(InputController.Instance.j2Buttons, j2Buttons);
            ButBindCopy(InputController.Instance.j3Buttons, j3Buttons);
            ButBindCopy(InputController.Instance.j4Buttons, j4Buttons);

            AxisBindCopy(InputController.Instance.j1Axis, j1Axis);
            AxisBindCopy(InputController.Instance.j2Axis, j2Axis);
            AxisBindCopy(InputController.Instance.j3Axis, j3Axis);
            AxisBindCopy(InputController.Instance.j4Axis, j4Axis);
        }

        public void To_IC()
        {
            KeyBindCopy(ref keyboard, ref InputController.Instance.keyboard);
            MouseBindCopy(mouse, InputController.Instance.mouse);
            SpaceNavBindCopy(spaceNav, InputController.Instance.spacenav);
            
            ButBindCopy(j1Buttons, InputController.Instance.j1Buttons);
            ButBindCopy(j2Buttons, InputController.Instance.j2Buttons);
            ButBindCopy(j3Buttons, InputController.Instance.j3Buttons);
            ButBindCopy(j4Buttons, InputController.Instance.j4Buttons);

            AxisBindCopy(j1Axis, InputController.Instance.j1Axis);
            AxisBindCopy(j2Axis, InputController.Instance.j2Axis);
            AxisBindCopy(j3Axis, InputController.Instance.j3Axis);
            AxisBindCopy(j4Axis, InputController.Instance.j4Axis);
        }

        private void KeyBindCopy(ref KeyBindings from, ref KeyBindings to)
        {
            to.Clear();
        
            foreach (string key in from.Keys)
            {
                to.Add(key, from[key]);
            }
        }

        private void MouseBindCopy(MouseBindings from, MouseBindings to)
        {
            to.Clear();

            foreach (string key in from.Keys)
            {
                to.Add(key, from[key]);
            }
        }
        
        private void SpaceNavBindCopy(SpaceNavBindings from, SpaceNavBindings to)
        {
            to.Clear();

            foreach (string key in from.Keys)
            {
                to.Add(key, from[key]);
            }
        }

        private void ButBindCopy(ButtonBindings from, ButtonBindings to)
        {
            to.Clear();

            foreach (string key in from.Keys)
            {
                to.Add(key, from[key]);
            }
        }

        private void AxisBindCopy(AxisBindings from, AxisBindings to)
        {
            to.Clear();
        
            foreach (string key in from.Keys)
            {
                to.Add(key, from[key]);
            }
        }
    }
}