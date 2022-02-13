﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.Examples;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FightingPlayerController : MonoBehaviour
{

	//for some reason Vector2.down doesn't exist in most Unity versions
	static Vector2 _v2down = new Vector2(0, -1);

	public enum ActionState { IDLE, CROUCH, WALK, RUN, JUMP, FALL, WALLSLIDE, SLIDE, ATTACK, DOWNATTACK, UPATTACK }

	public ActionState state
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state != value)
			{
				_state = value;
			}
		}
	}
	private ActionState _state;

	[Header("Input")]
	public float deadZone = 0.05f;
	public float runThreshhold = 0.5f;

	[Header("Raycasting")]
	public LayerMask characterMask;
	[HideInInspector]
	public LayerMask currentMask;
	public LayerMask groundMask;
	public LayerMask passThroughMask;

	[Header("Speeds & Timings")]
	public float walkSpeed = 1;
	public float runSpeed = 5;
	/// <summary>
	/// Initial jump velocity
	/// </summary>
	public float jumpSpeed = 12;
	/// <summary>
	/// Double and beyond jump velocity
	/// </summary>
	public float airJumpSpeed = 10;
	/// <summary>
	/// Jump speed when goomba stomping something
	/// </summary>
	public float headBounceSpeed = 16;
	/// <summary>
	/// How long you can hold onto the jump button to increase jump height
	/// </summary>
	public float jumpDuration = 0.5f;
	/// <summary>
	/// Horizontal speed when wall jumping
	/// </summary>
	public float wallJumpXSpeed = 3;
	/// <summary>
	/// How long a slide lasts
	/// </summary>
	public float slideDuration = 0.5f;
	/// <summary>
	/// How fast a slide is
	/// </summary>
	public float slideVelocity = 6;
	/// <summary>
	/// Y Scale of the Primary Collider during a slide
	/// </summary>
	public float slideSquish = 0.6f;
	/// <summary>
	/// How much velocity to apply during a punch animation when XVelocity Event is fired.
	/// </summary>
	public float punchVelocity = 7;
	/// <summary>
	/// How much velocity to apply during an uppercut when YVelocity Event is fired
	/// </summary>
	public float uppercutVelocity = 5;
	/// <summary>
	/// Downward velocity for duration of Down Attack
	/// </summary>
	public float downAttackVelocity = 20;
	/// <summary>
	/// How long it takes for a punch combo to timeout and go back to idle
	/// </summary>
	public float attackWatchdogDuration = 0.5f;
	/// <summary>
	/// How long to wait before falling off of a wall if no input
	/// </summary>
	public float wallSlideWatchdogDuration = 10f;
	/// <summary>
	/// Fall speed limit when wall sliding
	/// </summary>
	public float wallSlideSpeed = -2;
	/// <summary>
	/// How many times you can jump (1 = no double jumps)
	/// </summary>
	[Range(1, 3)]
	public int maxJumps = 1;

	[Header("Physics")]
	/// <summary>
	/// Additional fall gravity to feel more platformy
	/// </summary>
	public float fallGravity = -4;
	/// <summary>
	/// Friction applied when idle
	/// </summary>
	public float idleFriction = 20;
	/// <summary>
	/// Friction applied when moving
	/// </summary>
	public float movingFriction = 0;


	[Header("Animations")]
	[SpineAnimation]
	public string walkAnim;
	[SpineAnimation]
	public string runAnim;
	[SpineAnimation]
	public string idleAnim;
	[SpineAnimation]
	public string balanceBackward;
	[SpineAnimation]
	public string balanceForward;
	[SpineAnimation]
	public string jumpAnim;
	[SpineAnimation]
	public string fallAnim;
	[SpineAnimation]
	public string wallSlideAnim;
	[SpineAnimation]
	public string slideAnim;
	[SpineAnimation]
	public string attackAnim;
	[SpineAnimation]
	public string downAttackAnim;
	//the frame to skip to when a Down Attack impacts a solid ground
	public float downAttackFrameSkip;
	[SpineAnimation]
	public string upAttackAnim;
	//this animation is used strictly to turn off all attacking bounding box attachment based hitboxes
	[SpineAnimation]
	public string clearAttackAnim;
	[SpineAnimation]
	public string crouchAnim;

	[Header("Sounds")]
	public string footstepSound;
	public string landSound;
	public string jumpSound;
	public string slideSound;

	[Header("References")]
	public PolygonCollider2D primaryCollider;
	public SkeletonAnimation skeletonAnimation;
	public SkeletonGhost skeletonGhost;
	public GameObject groundJumpPrefab;
	public GameObject airJumpPrefab;
	[SpineBone(dataField: "skeletonAnimation")]
	public string footEffectBone;
	public GameObject downAttackPrefab;

	public Rigidbody2D RB
	{
		get
		{
			return this.rb;
		}
	}

	//input injection
	public System.Action<FightingPlayerController> HandleInput;

	//events
	public System.Action<Transform> OnFootstep;
	public System.Action<Transform> OnJump;

	Rigidbody2D rb;
	PhysicsMaterial2D characterColliderMaterial;
	Vector2 moveStick;
	bool doJump;
	bool jumpPressed;
	float jumpStartTime;
	bool doSlide;
	float slideStartTime;
	bool doPassthrough;
	//OneWayPlatform passThroughPlatform;
	//MovingPlatform movingPlatform;
	bool onIncline = false;

	bool attackWasPressed;
	bool waitingForAttackInput;
	float attackWatchdog;
	float airControlLockoutTime = 0;
	float wallSlideWatchdog;
	float wallSlideStartTime;
	bool wallSlideFlip;
	bool wasWallJump;
	int jumpCount = 0;
	bool upAttackUsed;
	bool downAttackRecovery = false;
	float downAttackRecoveryTime = 0;


	bool flipped;
	bool velocityLock;

	Vector3 backGroundCastOrigin;
	Vector3 centerGroundCastOrigin;
	Vector3 forwardGroundCastOrigin;
	Vector3 wallCastOrigin;
	float wallCastDistance;

	//box 2d workarounds
	float savedXVelocity;

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}

	//public override void IgnoreCollision(Collider2D collider, bool ignore)
	//{
	//	Physics2D.IgnoreCollision(primaryCollider, collider, ignore);
	//}

	void Start()
	{
		if (skeletonGhost == null)
			skeletonGhost = skeletonAnimation.GetComponent<SkeletonGhost>();

		rb = GetComponent<Rigidbody2D>();

		skeletonAnimation.state.Event += HandleEvent;
		skeletonAnimation.state.Complete += HandleComplete;

		CalculateRayBounds(primaryCollider);

		if (primaryCollider.sharedMaterial == null)
			characterColliderMaterial = new PhysicsMaterial2D("CharacterColliderMaterial");
		else
			characterColliderMaterial = Instantiate(primaryCollider.sharedMaterial);

		primaryCollider.sharedMaterial = characterColliderMaterial;

		currentMask = groundMask;
	}

	//Calculate where the collision rays should be based on a polygon collider
	void CalculateRayBounds(PolygonCollider2D coll)
	{
		Bounds b = coll.bounds;
		Vector3 min = transform.InverseTransformPoint(b.min);
		Vector3 center = transform.InverseTransformPoint(b.center);
		Vector3 max = transform.InverseTransformPoint(b.max);

		backGroundCastOrigin.x = min.x;
		backGroundCastOrigin.y = min.y + 0.1f;

		centerGroundCastOrigin.x = center.x;
		centerGroundCastOrigin.y = min.y + 0.1f;

		forwardGroundCastOrigin.x = max.x;
		forwardGroundCastOrigin.y = min.y + 0.1f;

		wallCastOrigin = center;
		wallCastDistance = b.extents.x + 0.1f;
	}

	//Handle Spine Animation Complete callbacks (only to clear attack states)
	void HandleComplete(Spine.TrackEntry entry)
	{
		if (entry.Animation.Name == attackAnim || entry.Animation.Name == upAttackAnim)
		{
			//attack complete
			skeletonAnimation.AnimationName = idleAnim;
			this.state = ActionState.IDLE;
		}
	}

	//Handle Spine Event callback
	//Used to apply various keyed effects, velocity augmentations, sounds, so on.
	//void HandleEvent (Spine.AnimationState state, int trackIndex, Spine.Event e) {
	void HandleEvent(TrackEntry entry, Spine.Event e)
	{
		if (entry != null)
		{
			if (entry.Animation.Name == attackAnim || entry.Animation.Name == upAttackAnim)
			{
				switch (e.Data.Name)
				{
					case "XVelocity":
						Vector2 velocity = rb.velocity;
						velocity.x = flipped ? -punchVelocity * e.Float : punchVelocity * e.Float;
						rb.velocity = velocity;
						break;
					case "YVelocity":
						velocity = rb.velocity;
						velocity.y = uppercutVelocity * e.Float;
						rb.velocity = velocity;
						break;
					case "Pause":
						attackWatchdog = attackWatchdogDuration;
						waitingForAttackInput = true;
						entry.TimeScale = 0;
						break;
				}
			}
			else if (entry.Animation.Name == downAttackAnim)
			{
				switch (e.Data.Name)
				{
					case "YVelocity":
						Vector2 velocity = rb.velocity;
						velocity.y = downAttackVelocity * e.Float;
						rb.velocity = velocity;
						break;
					case "Pause":
						velocityLock = e.Int == 1 ? true : false;
						break;
				}
			}

			switch (e.Data.Name)
			{
				case "Footstep":
					if (OnFootstep != null)
						OnFootstep(transform);
					break;
				case "Sound":
//					SoundPalette.PlaySound(e.String, 1f, 1, transform.position);
					break;
				case "Effect":
					switch (e.String)
					{
						case "GroundJump":
							if (groundJumpPrefab && OnGround)
								SpawnAtFoot(groundJumpPrefab, Quaternion.identity, new Vector3(flipped ? -1 : 1, 1, 1));
							break;
					}
					break;
			}
		}
	}

	//Graphics loop
	void Update()
	{
		if (HandleInput != null)
			HandleInput(this);

		UpdateAnim();
	}

	//Physics loop
	void FixedUpdate()
	{
		HandlePhysics();
	}

	public void Input(Vector2 moveStick, bool JUMP_isPressed, bool JUMP_wasPressed, bool SLIDE_wasPressed, bool ATTACK_wasPressed)
	{
		if (((OnGround) && state < ActionState.JUMP) || (state == ActionState.FALL && jumpCount < maxJumps))
		{
			if (!jumpPressed)
			{

				if (JUMP_wasPressed && moveStick.y < -0.25f)
				{
					doJump = true;
					jumpPressed = true;

				}
				else
				{
					doJump = JUMP_wasPressed;
					if (doJump)
					{
						jumpPressed = true;
					}
				}
			}

			if (!doJump && !doSlide)
			{
				if (state < ActionState.JUMP)
				{
					doSlide = SLIDE_wasPressed;
				}
			}
		}
		else if (OnGround && state == ActionState.SLIDE)
		{
			doJump = JUMP_wasPressed;
			if (doJump)
			{
				if (skeletonGhost != null)
					skeletonGhost.ghostingEnabled = false;
				jumpPressed = true;
			}
		}
		else if (state == ActionState.WALLSLIDE)
		{
			doJump = JUMP_wasPressed;
			if (doJump)
			{
				jumpPressed = true;
			}
		}

		this.moveStick = moveStick;
		attackWasPressed = ATTACK_wasPressed;
		jumpPressed = JUMP_isPressed;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Handles.Label(transform.position, state.ToString());
		if (!Application.isPlaying)
			return;

		if (OnGround)
			Gizmos.color = Color.green;
		else
			Gizmos.color = Color.grey;

		Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));

		Gizmos.DrawWireSphere(transform.TransformPoint(centerGroundCastOrigin), 0.07f);
		Gizmos.DrawWireSphere(transform.TransformPoint(backGroundCastOrigin), 0.07f);
		Gizmos.DrawWireSphere(transform.TransformPoint(forwardGroundCastOrigin), 0.07f);

		Gizmos.DrawLine(transform.TransformPoint(centerGroundCastOrigin), transform.TransformPoint(centerGroundCastOrigin + new Vector3(0, -0.15f, 0)));
		Gizmos.DrawLine(transform.TransformPoint(backGroundCastOrigin), transform.TransformPoint(backGroundCastOrigin + new Vector3(0, -0.15f, 0)));
		Gizmos.DrawLine(transform.TransformPoint(forwardGroundCastOrigin), transform.TransformPoint(forwardGroundCastOrigin + new Vector3(0, -0.15f, 0)));
	}
#endif

	public bool OnGround
	{
		get
		{
			return CenterOnGround || BackOnGround || ForwardOnGround;
		}
	}

	bool BackOnGround
	{
		get
		{
			return GroundCast(flipped ? forwardGroundCastOrigin : backGroundCastOrigin);
		}
	}

	bool ForwardOnGround
	{
		get
		{
			return GroundCast(flipped ? backGroundCastOrigin : forwardGroundCastOrigin);
		}
	}

	bool CenterOnGround
	{
		get
		{
			return GroundCast(centerGroundCastOrigin);
		}
	}

	//Detect being on top of a characters's head
	Rigidbody2D OnTopOfCharacter()
	{
		Rigidbody2D character = GetRelevantCharacterCast(centerGroundCastOrigin, 0.15f);
		if (character == null)
			character = GetRelevantCharacterCast(backGroundCastOrigin, 0.15f);
		if (character == null)
			character = GetRelevantCharacterCast(forwardGroundCastOrigin, 0.15f);

		return character;
	}

	//Raycasting stuff
	Rigidbody2D GetRelevantCharacterCast(Vector3 origin, float dist)
	{
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.TransformPoint(origin), _v2down, dist, characterMask);
		if (hits.Length > 0)
		{
			int index = 0;

			if (hits[0].rigidbody == rb)
			{
				if (hits.Length == 1)
					return null;

				index = 1;
			}
			if (hits[index].rigidbody == rb)
				return null;

			var hit = hits[index];
			if (hit.collider != null && hit.collider.attachedRigidbody != null)
			{
				return hit.collider.attachedRigidbody;
			}
		}

		return null;
	}

	//see if character is on the ground
	//throw onIncline flag
	bool GroundCast(Vector3 origin)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.TransformPoint(origin), _v2down, 0.15f, currentMask);
		if (hit.collider != null && !hit.collider.isTrigger)
		{
			if (hit.normal.y < 0.4f)
				return false;
			else if (hit.normal.y < 0.95f)
				onIncline = true;

			return true;
		}

		return false;
	}

	//cache x velocity to ensure speed restores after heavy impact that results in physics penalties
	void HandlePhysics()
	{
		onIncline = false;

		float x = moveStick.x;
		float y = moveStick.y;

		float absX = Mathf.Abs(x);
		float xVelocity = 0;
		float platformXVelocity = 0;
		float platformYVelocity = 0;
		Vector2 velocity = rb.velocity;


		if (doJump && state != ActionState.JUMP)
		{
			
			velocity.y = (jumpCount > 0 ? airJumpSpeed : jumpSpeed) + (platformYVelocity >= 0 ? platformYVelocity : 0);
			jumpStartTime = Time.time;
			state = ActionState.JUMP;
			doJump = false;
			if (airJumpPrefab != null && jumpCount > 0)
				Instantiate(airJumpPrefab, transform.position, Quaternion.identity);
			else if (groundJumpPrefab != null && jumpCount == 0)
			{
				if (wasWallJump)
				{
					SpawnAtFoot(groundJumpPrefab, Quaternion.Euler(0, 0, velocity.x >= 0 ? 90 : -90), new Vector3(velocity.x >= 0 ? 1 : -1, -1, 1));
				}
				else
				{
					SpawnAtFoot(groundJumpPrefab, Quaternion.identity, new Vector3(flipped ? -1 : 1, 1, 1));
				}

			}
			jumpCount++;

			if (OnJump != null)
				OnJump(transform);

		}
		else if (doSlide)
		{
			//SoundPalette.PlaySound(slideSound, 1, 1, transform.position);
			slideStartTime = Time.time;
			state = ActionState.SLIDE;
			SetFriction(movingFriction);
			doSlide = false;
			velocity.x = flipped ? -slideVelocity : slideVelocity;
			savedXVelocity = velocity.x;
			velocity.x += platformXVelocity;
			primaryCollider.transform.localScale = new Vector3(1, slideSquish, 1);
			IgnoreCharacterCollisions(true);

			if (skeletonGhost != null)
				skeletonGhost.ghostingEnabled = true;
		}

		//ground logic
		if (state < ActionState.JUMP)
		{
			if (OnGround)
			{
				jumpCount = 0;
				upAttackUsed = false;
				if (absX > runThreshhold)
				{
					xVelocity = runSpeed * Mathf.Sign(x);
					velocity.x = Mathf.MoveTowards(velocity.x, xVelocity + platformXVelocity, Time.deltaTime * 15);
					state = ActionState.RUN;
					SetFriction(movingFriction);
				}
				else if (absX > deadZone)
				{
					xVelocity = walkSpeed * Mathf.Sign(x);
					velocity.x = Mathf.MoveTowards(velocity.x, xVelocity + platformXVelocity, Time.deltaTime * 25);

					state = ActionState.WALK;
					SetFriction(movingFriction);
				}
				else if (y < -0.25f)
				{
					platformXVelocity = Mathf.MoveTowards(velocity.x, 0, Time.deltaTime * 10);
					velocity.x = platformXVelocity;

					state = ActionState.CROUCH;
					SetFriction(idleFriction);
				}
				else
				{
					platformXVelocity = Mathf.MoveTowards(velocity.x, 0, Time.deltaTime * 10);
					velocity.x = platformXVelocity;
					state = ActionState.IDLE;
					SetFriction(movingFriction); // or idle friction
				}

				if (attackWasPressed)
				{
					if (moveStick.y < 0.5f && state != ActionState.CROUCH)
					{
						state = ActionState.ATTACK;
						skeletonAnimation.AnimationName = attackAnim;
					}
					else
					{
						state = ActionState.UPATTACK;
						skeletonAnimation.AnimationName = upAttackAnim;
						upAttackUsed = true;
					}
				}
			}
			else
			{
				SetFallState(true);
			}
			//air logic
		}
		else if (state == ActionState.JUMP)
		{
			float jumpTime = Time.time - jumpStartTime;
			savedXVelocity = velocity.x;
			if (!jumpPressed || jumpTime >= jumpDuration || downAttackRecovery)
			{
				jumpStartTime -= jumpDuration;

				if (velocity.y > 0)
					velocity.y = Mathf.MoveTowards(velocity.y, 0, Time.deltaTime * 30);

				if (velocity.y <= 0 || (jumpTime < jumpDuration && OnGround))
				{
					if (!downAttackRecovery)
					{
						SetFallState(false);
					}
					else
					{
						downAttackRecovery = false;
					}

				}
			}

			//fall logic
		}
		else if (state == ActionState.FALL)
		{

			if (OnGround)
			{
//				SoundPalette.PlaySound(landSound, 1, 1, transform.position);
				if (absX > runThreshhold)
				{
					velocity.x = savedXVelocity;
					state = ActionState.RUN;
				}
				else if (absX > deadZone)
				{
					velocity.x = savedXVelocity;
					state = ActionState.WALK;
				}
				else
				{
					velocity.x = savedXVelocity;
					state = ActionState.IDLE;
				}
			}
			else
			{
				EnemyBounceCheck(ref velocity);
				savedXVelocity = velocity.x;

			}
			//wall slide logic
		}

		//air control
		if (state == ActionState.JUMP || state == ActionState.FALL)
		{
			if (Time.time > airControlLockoutTime)
			{
				if (absX > runThreshhold)
				{
					velocity.x = Mathf.MoveTowards(velocity.x, runSpeed * Mathf.Sign(x), Time.deltaTime * 8);
				}
				else if (absX > deadZone)
				{
					velocity.x = Mathf.MoveTowards(velocity.x, walkSpeed * Mathf.Sign(x), Time.deltaTime * 8);
				}
				else
				{
					velocity.x = Mathf.MoveTowards(velocity.x, 0, Time.deltaTime * 8);
				}



			}
			else
			{
				if (wasWallJump)
				{
					//cancel air control lockout if reverse joystick
					if (absX > deadZone)
					{
						airControlLockoutTime = Time.time - 1;
					}
				}
			}

			if (attackWasPressed && moveStick.y < -0.5f)
			{
				velocity.x = 0;
				velocity.y = 0;
				state = ActionState.DOWNATTACK;
				upAttackUsed = true;
				skeletonAnimation.AnimationName = downAttackAnim;
			}
			else if (attackWasPressed && moveStick.y > 0.5f)
			{
				if (!upAttackUsed)
				{
					state = ActionState.UPATTACK;
					skeletonAnimation.AnimationName = upAttackAnim;
					upAttackUsed = true;
					velocity.y = 1;
				}
			}

			if (state == ActionState.JUMP || state == ActionState.FALL)
			{
				if (Time.time != jumpStartTime)
				{
					if (Mathf.Abs(rb.velocity.x) > 0.1f || (state == ActionState.FALL && absX > deadZone))
					{
						if (!wasWallJump && state == ActionState.JUMP)
						{
							//dont do anything if still going up
						}
						else
						{
							//start wall slide
							state = ActionState.WALLSLIDE;
							jumpCount = 0;
							wallSlideWatchdog = wallSlideWatchdogDuration;
							wallSlideStartTime = Time.time;
							upAttackUsed = false;
							if (Mathf.Abs(rb.velocity.x) > 0.1)
							{
								wallSlideFlip = rb.velocity.x > 0;
							}
							else
							{
								wallSlideFlip = x > 0;
							}
						}


					}

				}
			}
		}

		//falling and wallslide
		if (state == ActionState.FALL)
			velocity.y += fallGravity * Time.deltaTime;

		//slide control
		if (state == ActionState.SLIDE)
		{
			float slideTime = Time.time - slideStartTime;

			if (slideTime > slideDuration)
			{
				primaryCollider.transform.localScale = Vector3.one;
				IgnoreCharacterCollisions(false);
				if (skeletonGhost != null)
					skeletonGhost.ghostingEnabled = false;
				state = ActionState.IDLE;
			}
			else
			{
				x = Mathf.Sign(savedXVelocity);
				velocity.x = savedXVelocity + platformXVelocity;
			}

			if (!OnGround)
			{
				//Fell off edge while sliding
				primaryCollider.transform.localScale = Vector3.one;
				IgnoreCharacterCollisions(false);
				if (skeletonGhost != null)
					skeletonGhost.ghostingEnabled = false;
				SetFallState(true);
			}
		}

		//attack control
		if (state == ActionState.ATTACK)
		{
			if (attackWasPressed)
			{
				attackWasPressed = false;

				//check if animation allows input now
				if (waitingForAttackInput)
				{
					waitingForAttackInput = false;
					skeletonAnimation.state.GetCurrent(0).TimeScale = 1;
				}
			}

			//apply some of the moving platform velocity
			velocity.x = Mathf.MoveTowards(velocity.x, platformXVelocity, Time.deltaTime * 8);

			//combo is paused, set idle mode and run watchdog
			if (waitingForAttackInput)
			{
				SetFriction(idleFriction);
				attackWatchdog -= Time.deltaTime;
				//cancel combo
				if (attackWatchdog < 0)
					state = ActionState.IDLE;
			}
			else
			{
				//attacking, set moving mode
				SetFriction(movingFriction);
			}
		}

		//generic motion flipping control
		if (state < ActionState.ATTACK && state != ActionState.WALLSLIDE)
		{
			if (Time.time > airControlLockoutTime)
			{
				if (x > deadZone)
					skeletonAnimation.Skeleton.FlipX = false;
				else if (x < -deadZone)
					skeletonAnimation.Skeleton.FlipX = true;
			}
			else
			{
				if (velocity.x > deadZone)
					skeletonAnimation.Skeleton.FlipX = false;
				else if (velocity.x < deadZone)
					skeletonAnimation.Skeleton.FlipX = true;
			}

		}

		//down attack
		if (state == ActionState.DOWNATTACK)
		{
			//recovering from down attack
			if (downAttackRecovery)
			{
				//time elapsed, jump back to feet using JUMP state
				if (downAttackRecoveryTime <= 0)
				{
//					SoundPalette.PlaySound(jumpSound, 1, 1, transform.position);
					velocity.y = jumpSpeed + (platformYVelocity >= 0 ? platformYVelocity : 0);
					jumpStartTime = Time.time;
					state = ActionState.JUMP;
					doJump = false;
					jumpPressed = false;
				}
				//wait for a bit
				else
				{
					downAttackRecoveryTime -= Time.deltaTime;
					velocity = Vector2.zero;
				}
			}
			else
			{
				//Has impacted the ground, advance sub-state and recover
				if (OnGround)
				{
//					SoundPalette.PlaySound(jumpSound, 1, 1, transform.position);
					downAttackRecoveryTime = 2f;  //hard coded value to add drama to recovery
					downAttackRecovery = true;

					//TODO: use set value
					//skeletonAnimation.skeleton.Data.FindAnimation(clearAttackAnim).Apply()
					skeletonAnimation.skeleton.Data.FindAnimation(clearAttackAnim).Apply(skeletonAnimation.skeleton, 0, 1, false, null, 1, MixBlend.Replace, MixDirection.In);
					skeletonAnimation.state.GetCurrent(0).TrackTime = (downAttackFrameSkip / 30f);

					//spawn effect
					if (downAttackPrefab)
						Instantiate(downAttackPrefab, transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity);

				}
				else
				{
					//TODO:  Watchdog and error case check
				}
			}

			//pause all movement, set by Pause event in animation for great dramatic posing.
			if (velocityLock)
				velocity = Vector2.zero;
		}

		flipped = skeletonAnimation.Skeleton.FlipX;
		rb.velocity = velocity;
	}

	//Bounce off a player in an angry way
	bool EnemyBounceCheck(ref Vector2 velocity)
	{
		var character = OnTopOfCharacter();
		if (character != null)
		{
//			SoundPalette.PlaySound(jumpSound, 1, 1, transform.position);
			character.SendMessage("Hit", 1, SendMessageOptions.DontRequireReceiver);
			velocity.y = headBounceSpeed;
			jumpStartTime = Time.time;
			state = ActionState.JUMP;
			doJump = false;
			return true;
		}
		return false;
	}

	//Enter the fall state, optionally using a jump counter.  IE: to prevent jumping after slipping off a platform
	void SetFallState(bool useJump)
	{
		if (useJump)
			jumpCount++;

		state = ActionState.FALL;
	}

	//Sync the Spine Animation with the current ActionState. Handle a few "edge" cases to do with inclines and uh... edges.
	void UpdateAnim()
	{
		switch (state)
		{
			case ActionState.IDLE:
				if (CenterOnGround)
				{
					skeletonAnimation.AnimationName = idleAnim;
				}
				else
				{
					if (onIncline)
						skeletonAnimation.AnimationName = idleAnim;
					else if (BackOnGround)
					{
						skeletonAnimation.AnimationName = balanceForward;
					}
					else if (ForwardOnGround)
					{
						skeletonAnimation.AnimationName = balanceBackward;
					}
				}
				break;
			case ActionState.WALK:
				skeletonAnimation.AnimationName = walkAnim;
				break;
			case ActionState.RUN:
				skeletonAnimation.AnimationName = runAnim;
				break;
			case ActionState.JUMP:
				skeletonAnimation.AnimationName = jumpAnim;
				break;
			case ActionState.FALL:
				skeletonAnimation.AnimationName = fallAnim;
				break;
			case ActionState.WALLSLIDE:
				skeletonAnimation.AnimationName = wallSlideAnim;
				break;
			case ActionState.SLIDE:
				skeletonAnimation.AnimationName = slideAnim;
				break;
			case ActionState.CROUCH:
				skeletonAnimation.AnimationName = crouchAnim;
				break;
		}
	}

	//work-around for Box2D not updating friction values at runtime...
	void SetFriction(float friction)
	{
		if (friction != characterColliderMaterial.friction)
		{
			characterColliderMaterial.friction = friction;
			primaryCollider.gameObject.SetActive(false);
			primaryCollider.gameObject.SetActive(true);
		}
	}


	//TODO:  deal with SetFriction workaround breaking ignore pairs.........
	void IgnoreCharacterCollisions(bool ignore)
	{
		Debug.Log("IgnoreCharacterCollisions");
		//foreach (GameCharacter gc in All)
		//	if (gc == this)
		//		continue;
		//	else
		//	{
		//		gc.IgnoreCollision(primaryCollider, ignore);
		//	}
	}

	//Special effects helper function
	void SpawnAtFoot(GameObject prefab, Quaternion rotation, Vector3 scale)
	{
		var bone = skeletonAnimation.Skeleton.FindBone(footEffectBone);
		Vector3 pos = skeletonAnimation.transform.TransformPoint(bone.WorldX, bone.WorldY, 0);
		((GameObject)Instantiate(prefab, pos, rotation)).transform.localScale = scale;
	}
}
