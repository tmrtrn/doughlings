using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class MorphStateBehaviour : MonoBehaviour {

	public abstract bool Enter(string enterGlobalStatus, Action enterAction);
	public abstract void Exit(string exitStatus,Action exitAction);
	public abstract string GetNameOnStorage{get;}
	public abstract MorphState GetState{get;}
	public abstract Vector2 ColliderSize{get;}
	public abstract void SetDeath(Action ended);
	public virtual bool CanWinScoreBallCollideWithItem{get{return false;}}
	public virtual float GetIndicatorDuration{get{return -1;}}
	public virtual void ReceivedDamage(){}
	public abstract event Action<GameInputController.TouchArea> OnCharacterStartedMoving;
	public abstract event Action OnCharacterEndedMoving;
	public abstract event Action OnCharacterMoving;
	public abstract void ForceEnableInputs(bool enable);
	public abstract void PlayIdle();
	public abstract void DoShowOff();



	protected List<AnimView> animViews = new List<AnimView>();

	public virtual void Wake()
	{
		Show();

	}
	
	public virtual void Sleep() {
		animViews.ForEach((obj) => obj.SetActiveGameObject(false));

	}
	
	public virtual void Show()
	{
		animViews.ForEach((obj) => obj.SetRender(true));
	}
	
	public virtual void Hide()
	{
		animViews.ForEach((obj) => obj.SetRender(false));
	}

	public void InstantinateAnimations(List<AnimationProperty> anims)
	{
		anims.ForEach((anim) => {
			
			GameObject goAnimParent = new GameObject(anim.name);
			goAnimParent.transform.parent = transform;
			goAnimParent.transform.localPosition = Vector3.zero;
			AnimView animView = goAnimParent.AddComponent<AnimView>();
			animView.LoadLimbs(anim, goAnimParent.transform);
			animViews.Add(animView);	
		});
	}

	public AnimView GetAnimView(string animationName)
	{
		return animViews.Find( (next)=> next.GetAnimationName() == animationName);
	}

	public virtual void DestroyBehaviourObject()
	{

	}

    public void SetTimeScaleIgnore(bool ignore) {
        animViews.ForEach((obj) => obj.SetIgnoreTimeScale(ignore));
    }

}
