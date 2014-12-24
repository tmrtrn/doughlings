using UnityEngine;
using System.Collections;

public abstract class MorphIndicatorComponent : MonoBehaviour {
	
	
	public abstract bool Enter(MorphState activeState, float duration, System.Action completedAction);
	public abstract void Exit();
	
	Transform trComponent;
	
	public Transform cTransform
	{
		get{
			if(trComponent == null)
				trComponent = transform;
			return trComponent;
		}
	}
	public virtual void Show()
	{
		cTransform.gameObject.SetActive(true);
	}
	
	public virtual void Hide()
	{
		cTransform.gameObject.SetActive(false);
	}

	public virtual bool HasTween{
		get{
			return false;
		}
	}
	
}
