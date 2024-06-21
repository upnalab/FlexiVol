using UnityEngine;
using UnityEngine.SceneManagement;
using Voxon;

namespace Voxon.Examples._7_LevelLoading
{
    /// <summary>
    /// Display Help Message on TouchScreen
    /// </summary>
    public class HelpMessage : MonoBehaviour
    {
        /// <summary>
        /// Called on Start
        /// Displays help message and currently active scene on TouchScreen
        /// </summary>
        void Start()
        {
            VXProcess.add_log_line("-");
            VXProcess.add_log_line("Ensure 7_LevelLoading_1 and 7_LevelLoading_2 are in 'File\\Build Settings\\Scenes in Build'");
            VXProcess.add_log_line("Current Level:" + SceneManager.GetActiveScene().name);
            VXProcess.add_log_line("Loading next scene...");
        }
    }
}
