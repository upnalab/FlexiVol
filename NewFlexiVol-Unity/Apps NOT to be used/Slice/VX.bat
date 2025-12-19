cls
@echo off
echo.
echo.###     ################# ####   ##################  ####      ##
echo. ###  ###   ###       ###   #######   ###        ##  #######   ##
echo.  ######    ###       ###   #######   ###        ##  ###  #######
echo.   ####     ##################   #### #################      ####
echo.
echo.               -= Voxon X Unity Plugin Launcher = -
echo.
echo.                    Launching your VxUnity App
echo.
echo.   Loading - please wait - this may take a few seconds to load.
echo.
echo.
echo.                   (c) 2024 - Voxon Photonics
echo.                          www.Voxon.co
echo.
echo.
echo.                              Note:
echo.   If you are launching your VxUnity App outside of this batch
echo.script, remember to launch your .Exe with the '-batchmode' argument.
echo.
 
 
 
 
start "" "FlexiVol.exe" -batchmode
TimeOut /T:10
