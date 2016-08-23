# ArmSwinger VR Locomotion System

- [**What is this?**](#what-is-this)
- [I hate reading, can't I just watch a video?](#i-hate-reading-cant-i-just-watch-a-video)
- [**Where can I get ArmSwinger?**](#where-can-i-get-armswinger)
- [License](#license)
- [Code contributions welcome!](#code-contributions-welcome)
- [Requirements](#requirements)
- [**Installation**](#installation)
- [Overview of Included Files](#overview-of-included-files)
- [Using ArmSwinger](#using-armswinger)
- [**ArmSwinger.cs Settings**](#armswingercs-settings)
  * [General Settings](#general-settings)
  * [Arm Swing Settings](#arm-swing-settings)
  * [Controller Smoothing Settings](#controller-smoothing)
  * [Inertia Settings](#inertia-settings)
  * [Raycast Settings](#raycast-settings)
  * [Prevent Wall Clip Settings](#prevent-wall-clip-settings)
  * [Prevent Climbing Settings](#prevent-climbing-settings)
  * [Prevent Falling Settings](#prevent-falling-settings)
  * [Prevent Wall Walking Settings](#prevent-wall-walking-settings)
  * [Instant Height Settings](#instant-height-settings)
  * [Check Settings](#check-settings)
  * [Push Back Override Settings](#push-back-override-settings)
  * [Rewind Settings](#rewind-settings)
- [Public Functions](#public-functions)
- [How does it work?](#how-does-it-work)
- [Special Thanks](#special-thanks)
- [**Contact**](#contact)

## What is this?
ArmSwinger is an artificial VR locomotion library developed on Unity 5.4.  ArmSwinger allows you to use your arms to control your position in 3D space in a natural way with minimal disorientation.

You can download the test level to see what ArmSwinger is capable of.
* [ArmSwinger VR Locomotion v1.1 Test Build](https://drive.google.com/open?id=0B7TmmTf6PGr1QlE1aFY5dlJVNnc)

There is a [release trailer](https://www.youtube.com/watch?v=JjZNLGN_k-A) for ArmSwinger that demonstrates many of the major features.

If you just want the basics, check out the [ArmSwinger - Quick Start](https://www.youtube.com/watch?v=hmX3ymwzzbI&list=PLJKrZBvXUBwbKkgbInXTece7lk3hpoarv&index=4) video.  You can view the [ArmSwinger Video Tutorial Series](https://www.youtube.com/playlist?list=PLJKrZBvXUBwawR6mZ0088Sg8rMrdvgYqT) to learn how to customize ArmSwinger's many settings to your game or application.



## I hate reading, can't I just watch a video?
Sure can.  [The ArmSwinger Tutorial series](https://www.youtube.com/playlist?list=PLJKrZBvXUBwawR6mZ0088Sg8rMrdvgYqT) on YouTube covers nearly all the settings shown here.  The videos also give additional background and demonstrations of the various features.

## Where can I get ArmSwinger?
The latest version of ArmSwinger will always be available on [GitHub](https://github.com/ElectricNightOwl/ArmSwinger).

If you'd like to support the continued development of ArmSwinger, a version of the library is also available on the [Unity Asset Store](https://www.assetstore.unity3d.com/#!/content/67602) for a small fee.  Your purchase on The Unity Asset Store also includes priority e-mail support.

There is no difference in script features, functionality, or documentation between the two versions.  The Unity Asset Store package does include an example scene not available on GitHub.  This scene features a series of locomotion-related obstacles and tests, as well as an in-game ArmSwing settings panel to try different options instantly.

## License
ArmSwinger is released under the [MIT License](http://choosealicense.com/licenses/mit/).  You may use this library in your commercial or personal project and there are no restrictions on what license you use for your own project.  You may also change the library without contributing changes back to the project.  No attribution is required, but is always appreciated in your credits.

## Code contributions welcome!
Do you have an improvement to ArmSwinger?  Pull Requests against the [GitHub project] (https://github.com/ElectricNightOwl/ArmSwinger/) are encouraged!  Submit your changes and they'll be reviewed by Electric Night Owl for inclusion into the master branch.

## Requirements
ArmSwinger is tested on...
* Unity 5.4 (may work on other versions)
* SteamVR Unity Plugin 1.1.0 (may work on other versions)
* HTC Vive

## Installation
[Open Source / GitHub Version](https://github.com/ElectricNightOwl/ArmSwinger/) only:

1. Create an ArmSwinger folder in your project's Assets directory
2. Download or clone the entire repository into the ArmSwinger folder
3. Ensure that SteamVR Unity Plugin has been imported into your project
4. If you haven't already, create a CameraRig prefab instance from the SteamVR Unity Plugin
5. Drag and drop the "Assets/ArmSwinger/scripts/ArmSwinger" script onto your CameraRig game object

[Unity Asset Store Version](https://www.assetstore.unity3d.com/en/#!/content/67602) only:

1. Browse to the [Unity Asset Store page for ArmSwinger](https://www.assetstore.unity3d.com/en/#!/content/67602)
2. Click "Download" or "Import", and import all files into your project
3. Ensure that SteamVR Unity Plugin has been imported into your project
4. If you haven't already, create a CameraRig prefab instance from the SteamVR Unity Plugin
5. Drag and drop the "Assets/ArmSwinger/scripts/ArmSwinger" script onto your CameraRig game object

## Overview of Included Files
### ArmSwinger/scripts/ArmSwinger.cs
The core ArmSwinger library.  Applied to your SteamVR CameraRig.  Includes extensive options for tweaking the feel and technical operation of the library.
### ArmSwinger/scripts/HeadsetCollider.cs
Manages the sphere collider component on your headset (Camera (eye)).  This component will be auto-created if it's needed and not manually applied.  No public settings.
### ArmSwinger/examples/ArmSwinger_Test_Scene.unity
Only in the Unity Asset Store version of ArmSwinger.  A locomotion test scene that includes ramps, walls, uneven terrain, and other locomotion-centric tests.  Includes the world-famous "Wall o' Settings" that allows you to tweak ArmSwinger settings in-game while adjusting look and feel for your own project.  You can also reference this scene to understand how ArmSwinger should be configured and applied to a scene.

**WARNING**: The test scene uses layers 7 and 8 for ground and walls.  If you open the test scene in an existing project that alters the collision matrix for these layers, ArmSwinger might fail.  It is recommended to open the test scene in a new project.
### ArmSwinger/resources/*
Only in the Unity Asset Store version of ArmSwinger.  Textures, scripts, and other resources needed for the test environment only.

## Using ArmSwinger
To begin moving, squeeze both grip buttons and swing your arms in the direction you'd like to go.  Speed and direction is controlled by the speed and rotation of your controllers.  You can move your headset freely while walking without affecting your direction.

ArmSwinger has the optional ability to "rewind" your position if you go "out of bounds".  This is enabled by default.  Reasons for out of bounds include any or all of - headset into a wall, trying to climb a surface that is too steep, trying to fall down a surface that is too steep, trying to wall walk a steep surface, and the headset colliding with geometry.  All of these features are enabled by default with sane values.

## ArmSwinger.cs Settings
Note that all settings in the inspector include hover text that will briefly explain what the setting does and what the default is.  This README may contain additional information as to the function and impact of the setting.

All settings are configured to sane defaults.  At a minimum, you should seriously consider customizing the following settings for your project:
- [Raycast - Ground Layer Mask](#raycast-ground-layer-mask)
- [Prevent Wall Clip - Layer Mask](#prevent-wall-clip-layer-mask)

### General Settings
#### General - Scale World Units To Camera Rig Scale
By default, several unit- and speed-based settings are in absolute world units regardless of CameraRig scale.  If this setting is true, all of those settings will be automatically scaled at to match the X scale of this CameraRig.  If you use a non-default CameraRig scale, enabling this setting will allow you to specify all settings in meters-per-second in relation to the CameraRig rather than in world units.

Settings that are affected by this are:
- [Arm Swing - Max Speed](#arm-swing-max-speed)
- [Raycast - Max Length](#raycast-max-length)
- [Rewind - Min Distance Change To Save Position](#rewind-min-distance-change-to-save-position)
- [Instant Height - Max Change](#instant-height-max-change)

If this feature is enabled, all of the above settings will be treated as meters or meters per second scaled to the X scale of the Camera Rig.

#### General - Auto Adjust Fixed Timestep
In order for ArmSwinger to handle movement and wall collisions correctly, Time.fixedDeltaTime must be 0.0111 (90 per second) or less.  If this feature is enabled, the setting will be adjusted automatically if it is higher than 0.0111.  If disabled, an error will be generated but the value will not be changed.

### Arm Swing Settings
#### Arm Swing - Navigation
Enables variable locomotion using the controllers to determine speed and direction.  Activated according to the selected Mode.

#### Arm Swing - Swing Mode
Only if Arm Swing Navigation is enabled.  Determines what is necessary to activate arm swing locomotion, and what controller is used when determining speed/direction.

- Both Grips Both Controllers
 - Activate by squeezing both grips.  Both controllers are used for speed/direction.
- Left Grip Both Controllers
 - Activate by squeezing left grip.  Both controllers are used for speed/direction.
- Right Grip Both Controllers
 - Activate by squeezing right grip.  Both controllers are used for speed/direction.
- One Grip Same Controller
 - Activate by squeezing either grip.  That controller is used for speed/direction.  Can be combined with the other controller.
- One Grip Same Controller Exclusive
 - Activate by squeezing either grip.  That controller is used for speed/direction.  Squeezing the grip on the other controller will have no effect until the first controller grip is released.

#### Arm Swing - Controller To Movement Curve
Only if Arm Swing Navigation is enabled.  Curve that determines how much a given controller change translates into camera rig movement.  The far left of the curve is no controller movement and no virtual movement.  The far right is "Arm Swing - Controller Speed For Max Speed" (controller movement) and "Arm Swing - Max Speed" (virtual momvement).

#### Arm Swing - Controller Speed For Max Speed
Only if Arm Swing Navigation is enabled.  The number of CameraRig local units per second a controller needs to be moving to be considered going max speed.

#### Arm Swing - Max Speed
Only if Arm Swing Navigation is enabled.  The fastest base speed (in world units) a player can travel when moving controllers at "Arm Swing - Controller Speed For Max Speed".  The actual max speed of the player will depend on the both/single controller coefficients you configure.

Affected by [General - Scale World Units To Camera Rig Scale](#general-scale-world-units-to-camera-rig-scale).

#### Arm Swing - Both Controllers Coefficient
Only if Arm Swinger Navigation is enabled and Swing Activation Mode allows both controllers to be used for arm swinging.  Used to boost or nerf the player's speed when using boths controllers for arm swinging.  A value of 1.0 will not modify the curve / max speed calculation.

#### Arm Swing - Single Controller Coefficient
Only if Arm Swinger Navigation is enabled and Swing Activation Mode allows a single controller to be used for arm swinging.  Used to boost or nerf the player's speed when using a single controller for arm swinging.  A value of 1.0 will not modify the curve / max speed calculation.

### Controller Smoothing Settings
#### Controller Smoothing
Uses controller movement sampling to help eliminate jerks and unpleasant movement when controllers suddenly change position due to tracking inaccuracies.

ArmSwinger tracks your controllers movement on a frame-to-frame basis by checking the distance between the position this frame and last frame.  If your controllers have a tracking 'hiccup' where they move a large distance over a single frame, ArmSwinger detects that as you swinging REALLY HARD.  When this happens at slow swing speeds, the player is suddenly jerked forward as though they were swinging at armSwingControllerSpeedForMaxSpeed for a single frame.

While the effect isn't terrible on it's own, the results are far worse when movingInertia is enabled.  movingInertia works by using the last frame's movement amount, and then steadily decreasing it over time to simulate momentum.  When the single-frame jerk happens, movingInertia diligency ramps you down from armSwingMaxSpeed to zero.  This manifests to the player as "I was moving very slowing, and suddenly I jerked forward before quickly slowing back down!"  

The purpose of Controller Smoothing is to eliminate this single-frame controller movement anomoly that causes all these issues.

#### Controller Smoothing - Mode
Only if Controller Smoothing is enabled.  Determines how controller smoothing calculates the smoothed movement value used by arm swinging.
- Lowest
 - Use the lowest value in the cache.  Should only be used with small cache sizes.
- Average
 - Use the average of all values in the cache.
- Average Minus Highest
 - Use the average of all values in the cache, but disregard the highest value.  When a controller jitters, the value change in that frame is almost always higher than normal values and will be discarded.
 
#### Controller Smoothing - Cache Size
Only if Controller Smoothing is enabled.  Sets the number of calculated controller movements to keep in the cache.  Setting this number too low may allow a jittering controller to cause jerky movements for the player.  Setting this number too high increases lag time from controller movement to camera rig movement.

### Inertia Settings
#### Moving Inertia
Simulates inertia while arm swinging.  If the controllers change position slower than the moving inertia calculation, the inertia calculation will be used to determine forward movement.

Without Moving Inertia, there is a brief moment of no movement when arm swinging as both controllers reach their apex simultaneously.  This option maintains momentum during that time, creating continuity of movement.

Note that there are NO PHYSICS being performed with this setting.  All interia is purely simulated by ArmSwinger in a linear fashion.

#### Moving Inertia - Time To Stop At Max Speed
Only if Moving Inertia is enabled.  The time it will take to go from armSwingMaxSpeed to 0 if arm swinging is engaged and the player does not move the controllers.  Speeds lower than armSwingMaxSpeed will scale their stopping time linearly.

#### Stopping Inertia
Simulates inertia when arm swinging stops.

#### Stopping Inertia - Time To Stop At Max Speed
Only if Stopping Inertia is enabled.  The time it will take to go from armSwingMaxSpeed to 0 when arm swinging is disengaged.  Speeds lower than armSwingMaxSpeed will scale their stopping time linearly.

### Raycast Settings
#### Raycast - Ground Layer Mask
Layers that ArmSwinger will consider 'the ground' when determining Y movement of the play space and when calculating angle-based prevention methods.

Set all terrain, ground, and walls in your scene to a layer listed in this mask.  If you are using Wall Clipping Prevention, these surfaces should also have a collider configured.

#### Raycast - Max Length
The length of the headset raycasts (in CameraRig local units) used for play height adjustment and falling/climbing prevention. Should be the value of the largest height difference you ever expect the player to come across.

If you use too low of a value here, you may have rewind misses (should have rewound, and didn't).  If you use too high a number, there may be very minor performance implications.

Affected by [General - Scale World Units To Camera Rig Scale](#general-scale-world-units-to-camera-rig-scale).

#### Raycast - Average Height Cache Size
Number of Raycasts to average together when determining where to place the play area.  These raycasts are done once per frame.  Lower numbers will make the play area moving feel more responsive.  Higher numbers will smooth out terrain bumps but may feel laggy.

#### Raycast - Only Height Adjust While Arm Swinging
Will prevent the camera rig height from being adjusted while the player is not Arm Swinging.

This is primarily a comfort option for those people who dislike the vertical movement while walking physically.

Note that this is tied closely to maxInstantHeightChange.  If the player stops Arm Swinging and walks around the play area physically, they are permitted to walk inside geometry around them.  All prevention systems are still active during this time, and attempting to walk into a surface that is too steep to climb (among other reasons) will result in a rewind.

If the player starts Arm Swinging again, a check is immediately done to see what height change needs to occur.  If the required height change is larger than maxInstantHeightChange, the player view is faded out before the height adjustment is made.  This ensures that players don't have the jarring experience of instantly being teleported several feet up/down from where they're currently standing.

### Prevent Wall Clip Settings
#### Prevent Wall Clip
Prevents players from putting their headset through walls and ground that are in the preventWallClipLayerMask list.

Note that enabling this feature will create a sphere collider and a rigidbody on the headset object.  This will allow the headset to collide with ground/terrain and trigger ArmSwinger to rewind when appropriate.  All terrain/ground/objects you want the player to be unable to put their head into should have a non-trigger collider on them.

By default, ArmSwinger will create a sphere collider component on the headset that is a non-trigger and is of radius preventWallClipHeadsetColliderRadius.  It will also create a rigidbody component on the headset that is non-kinematic with all constraints frozen.  If you already have either a collider (any type) or rigidbody in place, the script will not replace them, but they may not be setup to work well with the rest of Prevent Wall Clip.  YMMV.

#### Prevent Wall Clip - Layer Mask
Only if Prevent Wall Clip is enabled.  Layers that ArmSwinger will consider 'walls' when determining if the headset has gone out of bounds.

Any object that your player might be able to stick their head into should be in a layer that is included in this layer mask.  Walls, floors, crates, doors, hills, etc.

#### Prevent Wall Clip - Mode
Only if Prevent Wall Clip is enabled.  Changes how Prevent Wall Clip reacts when the player attempts to clip into a wall.
- Rewind
 - Fade out, rewind rewindrewindNumSavedPositionsToRewind postitions, fade back in.
- Push Back
 - Do not allow the player to make the move.  Instead, adjust the position of the play area so that they cannot enter the wall.

#### Prevent Wall Clip - Headset Collider Radius
Only if Prevent Wall Clip is enabled.  Sets the radius of the sphere collider used to detect the headset entering geometry.

#### Prevent Wall Clip - Min Angle To Trigger
Only if Prevent Wall Clip is enabled.  Sets the minimum angle a "wall" should be in order to trigger Prevent Wall Clip if the headset collides with it.  0 is flat ground, 90 degree is a straight up wall.  This prevents rewinds from happening if the headset is placed on the physical floor and the headset collides with the virtual floor.

#### Prevent Wall Clip - Speed Penalty Coefficient
Only if Prevent Wall Clip is enabled.  When players arm swing directly into the wall, their speed will be multiplied by this amount for preventWallClipSpeedPenaltyTime seconds.  This helps prevent judder while Prevent Wall Clip is active, and prevents the player from seeing through geometry.  Setting this to 1.0 disables the feature entirely.

#### Prevent Wall Clip - Speed Penalty Time
Only if Prevent Wall Clip is enabled.  Sets the amount of time in seconds the player's arm swinging speed will be reduced while wall clipping.  Each contact with the wall will reset the timer to this value.

### Prevent Climbing Settings
#### Prevent Climbing
Prevents the player from climbing walls and steep slopes.

#### Prevent Climbing - Max Angle Player Can Climb
Only if Prevent Climbing is enabled.  The maximum angle from the ground to the approached slope that a player can climb.  0 is flat ground, 90 is a vertical wall. 

### Prevent Falling Settings
#### Prevent Falling
Prevents the player from falling down steep slopes.

#### Prevent Falling - Max Angle Player Can Fall
Only if Prevent Falling is enabled.  The maximum angle a player can try to descend.  0 is flat ground, 90 is a sheer cliff.

### Prevent Wall Walking Settings
#### Prevent Wall Walking
Prevents the player from traversing across steep slopes.  Uses preventClimbingMaxAnglePlayerCanClimb when wall walking up, and preventFallingMaxAnglePlayerCanFall when wall walking down.

Prevent Climbing/Falling only measure the slope of the terrain as it passes under your headset.  Prevent Wall Walking measures a point perpendicular to your path of travel and determines the slope of the terrain you are walking across.  This prevents players from approaching a slope as a very gentle angle to overcome the other prevention methods.

### Instant Height Settings
#### Instant Height - Max Change
Only if Prevent Climbing / Falling or Only Height Adjust While Arm Swinging are enabled.  The maximum height in world units a player can climb or descend in a single frame without triggering a rewind.  Allows climbing of steps this size or below, and prevents jumping over walls or falling off cliffs.  Also affects raycastOnlyHeightAdjustWhileArmSwinging.

If at any time the player ascends/descends more than this value over a single frame, a rewind is triggered unconditionally (no sampling multiple times).

Affected by [General - Scale World Units To Camera Rig Scale](#general-scale-world-units-to-camera-rig-scale).

#### Instant Height - Climb Prevention Mode
Only if Prevent Climbing is enabled.  Changes how Prevent Climbing reacts when a player tried to instantly climb greater than instantHeightMaxChange.
- Rewind
 - Fade out, rewind rewindNumSavedPositionsToRewind postitions, fade back in.
- Push Back
 - Do not allow the player to make the move.  Instead, adjust the position of the play area so that they cannot fall down.

#### Instant Height - Fall Prevention Mode
Only if Prevent Falling is enabled.  Changes how Prevent Falling reacts when a player tried to instantly fall greater than instantHeightMaxChange.
- Rewind
 - Fade out, rewind rewindNumSavedPositionsToRewind postitions, fade back in.
- Push Back
 - Do not allow the player to make the move.  Instead, adjust the position of the play area so that they cannot fall down.

### Check Settings
 
#### Checks - Num Climb Fall Checks OOB Before Rewind
Only if Prevent Climbing / Falling is enabled.  The number of angle checks in a row the player must be falling or climbing to trigger a rewind.  Checks are performed in sync with rewinds (rewindMinDistanceChangeToSavePosition).  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.  

ArmSwinger will keep track of the last numClimbFallChecksOOBBeforeRewind checks.  All the checks must agree in order to trigger a rewind.  This weeds out tiny bumps in the terrain that are technically "too tall to climb", but are reasonably cleared by the player.

If a player tries to climb a slope that is too steep, they will be able to travel (rewindMinDistanceChangeToSavePosition * checksNumClimbFallChecksOOBBeforeRewind) world units (scaled by the Camera Rig's X scale if generalScaleWorldUnitsToCameraRigScale is enabled) before a rewind occurs.

#### Checks - Num Wall Walk Checks OOB Before Rewind
Only if Prevent Wall Walking is enabled.  The number of checks in a row the player must be considered wall walking to trigger a rewind.  Checks are performed in sync with rewinds (rewindMinDistanceChangeToSavePosition).  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.

Unlike Climb/Fall, we store twice the number of checks needed to trigger a rewind.  Only checksNumClimbFallChecksOOBBeforeRewind of those checks need to agree to trigger a rewind.  This ensures that a player cannot zig-zag their way up a wall by purposely adding a "dissenting" check periodically.

### Push Back Override Settings
#### Push Back Override
Only if a Prevention method is using mode Push Back.  Uses a token bucket system to determine if a player has been getting pushed back for too long.  Also helps players who have gotten stuck in geometry.

Push Back Override will rewind a player to a previous safe position if they continually make a move that results in a push back.  It also helps resolve situations where the player is stuck in a continuous push back loop.

Push Back Override uses a token bucket system to determine if the player should be pushed back or rewound.  If a Prevention method uses Push Back as its mode, the player will get pushed back when attempting a move that would cause them to be out of bounds.  Each of these push backs costs one token from the bucket.

The bucket starts with pushBackOverrideMaxTokens tokens in it, which means it is full.  Each second, pushBackOverrideRefillPerSec are added to the bucket.  In fact, partial tokens are added each frame so that the total across the entire second is pushBackOverrideRefillPerSec.  If the bucket is full (pushBackOverrideMaxTokens) and a token needs to be added, it "spills out" and is lost.

When the player does a move that results in a push back, one token is subtracted from the bucket.  As long as there is at least one token in the bucket, the push back happens normally.  If at any point the bucket has less than one token in it but a push back is called for, the player will instead be rewound according to the global rewind settings.

This allows the player to push against a wall or surface a reasonable amount of time without being rewound.  However if they try to continually run into a wall or push their head into the wall, they get rewound.  It also helps handle any unexpected situations where the player gets stuck in a push back loop that they cannot escape from.

#### Push Back Override - Refill Per Sec
Only if Push Back Override is enabled.  The amount of tokens that are added to the bucket every second.  The correct proportion of tokens are added each frame to add up to this number per second.

#### Push Back Override - Max Tokens
Only if Push Back Override is enabled.  The maximum number of tokens in the bucket.  Additional tokens 'spill out' and are lost.

### Rewind Settings
#### Rewind - Min Distance Change To Save Position
Only if a prevention method is enabled.  Minimum distance in world units that the player must travel to trigger another saved rewind position.

The measured distance traveled is a sum of the X and Z coordinate change of the player headset, but not Y.

Note that climb/fall/wall walk checks are only done every rewindMinDistanceChangeToSavePosition world units (so angle checks and rewind saves are sync'd).  This method ensures that players will get (mostly) identical results when crossing a given plane regardless of their speed and FPS. Also improves performance by not firing the side ray and doing all the math every frame, or when the player is standing still.

Affected by [General - Scale World Units To Camera Rig Scale](#general-scale-world-units-to-camera-rig-scale).

#### Rewind - Dont Save Unsafe Climb Fall Positions
Only if both Prevent Climbing and Prevent Falling are enabled.  If true, positions that can be climbed but not fallen down (or vice versa) won't be saved as rewind positions.  If false, the position will be saved anyways and the player might get stuck.

This ensures that when a rewind happens, the player will be moved to a place that they can either climb or descend safely.  For example, say the preventFallingMaxAnglePlayerCanFall is 60 and the preventClimbingMaxAnglePlayerCanClimb is 45 and the player descends a 50 degree ramp.  Near the middle of the ramp, they go Out of Bounds (for any reason).  If this feature is disabled, they could be rewound to a position on the ramp where they can only go down but can't climb, possibly trapping the player.  If this feature is enabled, the player will be rewound back to the top of the ramp (the last place the angle was such that they can both fall or climb).

#### Rewind - Dont Save Unsafe Wall Walk Positions
Only if Prevent Wall Walking is enabled.  If true, positions that are considered wall walking but that haven't yet triggered a rewind won't be saved as possible rewind positions.  If false, the position will be saved anyways and the player might get stuck.

#### Rewind - Num Positions To Store
Only if a prevention method is enabled.  The number of saved positions to cache total.  Allows multiple consecutive rewinds to go even further back in time as necessary.  Must be higher than rewindNumSavedPositionsToRewind.

#### Rewind - Num Positions To Rewind
Only if a prevention method is enabled.  The number of saved positions to rewind when a player goes out of bounds and a rewind is triggered.

Setting to 1 will rewind the player exactly one saved position from where they went Out of Bounds.  Depending on how close to the wall this position was saved, this could result in multiple fade in/outs.  Numbers higher than 1 will increase the distance the player is moved from where they went Out of Bounds.

#### Rewind - Fade Out Sec
Only if a prevention method is enabled.  Time in seconds to fade the player view OUT if a rewind is triggered.

#### Fade In Time
Only if a prevention method is enabled.  Time in seconds to fade the player view IN once the player position is corrected.  (Default: .2f)

## Public Functions
These functions may be called from other scripts or events in your scene.  You'll need to find the ArmSwinger component first.

`ArmSwinger armSwinger = GameObject.FindObjectOfType<ArmSwinger>();`

#### armSwinger.triggerRewind()
Triggers an unconditional manual rewind.

#### armSwinger.moveCameraRig(Vector3 newPosition)
Safely moves the Camera Rig (play area) to newPosition.  This move will not cause a rewind, and the previous position cache will be cleared.

### Pause Variables
You may have need to move your player in artificial ways separate from ArmSwinger.  If you use moveCameraRig, the move will be done safely.  If you need to do something more exotic, you can pause/unpause the various prevention features of ArmSwinger.

Note that pausing/unpausing will not affect whether or not the feature is ENABLED globally.  If the feature is enabled and you pause it, the feature will stop working until you unpause.  If the feature is disabled and you pause it, there will be no change in functionality, even when you unpause.

##### armSwinger.armSwingingPaused = {True, False}
Prevents the player from arm swinging while true.

##### armSwinger.preventionsPaused = {True, False}
Pauses all prevention methods (Climbing, Falling, Instant, Wall Clip, etc) while true.

##### armSwinger.anglePreventionsPaused = {True, False}
Pauses all angle-based prevention methods (Climbing, Falling, Instant) while true.

##### armSwinger.wallClipPreventionPaused = {True, False}
Pauses wall clip prevention while true.

##### armSwinger.playAreaHeightAdjustmentPaused = {True, False}
Pauses play area height adjustment unconditionally.  When this is changed from true to false, the play area will immediately be adjusted to the ground.

## How does it work?
### ArmSwing Locomotion
Your movement is based on the movement and rotation of both controllers.  When ArmSwinger is activated by holding one or both grip buttons (depending on Swing Mode), the script translates any X/Y/Z controller movement into play space movement.  The speed is based on the movement of one or both controllers.  The direction is the direction that one or both controllers are pointing.

### Play Area vertical adjustment
The play area is constantly adjusted based on the position of the headset and the terrain underneath it.  Your physical floor will always match up with the terrain directly under your headset.  This occurs both when moving artificially and when moving physically.

This behavior can be overriden when not arm swinging with the Only Height Adjust While Arm Swinging feature.

### Out of Bounds
Out of Bounds can be triggered in multiple ways - if the headset tries to go through the terrain, if the player tries to climb a slope that is too steep, if the player tries to fall down a slope that is too steep, if the player tries to walk along a steep slope, or if the headset goes outside the world entirely.  These conditions are toggleable and tweakable.  Alternatively, Out of Bounds can be disabled entirely, leaving it to you to handle these conditions.

### Vertical Raycast
Play Area vertical adjustment and Out of Bounds detection are handled by a raycast shot from the middle of the headset, downwards in world space.  The raycast is shot once per frame.  This allows ArmSwinger to place the play area vertically based on where the ray hits.  ArmSwinger also compares this frame's raycast to the last frame's raycast to determine the angle of terrain the user is crossing (and trigger an Out of Bounds condition if appropriate).  These angles are cached over multiple frames and analyzed together before making a rewind decision, allowing the user to traverse small steep obstacles without triggering a rewind.

Prevent Wall Walking uses a second raycast shot just to the left (relative to the direction of movement) of the height raycast.  It then calculates the angle between the two raycast shots to determine if the player is wallwalking.  This raycast is only shot when an angle check is done, not every frame.

### Rewind
If you use one of the Out of Bounds Prevention features and the player goes Out of Bounds, ArmSwinger will rewind the player to a previous safe position.  Safe positions are cached as the player moves across "safe" ground.  By default, only ground that the player can fall down, climb up, and walk across is considered "safe" and is cached.  This prevents situations where you fall down a slope you can't then climb back up if you get stuck halfway down.

## Special Thanks
Special thanks to the following people for their exceptional contributions to ArmSwinger.

- [/u/simburger](https://www.reddit.com/user/simburger) - for running into walls continuously for multiple ArmSwinger versions

## Contact
Questions?  Comments?  Snide remarks?  Valentines cards?  Please send all inquiries to [Electric Night Owl](mailto:armswinger@electricnightowl.com) or contact us via [Twitter (@ElecNightOwl)](https://twitter.com/ElecNightOwl).  Hoot!
