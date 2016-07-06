#ArmSwinger VR Locomotion System

This is an artificial VR locomotion library for Unity 5.4.  ArmSwinger allows you to use your arms to control your position in 3D space in a natural way with little or no disorientation.

ArmSwinger will be available on the Unity Asset Store soon, but will remain on GitHub under the LGPLv3 license as well.

##Contributions welcome!
Think you have an improvement to ArmSwinger?  Pull Requests against the GitHub project (https://github.com/ElectricNightOwl/ArmSwinger/) are encouraged and appreciated!  Submit your changes and they'll be reviewed by Electric Night Owl for inclusion into the master branch.

##Requirements
ArmSwinger is tested on...
* Unity 5.4
* SteamVR Unity Plugin 1.1.0
* HTC Vive

##Installation
1. Download or clone the entire repository to your local machine
2. Ensure that SteamVR Unity Plugin has been imported into your project
3. Copy the entire ArmSwinger directory into your project Assets folder
4. If you haven't already, create a CameraRig prefab instance from the SteamVR Unity Plugin
5. Drag and drop the "Assets/ArmSwinger/scripts/ArmSwinger" script onto your CameraRig game object

###Overview of Included Files
#####ArmSwinger/scripts/ArmSwinger.cs
The core ArmSwinger library.  Applied to your CameraRig.  Includes extensive options for tweaking the feel and technical operation of the library.
#####ArmSwinger/scripts/HeadsetCollider.cs
Manages the box collider component on your headset (Camera (eye)).  This component will be auto-created if it's needed and not manually applied.
#####ArmSwinger/examples/ArmSwinger_Test_Scene.unity
A locomotion test scene that includes ramps, walls, uneven terrain, and other locomotion-centric tests.  You can also reference this scene to understand how ArmSwinger should be configured and applied to a scene.
#####ArmSwinger/resources/*
Resources needed for the test environment

## Using ArmSwinger
To begin moving, squeeze both grip buttons and swing your arms in the direction you'd like to go.  Speed and direction is controlled by the speed and rotation of your controllers.  You can move your headset freely while walking without affecting your direction.

ArmSwinger has the optional ability to "rewind" your position if you go "out of bounds".  This is enabled by default.  Reasons for out of bounds include any or all of - headset into a wall, trying to climb a surface that is too steep, trying to fall down a surface that is too steep, and going outside the world.

Try loading the test scene.  Walk up the different ramps, try to enter the vertical walls cube, walk up and down the stairs.  Get a feel for how the script behaves and then tweak its settings to your liking.

 

