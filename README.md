#ArmSwinger VR Locomotion System

ArmSwinger is an artificial VR locomotion library developed on Unity 5.4.  ArmSwinger allows you to use your arms to control your position in 3D space in a natural way with minimal disorientation.

ArmSwinger will be available on the Unity Asset Store soon!

#### License
ArmSwinger is released under the [MIT License](http://choosealicense.com/licenses/mit/).  You may use this library in your commercial or personal project and there are no restrictions on what license you use for your own project. 

##Contributions welcome!
Do you have an improvement to ArmSwinger?  Pull Requests against the [GitHub project] (https://github.com/ElectricNightOwl/ArmSwinger/) are encouraged!  Submit your changes and they'll be reviewed by Electric Night Owl for inclusion into the master branch.

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

## ArmSwinger.cs Settings
Note that all settings in the inspector include hover text that will briefly explain what the setting does and what the default is.  This README may contain additional information as to the function and impact of the setting.

All settings are configured to sane defaults.  The one setting you should reconfigure most of the time is the Ground Ray Layer Mask.

#### Locomotion Settings
#####Enable Arm Swinger Navigation
Enables variable locomotion using the controllers to determine speed and direction.  Activate by holding both grip buttons. 

#####Ground Ray Layer Mask
Layers that ArmSwinger will consider 'the ground' when determining Y movement of the play space and when calculating out of bounds.

Set all terrain, ground, and walls in your scene to a layer listed in this mask.  If you are using Wall Clipping Prevention, these surfaces should also have a collider configured.

##### Swing Speed Linear Coefficient
The distance travelled in the world is the sum of the change in position of both controllers, times this value.

Allows you to speed up or slow down player movement.

#### Controller in Use Settings
#####Controller Coefficient When Trigger
A controller's movement is multiplied by this value when the trigger is depressed.  0.0 will cause the controller movement to be ignored when trigger is pulled.  1.0 will still use the controller's entire movement when computing movement speed.  

Useful for slowing the player down while using a controller, or ignoring an in-use controller while other movements occur.

#####Controller Ignore Rotation When Trigger
Enable to ignore a controller's rotation when determining direction while trigger is held down.

Useful for allowing the player to keep moving in a pointed direction with a non-triggered controller while doing something else with the triggered controller.

#### Raycast Settings
#####Max Ray Case Length
The length of the headset raycasts used for play height adjustment and falling/climbing prevention. Should be the value of the largest height difference you ever expect the player to come across.

If you use too low of a value here, you may have rewind false positives.  If you use too high a number, there may be very minor performance implications.

#####Num Raycasts To Average Across
Number of Raycasts to average together when determining where to place the play area.  Lower numbers will make the play area moving feel more responsive.  Higher numbers will smooth out terrain bumps but may feel laggy.  Setting this to 1 will disable the feature.

#### Prevent Wall Clipping Settings
#####Prevent Wall Clipping
Prevents players from putting their headset through walls and ground that are in the Ground Layer Mask list.

Enabling this will also create a box collider and a HeadsetCollider script on your headset.  This will allow the headset to collide with ground/terrain and trigger ArmSwinger to rewind when appropriate.  You can also pre-create these, but if you don't ArmSwinger will do it for you.

#### Prevent Climbing Settings
#####Prevent Climbing
Prevents the player from climbing walls and steep slopes.

##### Max Angle Player Can Climb
Only if Prevent Climbing is enabled.  The maximum angle from the ground to the approached slope that a player can climb.  0 is flat ground, 90 is a vertical wall. 

#### Prevent Falling Settings
#####Prevent Falling
Prevents the player from falling down steep slopes.

##### Max Angle Player Can Fall
Only if Prevent Falling is enabled.  The maximum angle a player can try to descend.  0 is flat ground, 90 is a sheer cliff.

#### Prevent Wall Walking Settings
##### Prevent Wall Walking
Prevents the player from traversing across steep slopes.

Prevent Climbing/Falling only measure the slope of the terrain as it passes under your headset.  Prevent Wall Walking measures a point perpendicular to your path of travel and also controls the slope of terrain you are going across.  This prevents players from approaching a slope as a very gentle angle to overcome the other prevention methods.

##### Max Angle Player Can Wall Walk
Only if Prevent Wall Walking is enabled.  The maximum angle that a player can wall walk across.  0 is flat ground, 90 is a vertical wall.

Measured perpendicular to the direction of travel, regardless of headset rotation.

#### Prevent Climbing/Falling/Wall Walking Settings
##### Min Distance Change To Check Angles
Only if Prevent Climbing / Falling / Wall Walking is enabled.  Minimum distance in world units that the player must travel to trigger the Climbing / Falling / Wall Walking checks.  Lower numbers will slightly increase performance but may miss situations that should be rewound.

Since checks are only done every minDistanceChangeToCheckAngles world units, this method ensures that players will get identical results when crossing a given plane regardless of their speed and FPS.  Also improves performance by not firing the side ray and doing all the math every frame, or when the player is standing still.

##### Num Checks OOB Before Rewind
Only if Prevent Climbing / Falling is enabled.  The number of angle checks in a row the player must be falling or climbing to trigger a rewind.  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.  

ArmSwinger will keep track of the last numChecksOOBBeforeRewind checks.  All the checks must agree in order to trigger a rewind.  This weeds out tiny bumps in the terrain that are technically "too tall to climb", but are reasonably cleared by the player.

If a player tries to climb a slope that is too steep, they will be able to travel (minDistanceChangeToCheckAngles * numChecksOOBBeforeRewind) world units before a rewind occurs.

##### Max Stair Height
Only if Prevent Climbing / Falling is enabled.  The maximum stair height in world units a player can climb or descend without triggering a rewind.  Set to the height of the tallest single step in your scene.

If at any time the player ascends/descends more than this value, a rewind is triggered unconditionally.

##### Dont Save Unsafe Positions
Only if both Prevent Climbing and Prevent Falling is enabled.  If true, positions that can be climbed but not fallen down (or vice versa) won't be saved as rewind positions.  If false, the position will be saved anyways.  

This ensures that when a rewind happens, the player will be moved to a place that they can either climb or descend safely.  For example, say the maxAnglePlayerCanFall is 60 and the maxAnglePlayerCanClimb is 45 and the player descends a 50 degree ramp.  Near the middle of the ramp, they go Out of Bounds (for any reason).  If this feature is disabled, they could be rewound to a position on the ramp where they can only go down but can't climb, possibly trapping the player.  If this feature is enabled, the player will be rewound back to the top of the ramp (the last place the angle was such that they can both fall or climb).

#### Rewind Settings
##### Min Distance Change To Save Position
Only if a prevention method is enabled.  Minimum distance in world units that the player must travel to trigger another saved rewind position.

The measured distance traveled is a sum of the X and Z coordinate change of the player headset.

##### Num Rewind Positions
Only if a prevention method is enabled.  The number of saved positions to rewind when a player goes out of bounds (climbing, falling, or headset through terrain).

Setting to 1 will rewind the player exactly one saved position from where they went Out of Bounds.  Depending on how close to the wall this position was saved, this could result in multiple fade in/outs.  Numbers higher than 1 will increase the distance the player is removed from where they went Out of Bounds.

#### Fade Settings
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
Out of Bounds can be triggered in multiple ways - if the headset tries to go through the terrain, if the player tries to climb a slope that is too steep, if the player tries to fall down a slope that is too steep, or if the headset goes outside the world entirely.  These conditions are toggleable and tweakable.  Alternatively, Out of Bounds can be disabled entirely, leaving it to you to handle these conditions.

##### The Star of the Show - Vertical Raycast
Play Area vertical adjustment and Out of Bounds detection are handled by a raycast shot from the middle of the headset, downwards in world space.  The raycast is shot once per frame.  This allows ArmSwinger to place the play area vertically based on where the ray hits.  ArmSwinger also compares this frame's raycast to the last frame's raycast to determine the angle of terrain the user is crossing (and trigger an Out of Bounds condition if appropriate).  These angles are cached over multiple frames and analyzed together before making a rewind decision, allowing the user to traverse small steep obstacles without triggering a rewind.

##### Rewind
If you use one of the Out of Bounds Prevention features and the player goes Out of Bounds, ArmSwinger will rewind the player to a previous safe position.  Safe positions are cached as the player moves across "safe" ground.  By default, only ground that the player can both fall down and climb up is considered "safe" and is cached.  This prevents situations where you fall down a slope you can't then climb back up if you get stuck halfway down.

## Contact
Questions?  Comments?  Hate mail?  Valentines cards?  Please send all inquiries to [Electric Night Owl](mailto:armswinger@electricnightowl.com).  Hoot!
