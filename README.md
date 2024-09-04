## Unity

Open the Scene "SlicingScene.unity"

At the moment there are two options that provide the slices to display the patterns.
Go into GameManager in the Hierarchy window: either you need to activate **both** "Generate Slice" and "Generate Bit Planes" **OR** only "Instantiate All Cut Sections".
The main difference is that one moves a cutting plane, as the other generates a whole set of planes at a given spacing and that don't move.

In Generate Slice, you can pick the frequency of the system for the slicing.
In GeneratebitPlanes - there was a function to update 24 bitplane at each frame (60 fps) - but it cant manage to display the image. I've added a delay - of a Fixed update - so each bitplane is calculated within a fixedupdate (i.e. the delay between the frames). To tweak this and make it faster - I've put a slider to change the Tinme scale, i.e. this delay. In the Console, I debug the delay between each full pattern (24 bitplanes): if you change the Time scale, you will see this delay decreasing.

The same principle is applied in InstantiateAllCutSections; though here we first need to write down how many Slices per sweep (this will be multiplied by 24 ofc) you want, when sweeping up and when sweeping down. It relies on the same principle as the Voxon when it says "slicesPerVol" (in the voxiebox_menu0.ini file I showed). Similarly, Console debugs the Sweep Up and Sweep down time (for the NbSlices patterns).


The generated image to show is RecontructedImage.bmp.
The two previous scripts contain a boolean "Display on Unity Plane" - when activated, the plane showed on Display3 will show the resulting image (but it's way slower than what is actually computed from the scripts as per the Console debugs).


## Projector

In Projectors, there is a jupyter notebook to compute the variable exposure pattern for the projector. There are multiple parameters to fill: the required frequency, the color, whether you want triggers, if you want a full sinus or only half.
