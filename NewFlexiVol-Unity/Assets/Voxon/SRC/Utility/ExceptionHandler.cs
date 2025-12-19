using System;
using UnityEngine;

namespace Voxon
{
    public static class ExceptionHandler
    {
        public static void Except(string customMessage, Exception e)
        {
            Debug.LogError(customMessage != "" ? $"{customMessage}\n{e.Message}" : e.Message);
            Windows.Error(customMessage != "" ? $"{customMessage}\n{e.Message}" : e.Message);
           // VXProcess.Runtime.Shutdown();
        }
    }
}