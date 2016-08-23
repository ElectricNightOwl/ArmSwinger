using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class ArmSwinger : MonoBehaviour {

	/***** CLASS VARIABLES *****/

	/** Inspector Variables **/

	// ReadMe Location
	[Tooltip("For your reference only, doesn't affect script operation in any way.")]
	public string GithubProjectAndDocs = "https://github.com/ElectricNightOwl/ArmSwinger";

	// General Settings
	[Header("General Settings")]
	[Tooltip("General - Scale World Units To Camera Rig Scale\n\nBy default, several unit- and speed-based settings are in absolute world units regardless of CameraRig scale.  If this setting is true, all of those settings will be automatically scaled to match the X scale of this CameraRig.  If you use a non-default CameraRig scale, enabling this setting will allow you to specify all settings in meters-per-second in relation to the CameraRig rather than in world units.\n\n(Default: false)")]
	public bool generalScaleWorldUnitsToCameraRigScale = false;
	[Tooltip("General - Auto Adjust Fixed Timestep\n\nIn order for ArmSwinger to handle movement and wall collisions correctly, Time.fixedDeltaTime must be 0.0111 (90 per second) or less.  If this feature is enabled, the setting will be adjusted automatically if it is higher than 0.0111.  If disabled, an error will be generated but the value will not be changed.\n\n(Default: true)")]
	public bool generalAutoAdjustFixedTimestep = true;

	// Arm Swing Settings
	[Header("Arm Swing Settings")]
	[Tooltip("Arm Swing - Navigation\n\nEnables variable locomotion using the controllers to determine speed and direction.  Activated according to the selected Mode. \n\n(Default: true)")]
	public bool armSwingNavigation = true;
	[SerializeField]
	[Tooltip("Arm Swing - Mode\nOnly if Arm Swing Navigation is enabled\n\nDetermines what is necessary to activate arm swing locomotion, and what controller is used when determining speed/direction.\n\nBoth Grips Both Controllers - Activate by squeezing both grips.  Both controllers are used for speed/direction.\n\nLeft Grip Both Controllers - Activate by squeezing left grip.  Both controllers are used for speed/direction.\n\nRight Grip Both Controllers - Activate by squeezing right grip.  Both controllers are used for speed/direction.\n\nOne Grip Same Controller - Activate by squeezing either grip.  That controller is used for speed/direction.  Can be combined with the other controller.\n\nOne Grip Same Controller Exclusive - Activate by squeezing either grip.  That controller is used for speed/direction.  Squeezing the grip on the other controller will have no effect until the first controller grip is released.\n\n(Default: One Grip Same Controller)")]
	private ArmSwingMode _armSwingMode = ArmSwingMode.OneGripSameController;
	[Tooltip("Arm Swing - Controller To Movement Curve\nOnly if Arm Swing Navigation is enabled.\n\nCurve that determines how much a given controller change translates into camera rig movement.  The far left of the curve is no controller movement and no virtual movement.  The far right is Controller Speed For Max Speed (controller movement) and Max Speed (virtual momvement).\n\n(Default: Linear)")]
	public AnimationCurve armSwingControllerToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
	[SerializeField]
	[Tooltip("Arm Swing - Controller Speed For Max Speed\nOnly if Arm Swing Navigation is enabled\n\nThe number of CameraRig local units per second a controller needs to be moving to be considered going max speed.\n\n(Default:3)")]
	private float _armSwingControllerSpeedForMaxSpeed = 3f;
	[SerializeField]
	[Tooltip("Arm Swing - Max Speed\nOnly if Arm Swing Navigation is enabled\n\nThe fastest base speed (in world units) a player can travel when moving controllers at Controller Movement Per Second For Max Speed.  The actual max speed of the player will depend on the both/single controller coefficients you configure.\n\n(Default: 8)")]
	private float _armSwingMaxSpeed = 8;
	[Tooltip("Arm Swing - Both Controllers Coefficient\nOnly if Arm Swing Navigation is enabled and Swing Activation Mode allows both controllers to be used for arm swinging.\n\nUsed to boost or nerf the player's speed when using boths controllers for arm swinging.  A value of 1.0 will not modify the curve / max speed calculation.\n\n(Default: 1.0)")]
	[Range(0f, 10f)]
	[SerializeField]
	private float _armSwingBothControllersCoefficient = 1.0f;
	[Tooltip("Arm Swing - Single Controller Coefficient\nOnly if Arm Swing Navigation is enabled and Swing Activation Mode allows a single controller to be used for arm swinging.\n\nUsed to boost or nerf the player's speed when using a single controller for arm swinging.  A value of 1.0 will not modify the curve / max speed calculation.\n\n(Default:.7)")]
	[Range(0f, 10f)]
	[SerializeField]
	private float _armSwingSingleControllerCoefficient = .7f;

	// Controller Smoothing Settings
	[Header("Controller Smoothing Settings")]
	[Tooltip ("Controller Smoothing\n\nUses controller movement sampling to help eliminate jerks and unpleasant movement when controllers suddenly change position due to tracking inaccuracies.  It is highly recommended to turn this setting on if using movingInertia\n\n(Default: true)")]
	public bool controllerSmoothing = true;
	[Tooltip ("Controller Smoothing - Mode\nOnly if Controller Smoothing is enabled\n\nDetermines how controller smoothing calculates the smoothed movement value used by arm swinging.\n\nLowest\nUse the lowest value in the cache.  Should only be used with small cache sizes.\n\nAverage\nUse the average of all values in the cache.\n\nAverage Minus Highest\nUse the average of all values in the cache, but disregard the highest value.  When a controller jitters, the value change in that frame is almost always higher than normal values and will be discarded.\n\n(Default: Average Minus Highest)")]
	public ControllerSmoothingMode controllerSmoothingMode = ControllerSmoothingMode.AverageMinusHighest;
	[Range(2,90)]
	[Tooltip ("Controller Smoothing - Cache Size\nOnly if Controller Smoothing is enabled\n\nSets the number of calculated controller movements to keep in the cache.  Setting this number too low may allow a jittering controller to cause jerky movements for the player.  Setting this number too high increases lag time from controller movement to camera rig movement.\n\n(Default: 3)")]
	public int controllerSmoothingCacheSize = 3;

	// Inertia Settings
	[Header("Inertia Settings")]
	[SerializeField]
    [Tooltip("Moving Inertia\n\nSimulates inertia while arm swinging.  If the controllers change position slower than the moving inertia calculation, the inertia calculation will be used to determine forward movement.\n\n(Default: true)")]
	private bool _movingInertia = true;
	[Tooltip("Moving Inertia - Time To Stop At Max Speed\nOnly if Moving Inertia is enabled\n\nThe time it will take to go from armSwingMaxSpeed to 0 if arm swinging is engaged and the player does not move the controllers.  Speeds lower than armSwingMaxSpeed will scale their stopping time linearly.\n\n(Default: .5)")]
	public float movingInertiaTimeToStopAtMaxSpeed = .5f;
    [SerializeField]
    [Tooltip("Stopping Inertia\n\nSimulates inertia when arm swinging stops.\n\n(Default: true)")]
	private bool _stoppingInertia = true;
	[Tooltip("Stopping Inertia - Time To Stop At Max Speed\nOnly if Stopping Inertia is enabled\n\nThe time it will take to go from armSwingMaxSpeed to 0 when arm swinging is disengaged.  Speeds lower than armSwingMaxSpeed will scale their stopping time linearly.\n\n(Default:.35)")]
	public float stoppingInertiaTimeToStopAtMaxSpeed = .35f;

	// Raycast Settings
	[Header("Raycast Settings")]
	[Tooltip("Raycast - Ground Layer Mask\n\nLayers that ArmSwinger will consider 'the ground' when determining Y movement of the play space and when calculating angle-based prevention methods. \n\n(Default: Everything)")]
	public LayerMask raycastGroundLayerMask = -1;
	[SerializeField]
	[Tooltip("Raycast - Max Length\n\nThe length of the headset raycasts (in CameraRig local units) used for play height adjustment and falling/climbing prevention. Should be the value of the largest height difference you ever expect the player to come across.\n\n(Default: 100)")]
	private float _raycastMaxLength = 100f;
	[Range(2f, 90f)]
	[Tooltip("Raycast - Average Height Cache Size\n\nNumber of Raycasts to average together when determining where to place the play area.  These raycasts are done once per frame.  Lower numbers will make the play area moving feel more responsive.  Higher numbers will smooth out terrain bumps but may feel laggy.\n\n(Default: 3)")]
	[SerializeField]
	private int _raycastAverageHeightCacheSize = 3;
	[Tooltip("Raycast - Only Height Adjust While Arm Swinging\n\nWill prevent the camera rig height from being adjusted while the player is not Arm Swinging.  See the README on Github for more details.\n\n(Default:false)")]
	public bool raycastOnlyHeightAdjustWhileArmSwinging = false;

	// Prevent Wall Clip Settings
	[Header("Prevent Wall Clip Settings")]
	[Tooltip("Prevent Wall Clip\n\nPrevents players from putting their headset through walls and ground that are in the preventWallClipLayerMask list.\n\n(Default: true)")]
	[SerializeField]
	private bool _preventWallClip = true;
	[Tooltip("Prevent Wall Clip - Layer Mask\nOnly if Prevent Wall Clip is enabled\n\nLayers that ArmSwinger will consider 'walls' when determining if the headset has gone out of bounds.\n\n(Default: Everything)")]
	public LayerMask preventWallClipLayerMask = -1;
	[Tooltip("Prevent Wall Clip - Mode\nOnly if Prevent Wall Clip is enabled\n\nChanges how Prevent Wall Clip reacts when the player attempst to clip into a wall.\n\nRewind\nFade out, rewind rewindNumSavedPositionsToRewind postitions, fade back in.\n\nPush Back\nDo not allow the player to make the move.  Instead, adjust the position of the play area so that they cannot enter the wall.\n\n(Default: Push Back)")]
	public PreventionMode preventWallClipMode = PreventionMode.PushBack;
	[SerializeField]
	[Tooltip("Prevent Wall Clip - Headset Collider Radius\nOnly if Prevent Wall Clip is enabled\n\nSets the radius of the sphere collider used to detect the headset entering geometry.\n\n(Default: (.11f)")]
	private float _preventWallClipHeadsetColliderRadius = .11f;
	[Range(0f, 90f)]
	[Tooltip("Prevent Wall Clip - Min Angle To Trigger\nOnly if Prevent Wall Clip is enabled\n\nSets the minimum angle a \"wall\" should be in order to trigger Prevent Wall Clip if the headset collides with it.  0 is flat ground, 90 degree is a straight up wall.  This prevents rewinds from happening if the headset is placed on the physical floor and the headset collides with the virtual floor.\n\n(Default: 20)")]
	[SerializeField]
	private float _preventWallClipMinAngleToTrigger = 20f;

	// Prevent Climbing Settings
	[Header("Prevent Climbing Settings")]
	[Tooltip("Prevent Climbing\n\nPrevents the player from climbing walls and steep slopes.  \n\n(Default: true)")]
	public bool preventClimbing = true;
	[Range(0f, 90f)]
	[Tooltip("Prevent Climbing - Max Angle Player Can Climb\nOnly if Prevent Climbing is enabled\n\nThe maximum angle from the ground to the approached slope that a player can climb.  0 is flat ground, 90 is a vertical wall.  \n\n(Default: 45)")]
	[SerializeField]
	private float _preventClimbingMaxAnglePlayerCanClimb = 45f;

	// Prevent Falling Settings
	[Header("Prevent Falling Settings")]
	[Tooltip("Prevent Falling\n\nPrevents the player from falling down steep slopes.\n\n(Default: true)")]
	public bool preventFalling = true;
	[Range(0f, 90f)]
	[Tooltip("Prevent Falling - Max Angle Player Can Fall\nOnly if Prevent Falling is enabled\n\nThe maximum angle a player can try to descend.  0 is flat ground, 90 is a sheer cliff.  (Default: 60)")]
	[SerializeField]
	private float _preventFallingMaxAnglePlayerCanFall = 60f;

	// Prevent Wall Walking Settings
	[Header("Prevent Wall Walking Settings")]
	[Tooltip("Prevent Wall Walking\n\nPrevents the player from traversing across steep slopes.  Uses preventClimbingMaxAnglePlayerCanClimb when wall walking up, and preventClimbingMaxAnglePlayerCanClimb when wall walking down.\n\n(Default: true)")]
	public bool preventWallWalking = true;

	// Instant Height Settings
	[Header("Instant Height Settings")]
	[SerializeField]
	[Tooltip("Instant Height - Max Change\nOnly if Prevent Climbing / Falling or Only Height Adjust While Arm Swinging are enabled\n\nThe maximum height in world units a player can climb or descend in a single frame without triggering a rewind.  Allows climbing of steps this size or below, and prevents jumping over walls or falling off cliffs.  Also affects raycastOnlyHeightAdjustWhileArmSwinging.\n\n(Default: .2)")]
	private float _instantHeightMaxChange = .2f;
	[Tooltip("Instant Height - Climb Prevention Mode\nOnly if Prevent Climbing is enabled\n\nChanges how Prevent Climbing reacts when a player tries to instantly climb greater than instantHeightMaxChange.\n\nRewind\nFade out, rewind rewindNumSavedPositionsToRewind postitions, fade back in.\n\nPush Back\nDo not allow the player to make the move.  Instead, adjust the position of the play area so that they cannot climb the height.\n\n(Default: Push Back)")]
	public PreventionMode instantHeightClimbPreventionMode = PreventionMode.PushBack;
	[Tooltip("Instant Height - Fall Prevention Mode\nOnly if Prevent Falling is enabled\n\nChanges how Prevent Falling reacts when a player tried to instantly fall greater than instantHeightMaxChange.\n\nRewind\nFade out, rewind rewindNumSavedPositionsToRewind postitions, fade back in.\n\nPush Back\nDo not allow the player to make the move.  Instead, adjust the position of the play area so that they cannot fall down.\n\n(Default: Rewind)")]
	public PreventionMode instantHeightFallPreventionMode = PreventionMode.Rewind;

	// Check Settings
	[Header("Check Settings")]
	[Tooltip("Checks - Num Climb Fall Checks OOB Before Rewind\nOnly if Prevent Climbing / Falling is enabled\n\nThe number of checks in a row the player must be falling or climbing to trigger a rewind.  Checks are performed in sync with rewinds (rewindMinDistanceChangeToSavePosition).  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.\n\n(Default: 5)")]
	public int checksNumClimbFallChecksOOBBeforeRewind = 5;
	[Tooltip("Checks - Num Wall Walk Checks OOB Before Rewind\nOnly if Prevent Wall Walking is enabled\n\nThe number of checks in a row the player must be considered wall walking to trigger a rewind.  Checks are performed in sync with rewinds (rewindMinDistanceChangeToSavePosition).  Lower numbers will result in more false positives.  Higher numbers may allow the player to overcome the limits you set.\n\n(Default: 15)")]
	public int checksNumWallWalkChecksOOBBeforeRewind = 15;

	// Push Back Override Settings
	[Header("Push Back Override Settings")]
	[Tooltip("Push Back Override\nOnly if a Prevention method is using mode Push Back\n\nUses a token bucket system to determine if a player has been getting pushed back for too long.  Also helps players who have gotten stuck in geometry.  For more information, see the README file on GitHub.\n\n(Default: true)")]
	public bool pushBackOverride = true;
	[Tooltip("Push Back Override - Refill Per Sec\nOnly if Push Back Override is enabled\n\nThe amount of tokens that are added to the bucket every second.  The correct proportion of tokens are added each frame to add up to this number per second.\n\n(Default: 30)")]
	public float pushBackOverrideRefillPerSec = 30;
	[Tooltip("Push Back Override - Max Tokens\nOnly if Push Back Override is enabled\n\nThe maximum number of tokens in the bucket.  Additional tokens 'spill out' and are lost.\n\n(Default: 90")]
	public float pushBackOverrideMaxTokens = 90;

	// Rewind Settings
	[Header("Rewind Settings")]
	[SerializeField]
	[Tooltip("Rewind - Min Distance Change To Save Position\nOnly if a prevention method is enabled\n\nMinimum distance in world units that the player must travel to trigger another saved rewind position.\n\n(Default: .05)")]
	private float _rewindMinDistanceChangeToSavePosition = .05f;
	[Tooltip("Rewind - Dont Save Unsafe Climb Fall Positions\nOnly if both Prevent Climbing and Prevent Falling are enabled\n\nIf true, positions that can be climbed but not fallen down (or vice versa) won't be saved as rewind positions.  If false, the position will be saved anyways and the player might get stuck.\n\n(Default: true)")]
	public bool rewindDontSaveUnsafeClimbFallPositions = true;
	[Tooltip("Rewind - Dont Save Unsafe Wall Walk Positions\nOnly if Prevent Wall Walking is enabled\n\nIf true, positions that are considered wall walking but that haven't yet triggered a rewind won't be saved as possible rewind positions.  If false, the position will be saved anyways and the player might get stuck.\n\n(Default: true)")]
	public bool rewindDontSaveUnsafeWallWalkPositions = true;
	[Tooltip("Rewind - Num Positions To Store\nOnly if a prevention method is enabled\n\nThe number of saved positions to cache total.  Allows multiple consecutive rewinds to go even further back in time as necessary.  Must be higher than rewindNumSavedPositionsToRewind.\n\n(Default:28)")]
	public int rewindNumSavedPositionsToStore = 28;
	[Tooltip("Rewind - Num Positions To Rewind\nOnly if a prevention method is enabled\n\nThe number of saved positions to rewind when a player goes out of bounds and a rewind is triggered.\n\n(Default: 7)")]
	public int rewindNumSavedPositionsToRewind = 7;
	[Tooltip("Rewind - Fade Out Sec\nOnly if a prevention method is enabled\n\nTime in seconds to fade the player view OUT if a rewind is triggered.\n\n(Default: .15f)")]
	public float rewindFadeOutSec = .15f;
	[Tooltip("Rewind - Fade In Sec\nOnly if a prevention method is enabled\n\nTime in seconds to fade the player view IN once the player position is corrected.\n(Default: .35f)")]
	public float rewindFadeInSec = .35f;

	// Enums
	public enum ArmSwingMode {
		BothGripsBothControllers,
		LeftGripBothControllers,
		RightGripBothControllers,
		OneGripSameController,
		OneGripSameControllerExclusive
	};

	public enum PreventionReason { CLIMBING, FALLING, INSTANT_CLIMBING, INSTANT_FALLING, OHAWAS, HEADSET, WALLWALK, MANUAL, NO_GROUND, NONE };
	public enum PreventionMode { Rewind, PushBack };
	private enum PreventionCheckType { CLIMBFALL, WALLWALK };

	public enum ControllerSmoothingMode { Lowest, Average, AverageMinusHighest };

	// Informational public bools
	[HideInInspector]
	public bool outOfBounds = false;
	[HideInInspector]
	public bool rewindInProgress = false;
	[HideInInspector]
	public bool armSwinging = false;

	// Pause Variables
	[Header("Pause Variables")]
	[SerializeField]
	[Tooltip("Arm Swinging Paused\n\nPrevents the player from arm swinging while true.\n\n(Default: false)")]
	private bool _armSwingingPaused = false;
	[SerializeField]
	[Tooltip("Preventions Paused\n\nPauses all prevention methods (Climbing, Falling, Instant, Wall Clip, etc) while true.\n\n(Default: false)")]
	private bool _preventionsPaused = false;
	[SerializeField]
	[Tooltip("Angle Preventions Paused\n\nPauses all angle-based prevention methods (Climbing, Falling, Instant) while true.\n\n(Default: false)")]
	private bool _anglePreventionsPaused = false;
	[SerializeField]
	[Tooltip("Wall Clip Prevention Paused\n\nPauses wall clip prevention while true.\n\n(Default: false)")]
	private bool _wallClipPreventionPaused = false;
	[SerializeField]
	[Tooltip("Play Area Height Adjustment Paused\n\nPauses play area height adjustment unconditionally.  When this is changed from true to false, the play area will immediately be adjusted to the ground.\n\n(Default: false)")]
	private bool _playAreaHeightAdjustmentPaused = false;
	
	// Controller positions
	private Vector3 leftControllerLocalPosition;
	private Vector3 rightControllerLocalPosition;
	private Vector3 leftControllerPreviousLocalPosition;
	private Vector3 rightControllerPreviousLocalPosition;

	// Headset/Camera Rig saved position history
	private LinkedList<Vector3> headsetPreviousLocalPositions = new LinkedList<Vector3>();
	private Vector3 lastHeadsetLocalPositionSaved = new Vector3(0, 0, 0);
	private LinkedList<Vector3> cameraRigPreviousPositions = new LinkedList<Vector3>();
	private Vector3 lastCameraRigPositionSaved = new Vector3(0, 0, 0);
	private Vector3 previousAngleCheckHeadsetPosition;

	private int previousPositionSize = 5;
	private LinkedList<Vector3> previousCameraRigPositions = new LinkedList<Vector3>();
	private LinkedList<Vector3> previousHeadsetLocalPositions = new LinkedList<Vector3>();

	// RaycastHit histories
	private List<RaycastHit> headsetCenterRaycastHitHistoryPrevention = new List<RaycastHit>(); // History of headset center RaycastHits used for prevention checks
	private List<RaycastHit> headsetCenterRaycastHitHistoryHeight = new List<RaycastHit>(); // History of headset center RaycastHits used for height adjustments each frame
	private RaycastHit lastRaycastHitWhileArmSwinging; // The last every-frame headset center RaycastHit that was seen while the player was arm swinging

	// Prevention Reason histories
	private Queue<PreventionReason> climbFallPreventionReasonHistory = new Queue<PreventionReason>();
	private Queue<PreventionReason> wallWalkPreventionReasonHistory = new Queue<PreventionReason>();
	private PreventionReason _currentPreventionReason = PreventionReason.NONE;

	// Controller Movement Result History
	private LinkedList<float> controllerMovementResultHistory = new LinkedList<float>(); // The controller movement results after the Swing Mode calculations but before inertia and 1/2-hand coefficients

	// Saved angles
	private float latestCenterChangeAngle;
	private float latestSideChangeAngle;

	// Saved movement
	private float latestArtificialMovement;
	private Quaternion latestArtificialRotation;
	private float previousTimeDeltaTime = 0f;
	private Vector3 previousAngleCheckCameraRigPosition;

	// Inertia curves
	// WARNING: must be linear for now
	private AnimationCurve movingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));
	private AnimationCurve stoppingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));

	// Used for test scene only
	private bool _useNonLinearMovementCurve = true;
	private AnimationCurve inspectorCurve;

	//// Controller buttons ////
	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
	private bool leftGripButtonPressed = false;
	private bool rightGripButtonPressed = false;

	//// Controllers ////
	private SteamVR_ControllerManager controllerManager;
	private GameObject leftControllerGameObject;
	private GameObject rightControllerGameObject;
	private SteamVR_TrackedObject leftControllerTrackedObj;
	private SteamVR_TrackedObject rightControllerTrackedObj;
	private SteamVR_Controller.Device leftController;
	private SteamVR_Controller.Device rightController;
	private int leftControllerIndex;
	private int rightControllerIndex;

	// Wall Clip tracking
	[HideInInspector]
	public bool wallClipThisFrame = false;
	[HideInInspector]
	public bool rewindThisFrame = false;

	// Push back override
	private float pushBackOverrideValue;
	private bool pushBackOverrideActive = false;

	// One Grip Same Controller Exclusive mode only
	private GameObject activeSwingController = null;

	// Prevent Wall Clip's HeadsetCollider script
	private HeadsetCollider headsetCollider;

	// GameObjects
	private GameObject headsetGameObject;
	private GameObject cameraRigGameObject;

	// Camera Rig scaling
	private float cameraRigScaleModifier = 1.0f;
	
	/****** INITIALIZATION ******/
	void Awake() {

		// Find an assign components and objects
		controllerManager = this.GetComponent<SteamVR_ControllerManager>();
		leftControllerGameObject = controllerManager.left;
		rightControllerGameObject = controllerManager.right;
		leftControllerTrackedObj = leftControllerGameObject.GetComponent<SteamVR_TrackedObject>();
		rightControllerTrackedObj = rightControllerGameObject.GetComponent<SteamVR_TrackedObject>();

		headsetGameObject = GameObject.FindObjectOfType<SteamVR_Camera>().gameObject;
		cameraRigGameObject = GameObject.FindObjectOfType<SteamVR_ControllerManager>().gameObject;

		// Setup wall clipping on the headset gameobject, if enabled
		if (preventWallClip) {
			setupHeadsetCollider();
		}

		// Save the initial movement curve, in case it's switched off
		inspectorCurve = armSwingControllerToMovementCurve;

		// There is some variation in the degree measurement per-frame in Unity.  As such, we increase the maximum allowed angle
		// very slightly to provide consistent results when climbing/falling a surface that is exactly the user-set value.
		// Yes, this is lame.  Sorry.
		preventClimbingMaxAnglePlayerCanClimb += .005f;
		preventFallingMaxAnglePlayerCanFall += .005f;

		// Seed the initial previousLocalPositions
		leftControllerPreviousLocalPosition = leftControllerGameObject.transform.localPosition;
		rightControllerPreviousLocalPosition = rightControllerGameObject.transform.localPosition;

		seedSavedPositions();

		// Seed the initial saved position
		saveRewindPosition(true);
		previousAngleCheckCameraRigPosition = cameraRigGameObject.transform.position;
		lastRaycastHitWhileArmSwinging = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask);
		
		// Pre-seed previousTimeDeltaTime
		previousTimeDeltaTime = 1f/90f;

		// Refill Push Back Override completely
		pushBackOverrideValue = pushBackOverrideMaxTokens;

        // Verify and fix settings
        verifySettings();

    }

	/***** FIXED UPDATE *****/
	void FixedUpdate() {

		// Set scale as necessary (defaults to 1.0)
		// Doing this in Update() allows the Camera Rig to be scaled during runtime but keep the same ArmSwinger feel
		if (generalScaleWorldUnitsToCameraRigScale) {
			cameraRigScaleModifier = this.transform.localScale.x;
		}

		// Push Back Override
		if (pushBackOverride && !rewindInProgress) {
			incrementPushBackOverride();
		}

		// Store controller button states
		getControllerButtons();

		// reset this frame counters
		rewindThisFrame = false;
		wallClipThisFrame = false;

		// Check for wall clipping
		if (preventWallClip && headsetCollider.inGeometry) {
			triggerRewind(PreventionReason.HEADSET);
		}

		// Save the current controller positions for our use
		leftControllerLocalPosition = leftControllerGameObject.transform.localPosition;
		rightControllerLocalPosition = rightControllerGameObject.transform.localPosition;

		// Variable motion based on controller movement
		if (armSwingNavigation &&
			!outOfBounds &&
			!wallClipThisFrame &&
			!rewindThisFrame &&
			!armSwingingPaused &&
			!rewindInProgress) {
			transform.position += variableArmSwingMotion();
		}

		// Save the current controller positions for next frame
		leftControllerPreviousLocalPosition = leftControllerGameObject.transform.localPosition;
		rightControllerPreviousLocalPosition = rightControllerGameObject.transform.localPosition;

		// Only copy safe spots for push backs
		if (!outOfBounds && !wallClipThisFrame && !rewindThisFrame) {
			if ((previousCameraRigPositions.Last.Value != cameraRigGameObject.transform.position) ||
				(previousHeadsetLocalPositions.Last.Value != headsetGameObject.transform.localPosition)) {
				//Debug.Log(Time.frameCount + "|ArmSwinger.FixedUpdate():: Saving " + cameraRigGameObject.transform.position + headsetGameObject.transform.localPosition);

				savePosition(previousCameraRigPositions, cameraRigGameObject.transform.position, previousPositionSize);
				savePosition(previousHeadsetLocalPositions, headsetGameObject.transform.localPosition, previousPositionSize);
			}
		}

		// Adjust the camera rig height, and prevent climbing/falling as configured
		if (!wallClipThisFrame && !rewindThisFrame && !outOfBounds) {
			adjustCameraRig();
		}

		// Save this Time.deltaTime for next frame (inertia simulation)
		previousTimeDeltaTime = Time.deltaTime;
	}

    /***** VERIFY SETTINGS *****/
    void verifySettings() {

        // Camera Rig checking
        if (!this.GetComponent<SteamVR_ControllerManager>()) {
            Debug.LogError("ArmSwinger.verifySettings():: ArmSwinger is applied on a GameObject that is not a SteamVR CameraRig, or is a CameraRig without a SteamVR Controller Manager.  Please review the ArmSwinger instructions.  ArmSwinger will fail.");
        }

        // Rewind Settings
        if (rewindNumSavedPositionsToRewind > rewindNumSavedPositionsToStore) {
            Debug.LogError("ArmSwinger.verifySettings():: rewindNumSavedPositionsToRewind is greater than rewindNumSavedPositionsToStore, rewinding will fail.");
        }

        // Ground Ray Layer Mask and Controllers
        if (((raycastGroundLayerMask.value & 1 << leftControllerGameObject.layer) != 0) && leftControllerGameObject.GetComponent<Collider>()) {
            Debug.LogWarning("ArmSwinger.verifySettings():: raycastGroundLayerMask includes the layer for your left controller, which also has a collider on it.  The raycast from the center of the headset downwards will hit your controller if it gets in the way, causing false positive rewinds/pushbacks.  You should change raycastGroundLayerMask to not include the controller's layer, or change the controller to a different layer.");
        }
        if (((raycastGroundLayerMask.value & 1 << rightControllerGameObject.layer) != 0) && rightControllerGameObject.GetComponent<Collider>()) {
            Debug.LogWarning("ArmSwinger.verifySettings():: raycastGroundLayerMask includes the layer for your right controller, which also has a a collider on it.  The raycast from the center of the headset downwards will hit your controller if it gets in the way, causing false positive rewinds/pushbacks.  You should change raycastGroundLayerMask to not include the controller's layer, or change the controller to a different layer.");
        }

		// Check fixed time setting
		if (Time.fixedDeltaTime > 1f/90f) {
			if (generalAutoAdjustFixedTimestep) {
				Debug.LogWarning("ArmSwinger.verifySettings():: Fixed Timestep is set to " + Time.fixedDeltaTime + ".  Since you have generalAutoAdjustFixedTimestep set to true, ArmSwinger will auto adjust this value to " + 1f / 90f + " (90 steps per second) for you.");
				Time.fixedDeltaTime = 1f / 90f;
			} else {
				Debug.LogError("ArmSwinger.verifySettings():: Fixed Timestep is set to " + Time.fixedDeltaTime + ".  This will cause stuttering movement when arm swinging.  Consider changing your Fixed Timestep to " + 1f / 90f + " (90 steps per second) by going to Edit -> Project Settings -> Time -> Fixed Timestep.");
			}
		}
    }

    /***** CORE FUNCTIONS *****/
    // Variable Arm Swing locomotion
    Vector3 variableArmSwingMotion() {

		// Initialize movement variables
		float movementAmount = 0f;
		Quaternion movementRotation = Quaternion.identity;
		bool movedThisFrame = false;

		// if the player isn't ArmSwinging (or at least wasn't as of the last frame), clear the movement amount history
		if (!armSwinging) {
			controllerMovementResultHistory.Clear();
		}

		// Each swing activation mode has its own associated function
		switch (armSwingMode) {
			case ArmSwingMode.BothGripsBothControllers:
				movedThisFrame = swingBothGripsBothControllers(ref movementAmount, ref movementRotation);
				break;
			case ArmSwingMode.LeftGripBothControllers:
				movedThisFrame = swingLeftRightGripBothControllers(ref movementAmount, ref movementRotation);
				break;
			case ArmSwingMode.RightGripBothControllers:
				movedThisFrame = swingLeftRightGripBothControllers(ref movementAmount, ref movementRotation);
				break;
			case ArmSwingMode.OneGripSameController:
				movedThisFrame = swingOneGripSameController(ref movementAmount, ref movementRotation);
				break;
			case ArmSwingMode.OneGripSameControllerExclusive:
				movedThisFrame = swingOneGripSameControllerExclusive(ref movementAmount, ref movementRotation);
				break;
		}

		if (movedThisFrame) {
			// If raycastOnlyHeightAdjustWhileArmSwinging is enabled, check to see if the Y distance between the previous arm swinging position and
			// the current ArmSwinging position are higher than the instant height change max.  This ensures that players who have 
			// raycastOnlyHeightAdjustWhileArmSwinging enabled are not instantly teleported to the terrain when they start arm swinging.
			if (!armSwinging && raycastOnlyHeightAdjustWhileArmSwinging) {
				armSwinging = true;

				bool didStartSwingingRayHit;
				RaycastHit startSwingingRaycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didStartSwingingRayHit);
				
				if (didStartSwingingRayHit) {
					ohawasInstantHeightChangeCheck(startSwingingRaycastHit.point.y, lastRaycastHitWhileArmSwinging.point.y);
					// If we need to rewind, don't arm swing this frame
					if (currentPreventionReason != PreventionReason.NONE) {
						return Vector3.zero;
					}
				}
			}

			armSwinging = true;
			
			latestArtificialMovement = movementAmount;
			latestArtificialRotation = movementRotation;

			// Move forward in the X and Z axis only (no flying!)
			return getForwardXZ(movementAmount, movementRotation);

		}
		else {
			armSwinging = false;

			return Vector3.zero;
		}
	}

	// Arm Swing when armSwingMode is BothGripsBothControllers
	bool swingBothGripsBothControllers(ref float movement, ref Quaternion rotation) {
		if (leftGripButtonPressed && rightGripButtonPressed) {
			// The rotation is the average of the two controllers
			rotation = determineAverageControllerRotation();

			// Find the change in controller position since last Update()
			float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);
			float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

			// Calculate what camera rig movement the change should be converted to
			float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);
			float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

			// Controller movement is the average of the two controllers' change times the both controllers coefficient
			float controllerMovement = (leftMovement + rightMovement) / 2 * armSwingBothControllersCoefficient;

			// If movingInertia is enabled, the higher of inertia or controller movement is used
			if (movingInertia) {
				movement = movingInertiaOrControllerMovement(controllerMovement);
			}
			else {
				movement = controllerMovement;
			}

			return true;
		}
		// If stopping inertia is enabled
		else if (stoppingInertia && latestArtificialMovement != 0) {

			// The rotation is the cached one
			rotation = latestArtificialRotation;
			// The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
			movement = inertiaMovementChange(stoppingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

			return true;
		}
		else {
			return false;
		}
	}

	// Arm Swing when armSwingMode is LeftGripBothControllers or RightGripBothControllers
	bool swingLeftRightGripBothControllers(ref float movement, ref Quaternion rotation) {
		if (armSwingMode == ArmSwingMode.LeftGripBothControllers && leftGripButtonPressed ||
			armSwingMode == ArmSwingMode.RightGripBothControllers && rightGripButtonPressed) {

			// The rotation is the average of the two controllers
			rotation = determineAverageControllerRotation();

			// Find the change in controller position since last Update()
			float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);
			float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

			// Calculate what camera rig movement the change should be converted to
			float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);
			float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

			// Controller movement is the average of the two controllers' change times the both controllers coefficient
			float controllerMovement = (leftMovement + rightMovement) / 2 * armSwingBothControllersCoefficient;

			// If movingInertia is enabled, the higher of inertia or controller movement is used
			if (movingInertia) {
				movement = movingInertiaOrControllerMovement(controllerMovement);
			}
			else {
				movement = controllerMovement;
			}

			return true;
		}
		// If stopping inertia is enabled
		else if (stoppingInertia && latestArtificialMovement != 0) {

			// The rotation is the cached one
			rotation = latestArtificialRotation;
			// The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
			movement = inertiaMovementChange(stoppingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

			return true;
		}
		else {
			return false;
		}
	}

	// Arm Swing when armSwingMode is OneGripSameController
	bool swingOneGripSameController(ref float movement, ref Quaternion rotation) {
		if (leftGripButtonPressed && rightGripButtonPressed) {
			// The rotation is the average of the two controllers
			rotation = determineAverageControllerRotation();

			// Find the change in controller position since last Update()
			float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);
			float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

			// Calculate what camera rig movement the change should be converted to
			float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);
			float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

			// Both controllers are in use, so controller movement is the average of the two controllers' change times the both controller coefficient
			float controllerMovement = (leftMovement + rightMovement) / 2 * armSwingBothControllersCoefficient;

			// If movingInertia is enabled, the higher of inertia or controller movement is used
			if (movingInertia) {
				movement = movingInertiaOrControllerMovement(controllerMovement);
			}
			else {
				movement = controllerMovement;
			}

			return true;
		}
		else if (leftGripButtonPressed) {
			// The rotation is the rotation of the left controller
			rotation = leftControllerGameObject.transform.rotation;

			// Find the change in controller position since last Update()
			float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);

			// Calculate what camera rig movement the change should be converted to
			float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

			// controller movement is the change of the left controller times the single controller coefficient
			float controllerMovement = leftMovement * armSwingSingleControllerCoefficient;

			// If movingInertia is enabled, the higher of inertia or controller movement is used
			if (movingInertia) {
				movement = movingInertiaOrControllerMovement(controllerMovement);
			}
			else {
				movement = controllerMovement;
			}

			return true;
		}
		else if (rightGripButtonPressed) {
			// The rotation is the rotation of the right controller
			rotation = rightControllerGameObject.transform.rotation;

			// Find the change in controller position since last Update()
			float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

			// Calculate what camera rig movement the change should be converted to
			float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

			// controller movement is the change of the right controller times the single controller coefficient
			float controllerMovement = rightMovement * armSwingSingleControllerCoefficient;

			// If movingInertia is enabled, the higher of inertia or controller movement is used
			if (movingInertia) {
				movement = movingInertiaOrControllerMovement(controllerMovement);
			}
			else {
				movement = controllerMovement;
			}

			return true;
		}
		// If stopping inertia is enabled
		else if (stoppingInertia && latestArtificialMovement != 0) {

			// The rotation is the cached one
			rotation = latestArtificialRotation;
			// The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
			movement = inertiaMovementChange(stoppingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

			return true;
		}
		else {
			return false;
		}
	}

	// Arm Swing when armSwingMode is OneGripSameControllerExclusive
	bool swingOneGripSameControllerExclusive(ref float movement, ref Quaternion rotation) {

		// First, we clear the active controller if it's not being used anymore
		// If there is an active controller (previously defined), that active controller is the left controller, and the left grip is NOT pressed
		// OR
		// If there is an active controller (previously defined), that active controller is the right controller, and the right grip is NOT pressed
		if ((activeSwingController && activeSwingController == leftControllerGameObject && !leftGripButtonPressed) ||
			(activeSwingController && activeSwingController == rightControllerGameObject && !rightGripButtonPressed)) {
			activeSwingController = null;
		}

		// If there is no currently active swing controller and the left grip is pushed, make left controller active
		if (!activeSwingController && leftGripButtonPressed) {
			activeSwingController = leftControllerGameObject;
		}
		// If there is no currently active swing controller and the right grip is pushed, make right controller active
		else if (!activeSwingController && rightGripButtonPressed) {
			activeSwingController = rightControllerGameObject;
		}

		// If there is an active controller (either just set or previously set), do swinging operations on it
		if (activeSwingController) {
			// The rotation is the rotate of the active controller
			rotation = activeSwingController.transform.rotation;

			// The movement is the change of the active controller times the single controller coefficient
			if (activeSwingController == leftControllerGameObject) {
				// Find the change in controller position since last Update()
				float leftControllerChange = Vector3.Distance(leftControllerPreviousLocalPosition, leftControllerLocalPosition);

				// Calculate what camera rig movement the change should be converted to
				float leftMovement = calculateMovement(armSwingControllerToMovementCurve, leftControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

				// controller movement is the left controller movement times the single coefficient
				float controllerMovement = leftMovement * armSwingSingleControllerCoefficient;

				// If movingInertia is enabled, the higher of inertia or controller movement is used
				if (movingInertia) {
					movement = movingInertiaOrControllerMovement(controllerMovement);
				}
				else {
					movement = controllerMovement;
				}
			}
			else if (activeSwingController == rightControllerGameObject) {
				// Find the change in controller position since last Update()
				float rightControllerChange = Vector3.Distance(rightControllerPreviousLocalPosition, rightControllerLocalPosition);

				// Calculate what camera rig movement the change should be converted to
				float rightMovement = calculateMovement(armSwingControllerToMovementCurve, rightControllerChange, armSwingControllerSpeedForMaxSpeed, armSwingMaxSpeed);

				// controller movement is the right controller movement times the single coefficient
				float controllerMovement = rightMovement * armSwingSingleControllerCoefficient;

				// If movingInertia is enabled, the higher of inertia or controller movement is used
				if (movingInertia) {
					movement = movingInertiaOrControllerMovement(controllerMovement);
				}
				else {
					movement = controllerMovement;
				}
			}

			return true;
		}
		// If stopping inertia is enabled
		else if (stoppingInertia && latestArtificialMovement != 0) {

			// The rotation is the cached one
			rotation = latestArtificialRotation;
			// The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
			movement = inertiaMovementChange(stoppingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

			return true;
		}
		else {
			return false;
		}
	}

	float movingInertiaOrControllerMovement(float movement) {

		if (controllerSmoothing) {
			// Save the movement amount for moving inertia calculations
			saveFloatToLinkedList(controllerMovementResultHistory, movement, controllerSmoothingCacheSize);

			movement = smoothedControllerMovement(controllerMovementResultHistory);

		}

		float inertiaMovement = inertiaMovementChange(movingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, armSwingMaxSpeed, movingInertiaTimeToStopAtMaxSpeed);
				
		if (inertiaMovement >= movement) {
			return inertiaMovement;
		}
		else {
			return movement;
		}
	}

	// Using a linear curve, the movement last frame, the max speed we can go, and the time it should take to stop at that max speed,
	// compute the amount of movement that should happen THIS frame.  Note that timeToStopAtMaxSpeed is only if the player is going armSwingMaxSpeed.  If the
	// player is going LESS than armSwingMaxSpeed, the time to stop will be a percentage of timeToStopAtMaxSpeed.
	//
	// I tried implemeting this with custom curves, but ran into an issue where I couldn't determine where in the curve to start given the player's 
	// current speed.  For now, I'm making it linear-only, which works fine.  Would be amazing to make it work with arbitrary curves in the future.
	//
	// Also, I guarantee this can be done better.  I should have paid more attention in math.  Sorry, Mrs. Powell.

	// TODO: Make work with custom curves instead of linear only
	static float inertiaMovementChange(AnimationCurve curve, float latestMovement, float latestTimeDeltaTime, float maxSpeed, float timeToStopAtMaxSpeed) {

		// Max speed in Movement Per Frame
		float maxSpeedInMPF = maxSpeed * Time.deltaTime;

		// Frames per second
		float fps = 1 / Time.deltaTime;

		// The percentage through the curve the last movement was (based on the previous frame's Time.deltaTime)
		float percentThroughCurve = 1 - (latestMovement / (maxSpeed * latestTimeDeltaTime));

		// The percentage change in speed that needs to happen each frame based on the current frame rate
		float percentChangeEachFrame = 1 / (timeToStopAtMaxSpeed * fps);

		// Calculate the new percentage through the curve by adding the percent change each frame to the last percent
		float newPercentThroughCurve = percentThroughCurve + percentChangeEachFrame;

		// Evaluate the curve at the new percentage to determine how what percentage of armSwingMaxSpeed we should be going this frame
		float curveEval = curve.Evaluate(newPercentThroughCurve);

		// Set the movement value based on the evaluated curve, multiplied by the max Speed in Movement Per Frame
		float inertiaMovement = curveEval * maxSpeedInMPF;

		if (inertiaMovement <= 0f) {
			inertiaMovement = 0f;
		}

		return inertiaMovement;

	}

	// adjustCameraRig has two main functions, Prevent Climbing/Falling/Wall Walk/Wall Clip and adjustment of the play area height.
	// A raycast from the HMD downwards to the terrain is used to determine how far away the headset is from the ground.
	// That raycast is then compared to the previous raycast to determine the angle of the ground and if a prevention method
	// should be tripped.  Those decisions are collected, and when numChecksOOBBeforeRewind frames are in agreement, a rewind is
	// initiated.
	//
	// Second, the same raycast information is used to adjust the play area's Y coordinates so that the play space follows the
	// contours of the terrain as the player moves around.
	void adjustCameraRig() {

		if (outOfBounds) {
			return;
		}

		//// Center Headset Raycast ////
		bool didHeadsetCenterRayHit;
		RaycastHit headsetCenterRaycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didHeadsetCenterRayHit);
		
		// Save the raycast hit for averaging movement (unconditional)
		saveRaycastHit(headsetCenterRaycastHitHistoryHeight, headsetCenterRaycastHit, raycastAverageHeightCacheSize);

		// If there are not at least 2 RaycastHits in the history, add this one again
		while (headsetCenterRaycastHitHistoryHeight.Count < 2) {
			saveRaycastHit(headsetCenterRaycastHitHistoryHeight, headsetCenterRaycastHit, raycastAverageHeightCacheSize);
		}

		// Only if this Ray hits the ground
		if (didHeadsetCenterRayHit) {

			if (armSwinging) {
				lastRaycastHitWhileArmSwinging = headsetCenterRaycastHit;
			}

			//// Prevent Climbing/Falling/Wall Walking ////
			bool checkAnglesThisFrame = false;
			if ((preventClimbing || preventFalling || preventWallWalking) && !outOfBounds) {

				checkAnglesThisFrame = isDistanceFarEnough(previousAngleCheckHeadsetPosition, headsetGameObject.transform.position, rewindMinDistanceChangeToSavePosition);

				// Check for too much travel either up or down (climb a cliff in one bound, or fell off a cliff)
				// This is fired every frame
				RaycastHit previousHeadsetCenterRaycastHit = headsetCenterRaycastHitHistoryHeight[headsetCenterRaycastHitHistoryHeight.Count - 2];
				instantHeightChangeCheck(headsetCenterRaycastHit.point.y, previousHeadsetCenterRaycastHit.point.y);

				// Only fire angle-based checks if the player has travelled far enough since the last check
				// Uses the headset position to determine this
				// Only fires if we're not already out of bounds due to instantHeightChangeCheck
				if (checkAnglesThisFrame && !outOfBounds) {

					while (headsetCenterRaycastHitHistoryPrevention.Count < 2) {
						saveRaycastHit(headsetCenterRaycastHitHistoryPrevention, headsetCenterRaycastHit, 2);
					}

					saveRaycastHit(headsetCenterRaycastHitHistoryPrevention, headsetCenterRaycastHit, 2);

					// Check for Climbing/Falling too steeply
					centerPreventionCheck(headsetCenterRaycastHit, headsetCenterRaycastHitHistoryPrevention[headsetCenterRaycastHitHistoryPrevention.Count - 2]);

					// If Prevent Wall Walking is enabled, generate a second ray cast just to the side of the headset middle one.
					if (preventWallWalking && !outOfBounds) {

						// First, get the correct position of the side ray in 2D space
						Vector3 headsetSideRayOriginXZ = new Vector3();
						if (headsetCenterRaycastHitHistoryPrevention.Count >= 2) {
							headsetSideRayOriginXZ = determineSideRayOriginXZ(headsetCenterRaycastHitHistoryPrevention[1], headsetCenterRaycastHitHistoryPrevention[0], .001f);
						}
						else {
							headsetSideRayOriginXZ = headsetCenterRaycastHitHistoryPrevention[0].point;
						}

						// Raise the result to match the height of the headset
						Vector3 headsetSideRayOrigin = headsetSideRayOriginXZ + new Vector3(0f, headsetGameObject.transform.position.y, 0f);

						//// Left Headset Raycast ////
						// Shoot the Ray from the headset position, left (local) a smidge, and .1m above (local) the head, straight down (world)
						bool didHeadsetLeftRayHit;
						RaycastHit headsetSideRaycastHit = raycast(headsetSideRayOrigin, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didHeadsetLeftRayHit);
						sidePreventionCheck(headsetCenterRaycastHit, headsetSideRaycastHit);
					}

					// Save this position as the last one prevention checked
					previousAngleCheckHeadsetPosition = headsetGameObject.transform.position;
					previousAngleCheckCameraRigPosition = cameraRigGameObject.transform.position;

				}
			}

			if (checkAnglesThisFrame && !outOfBounds && !wallClipThisFrame && !rewindThisFrame) {
				// Save the current non-out-of-bounds headset and play area position
				saveRewindPosition();
			}

			if (!outOfBounds && !wallClipThisFrame && currentPreventionReason == PreventionReason.NONE && !playAreaHeightAdjustmentPaused) {
				if ((raycastOnlyHeightAdjustWhileArmSwinging && armSwinging) || (!raycastOnlyHeightAdjustWhileArmSwinging)) {
					if (headsetCenterRaycastHitHistoryHeight.Count > 0) {
						// Move the camera to adjust to the ground
						cameraRigGameObject.transform.position = new Vector3(
							cameraRigGameObject.transform.position.x,
							averageRaycastHitY(headsetCenterRaycastHitHistoryHeight),
							cameraRigGameObject.transform.position.z);
					}
				}
			}
		} else {
			triggerRewind(PreventionReason.NO_GROUND);
		}
	}

	PreventionReason instantHeightChangeReason(float thisYValue, float lastYValue) {

		float heightDifference = thisYValue - lastYValue;

		if (preventClimbing && heightDifference >= instantHeightMaxChange) {
			return PreventionReason.INSTANT_CLIMBING;
		}
		else if (preventFalling && heightDifference <= -instantHeightMaxChange) {
			return PreventionReason.INSTANT_FALLING;
		} else {
			return PreventionReason.NONE;
		}
	}

	void instantHeightChangeCheck(float thisYValue, float lastYValue) {

		if (preventionsPaused || anglePreventionsPaused) {
			return;
		}

		// We allow players to climb/descend stairs instantly (across one frame) as long as the stair is shorter than instantHeightMaxChange
		// If the current raycast and the previous raycast indicate a height difference larger than instantHeightMaxChange,
		// we know for sure that the player needs to be rewound.

		// This also prevents players from taking a long fall over a single frame (since multiple frames in a row need to agree to rewind normally)

		// Finally, this also affects raycastOnlyHeightAdjustWhileArmSwinging.  If the player moves around physically, and then starts Arm Swinging again,
		// we'll instantly check to see if they've changed more than instantHeightMaxChange.  If they have, we'll do a quick rewind to ensure
		// they start in a comfortable position

		PreventionReason preventionReason = instantHeightChangeReason(thisYValue, lastYValue);

		if (preventionReason != PreventionReason.NONE) {
			triggerRewind(preventionReason);
		}
	}

	// Instant Height Change Check for when raycastOnlyHeightAdjustWhileArmSwinging (OHAWAS) is enabled, and the player starts ArmSwinging
	void ohawasInstantHeightChangeCheck(float thisYValue, float lastYValue) {
		if (instantHeightChangeReason(thisYValue, lastYValue) != PreventionReason.NONE) {
			triggerRewind(PreventionReason.OHAWAS);
		}
	}

	// Adjusts the camera rig for raycastOnlyHeightAdjustWhileArmSwinging (OHAWAS) without triggering other Prevention mechanisms
	void ohawasCameraRigAdjust() {
		bool didRayHit;
		RaycastHit raycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didRayHit);

		if (didRayHit) {
			moveCameraRig(new Vector3(cameraRigGameObject.transform.position.x, raycastHit.point.y, cameraRigGameObject.transform.position.z), PreventionReason.OHAWAS);
			fadeIn();
		} else {
			fadeIn();
			outOfBounds = false;
		}
	}

	void centerPreventionCheck(RaycastHit thisRaycastHit, RaycastHit lastRaycastHit) {

		PreventionReason thisOOBReason = getOutOfBoundsReason(PreventionCheckType.CLIMBFALL, thisRaycastHit, lastRaycastHit);
		saveReason(climbFallPreventionReasonHistory, thisOOBReason, checksNumClimbFallChecksOOBBeforeRewind);

		if (thisOOBReason != PreventionReason.NONE) {
		}

		if (thisOOBReason != PreventionReason.NONE && !outOfBounds && climbFallPreventionReasonHistory.Count == checksNumClimbFallChecksOOBBeforeRewind) {
			bool allReasonsAgree = true;

			Queue<PreventionReason> checkQueue = climbFallPreventionReasonHistory;
			while (allReasonsAgree && checkQueue.Count > 0) {
				if (checkQueue.Dequeue() != thisOOBReason) {
					allReasonsAgree = false;
				}
			}

			if (allReasonsAgree) {
				triggerRewind(thisOOBReason);
			}
		}
	}

	void sidePreventionCheck(RaycastHit centerRaycastHit, RaycastHit sideRaycastHit) {
		PreventionReason thisOOBReason = getOutOfBoundsReason(PreventionCheckType.WALLWALK, centerRaycastHit, sideRaycastHit);

		// We save 2x the wall walk reasons required for a rewind
		saveReason(wallWalkPreventionReasonHistory, thisOOBReason, checksNumWallWalkChecksOOBBeforeRewind * 2);

		// Some logic explanation is needed here.  If a player is wall walking up and at a constant angle, using a cache of size numWallWalkChecksOOBBeforeRewind
		// works fine.  However, if the player periodically angles themselves so that a wall walk is not detected, all the reasons in the cache will NEVER agree.
		// To prevent this exploit, we do a form of sampling.  The wall walk history is 2*numWallWalkChecksOOBBeforeRewind, but only numWallWalkChecksOOBBeforeRewind
		// checks need to agree to trigger a rewind.  
		//
		// TODO: Make the 2x multiplier for wall walk check cache adjustable

		if (thisOOBReason != PreventionReason.NONE && !outOfBounds && wallWalkPreventionReasonHistory.Count >= checksNumWallWalkChecksOOBBeforeRewind) {
			int wallWalkCount = 0;

			// Examine the entire cache and count the number of wall walks
			Queue<PreventionReason> checkQueue = new Queue<PreventionReason>(wallWalkPreventionReasonHistory);

			while (wallWalkCount < checksNumWallWalkChecksOOBBeforeRewind && checkQueue.Count > 0) {
				if (checkQueue.Dequeue() == PreventionReason.WALLWALK) {
					wallWalkCount++;
				}
			}

			// If the number of wall walks is >= numWallWalkChecksOOBBeforeRewind, trigger a rewind
			if (wallWalkCount >= checksNumWallWalkChecksOOBBeforeRewind) {
				triggerRewind(thisOOBReason);
			}
		}
	}

	PreventionReason getOutOfBoundsReason(PreventionCheckType checkType, RaycastHit thisHit, RaycastHit lastHit) {

		if (currentPreventionReason == PreventionReason.HEADSET) {
			return PreventionReason.HEADSET;
		}
		else if (preventionsPaused || anglePreventionsPaused) {
			return PreventionReason.NONE;
		}

		//  |\
		//  | \
		// A|  \ C
		//  |   \
		//  |____\
		//     B
		float a = thisHit.point.y - lastHit.point.y;
		float b = Vector2.Distance(new Vector2(thisHit.point.x, thisHit.point.z), new Vector2(lastHit.point.x, lastHit.point.z));

		float angleOfChange = Mathf.Atan(a / b) * Mathf.Rad2Deg;

		// Climbing/falling checking
		if (checkType == PreventionCheckType.CLIMBFALL) {

			latestCenterChangeAngle = angleOfChange;

			// Prevent Climbing
			if (preventClimbing && angleOfChange > preventClimbingMaxAnglePlayerCanClimb && currentPreventionReason != PreventionReason.FALLING) {
				return PreventionReason.CLIMBING;
			}
			// Prevent Falling
			else if (preventFalling && angleOfChange < -preventFallingMaxAnglePlayerCanFall && currentPreventionReason != PreventionReason.CLIMBING) {
				return PreventionReason.FALLING;
			}
			else {
				return PreventionReason.NONE;
			}
		}
		// Prevent Wall Walking
		else if (checkType == PreventionCheckType.WALLWALK) {

			latestSideChangeAngle = angleOfChange;

			float cameraRigPositionDifferenceY = cameraRigGameObject.transform.position.y - previousAngleCheckCameraRigPosition.y;

			// If the camera rig is moving up, check against preventClimbingMaxAnglePlayerCanClimb
			// If the camera rig is moving down, check against preventFallingMaxAnglePlayerCanFall
			if ((cameraRigPositionDifferenceY >= .0001f && Mathf.Abs(angleOfChange) > preventClimbingMaxAnglePlayerCanClimb) ||
				(cameraRigPositionDifferenceY <= -.0001f && Mathf.Abs(angleOfChange) > preventFallingMaxAnglePlayerCanFall)) {
				if (Mathf.Abs(angleOfChange) > preventClimbingMaxAnglePlayerCanClimb) {
					return PreventionReason.WALLWALK;
				}
				else {
					return PreventionReason.NONE;
				}
			}
		}

		return PreventionReason.NONE;
	}

	public void triggerRewind(PreventionReason reason = PreventionReason.MANUAL) {

        currentPreventionReason = reason;

		if (reason == PreventionReason.HEADSET) {
			wallClipThisFrame = true;
		}

        //Debug.Log(Time.frameCount + "|ArmSwinger.triggerRewind():: Rewind triggered due to " + reason);

		if (!outOfBounds) {
			// Special handling for raycastOnlyHeightAdjustWhileArmSwinging (OHAWAS) events where the player walks into geometry and then starts arm swinging.
			if (reason == PreventionReason.OHAWAS) {
				outOfBounds = true;
				fadeOut();
				Invoke("ohawasCameraRigAdjust", rewindFadeOutSec);
			}
			// Everything else
			else {
				outOfBounds = true;

                // If the prevention mode is REWIND and a rewind isn't already pending - fade out, rewind, fade back in
                if (currentPreventionMode == PreventionMode.Rewind && !rewindInProgress) {
					rewindInProgress = true;
					fadeOut();
					Invoke("rewindPositionModeRewind", rewindFadeOutSec);
					Invoke("fadeIn", rewindFadeOutSec);
				}
				// Otherwise the mode is PushBack, so we instantly push back
				else {
					rewindPosition(PreventionMode.PushBack);
					if (pushBackOverride) {
						decrementPushBackOverride();
					}
				}
			}			
		}
	}

	// Helper function for the Invoke() in triggerRewind, since Invoke doesn't support any parameters in called functions
	void rewindPositionModeRewind() {
		rewindPosition(PreventionMode.Rewind);
	}

	void rewindPosition(PreventionMode preventionMode) {

		// Let other features know that a rewind occured this frame
		rewindThisFrame = true;

		// Reset all caches
		resetReasonHistory();
		resetRaycastHitHistory();
		latestArtificialMovement = 0f;
		latestArtificialRotation = Quaternion.identity;

		// The positions we'll be rewinding to
		Vector3 cameraRigPreviousPositionToRewindTo = new Vector3();
		Vector3 headsetPreviousPositionToRewindTo = new Vector3();

		// Determine what previous positions we need to rewind to
		determinePreviousPositionToRewindTo(ref cameraRigPreviousPositionToRewindTo, ref headsetPreviousPositionToRewindTo, preventionMode);

		//Debug.Log(Time.frameCount + "|ArmSwinger.rewindPosition():: Mode " + preventionMode + " to " + cameraRigPreviousPositionToRewindTo + headsetPreviousPositionToRewindTo);

		Vector3 newCameraRigPosition = calculateCameraRigRewindPosition(cameraRigPreviousPositionToRewindTo, headsetPreviousPositionToRewindTo, cameraRigGameObject.transform.position, headsetGameObject.transform.localPosition, preventionMode);

		cameraRigGameObject.transform.position = newCameraRigPosition;

		previousAngleCheckHeadsetPosition = headsetGameObject.transform.position;

		// Seed caches with the new, safe position
		seedRaycastHitHistory();
		seedSavedPositions();

		//resetOOB();
		if (preventionMode == PreventionMode.Rewind) {
			Invoke("resetOOB", rewindFadeInSec);
			rewindInProgress = false;
		}
		else if (preventionMode == PreventionMode.PushBack) {
			resetOOB();
		}
	}

	void determinePreviousPositionToRewindTo(ref Vector3 cameraRigPreviousPositionToRewindTo, ref Vector3 headsetPreviousPositionToRewindTo, PreventionMode preventionMode) {

		// There are two main branches of this function, rewind and push back
		// Push Back uses the immediately previous position
		// Rewind uses the previously-saved safe positions
		
		// PUSH BACK
		if (preventionMode == PreventionMode.PushBack) {

			// If the player is ArmSwinging, we need to push back farther in order to ensure their headset doesn't get stuck in the wall
			// So, we grab the oldest position in the cache
			if (armSwinging) {
				cameraRigPreviousPositionToRewindTo = previousCameraRigPositions.First.Value;
				headsetPreviousPositionToRewindTo = previousHeadsetLocalPositions.First.Value;

				// Replace all positions with the safe ones we just found
				seedSavedPositions(cameraRigPreviousPositionToRewindTo, headsetPreviousPositionToRewindTo);
			}
			// If the player is not ArmSwinging, they are physically moving and the second-most-recent position works fine.
			else {

				// The last (most recent) position stored is what got us in this mess to begin with
				previousCameraRigPositions.RemoveLast();
				previousHeadsetLocalPositions.RemoveLast();

				// The "last" position is now the one before this frame, so we know it's safe
				cameraRigPreviousPositionToRewindTo = previousCameraRigPositions.Last.Value;
				headsetPreviousPositionToRewindTo = previousHeadsetLocalPositions.Last.Value;

				// Note that we don't drain / re-seed the cache.  This will allow us to roll back even farther if necessary.
			}
		}
		// REWIND		
		// If the headset/rig caches have at least rewindNumSavedPositionsToRewind worth of positions in the cache
		else if (headsetPreviousLocalPositions.Count >= rewindNumSavedPositionsToRewind && cameraRigPreviousPositions.Count >= rewindNumSavedPositionsToRewind) {

			for (int trimCounter = 1; trimCounter < rewindNumSavedPositionsToRewind; trimCounter++) {
				headsetPreviousLocalPositions.RemoveLast();
				cameraRigPreviousPositions.RemoveLast();
			}

			headsetPreviousPositionToRewindTo = headsetPreviousLocalPositions.Last.Value;
			cameraRigPreviousPositionToRewindTo = cameraRigPreviousPositions.Last.Value;
		}
		// if the caches have less than rewindNumSavedPositionsToRewind postions, drain them to 1 and use that
		else {
			for (int trimCounter = 1; trimCounter < headsetPreviousLocalPositions.Count; trimCounter++) {
				headsetPreviousLocalPositions.RemoveLast();
				cameraRigPreviousPositions.RemoveLast();
			}

			headsetPreviousPositionToRewindTo = headsetPreviousLocalPositions.Last.Value;
			cameraRigPreviousPositionToRewindTo = cameraRigPreviousPositions.Last.Value;

		}
	}

	static Vector3 calculateCameraRigRewindPosition(Vector3 cameraRigPreviousPosition, Vector3 headsetPreviousLocalPosition, Vector3 cameraRigPosition, Vector3 headsetLocalPosition, PreventionMode preventionMode) {

		// We only care about the X/Z positioning of the headset
		Vector3 headsetPositionDifference = ArmSwinger.vector3XZOnly(headsetPreviousLocalPosition) - ArmSwinger.vector3XZOnly(headsetLocalPosition);

		Vector3 returnPosition = cameraRigPreviousPosition + headsetPositionDifference;

		return returnPosition;
	}

	PreventionMode calculateCurrentPreventionMode() {
		if (currentPreventionReason == PreventionReason.INSTANT_CLIMBING && instantHeightClimbPreventionMode == PreventionMode.PushBack ||
			currentPreventionReason == PreventionReason.INSTANT_FALLING && instantHeightFallPreventionMode == PreventionMode.PushBack ||
			currentPreventionReason == PreventionReason.HEADSET && preventWallClipMode == PreventionMode.PushBack) {

			// If Push Back Override is enabled and active, do a rewind instead of a push back
			// Also rewind if player is ArmSwinging
			//if ((pushBackOverride && pushBackOverrideActive && !rewindInProgress) ||
			//	armSwinging) {
			if (pushBackOverride && pushBackOverrideActive && !rewindInProgress) {
				return PreventionMode.Rewind;
			}
			else {
				return PreventionMode.PushBack;
			}
		}
		else {
			return PreventionMode.Rewind;
		}
	}

	void saveRewindPosition(bool force = false) {

		// Unconditionally save regardless of settings
		if (force) {
			cameraRigPreviousPositions.AddLast(cameraRigGameObject.transform.position);
			headsetPreviousLocalPositions.AddLast(headsetGameObject.transform.localPosition);
			lastCameraRigPositionSaved = cameraRigGameObject.transform.position;
			lastHeadsetLocalPositionSaved = headsetGameObject.transform.localPosition;
		}

		// If you're out of bounds, DEFINITELY don't want to save this position
		if (outOfBounds) {
			return;
		}

		Vector3 previousSavedPosInWorldUnits = lastCameraRigPositionSaved + lastHeadsetLocalPositionSaved;

		// If the distance between the previous saved position and the current position is too small, bail out 
		if (isDistanceFarEnough(previousSavedPosInWorldUnits, headsetGameObject.transform.position, rewindMinDistanceChangeToSavePosition) == false) {
			return;
		}

		// If both Prevent Falling and Climbing features are enabled, it is important that we don't save a position to the rewind queue that could 
		// get the player stuck (too steep to go down, too steep to climb back up).  So, only save rewind positions if the ground
		// underneath the player's feet is safe to both fall down and climb up.
		// Also, if Prevent Wall Walking is enabled, don't save positions that aren't okay to wall-walk on

		// start with the assumption that the position is safe
		// if we the features and options enabled below have us check and find an unsafe position, we'll change it then
		bool isPositionSafe = true;

		// if the headset is in geometry, don't save this position
		if (preventWallClip && headsetCollider.inGeometry) {
			isPositionSafe = false;
		}

		// only applies if both preventFalling and preventClimbing are enabled, and if the user wants to skip unsafe positions
		if (isPositionSafe && preventFalling && preventClimbing && rewindDontSaveUnsafeClimbFallPositions) {
			// if the angle is greater than we can climb, not safe
			// if angle is greater than we can fall, not safe
			if ((Mathf.Abs(latestCenterChangeAngle) > preventClimbingMaxAnglePlayerCanClimb) || (Mathf.Abs(latestCenterChangeAngle) > preventFallingMaxAnglePlayerCanFall)) {
				isPositionSafe = false;
			}
		}

		// only applies if the climb fall check above didn't find an unsafe position, if preventWallWalking is enabled, 
		// and if the user wants to skip unsafe positions
		if (isPositionSafe && preventWallWalking && rewindDontSaveUnsafeWallWalkPositions) {
			// if the current position is considered wall walking but the player hasn't been wall walking long enough to trigger a rewind,
			// this is an unsafe position

			// If the camera rig is moving up, compare to preventClimbingMaxAnglePlayerCanClimb
			if (cameraRigGameObject.transform.position.y > cameraRigPreviousPositions.Last.Value.y) {
				if (Mathf.Abs(latestSideChangeAngle) > preventClimbingMaxAnglePlayerCanClimb) {
					isPositionSafe = false;
				}
			}
			// If the camera rig is moving down, compare to preventFallingMaxAnglePlayerCanFall
			if (cameraRigGameObject.transform.position.y < cameraRigPreviousPositions.Last.Value.y) {
				if (Mathf.Abs(latestSideChangeAngle) > preventFallingMaxAnglePlayerCanFall) {
					isPositionSafe = false;
				}
			}
		}

		// if the assumption that the position is safe survives this long, it must be safe!
		if (isPositionSafe) {
			cameraRigPreviousPositions.AddLast(cameraRigGameObject.transform.position);
			headsetPreviousLocalPositions.AddLast(headsetGameObject.transform.localPosition);
			lastCameraRigPositionSaved = cameraRigGameObject.transform.position;
			lastHeadsetLocalPositionSaved = headsetGameObject.transform.localPosition;
		}

		// If the number of positions in the queue is > the number of rewind frames we store, pop the oldest stored position
		while (cameraRigPreviousPositions.Count > rewindNumSavedPositionsToStore) {
			cameraRigPreviousPositions.RemoveFirst();
		}
		while (headsetPreviousLocalPositions.Count > rewindNumSavedPositionsToStore) {
			headsetPreviousLocalPositions.RemoveFirst();
		}
	}

	void saveReason(Queue<PreventionReason> reasonQueue, PreventionReason reason, int maxListSize) {
		if (outOfBounds) {
			return;
		}

		// Store the reason
		reasonQueue.Enqueue(reason);

		// If the number of reasons in the queue is >= the number of rewind frames we store, pop the oldest stored reason
		while (reasonQueue.Count > maxListSize) {
			reasonQueue.Dequeue();
		}
	}

	void saveRaycastHit(List<RaycastHit> raycastHitList, RaycastHit raycastHit, int maxListSize) {
		// Store the raycast
		raycastHitList.Add(raycastHit);

		while (raycastHitList.Count > maxListSize) {
			raycastHitList.RemoveAt(0);
		}
	}

	void savePosition(LinkedList<Vector3> positionList, Vector3 position, int maxListSize) {
		// Store the position
		positionList.AddLast(position);

		while (positionList.Count > maxListSize) {
			positionList.RemoveFirst();
		}
	}

	void saveFloatToLinkedList(LinkedList<float> linkedList, float value, int maxListSize) {
		// Store the position
		linkedList.AddLast(value);
		
		while (linkedList.Count > maxListSize) {
			linkedList.RemoveFirst();
		}

	}

	void incrementPushBackOverride() {
		pushBackOverrideValue = pushBackOverrideValue + (pushBackOverrideRefillPerSec * Time.deltaTime);
		pushBackOverrideValue = Mathf.Clamp(pushBackOverrideValue, 0, pushBackOverrideMaxTokens);

		if (pushBackOverrideValue < 1) {
			pushBackOverrideActive = true;
		}
		else {
			pushBackOverrideActive = false;
		}
	}

	void decrementPushBackOverride() {
		pushBackOverrideValue = pushBackOverrideValue - 1;
		pushBackOverrideValue = Mathf.Clamp(pushBackOverrideValue, 0f, pushBackOverrideMaxTokens);

		if (pushBackOverrideValue < 1) {
			pushBackOverrideActive = true;
		}
	}



	/***** HELPER FUNCTIONS *****/

	// Sets the button variables each frame
	void getControllerButtons() {
		// Left
		int newLeftControllerIndex = (int) leftControllerTrackedObj.index;

		if (newLeftControllerIndex != -1 && newLeftControllerIndex != leftControllerIndex) {
			leftControllerIndex = newLeftControllerIndex;
			leftController = SteamVR_Controller.Input(leftControllerIndex);
		}

		if (newLeftControllerIndex != -1) {
			leftGripButtonPressed = leftController.GetPress(gripButton);
		}
		else {
			leftGripButtonPressed = false;
		}

		//Right
		int newRightControllerIndex = (int) rightControllerTrackedObj.index;

		if (newRightControllerIndex != -1 && newRightControllerIndex != rightControllerIndex) {
			rightControllerIndex = newRightControllerIndex;
			rightController = SteamVR_Controller.Input(rightControllerIndex);
		}

		if (newRightControllerIndex != -1) {
			rightGripButtonPressed = rightController.GetPress(gripButton);
		}
		else {
			rightGripButtonPressed = false;
		}
	}

	// Returns the average of two Quaternions
	Quaternion averageRotation(Quaternion rot1, Quaternion rot2) {
		return Quaternion.Slerp(rot1, rot2, 0.5f);
	}

	// Fade the screen to black
	void fadeOut() {
		// SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
		SteamVR_Fade.View(Color.black, rewindFadeOutSec);
#else
				SteamVR_Fade.View(Color.black, rewindFadeOutSec * .666f);
#endif
    }

    // Fade the screen back to clear
    void fadeIn() {
		// SteamVR_Fade is too fast in builds.  We compenstate for this here.
#if UNITY_EDITOR
		SteamVR_Fade.View(Color.clear, rewindFadeInSec);
#else
				SteamVR_Fade.View(Color.clear, rewindFadeInSec * .666f);
#endif
    }

    // Returns a Vector3 with only the X and Z components (Y is 0'd)
    public static Vector3 vector3XZOnly(Vector3 vec) {
		return new Vector3(vec.x, 0f, vec.z);
	}

	// Returns a forward vector given the distance and direction
	public static Vector3 getForwardXZ(float forwardDistance, Quaternion direction) {
		Vector3 forwardMovement = direction * Vector3.forward * forwardDistance;
		return vector3XZOnly(forwardMovement);
	}

	// Returns the average Y value of a RaycastHit list
	public static float averageRaycastHitY(List<RaycastHit> list) {

		float avgY = 0;

		foreach (RaycastHit item in list) {
			avgY += item.point.y;
		}

		avgY /= list.Count;

		return avgY;
	}

	// Returns the average rotation of the two controllers
	Quaternion determineAverageControllerRotation() {
		// Build the average rotation of the controller(s)
		Quaternion newRotation;

		// Both controllers are present
		if (leftController != null && rightController != null) {
			newRotation = averageRotation(leftControllerGameObject.transform.rotation, rightControllerGameObject.transform.rotation);
		}
		// Left controller only
		else if (leftController != null && rightController == null) {
			newRotation = leftControllerGameObject.transform.rotation;
		}
		// Right controller only
		else if (rightController != null && leftController == null) {
			newRotation = rightControllerGameObject.transform.rotation;
		}
		// No controllers!
		else {
			newRotation = Quaternion.identity;
		}

		return newRotation;
	}

	float smoothedControllerMovement(LinkedList<float> controllerMovementHistory) {

		// Chose the lowest value in the cache
		if (controllerSmoothingMode == ControllerSmoothingMode.Lowest) {
			float low = controllerMovementHistory.First.Value;
						
			foreach (float val in controllerMovementHistory) {
				if (val < low) {
					low = val;
				}
			}

			return low;
		}
		// Compute the average of all values in the cache
		else if (controllerSmoothingMode == ControllerSmoothingMode.Average) {
			float total = 0;

			foreach (float val in controllerMovementHistory) {
				total += val;
			}

			return (total / controllerMovementHistory.Count);
		}
		// Compute the average of all values in the cache, but throw out the highest one
		// Functions the same as "Lowest" if the cache size is 2
		else if (controllerSmoothingMode == ControllerSmoothingMode.AverageMinusHighest) {
            // If the controllerMovementHistory has a length of 1, just return that value
            if (controllerMovementHistory.Count == 1) {
                return controllerMovementHistory.First.Value;
            }

            float high = controllerMovementHistory.First.Value;
			float total = 0;

			foreach (float val in controllerMovementHistory) {
				total += val;
				if (val > high) {
					high = val;
				}
			}
                        
            return ((total - high) / (controllerMovementHistory.Count - 1));
		}
		else {
			Debug.LogError("ArmSwinger.smoothedControllerMovement():: Invalid value for controllerSmoothingMode!");
			return 0;
		}			
	}

	float linkedListAverage(LinkedList<float> linkedList) {
		float sum = 0;
		foreach (float val in linkedList) {
			sum += val;
		}

		return sum / linkedList.Count;

	}

	// Thanks to cjdev from "http://answers.unity3d.com/questions/564166/how-to-find-perpendicular-line-in-2d.html"
	Vector3 determineSideRayOriginXZ(RaycastHit thisRaycastHit, RaycastHit lastRaycastHit, float offsetDistance) {

		Vector2 p1 = new Vector2(thisRaycastHit.point.x, thisRaycastHit.point.z);
		Vector2 p2 = new Vector2(lastRaycastHit.point.x, lastRaycastHit.point.z);

		Vector2 v = p2 - p1;

		Vector2 p3 = new Vector2(-v.y, v.x) / Mathf.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2)) * -offsetDistance;
		p3 += p1;

		Vector3 sideRayOrigin = new Vector3(p3.x, 0f, p3.y);

		return sideRayOrigin;
	}

	bool isDistanceFarEnough(Vector3 position1, Vector3 position2, float minDistance) {
		return (Vector3.Distance(position1, position2) >= minDistance);
	}

	// Adds the Collider script to the headset if it doesn't already exist
	void setupHeadsetCollider() {
		headsetCollider = headsetGameObject.GetComponent<HeadsetCollider>();
		if (!headsetCollider) {
			headsetCollider = headsetGameObject.AddComponent<HeadsetCollider>();

		}
		headsetCollider.setLayerMask(preventWallClipLayerMask);
		headsetCollider.setHeadsetSphereColliderRadius(preventWallClipHeadsetColliderRadius);
		headsetCollider.setMinAngleWallClipForOOB(preventWallClipMinAngleToTrigger);
	}

	static float calculateMovement(AnimationCurve curve, float change, float maxInput, float maxSpeed) {
		float changeInWUPS = change / Time.deltaTime;
		float movement = Mathf.Lerp(0, maxSpeed, curve.Evaluate(changeInWUPS / maxInput)) * Time.deltaTime;

		return movement;
	}

	RaycastHit raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask, out bool didRaycastHit) {
		Ray ray = new Ray(origin, direction);
		RaycastHit raycastHit;
		didRaycastHit = Physics.Raycast(ray, out raycastHit, maxDistance, layerMask);

		return raycastHit;
	}

	RaycastHit raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask) {
		bool dummyBool;
		return raycast(origin, direction, maxDistance, layerMask, out dummyBool);
	}

	/***** CLEANUP *****/
	void resetOOB() {
		if (!rewindInProgress) {
			currentPreventionReason = PreventionReason.NONE;
			outOfBounds = false;
		}
		if (pushBackOverrideActive) {
			pushBackOverrideValue = pushBackOverrideMaxTokens;
		}
	}

	void resetReasonHistory() {
		climbFallPreventionReasonHistory.Clear();
		wallWalkPreventionReasonHistory.Clear();
	}

	void resetRaycastHitHistory() {
		headsetCenterRaycastHitHistoryHeight.Clear();
		headsetCenterRaycastHitHistoryPrevention.Clear();
	}

	void resetSavedPositions() {
		previousCameraRigPositions.Clear();
		previousCameraRigPositions.Clear();
	}

	void seedRaycastHitHistory() {
		bool didRaycastHit = false;
		RaycastHit raycastHit = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask, out didRaycastHit);

		if (didRaycastHit) {
			saveRaycastHit(headsetCenterRaycastHitHistoryHeight, raycastHit, 1);
		}
	}

	void seedSavedPositions() {
		for (int count = 0; count < previousPositionSize; count++) {
			savePosition(previousCameraRigPositions, cameraRigGameObject.transform.position, previousPositionSize);
			savePosition(previousHeadsetLocalPositions, headsetGameObject.transform.localPosition, previousPositionSize);
		}
	}

	void seedSavedPositions(Vector3 cameraRigPosition, Vector3 headsetLocalPosition) {
		for (int count = 0; count < previousPositionSize; count++) {
			savePosition(previousCameraRigPositions, cameraRigPosition, previousPositionSize);
			savePosition(previousHeadsetLocalPositions, headsetLocalPosition, previousPositionSize);
		}
	}


	void resetRewindPositions() {
		cameraRigPreviousPositions.Clear();
		headsetPreviousLocalPositions.Clear();
	}

	/***** PUBLIC FUNCTIONS *****/
	// Moves the camera to another world position without a rewind
	// Also resets all caches and saved variables to prevent false OOB events
	// Allows other scripts and ArmSwinger mechanisms to artifically move the player without a rewind happening
	public void moveCameraRig(Vector3 newPosition, PreventionReason reason = PreventionReason.MANUAL) {
		outOfBounds = true;
		currentPreventionReason = reason;

		// Reset all caches
		resetReasonHistory();
		resetRaycastHitHistory();
		latestArtificialMovement = 0f;
		latestArtificialRotation = Quaternion.identity;

		resetRewindPositions();
		resetSavedPositions();

		cameraRigGameObject.transform.position = newPosition;

		seedRaycastHitHistory();
		seedSavedPositions();

		outOfBounds = false;
		currentPreventionReason = PreventionReason.NONE;

		saveRewindPosition(true);
		if (raycastOnlyHeightAdjustWhileArmSwinging) {
			lastRaycastHitWhileArmSwinging = raycast(headsetGameObject.transform.position, Vector3.down, raycastMaxLength, raycastGroundLayerMask);
		}

		// If raycastOnlyHeightAdjustWhileArmSwinging is enabled or if play area height adjustment is paused, we need to force an adjustment of the camera rig
		if (raycastOnlyHeightAdjustWhileArmSwinging || playAreaHeightAdjustmentPaused) {
			adjustCameraRig();
		}
	}

	public Vector3 getHeadsetLocalPosition() {
		return headsetGameObject.transform.localPosition;
	}

	public Vector3 getCameraRigPosition() {
		return cameraRigGameObject.transform.position;
	}

	/***** GET SET *****/
	public float armSwingBothControllersCoefficient {
		get {
			return _armSwingBothControllersCoefficient;
		}

		set {
			float min = 0f;
			float max = 10f;

			if (value >= min && value <= max) {
				_armSwingBothControllersCoefficient = value;
			}
			else {
				Debug.LogWarning("ArmSwinger:armSwingBothControllersCoefficient:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
			}
		}

	}

	public float armSwingSingleControllerCoefficient {
		get {
			return _armSwingSingleControllerCoefficient;
		}

		set {
			float min = 0f;
			float max = 10f;

			if (value >= min && value <= max) {
				_armSwingSingleControllerCoefficient = value;
			}
			else {
				Debug.LogWarning("ArmSwinger:armSwingSingleControllerCoefficient:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
			}
		}
	}

	public int raycastAverageHeightCacheSize {
		get {
			return _raycastAverageHeightCacheSize;
		}

		set {
			int min = 2;
			int max = 90;

			if (value >= min && value <= max) {
				_raycastAverageHeightCacheSize = value;
			}
			else {
				Debug.LogWarning("ArmSwinger:raycastAverageHeightCacheSize:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
			}
		}
	}

	public bool preventWallClip {
		get {
			return _preventWallClip;
		}

		set {
			_preventWallClip = value;
			if (_preventWallClip) {
				setupHeadsetCollider();
			}
		}
	}

	public float preventWallClipMinAngleToTrigger {
		get {
			return _preventWallClipMinAngleToTrigger;
		}

		set {
			float min = 0f;
			float max = 90f;

			if (value >= min && value <= max) {
				_preventWallClipMinAngleToTrigger = value;
				headsetCollider.setMinAngleWallClipForOOB(value);
			}
			else {
				Debug.LogWarning("ArmSwinger:preventWallClipMinAngleToTrigger:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
			}
		}
	}

	public float preventClimbingMaxAnglePlayerCanClimb {
		get {
			return _preventClimbingMaxAnglePlayerCanClimb;
		}

		set {
			float min = 0f;
			float max = 90f;

			if (value >= min && value <= max) {
				_preventClimbingMaxAnglePlayerCanClimb = value;
			}
			else {
				Debug.LogWarning("ArmSwinger:preventClimbingMaxAnglePlayerCanClimb:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
			}
		}
	}

	public float preventFallingMaxAnglePlayerCanFall {
		get {
			return _preventFallingMaxAnglePlayerCanFall;
		}

		set {
			float min = 0f;
			float max = 90f;

			if (value >= min && value <= max) {
				_preventFallingMaxAnglePlayerCanFall = value;
			}
			else {
				Debug.LogWarning("ArmSwinger:preventFallingMaxAnglePlayerCanFall:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
			}
		}
	}

	public PreventionReason currentPreventionReason {
		get {
			return _currentPreventionReason;
		}

		set {
			_currentPreventionReason = value;
		}
	}

	public bool useNonLinearMovementCurve {
		get {
			return _useNonLinearMovementCurve;
		}
		set {
			if (value) {
				armSwingControllerToMovementCurve = inspectorCurve;
			}
			else {
				armSwingControllerToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
			}

			_useNonLinearMovementCurve = value;
		}
	}

	public bool preventionsPaused {
		get {
			return _preventionsPaused;
		}
		set {
			// If the pause is being set to false, use moveCameraRig to reset and seed necessary caches
			if (value == false) {
				moveCameraRig(cameraRigGameObject.transform.position);
			}
			_preventionsPaused = value;
		}
	}

	public bool anglePreventionsPaused {
		get {
			return _anglePreventionsPaused;
		}
		set {
			// If the pause is being set to false, use moveCameraRig to reset and seed necessary caches
			if (value == false) {
				moveCameraRig(cameraRigGameObject.transform.position);
			}
			_anglePreventionsPaused = value;
		}
	}

	public bool wallClipPreventionPaused {
		get {
			return _wallClipPreventionPaused;
		}
		set {
			_wallClipPreventionPaused = value;
		}
	}

	public bool playAreaHeightAdjustmentPaused {
		get {
			return _playAreaHeightAdjustmentPaused;
		}
		set {
			// If the pause is being set to false, use moveCameraRig to reset and seed necessary caches
			if (value == false) {
				moveCameraRig(cameraRigGameObject.transform.position);
			}
			_playAreaHeightAdjustmentPaused = value;
		}
	}

	public bool armSwingingPaused {
		get {
			return _armSwingingPaused;
		}
		set {
			// If the pause is being set to false, reset the cached movement values so that unexpected inertia does not occur
			if (value == false) {
				latestArtificialMovement = 0f;
				latestArtificialRotation = Quaternion.identity;
			}
			_armSwingingPaused = value;
		}
	}

	public PreventionMode currentPreventionMode {
		get {
			return calculateCurrentPreventionMode();
		}
	}

	public float armSwingControllerSpeedForMaxSpeed {
		get {
			return _armSwingControllerSpeedForMaxSpeed;
		}
		set {
			_armSwingControllerSpeedForMaxSpeed = value;
		}
	}

	public float armSwingMaxSpeed {
		get {
			return _armSwingMaxSpeed * cameraRigScaleModifier;
		}
		set {
			_armSwingMaxSpeed = value;
		}
	}

	public float raycastMaxLength {
		get {
			return _raycastMaxLength * cameraRigScaleModifier;
		}
		set {
			_raycastMaxLength = value;
		}
	}

	public float preventWallClipHeadsetColliderRadius {
		get {
			return _preventWallClipHeadsetColliderRadius;
		}
		set {
			_preventWallClipHeadsetColliderRadius = value;
		}
	}

	public float instantHeightMaxChange {
		get {
			return _instantHeightMaxChange * cameraRigScaleModifier;
		}
		set {
			_instantHeightMaxChange = value;
		}
	}

	public float rewindMinDistanceChangeToSavePosition {
		get {
			return _rewindMinDistanceChangeToSavePosition * cameraRigScaleModifier;
		}
		set {
			_rewindMinDistanceChangeToSavePosition = value;
		}
	}

	public ArmSwingMode armSwingMode {
		get {
			return _armSwingMode;
		}
		set {
			controllerMovementResultHistory.Clear();
			_armSwingMode = value;
		}
    }

    public bool movingInertia {
		get {
			return _movingInertia;
		}
		set {
			latestArtificialMovement = 0f;
			_movingInertia = value;
		}
	}

	public bool stoppingInertia {
		get {
			return _stoppingInertia;
		}
		set {
			latestArtificialMovement = 0f;
			_stoppingInertia = value;
		}
	}
}
