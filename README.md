## FlexiVol: Demoing

# Material: 
* Foci TP-02 Subwoofer audio amplifier
* DLP LightCrafter 4500 Projector
*  Speaker: VDSSP6.5/8 HQ Power speaker
* Adaptor Jack-jumper cables
* Breadboard
* ESP32
* Crocodile clamps
* Jumper cables
* Crimp terminal (connector for projector) https://www.molex.com/en-us/products/part-detail/500798100
* Diffuser (fabric strips or membrane)
* Steel tubes (or carbon fiber) (4)

# 3D-Printed parts:
* Frame
* Support speaker
* Support Projector
* Spring
* Tube spring to speaker
* Link tube to spring
* Support tube-speaker

# Attachment:
* zip-ties (7)
* M4 screw and bold (1)
* cutting pliers (for zip ties)
* drill to create hole for through tube to enter its support

# Computer with:
* Arduino IDE
* Arduino scripts: generate3volts_squareSignal.ino; generateSineWaveESP
* LightCrafter4500
* Jupyter notebook: “Pattern with Sinusoidal Exposure.ipynb” 
* Pyramid, numbers, sphere images
* Printing 3D parts for FlexiVol
* Frame: missing
* Support speaker: “2024.06.12. Speaker holder v4”
* Support Projector: missing
* Spring: “Spiral3Holes 1.STL”
* Tube spring to speaker: “Tube.STL”
* Link tube to spring: “LinkToThreeHoles 1.STL”
* Support tube-speaker: missing

# Mounting FlexiVol

* Put the tube into the Support tube - and drill a hole to fit your screw into it.
* Put the zipties between the Spiral and the link tube to spring. 
* Put the Link tube to spring into the tube and glue it with hot glue.
* Link the frame with the spiral assembly using zip ties.
* Put the speaker into the support speaker.
* Put the steel/carbon fiber bars into the support speaker.
* Slide the frame with the 4 bars.
* Insert the Support tube-speaker into the speaker.
* Glue it with hot glue.
* Put the diffuser onto the frame.
* Add the projector support.
* Insert the projector.

## Device is ready!


# ESP32 Setting up
* Pin 13: Connect trigger from projector
* Pin 25: Connect to amplifier input (one on ground, one on left or right)

# Using  generate3volts_squareSignal.ino:
* In the Serial (115200):
** Enter (int) frequency wanted (e.g., “10” or “11”)

* Using  generateSineWaveESP.ino:
** In the Serial (115200): 
*** Enter (int) command (float) frequency (e.g, “1 11.5” to enter an 11.5 freq)

# Mechanical Displacement
* Connect the speaker to the audio amplifier output using crocodile clamps (the red/black order will define whether the pyramid is facing down or up for instance).
* Once the 10 Hz applied (I suggest 10Hz with square signal ino), turn on the audio amplifier. The volume will tune the amplitude of the displacement - don’t put it too strong, but show how big the displacement can be when demoing.

# Setting up the projector sequence
* Open LightCrafter 4500.
* Connect the HMDI, and USB between the projector and the computer.
* In the window, it will show a green light “connected”.
* Click Pattern Sequence.
* Click Sequence Settings.
* Click Apply Solution (see next Figure) and select the sequence with the chosen frequency (it needs to contain the frequency, “externalTrigger” and “linear” in its name (e.g. Blue Sinusoid 11Hz-Trigger external - Linear). I suggest using the Blue 11Hz Linear Trigger external thingy.
* The operating mode will update to “pattern sequence” on its own.
* Send to the projector. Another window opens.
* Validate sequence.
* Send

* Note: if the ESP isn’t plugged in, the projector won’t turn on -> as it is waiting for a trigger 🙃

* Now you can select one of the image - alone in their folders (e.g., Imagenes/Pyramid; or Documentos/FlexiVol/Projectors/SingleImage/PyramidThingy; or Documentos/FlexiVol/Projectors/binaryImage).
Press F11 to put in full screen.
* Apply a tape to hide the bottom of the projection (white).

# Demo is ready!





