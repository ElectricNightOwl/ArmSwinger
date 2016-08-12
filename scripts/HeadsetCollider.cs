using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadsetCollider : MonoBehaviour {

	/////////////////////
	// CLASS VARIABLES //
	/////////////////////

	//// Public variables ////
	public float headsetSphereColliderRadius;
	public bool inGeometry = false;

	//// Public objects ////

	//// Private variables ////
	private float minAngleToRewindDueToWallClip;

	//// Private objects ////
	private ArmSwinger armSwinger;

	private SphereCollider headsetSphereCollider;
	private Rigidbody headsetRigidbody;

	private LayerMask groundRayLayerMask;

	//////////////////////
	// INITIATILIZATION //
	//////////////////////
	void Start() {
		// Create a box collider for the headset if it doesn't already exist
		// The collider is a non-trigger
		headsetSphereCollider = GetComponent<SphereCollider>();
		if (!headsetSphereCollider) {
			headsetSphereCollider = this.gameObject.AddComponent<SphereCollider>();
			headsetSphereCollider.isTrigger = false;
			headsetSphereCollider.radius = headsetSphereColliderRadius;
		}
		else {
			if (headsetSphereCollider.isTrigger == true) {
				Debug.LogError("HeadsetCollider.Start():: There is already a sphere collider on your headset, but it is a trigger. Prevent Wall Clip will fail.");
			}

			Debug.LogWarning("HeadsetCollider.Start():: There is already a sphere collider on your headset.  Please ensure that it is an appropriate radius for Prevent Wall Clip to work.");
		}

		// Create a rigidbody for the headset if it doesn't already exit
		// The rigidbody is non-kinematic, and frozen for position and rotation
		// This was done to allow OnCollisionEnter's collision.point to work when detecting the normals of surfaces the headset runs into
		// If you have a better way to do this, please PLEASE submit it.  This solution makes me sad.
		headsetRigidbody = GetComponent<Rigidbody>();
		if (!headsetRigidbody) {
			headsetRigidbody = this.gameObject.AddComponent<Rigidbody>();
			headsetRigidbody.isKinematic = false;
			headsetRigidbody.useGravity = false;
			headsetRigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		else {
			if (headsetRigidbody.isKinematic == true) {
				Debug.LogError("HeadsetCollider.Start():: There is already a RigidBody on your headset, but it is kinematic.  Prevent Wall Clip will fail.");
			}
			if (headsetRigidbody.constraints != RigidbodyConstraints.FreezeAll) {
				Debug.LogWarning("HeadsetCollider.Start():: There is already a RigidBody on your headset, but it does not have all constraints frozen.  Physics with the headset may be erratic.");
			}
		}

		armSwinger = GameObject.FindObjectOfType<ArmSwinger>();

	}

	///////////////////
	// MONOBEHAVIOUR //
	///////////////////
	void OnCollisionEnter(Collision collision) {

        // Never collide with any SteamVR tracked objects
        if (collision.gameObject.GetComponent<SteamVR_TrackedObject>()) {
            return;
        }
        
        if (armSwinger.preventWallClip == false || armSwinger.wallClipPreventionPaused || armSwinger.preventionsPaused) {
			return;
		}

		// If we're already in geometry, unconditionally push back
		if (inGeometry) {
			armSwinger.triggerRewind(ArmSwinger.PreventionReason.HEADSET);
			armSwinger.wallClipThisFrame = true;
			return;
		}

		if (armSwinger.currentPreventionReason == ArmSwinger.PreventionReason.NONE || armSwinger.currentPreventionReason == ArmSwinger.PreventionReason.HEADSET) {
			if ((groundRayLayerMask.value & 1 << collision.gameObject.layer) != 0) {

				foreach (ContactPoint contactPoint in collision.contacts) {
					Vector3 normalOfCollisionPoint = contactPoint.normal;
					float angleOfCollisionPoint = Vector3.Angle(Vector3.up, normalOfCollisionPoint);

					if (angleOfCollisionPoint >= minAngleToRewindDueToWallClip) {
						inGeometry = true;
						armSwinger.triggerRewind(ArmSwinger.PreventionReason.HEADSET);
						armSwinger.wallClipThisFrame = true;
						return;
					}
				}
			}
		}
	}

	public void OnCollisionExit(Collision collision) {
		inGeometry = false;
	}
	
	/////////////
	// COMPUTE //
	/////////////

	/////////
	// GET //
	/////////

	/////////
	// SET //
	/////////
	public void setLayerMask(LayerMask newMask) {
		groundRayLayerMask = newMask;
	}

	public void setHeadsetSphereColliderRadius(float newRadius) {
		headsetSphereColliderRadius = newRadius;
	}

	public void setMinAngleWallClipForOOB(float newAngle) {
		minAngleToRewindDueToWallClip = newAngle;
	}

}

