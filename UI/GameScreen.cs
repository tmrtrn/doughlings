using UnityEngine;
using System;
using System.Collections;

public abstract class GameScreen : MonoBehaviour {

	public enum ScreenType
	{
		PAUSE,
		GAMEHUD,
		GAMEOVERFAILED,
		GAMEOVERVICTORY
	}

	public Transform trLeftSpiral;
	public Transform trRightSpiral;
	private float leftSpiralTargetPointX = 1.41f;
	private float rightSpiralTargetPoint = 17.79f;
	float duration = 0.3f;

	
	public Vector3 onScreenPositionOffset = new Vector3(0,0,50);
	public Vector3 offScreenPosition;

	public bool hasTransition;

	public virtual int GetBlackCurtainSortOrder
	{
		get{
			return -1;
		}
	}

	public abstract ScreenType Type{get;}

	public virtual void TransitionIn() {

		bool wait = true;
		Action<GameObject> endedAction = (go)=>{
//			wait = false;	
		};
		if(trLeftSpiral != null)
		iTween.MoveTo(trLeftSpiral.gameObject , iTween.Hash("x", leftSpiralTargetPointX, "islocal",true, "time",duration,   "easeType", iTween.EaseType.linear, 
			                                                    "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction","ignoretimescale",true));
		if(trRightSpiral != null)
		iTween.MoveTo(trRightSpiral.gameObject , iTween.Hash("x", rightSpiralTargetPoint, "islocal",true, "time",duration,  "easeType", iTween.EaseType.linear, 
			                                                     "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction","ignoretimescale",true));

		if(trLeftSpiral == null && trRightSpiral == null) {
			endedAction(gameObject);
		}

//		while(wait){
//
//			yield return null;
//		}
//		Show();
//		yield break;
	}
	
	public virtual void TransitionOut() {

//		bool wait = true;
		Action<GameObject> endedAction = (go)=>{
//			wait = false;	
		};

		if(trLeftSpiral != null)
		iTween.MoveTo(trLeftSpiral.gameObject , iTween.Hash("x", leftSpiralTargetPointX-4, "islocal",true, "time",duration,   "easeType", iTween.EaseType.linear, 
			                                                    "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction","ignoretimescale",true));
		if(trRightSpiral != null)
		iTween.MoveTo(trRightSpiral.gameObject , iTween.Hash("x", rightSpiralTargetPoint+4, "islocal",true, "time",duration,  "easeType", iTween.EaseType.linear, 
			                                                     "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction","ignoretimescale",true));
		if(trLeftSpiral == null && trRightSpiral == null) {
			endedAction(gameObject);
		}
//		while(wait){
//
//			yield return null;
//		}
//
//		Hide();
//		yield break;
	}
	
	
	public virtual void Show() {
		Time.timeScale = 1;
		transform.localPosition = GUIController.Instance.GUICamera.transform.position + onScreenPositionOffset;
	}
	
	public virtual void Hide() {
		Time.timeScale = 0;
		transform.localPosition = offScreenPosition;
	}
}
