using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.Compilation;

namespace Voxon
{
	class UnityBuildPostProcessor : IPostprocessBuildWithReport
	{
		public int callbackOrder => 0;

		public void OnPostprocessBuild(BuildReport report)
		{
			string fileName = "";
			string outputDirectory = "";

			string sPattern = ".exe";
			string ePattern = "UnityCrashHandler";


			foreach (BuildFile file in report.files)
			{
				// find the first EXE we will exclude the ones that are always included
				if (System.Text.RegularExpressions.Regex.IsMatch(file.path, sPattern))
				{
					// if the EXE is UnityCrashHandler - FORGET IT 
					if (System.Text.RegularExpressions.Regex.IsMatch(file.path, ePattern))
					{
						continue;
					}
					fileName = Path.GetFileName(file.path);
					outputDirectory = Path.GetDirectoryName(file.path);
					break;
				}

				/* Old solution - stopped working with new versions of Unity as 
				* For some reason Unity in later versions changes the file.role  to "exe" which broke this... */
				if (file.role == "Executable" ) continue;

				fileName = Path.GetFileName(file.path);
				outputDirectory = Path.GetDirectoryName(file.path);
				Debug.Log($"File: {fileName}, Folder: {outputDirectory}");
				
			}

			// Generate VX.Bat Batch File - Unity Projects have to be run in -batchmode 

			try
			{
				//string batchContents = $"{fileName} -batchmode";
				string batchContents = $"start \"\" \"{fileName}\" -batchmode"; //--this is a safer if the project's .EXE has spaces in it 

//				string batchContents = $"start \"\" \"{fileName}\" -batchmode";  --  this is a safer if the project's .EXE has spaces in it 
				StreamWriter writer = new StreamWriter(outputDirectory + "\\VX.bat");

				writer.WriteLine("cls");
				writer.WriteLine("@echo off");
			    writer.WriteLine("echo.");
				writer.WriteLine("echo.###     ################# ####   ##################  ####      ##");
				writer.WriteLine("echo. ###  ###   ###       ###   #######   ###        ##  #######   ##");
				writer.WriteLine("echo.  ######    ###       ###   #######   ###        ##  ###  #######");
				writer.WriteLine("echo.   ####     ##################   #### #################      ####");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.               -= Voxon X Unity Plugin Launcher = -");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.                    Launching your VxUnity App");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.   Loading - please wait - this may take a few seconds to load.");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.                   (c) 2024 - Voxon Photonics");
				writer.WriteLine("echo.                          www.Voxon.co");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.");
				writer.WriteLine("echo.                              Note:");
				writer.WriteLine("echo.   If you are launching your VxUnity App outside of this batch");
				writer.WriteLine("echo.script, remember to launch your .Exe with the '-batchmode' argument.");
				writer.WriteLine("echo.");
				writer.WriteLine(" ");
				writer.WriteLine(" ");
				writer.WriteLine(" ");
				writer.WriteLine(" ");
				writer.WriteLine(batchContents);
				writer.WriteLine("TimeOut /T:10");
				writer.Close();

				Debug.Log("Batch file written with Contents: \"" + batchContents + "\"");
			}
			catch (Exception e)
			{

				Debug.LogError("Unable to write batch file");
				Debug.LogError(e.Message);
			}


			// complex way to build the paths but at least its safe
			string[] paths = { $"{Application.dataPath}", "Voxon", "Plugins", "C#-bridge-interface.dll" };
			string InterfacePath = Path.Combine(paths);
			string outputDir = $"{outputDirectory}\\C#-bridge-interface.dll";
			string[] paths2 = { $"{Application.dataPath}", "Voxon", "Plugins", "C#-Runtime.dll" };
			string RuntimePath = Path.Combine(paths2);
			try
			{ 
				File.Copy(InterfacePath, outputDir);
				outputDir = $"{outputDirectory}\\C#-Runtime.dll";
				File.Copy(RuntimePath, outputDir);
			}
			catch (Exception e)
            {

				Debug.LogWarning("UnityBuildPostProcessor.cs - Unable to copy libs! (ignore if you are overriding a build) - Interface Path : " + InterfacePath + "Runtime : " + RuntimePath + "Output : " + outputDir + "Reason E:" + e.Message );

			}
			
		
			
		}
	}
}