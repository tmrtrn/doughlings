using UnityEngine;
using System.Collections;
using System;

public class LimbView : MonoBehaviour {
	
	
	SubAnimProperty subAnim;
	tk2dSpriteAnimator animator;
	bool loop;
	Vector3 defaultPosition;

	
	public void LoadSubAnim(string dependedSubAnimName, bool IsFlip, bool loop, string limbName)
	{
		this.loop = loop;
		subAnim = DataStorage.Instance.GetMorphSubanims.Find((obj) => obj.name == dependedSubAnimName);
		tk2dSprite baseSprite = gameObject.AddComponent<tk2dSprite>();
		tk2dSpriteAnimationClip selectedClip = DataStorage.Instance.GetMorph2dToolkitAnimation.GetClipByName(dependedSubAnimName);
		if(loop)
			selectedClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
		else
			selectedClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
		
		animator = tk2dSpriteAnimator.AddComponent(gameObject,DataStorage.Instance.GetMorph2dToolkitAnimation, DataStorage.Instance.GetMorph2dToolkitAnimation.GetClipIdByName(dependedSubAnimName));
		animator.AnimationEventTriggered += OnTriggered;
		defaultPosition = cTransform.localPosition;
		gameObject.GetComponent<tk2dSprite>().FlipX = IsFlip;

		baseSprite.SortingOrder = 3;
	
	}

    public bool IgnoreTimeScale
    {
        get {
            return animator.timescaleIndependant;
        }
        set {
            animator.timescaleIndependant = value;
        }
    }
	
	
	public void PlaySubAnim(Action endAction = null)
	{
		animator.AnimationCompleted = null;
		animator.AnimationCompleted += (animatorArg,clip) => {
			if(endAction != null){
				endAction();
			}
		};
		
		animator.Play(subAnim.name);
	}
	
	public void StopAnimation()
	{
		animator.Stop();
	}

	Transform _tLimb;

	Transform cTransform
	{
		get{
			if(_tLimb == null)
				_tLimb = transform;
			return _tLimb;
		}
	}
	
	void OnTriggered(tk2dSpriteAnimator animator,tk2dSpriteAnimationClip clip,int arg)
	{
		string[] values = clip.frames[arg].eventInfo.Split("\n"[0]);
		
//		float targetX = defaultPosition.x + float.Parse(values[0]);
//		float targetY = defaultPosition.y - float.Parse(values[1]);
//		
	//	iTween.MoveUpdate(gameObject,iTween.Hash("x",targetX, "y",targetY ,"islocal",true, "time",0.2f));
		float dX = float.Parse(values[0])/100;
		float dY = -float.Parse(values[1])/100;
		
		cTransform.localPosition = defaultPosition + new Vector3(dX,dY,0);
	}
	
	
	
	/*
	AnimationXmlHelper animationViewer;
	tk2dSpriteAnimator animator;
	Vector3 defaultPosition;
	
	public void Render(AnimationXmlHelper animationViewer)
	{
		this.animationViewer = animationViewer;
	}
	
	public void PlayAnimation(string animName)
	{
			
		defaultPosition = transform.localPosition;
		animator = GetComponent<tk2dSpriteAnimator>();
		animator.AnimationEventTriggered += OnTriggered;
		animator.Play(animName);
	}
	
	public void Destroy()
	{
		animator.AnimationEventTriggered -= OnTriggered;
	}
	void OnTriggered(tk2dSpriteAnimator animator,tk2dSpriteAnimationClip clip,int arg)
	{
		string[] values = clip.frames[arg].eventInfo.Split("\n"[0]);
		transform.localPosition = defaultPosition + new Vector3(float.Parse(values[0]),-float.Parse(values[1]),name == "morpheusHand2" ? -1 : 0);
	}
	*/
}
