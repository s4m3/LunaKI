using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AGAnimationClip
{
	public AnimationClip Clip;
	public float Chance = 1.0f;
}

[System.Serializable]
public class AGAnimationSingle
{
	public AnimationClip m_Clip;
	public WrapMode m_WrapMode = WrapMode.Loop;
	private AnimationState m_State;
	public Transform[] m_StartBones;
	private int m_Layer = 10; // fails when public x(
//	[HideInInspector]
	protected float m_CrossFadeSpeed = 0.1f;
//	[HideInInspector]
	protected Animation m_myAnimation = null;
//	[HideInInspector]
	protected string m_ClipIdle;
	private AnimationState m_StateIdle = null;
	public float m_Speed = 1.0f;
	private float m_SpeedFactor = 1.0f;
	
	public virtual void Initialize (Animation animation)
	{
		Print.Assert (animation != null);
		m_myAnimation = animation;
		setupClips ();
		Update ();
	}
	
	private void setupClips ()
	{
		if (!m_Clip) {
			Print.Warning ("Clip missing for " + this);
			return;
		}
		m_myAnimation.AddClip (m_Clip, m_Clip.name);
		m_State = m_myAnimation [m_Clip.name];
		m_ClipIdle = m_Clip.name;
		if (m_StartBones.Length != 0) {
			m_ClipIdle = m_Clip.name + "Idle";
			m_myAnimation.AddClip (m_Clip, m_ClipIdle);
			m_StateIdle = m_myAnimation [m_ClipIdle];
			setupClip (m_StateIdle);
			for (int bone = 0; bone < m_StartBones.Length; ++bone) {
				m_State.AddMixingTransform (m_StartBones [bone], true);
			}
		}
		setupClip (m_State);
	}
	
	private void setupClip (AnimationState clip)
	{
		clip.weight = 1.0f;
		clip.layer = m_Layer;
		clip.blendMode = AnimationBlendMode.Blend;
		clip.wrapMode = m_WrapMode;
		clip.enabled = true;
	}
	
	public void setWrapMode (WrapMode wrapMode)
	{
		m_WrapMode = wrapMode;
		m_State.wrapMode = wrapMode;
		if (m_StateIdle)
			m_StateIdle.wrapMode = wrapMode;
	}
	
	public void setEnable (bool enable)
	{
		m_State.enabled = enable;
		if (m_StateIdle != null)
			m_StateIdle.enabled = enable;
	}
	
	public virtual void Update ()
	{
		setSpeed ();
	}
	
	private void setSpeed ()
	{
		m_State.speed = m_Speed * m_SpeedFactor;
		if (m_StateIdle != null) {
			m_StateIdle.speed = m_Speed * m_SpeedFactor;
		}
//		if( m_State.name.ToLower().Contains("melee"))
//		Print.Log (m_State.name + " Speed: " + m_State.speed);
	}
	
	public void printSpeeds ()
	{
		var s1 = m_State.speed;
		var s2 = 0.0;
		if (m_StateIdle != null) {
			s2 = m_StateIdle.speed;
		}
		Print.Log(s1 + " " + s2);
	}
	
	public void setSpeedFactor (float factor)
	{
		m_SpeedFactor = factor;
	}
	
	public int getLayer ()
	{
		return m_Layer;
	}
	
	public void printStatistics ()
	{
		Print.Log (m_State.name + ": " + m_State.enabled + ", " + m_State.layer + ", " + m_State.speed + ", " + m_State.wrapMode + ", " + m_State.time);
	}
	
	public void setLayer (int layer)
	{
		m_Layer = layer;
		if (m_Clip != null) {
			m_State.layer = m_Layer;
			if (m_StateIdle)
				m_StateIdle.layer = m_Layer;
		}
	}
	
	public virtual void CrossFadeImmediate (bool isIdle)
	{
		if (m_State.speed < 0) {
			m_State.normalizedTime = 1;
		} else {
			m_State.normalizedTime = 0;
		}
		m_myAnimation.CrossFade ((isIdle ? m_ClipIdle : m_Clip.name), m_CrossFadeSpeed);
	}
	
//	public virtual void PlayImmediate (bool isIdle)
//	{
//		m_myAnimation.Play ((isIdle ? m_ClipIdle : m_Clip.name), PlayMode.StopSameLayer);
//	}
	
	public void setWeight (float w)
	{
		m_State.weight = w;
		if (m_StateIdle)
			m_StateIdle.weight = w;
	}
	
	public float getWeight ()
	{
		return m_State.weight;
	}
}

[System.Serializable]
public class Float3
{
	public float m_Start = 1.0f;
	public float m_Middle = 1.0f;
	public float m_End = 1.0f;
	public float m_Speed = 1.0f;
	
	public Float3 ()
	{
	}
	
	public Float3 (float start, float middle, float end, float speed)
	{
		m_Start = start;
		m_Middle = middle;
		m_End = end;
		m_Speed = speed;
	}
}

[System.Serializable]
public class AGAnimationAction : AGAnimationSingle
{
	public bool m_isStiff = false;
	public Float3 m_StiffTimesBase;
	[HideInInspector]
	public  Float3 m_StiffTimes = new Float3 (0, 0, 0, 1);
	protected float m_targetWeight = 1.0f;
	protected const int m_ActionLayer = 15;
	
	public override void  Initialize (Animation animation)
	{
		base.Initialize (animation);
		setLayer (m_isStiff ? m_ActionLayer+1 : m_ActionLayer);
		setWeight (0);
	}
	
	public virtual void resetTimes ()
	{
		m_StiffTimes = new Float3 (0, 0, 0, m_StiffTimesBase.m_Speed);
	}
	
	public virtual void clampTimes ()
	{
		m_StiffTimes.m_Start = Mathf.Min (m_StiffTimes.m_Start, m_StiffTimesBase.m_Start);
		m_StiffTimes.m_Middle = Mathf.Min (m_StiffTimes.m_Middle, m_StiffTimesBase.m_Middle);
		m_StiffTimes.m_End = Mathf.Min (m_StiffTimes.m_End, m_StiffTimesBase.m_End);
	}
	
	public virtual void increaseUpdate ()
	{
		float step = m_StiffTimes.m_Speed * Time.deltaTime;
		float stepsLeft = (m_StiffTimesBase.m_Start - m_StiffTimes.m_Start) / step;
		m_StiffTimes.m_Start += step;
		float weightLeft = m_targetWeight - getWeight ();
		setWeight (getWeight () + weightLeft / stepsLeft);
	}
	
	public virtual void middleUpdate ()
	{
		m_StiffTimes.m_Middle += (m_StiffTimes.m_Speed * Time.deltaTime);
	}
	
	public virtual void decreaseUpdate ()
	{
		float step = m_StiffTimes.m_Speed * Time.deltaTime;
		float stepsLeft = (m_StiffTimesBase.m_End - m_StiffTimes.m_End) / step;
		m_StiffTimes.m_End += step;
		float weightLeft = getWeight ();
		setWeight (getWeight () - weightLeft / stepsLeft);
	}
}

public class ActionHandler
{
	protected List<AGAnimationAction> m_StartingActions = new List<AGAnimationAction> ();
	protected List<AGAnimationAction> m_CurrentActions = new List<AGAnimationAction> ();
	protected List<AGAnimationAction> m_EndingActions = new List<AGAnimationAction> ();
	
	public void addAction (AGAnimationAction animAction)
	{
		m_StartingActions.Add (animAction);
	}

	public void UpdateActions ()
	{
		List<AGAnimationAction> removables = new List<AGAnimationAction> ();
		foreach (var action in m_StartingActions) {
			if (action.m_StiffTimes.m_Start >= action.m_StiffTimesBase.m_Start) {
				removables.Add (action);
			} else {
				action.increaseUpdate ();
			}
		}
		
		foreach (var item in removables) {
			m_StartingActions.Remove (item);
			item.clampTimes ();
			m_CurrentActions.Add (item);
		}
		
		removables = new List<AGAnimationAction> ();
		foreach (var action in m_CurrentActions) {
			if (action.m_StiffTimes.m_Middle >= action.m_StiffTimesBase.m_Middle) {
				removables.Add (action);
			} else {
				action.middleUpdate ();
			}
		}
		
		foreach (var item in removables) {
			m_CurrentActions.Remove (item);
			item.clampTimes ();
			m_EndingActions.Add (item);
		}
		
		removables = new List<AGAnimationAction> ();
		foreach (var action in m_EndingActions) {
			if (action.m_StiffTimes.m_End >= action.m_StiffTimesBase.m_End) {
				removables.Add (action);
			} else {
				action.decreaseUpdate ();
			}
		}
		
		foreach (var item in removables) {
			m_EndingActions.Remove (item);
		}
	}
}

[System.Serializable]
public class FourWayDash : AGAnimationAction
{
	public AGAnimationAction m_Forward;
	public AGAnimationAction m_Backward;
	public AGAnimationAction m_Left;
	public AGAnimationAction m_Right;
	private DirectionUtility m_directionUtility;
	private List<AGAnimationAction> m_dashAnimations = new List<AGAnimationAction> ();
	protected float m_weight = 0.0f;

	public bool isSet ()
	{
		return m_Forward != null && m_Backward != null && m_Left != null && m_Right != null && m_Clip != null;
	}
	
	public override void  Initialize (Animation animation)
	{
		Print.Error ("TODO");
	}
	
	public void  Initialize (Animation animation, AGPawn pawn)
	{
		
		m_dashAnimations.Add (m_Forward);
		m_dashAnimations.Add (m_Backward);
		m_dashAnimations.Add (m_Left);
		m_dashAnimations.Add (m_Right);
		
		InitAnimations ();
		foreach (var dash in m_dashAnimations) {
			dash.Initialize (animation);
			dash.setLayer(m_ActionLayer-1);
		}
		m_directionUtility = new DirectionUtility (m_Forward, m_Backward, m_Left, m_Right, pawn);
		
	}
	
	private void InitAnimations ()
	{
		foreach (var dash in m_dashAnimations) {
			dash.m_isStiff = m_isStiff;
			dash.m_WrapMode = m_WrapMode;
			dash.m_Speed = m_Speed;
			dash.m_StiffTimesBase.m_Start = m_StiffTimesBase.m_Start;
			dash.m_StiffTimesBase.m_Middle = m_StiffTimesBase.m_Middle;
			dash.m_StiffTimesBase.m_End = m_StiffTimesBase.m_End;
			dash.m_StiffTimesBase.m_Speed = m_StiffTimesBase.m_Speed;
			dash.resetTimes ();
		}
	}
	
	private void UpdateAnimations ()
	{
		foreach (var dash in m_dashAnimations) {
			dash.m_isStiff = m_isStiff;
			dash.m_WrapMode = m_WrapMode;
			dash.m_Speed = m_Speed;
			dash.m_StiffTimesBase.m_Start = m_StiffTimesBase.m_Start;
			dash.m_StiffTimesBase.m_Middle = m_StiffTimesBase.m_Middle;
			dash.m_StiffTimesBase.m_End = m_StiffTimesBase.m_End;
			dash.m_StiffTimesBase.m_Speed = m_StiffTimesBase.m_Speed;
			dash.Update ();
		}
	}
	
	public override void Update ()
	{
		UpdateAnimations ();
	}
	
	public override void resetTimes ()
	{
		base.resetTimes ();
		foreach (var item in m_dashAnimations) {
			item.resetTimes ();
		}
	}
	
	public override void clampTimes ()
	{
		foreach (var item in m_dashAnimations) {
			item.clampTimes ();
		}
	}
	
	public override void increaseUpdate ()
	{
		float step = m_StiffTimes.m_Speed * Time.deltaTime;
		float stepsLeft = (m_StiffTimesBase.m_Start - m_StiffTimes.m_Start) / step;
		m_StiffTimes.m_Start += step;
		float weightLeft = m_targetWeight - m_weight;
		m_weight += weightLeft / stepsLeft;
		UpdateAnimationWeights ();
	}
	
	public override void middleUpdate ()
	{
		m_StiffTimes.m_Middle += (m_StiffTimes.m_Speed * Time.deltaTime);
	}
	
	public override void decreaseUpdate ()
	{
		float step = m_StiffTimes.m_Speed * Time.deltaTime;
		float stepsLeft = (m_StiffTimesBase.m_End - m_StiffTimes.m_End) / step;
		m_StiffTimes.m_End += step;
		float weightLeft = m_weight;
		m_weight -= weightLeft / stepsLeft;
		UpdateAnimationWeights ();
	}
	
	protected void UpdateAnimationWeights ()
	{
		m_directionUtility.setDirection (m_weight);
	}
}

public class DirectionUtility
{
	public AGAnimationSingle m_Forward;
	public AGAnimationSingle m_Backward;
	public AGAnimationSingle m_Left;
	public AGAnimationSingle m_Right;
	public AGPawn m_myPawn;
	
	public DirectionUtility (AGAnimationSingle forward, AGAnimationSingle backward, AGAnimationSingle left, AGAnimationSingle right, AGPawn pawn)
	{
		m_Forward = forward;
		m_Backward = backward;
		m_Left = left;
		m_Right = right;
		m_myPawn = pawn;
	}
	
	private float[] getLCR (float velocity, float LCR)
	{
		float[] lcr = new float[3];
		
		lcr [0] = Mathf.Min (LCR, 0) * (-1.0f) * velocity;
		lcr [1] = (1.0f - Mathf.Abs (LCR)) * velocity;
		lcr [2] = Mathf.Max (LCR, 0) * velocity;
		
		return lcr;
	}
	
	public void setWeights (float f, float b, float l, float r)
	{
		m_Forward.setWeight (f);
		m_Backward.setWeight (b);
		m_Left.setWeight (l);
		m_Right.setWeight (r);
	}
	
	public void setDirection (float velocity)
	{
		float angle = Vector3.Angle (m_myPawn.Velocity, m_myPawn.LookDirection);
		if (angle == 0.0f) {
			float direction = Mathf.Sign (Vector3.Dot (m_myPawn.Velocity, m_myPawn.LookDirection));
			if (direction == 1.0f) {
				setWeights (velocity, 0, 0, 0);
			} else if (direction == -1.0f) {
				setWeights (0, velocity, 0, 0);
			}
		} else {
			float leftright = Vector3.Dot (Vector3.Cross (m_myPawn.Velocity, m_myPawn.LookDirection), m_myPawn.transform.up);
			leftright /= 4.0f;
			var lcr = getLCR (velocity, -leftright); // angle -> -1 0 1
				
			float[] fblr = new float[2];
			if (angle < 90f) { // Forward
				fblr [0] = lcr [1];
				fblr [1] = 0;
			} else { // Backward
				fblr [0] = 0;
				fblr [1] = lcr [1];
			}

			setWeights (fblr [0], fblr [1], lcr [0], lcr [2]);
		}
	}
}

[System.Serializable]
public class FourWayMove
{
	public float m_Speed = 1.0f;
	public AGAnimationSingle m_Idle;
	public AGAnimationSingle m_Forward;
	public AGAnimationSingle m_Backward;
	public AGAnimationSingle m_Left;
	public AGAnimationSingle m_Right;
	private DirectionUtility m_directionUtility;
	private List<AGAnimationSingle> m_directions = new List<AGAnimationSingle> ();
	[HideInInspector]
	public AGPawn m_myPawn;
	[HideInInspector]
	public Animation m_anim;
	private float m_weight = 0;
	private const int m_Layer = 10;
	
	public void setWeight (float w)
	{
		m_weight = w;
	}
	
	public float getWeight ()
	{ 
		return m_weight;
	}
	
	public void Update ()
	{
		foreach (var direction in m_directions)
			direction.setSpeedFactor (m_Speed);
		
		foreach (var direction in m_directions)
			direction.Update ();
	}
	
	private int getLayer ()
	{
		return m_Forward.getLayer ();
	}
	
	public void Initialize (AGPawn pawn, Animation animation)
	{
		m_myPawn = pawn;
		m_anim = animation;
		
		m_directionUtility = new DirectionUtility (m_Forward, m_Backward, m_Left, m_Right, m_myPawn);
		
		initFBLR ();
		setLayer (m_Layer);
		
		animation.SyncLayer (m_Idle.getLayer ());
		animation.SyncLayer (this.getLayer ());
		
		m_directions.Add (m_Idle);
		m_directions.Add (m_Forward);
		m_directions.Add (m_Backward);
		m_directions.Add (m_Left);
		m_directions.Add (m_Right);
	}
	
	private void addClip (ref AGAnimationSingle inAnim, AGAnimationSingle anim, string extension)
	{
		if (inAnim.m_Clip == null) {
			m_anim.AddClip (anim.m_Clip, anim.m_Clip.name + extension);
			inAnim.m_Clip = m_anim [anim.m_Clip.name + extension].clip;
		}
		inAnim.Initialize (m_anim);
	}
	
	private void initFBLR ()
	{
		m_Idle.Initialize (m_anim);
		
		m_Forward.Initialize (m_anim);
		addClip (ref m_Backward, m_Forward, "Backward");
		addClip (ref m_Left, m_Forward, "Left");
		addClip (ref m_Right, m_Forward, "Right");
		
		m_directionUtility.setWeights (0, 0, 0, 0);
	}
	
	private void setLayer (int layer)
	{
		m_Idle.setLayer (layer++);
		m_Forward.setLayer (layer);
		m_Backward.setLayer (layer);
		m_Left.setLayer (layer);
		m_Right.setLayer (layer);
	}
	
	private float[] getLCR (float velocity, float LCR)
	{
		float[] lcr = new float[3];
		
		lcr [0] = Mathf.Min (LCR, 0) * (-1.0f) * velocity;
		lcr [1] = (1.0f - Mathf.Abs (LCR)) * velocity;
		lcr [2] = Mathf.Max (LCR, 0) * velocity;
		
		return lcr;
	}
	
	public void setVelocity (float velocity, float idleThreshold)
	{
		if (velocity < idleThreshold) {
			m_directionUtility.setWeights (0, 0, 0, 0);
		} else {
			m_directionUtility.setDirection (velocity);
		}
		
		if (Debug.isDebugBuild) {
			Update ();
		}
	}
}

[System.Serializable]
public class HighPriorityAnimation : AGAnimationAction{
	public override void Initialize (Animation animation)
	{
		base.Initialize (animation);
		base.setLayer(18);
	}
//	protected override int m_ActionLayer = 19;
	
//	public override void CrossFadeImmediate (bool isIdle)
//	{
//		m_myAnimation.CrossFade ((isIdle ? m_ClipIdle : m_Clip.name), m_CrossFadeSpeed);
////		m_myAnimation.CrossFadeQueued ((isIdle ? m_ClipIdle : m_Clip.name), m_CrossFadeSpeed);
//	}
}

[System.Serializable]
public class WinAnimation : HighPriorityAnimation{
	public bool m_useIdleAnim = false;
	public HighPriorityAnimation m_WinIdle = null;
	
	public override void CrossFadeImmediate (bool isIdle)
	{
		m_myAnimation.CrossFade ((isIdle ? m_ClipIdle : m_Clip.name), m_CrossFadeSpeed);
		if (m_WinIdle != null && m_useIdleAnim == true) {
			m_myAnimation.CrossFadeQueued (m_WinIdle.m_Clip.name);
//			Print.Log(m_WinIdle.m_Clip.name + " ... ");
//			m_WinIdle.printSpeeds ();
		}
//		m_myAnimation.CrossFadeQueued ((isIdle ? m_ClipIdle : m_Clip.name), m_CrossFadeSpeed);
	}
	
	public override void Initialize (Animation animation)
	{
		base.Initialize (animation);
		base.setLayer (19);
		if (m_WinIdle != null && m_useIdleAnim == true) {
			m_WinIdle.Initialize (animation);
			m_WinIdle.setLayer (19);
		}

	}
}

public class AGAnimationPawn : MonoBehaviour
{
	public AGPawn MyPawn;
	public FourWayMove Walk;
	[HideInInspector]
	public float MaxSpeedToIdle = 0.0f;
	[HideInInspector]
	public float MaxSpeedToWalk = 4.0f;
	public FourWayDash ActionAnimationDash;
	public HighPriorityAnimation DeathAnimation;
	public WinAnimation WinAnimation;
	public List<AGAnimationAction> ActionAnimation;
	public AnimationState anistate;
	protected bool m_isIdle = false;
	protected ActionHandler m_ActionHandler;
	
		
	private bool m_isOver = false;

	public void Die ()
	{
		m_isOver = true;
		if (DeathAnimation != null) {
			DeathAnimation.CrossFadeImmediate (false);
		}
	}
	
	public void Win ()
	{
		m_isOver = true;
		if (WinAnimation != null) {
			WinAnimation.CrossFadeImmediate (false);
		}
	}

	public void SetPawn (AGPawn p)
	{
		MyPawn = p;
//		m_isDying = false;
		
		Walk.Initialize (MyPawn, animation);
		MyPawn.registerAnimationPawn (this);
		
		
		foreach (var actionAnim in ActionAnimation) {
			actionAnim.Initialize (animation);
		}
		if (ActionAnimationDash.isSet ()) {
			ActionAnimationDash.Initialize (animation, MyPawn);
			ActionAnimation.Add (ActionAnimationDash);
		}
		if (DeathAnimation != null) {
			DeathAnimation.Initialize (animation);
			ActionAnimation.Add (DeathAnimation);
		}
		if (WinAnimation != null) {
			WinAnimation.Initialize (animation);
			ActionAnimation.Add (WinAnimation);
		}
		
		m_ActionHandler = new ActionHandler ();
		
	}
	
	void FixedUpdate ()
	{
		if (Input.GetKey (KeyCode.H)) {
			doAnimation ("spawn");
		}
		if (MyPawn) {
			var m = MyPawn.Velocity.magnitude;
			m_isIdle = false;
			if (m <= MaxSpeedToIdle) {
				m_isIdle = true;
			}
			Walk.setVelocity (m / MaxSpeedToWalk, MaxSpeedToIdle);
			m_ActionHandler.UpdateActions ();
		}
	}

	
	public void doAnimation (string anim)
	{
//		Print.Log ("wanna play: " + anim);
		if (anim.CompareTo ("") == 0 || ActionAnimation == null || m_isOver == true)
			return;
		anim = anim.ToLower ();
		foreach (var a in ActionAnimation) {
			string action = a.m_Clip.name;
			if (action.ToLower ().Contains (anim)) {
				if (Debug.isDebugBuild) {
					a.Update ();
				}
//				Print.Log ("Playing: " + action);
				if (a.m_isStiff == false) {
//					if (anim.ToLower () == "shoot") {
//						Print.Log (anim.ToLower ()  + " PlayImmediate");
//						a.PlayImmediate (m_isIdle);
//					} else {
//						Print.Log (anim.ToLower ()  + " CrossFadeImmediate");
						a.CrossFadeImmediate (m_isIdle);
//					}
				} else {
					a.resetTimes ();
					m_ActionHandler.addAction (a);
				}
				return;
			}
		}
	}
}
