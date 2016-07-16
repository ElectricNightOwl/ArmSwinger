#ArmSwinger VR Locomotion System

ArmSwinger is an artificial VR locomotion library developed on Unity 5.4.  ArmSwinger allows you to use your arms to control your position in 3D space in a natural way with minimal disorientation.

ArmSwinger will be available on the Unity Asset Store soon!

#### License
ArmSwinger is released under the [MIT License](http://choosealicense.com/licenses/mit/).  You may use this library in your commercial or personal project and there are no restrictions on what license you use for your own project.  You may also change the library without contributing changes back to the project.  No attribution is required, but is always appreciated in your credits.

##Contributions welcome!
Do you have an improvement to ArmSwinger?  Pull Requests against the [GitHub project] (https://github.com/ElectricNightOwl/ArmSwinger/) are encouraged!  Submit your changes and they'll be reviewed by Electric Night Owl for inclusion into the master branch.p

##Requirements
ArmSwinger is tested on...
* Unity 5.4 (may work on other versions)
* SteamVR Unity Plugin 1.1.0 (may work on other versions)
* HTC Vive

##Installation
1. Download or clone the entire repository to your local machine
2. Ensure that SteamVR Unity Plugin has been imported into your project
3. Copy the entire ArmSwinger directory into your project Assets folder
4. If you haven't already, create a CameraRig prefab instance from the SteamVR Unity Plugin
5. Drag and drop the "Assets/ArmSwinger/scripts/ArmSwinger" script onto your CameraRig game object

##Overview of Included Files
#####ArmSwinger/scripts/ArmSwinger.cs
The core ArmSwinger library.  Applied to your CameraRig.  Includes extensive options for tweaking the feel and technical operation of the library.
#####ArmSwinger/scripts/HeadsetCollider.cs
Manages the box collider component on your headset (Camera (eye)).  This component will be auto-created if it's needed and not manually applied.  No public settings.
#####ArmSwinger/examples/ArmSwinger_Test_Scene.unity
A locomotion test scene that includes ramps, walls, uneven terrain, and other locomotion-centric tests.  You can also reference this scene to understand how ArmSwinger should be configured and applied to a scene.
#####ArmSwinger/resources/*
Resources needed for the test environment

## Using ArmSwinger
To begin moving, squeeze both grip buttons and swing your arms in the direction you'd like to go.  Speed and direction is controlled by the speed and rotation of your controllers.  You can move your headset freely while walking without affecting your direction.

ArmSwinger has the optional ability to "rewind" your position if you go "out of bounds".  This is enabled by default.  Reasons for out of bounds include any or all of - headset into a wall, trying to climb a surface that is too steep, trying to fall down a surface that is too steep, trying to wall walk a steep surface, and the headset colliding with geometry.  All of these features are enabled by default with sane values.

Try loading the test scene.  Walk up the different ramps, try to enter the vertical walls cube, walk up and down the stairs.  Get a feel for how the script behaves and then tweak its settings to your liking.

## ArmSwinger.cs Settings
Note that all settings in the inspector include hover text that will briefly explain what the setting does and what the default is.  This README may contain additional information as to the function and impact of the setting.

All settings are configured to sane defaults.  The one setting you should reconfigure most of the time is the Ground Ray Layer Mask.

### Locomotion Settings
#####Enable Arm Swing Navigation
Enables variable locomotion using the controllers to determine speed and direction.  Activate by holding both grip buttons. 

### Movement Settings
#####Swing Speed Both Controllers Coefficient
Only if Arm Swinger Navigation is enabled and Swing Activation Mode allows both controllers to be used for arm swinging.  When both controllers are being used for the speed calculation, the distance travelled in the world is the average of the change in position of both controllers, times this value.

#####Swing Speed Single Controller Coefficient
Only if Arm Swinger Navigation is enabled and Swing Activation Mode allows a single controller to be used for arm swinging.  When only one controller is being used for the speed calculation, the distance travelled in the world is change in position of that one controller times this value.

#####Swing Mode
Only if Arm Swinger Navigation is enabled.  Determines what is necessary to activate arm swing locomotion, and what controller is used when determining speed/direction.

######Both Grips Both Controllers
Activate by squeezing both grips.  Both controllers are used for speed/direction.
######Left Grip Both Controllers
Activate by squeezing left grip.  Both controllers are used for speed/direction.
######Right Grip Both Controllers
Activate by squeezing right grip.  Both controllers are used for speed/direction.
######One Grip Same Controller
Activate by squeezing either grip.  That controller is used for speed/direction.  Can be combined with the other controller.
######One Grip Same Controller Exclusive
Activate by squeezing either grip.  That controller is used for speed/direction.  Squeezing the grip on the other controller will have no effect until the first controller grip is released.

### Raycast Settings
#####Ground Ray Layer Mask
Layers that ArmSwinger will consider 'the ground' when determining Y movement of the play space and when calculating angle-based prevention methods.

Set all terrain, ground, and walls in your scene to a layer listed in this mask.  If you are using Wall Clipping Prevention, these surfaces should also have a collider configured.

#####Max Ray Case Length
The length of the headset raycasts used for play height adjustment and falling/climbing prevention. Should be the value of the largest height difference you ever expect the player to come across.

If you use too low of a value here, you may have rewind false positives.  If you use too high a number, there may be very minor performance implications.

#####Num Height Raycasts To Average Across
Number of Raycasts to average together when determining where to place the play area.  These raycasts are done once per frame.  Lower numbers will make the play area moving feel more responsive.  Higher numbers will smooth out terrain bumps but may feel laggy.

### Prevent Wall Clipping Settings
#####Prevent Wall Clipping
Prevents players from putting their headset through walls and ground that are in the Ground Layer Mask list.

Enabling this will also create a box collider and a HeadsetCollider script on your headset.  This will allow the headset to collide with ground/terrain and trigger ArmSwinger to rewind when appropriate.  

Note that enabling this feature will create a box collider and a rigidbody on your headset object.  By default, ArmSwinger will create a box collider component on the headset that is a non-trigger and is of size headsetBoxColliderSize.  It will also create a rigidbody component on the headset that is non-kinematic with all constraints frozen.  If you already have either of these in place, the script will not replace them, but they may not be setup to work well with the rest of Prevent Wall Clipping.  YMMV.

#####Wall Clip Layer Mask
Only if Prevent Wall Clipping is enabled.  Layers that ArmSwinger will consider 'walls' when determining if the headset has gone out of bounds.

#####Headset Collider Size
Only if Prevent Wall Clipping is enabled.  Sets the size of the box collider used to detect the headset entering geometry.

#####Min Angle To Rewind Due To Wall Clip
Only if Prevent Wall Clipping is enabled.  Sets the minimum angle a "wall" should be in order to trigger a rewind if the headset collides with it.  0 is flat ground, 90 degree is a straight up wall.  This prevents rewinds from happening if the headset is placed on the physical floor and the headset collides with the virtual floor.

### Prevent Climbing Settings
#####Prevent Climbing
Prevents the player from climbing walls and steep slopes.

##### Max Angle Player Can Climb
Only if Prevent Climbing is enabled.  The maximum angle from the ground to the approached slope that a player can climb.  0 is flat ground, 90 is a vertical wall. 

### Prevent Falling Settings
#####Prevent Falling
Prevents the player from falling down steep slopes.

##### Max Angle Player Can Fall
Only if Prevent Falling is enabled.  The maximum angle a player can try to descend.  0 is flat ground, 90 is a sheer cliff.

#### Prevent Wall Walking Settings
##### Prevent Wall Walking
Prevents the player from traversing across steep slopes.

Prevent Climbing/Falling only measure the slope of the terrain as it passes under your headset.  Prevent Wall Walking measures a point perpendicular to your path of travel and determines the slope of the terrain you are walking across.  This prevents players from approaching a slope as a very gentle angle to overcome the other prevention methods.

##### Max Angle Player Can Wall Walk
Only if Prevent Wall Walking is enabled.  The maximum angle that a player can wall walk across.  0 is flat ground, 90 is a vertical wall.

Measured perpendicular to the direction of travel, regardless of headset rotation.

### Prevent Climbing/Falling/Wall Walking Settings
##### Min Distance Change To Check Angles
Only if Prevent Climbing / Falling / Wall Walking is enabled.  Minimum distance in world units that the player must travel to trigger the Climbing / Falling / Wall Walking checks.  Lower numbers will slightly increase performance but may miss situations that should be rewound.

Since checks are only done every minDistanceChangeToCheckAngles world units, this method ensures that players will get identical results when crossing a given plane regardless of their speed and FPS.  Also improves performance by not firing the side ray and doing all the math every frame, or when the player is standing still.

##### Num Climb Fall Checks OOB Before Rewind
Only if Prevent Climbing / Falling is enabled.  The number of angle checks in a row the player must be falling or climbing to trigger a rewind.  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.  

ArmSwinger will keep track of the last numClimbFallChecksOOBBeforeRewind checks.  All the checks must agree in order to trigger a rewind.  This weeds out tiny bumps in the terrain that are technically "too tall to climb", but are reasonably cleared by the player.

If a player tries to climb a slope that is too steep, they will be able to travel (minDistanceChangeToCheckAngles * numChecksOOBBeforeRewind) world units before a rewind occurs.

##### Num Wall Walk Checks OOB Before Rewind
Only if Prevent Wall Walking is enabled.  The number of checks in a row the player must be considered wall walking to trigger a rewind.  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.

Works identically to numClimbFallChecksOOBBeforeRewind, but for wall walking detection.

##### Max Stair Height
Only if Prevent Climbing / Falling is enabled.  The maximum stair height in world units a player can climb or descend without triggering a rewind.  Set to the height of the tallest single step in your scene.

If at any time the player ascends/descends more than this value, a rewind is triggered unconditionally (no sampling multiple times).

##### Dont Save Unsafe Climb Fall Positions
Only if both Prevent Climbing and Prevent Falling is enabled.  If true, positions that can be climbed but not fallen down (or vice versa) won't be saved as rewind positions.  If false, the position will be saved anyways.  

This ensures that when a rewind happens, the player will be moved to a place that they can either climb or descend safely.  For example, say the maxAnglePlayerCanFall is 60 and the maxAnglePlayerCanClimb is 45 and the player descends a 50 degree ramp.  Near the middle of the ramp, they go Out of Bounds (for any reason).  If this feature is disabled, they could be rewound to a position on the ramp where they can only go down but can't climb, possibly trapping the player.  If this feature is enabled, the player will be rewound back to the top of the ramp (the last place the angle was such that they can both fall or climb).

##### Dont Save Unsafe Wall Walk Positions
Only if Prevent Wall Walking is enabled.  If true, positions that are considered wall walking but that haven't yet triggered a rewind won't be saved as possible rewind positions.  If false, the position will be saved anyways and the player might get stuck.

### Rewind Settings
##### Min Distance Change To Save Position
Only if a prevention method is enabled.  Minimum distance in world units that the player must travel to trigger another saved rewind position.

The measured distance traveled is a sum of the X and Z coordinate change of the player headset.

##### Num Rewind Positions
Only if a prevention method is enabled.  The number of saved positions to rewind when a player goes out of bounds (climbing, falling, or headset through terrain).

Setting to 1 will rewind the player exactly one saved position from where they went Out of Bounds.  Depending on how close to the wall this position was saved, this could result in multiple fade in/outs.  Numbers higher than 1 will increase the distance the player is removed from where they went Out of Bounds.

### Fade Settings
##### Fade On OOB
Only if a prevention method is enabled.  If true, the screen will fade to black and back to clear when the player goes out of bounds.  If false, the player is instantly teleported without interruption.

##### Fade Out Time
Only if a prevention method is enabled.  Time in seconds to fade the player view OUT if player goes out of bounds (climbing, falling, or headset through terrain).

##### Fade In Time
Only if a prevention method is enabled.  Time in seconds to fade the player view IN once the player position is corrected.  (Default: .2f)

## How does it work?
##### ArmSwing Locomotion
Your movement is based on the movement and rotation of both controllers.  When ArmSwinger is activated by holding both grip buttons, the script translates any X/Y/Z controller movement into play space movement.  The speed is based on the average movement of the two controllers.  The direciton is the average direction that the controllers are pointing.  Optionally, either controller's movement and rotation can be ignored while it is "in use" (trigger held down).

##### Play Area vertical adjustment
The play area is constantly adjusted based on the position of the headset and the terrain underneath it.  Your physical floor will always match up with the terrain directly under your headset.  This occurs both when moving artificially and with moving physically.

##### Out of Bounds
Out of Bounds can be triggered in multiple ways - if the headset tries to go through the terrain, if the player tries to climb a slope that is too steep, if the player tries to fall down a slope that is too steep, if the player tries to walk along a steep slope, or if the headset goes outside the world entirely.  These conditions are toggleable and tweakable.  Alternatively, Out of Bounds can be disabled entirely, leaving it to you to handle these conditions.

##### The Star of the Show - Vertical Raycast
Play Area vertical adjustment and Out of Bounds detection are handled by a raycast shot from the middle of the headset, downwards in world space.  The raycast is shot once per frame.  This allows ArmSwinger to place the play area vertically based on where the ray hits.  ArmSwinger also compares this frame's raycast to the last frame's raycast to determine the angle of terrain the user is crossing (and trigger an Out of Bounds condition if appropriate).  These angles are cached over multiple frames and analyzed together before making a rewind decision, allowing the user to traverse small steep obstacles without triggering a rewind.

Prevent Wall Walking uses a second raycast shot just to the left (relative to the direction of movement) of the height raycast.  It then calculates the angle between the two raycast shots to determine if the player is wallwalking.  This raycasy is only shot when an angle check is done, not every frame.

##### Rewind
If you use one of the Out of Bounds Prevention features and the player goes Out of Bounds, ArmSwinger will rewind the player to a previous safe position.  Safe positions are cached as the player moves across "safe" ground.  By default, only ground that the player can fall down, climb up, and walk across is considered "safe" and is cached.  This prevents situations where you fall down a slope you can't then climb back up if you get stuck halfway down.

## Contact
Questions?  Comments?  Hate mail?  Valentines cards?  Please send all inquiries to [Electric Night Owl](mailto:armswinger@electricnightowl.com).  Hoot!
