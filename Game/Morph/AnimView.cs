using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimView : MonoBehaviour {
	
	public event System.Action OnAnimationEnded;
	AnimationProperty anim;
	public List<LimbView> limbViews = new List<LimbView>();

	public void LoadLimbs(AnimationProperty anim , Transform rootAnimObject)
	{
		this.anim = anim;
		
		anim.limbElements.ForEach((limbProp) => {
			GameObject goLimb = new GameObject(limbProp.name);
			goLimb.transform.parent = rootAnimObject;
			goLimb.transform.localPosition = new Vector3(limbProp.left/100, -limbProp.top/100, -limbProp.z);
			LimbView limbView = goLimb.AddComponent<LimbView>();
			limbView.LoadSubAnim(limbProp.dependedSubAnimName , limbProp.IsFlip, anim.loop, limbProp.name);
			
			limbViews.Add(limbView);
			
		});
	}
	
	public string GetAnimationName()
	{
		return anim.name;
	}
	
	public void Play()
	{
		int receivedAnim = 0;
		limbViews.ForEach((obj) => obj.PlaySubAnim( () => {
			receivedAnim += 1;
			if(receivedAnim == limbViews.Count)
				if(OnAnimationEnded != null)
					OnAnimationEnded();
		}));
	}
	
	public void Stop()
	{
		OnAnimationEnded = null;
		limbViews.ForEach((obj) => obj.StopAnimation());
	}
	
	public void SetRender(bool active)
	{
		limbViews.ForEach((obj) => obj.gameObject.renderer.enabled = active);
	}
	
	public void SetActiveGameObject(bool active)
	{
		limbViews.ForEach((obj) => obj.gameObject.SetActive(active));
	}

    public void SetIgnoreTimeScale(bool enable) {
        limbViews.ForEach((obj) => obj.IgnoreTimeScale = enable);
    }
}
