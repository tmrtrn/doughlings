using UnityEngine;
using System.Collections;
using System;

public class MorphIndicatorBarComponent : MorphIndicatorComponent {
	
	public tk2dClippedSprite clippedSprite;
	public float slicedCount = 5;
	
	private Action completedAction;
	private float duration;
	private string lastActiveSpriteBarName;
	bool animating = false;
	
	
	#region implemented abstract members of MorphIndicatorComponent
	
	public override bool Enter (MorphState activeState, float duration, System.Action completedAction)
	{
		if(activeState == MorphState.Morpheus || activeState == MorphState.None) {
			base.Hide();
			return false;
		}
		base.Show();
		iTween.Stop(gameObject);
		lastActiveSpriteBarName = "morphBar"+activeState.ToString();
		this.completedAction = completedAction;
		this.duration = duration;
		clippedSprite.SetSprite(lastActiveSpriteBarName);
		clippedSprite.ClipRect = new Rect(0,0,1,1);
		animating = true;
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1f, "to", 0f,
			"time", duration, "easetype", iTween.EaseType.linear,
			"onupdate", "OnUpdateDecreaseBar",
			"oncomplete","OnCompleteDecreaseBar")); 
		return true;
	}
	float timeHandler = 0;
	
	IEnumerator DecreaseBarClippedSprite()
	{
		Debug.Log("DecreaseBarClippedSprite "+Time.time);
		float interpolate = 1/2f;
		while(animating) {

			Rect rect = clippedSprite.ClipRect;

			yield return null;
			/*yield return new WaitForSeconds((duration * interpolate)/100);
			rect.height -= 0.01f * interpolate;

			if(rect.height <= 0.01f)
			{
				Debug.Log("END DecreaseBarClippedSprite "+Time.time);
				rect.height = 0;
				clippedSprite.ClipRect = rect;
				completedAction();
				animating = false;
				yield break;
			}
			else {
				clippedSprite.ClipRect = rect;
			}
			*/
		}
	}

	void OnUpdateDecreaseBar(float newVal) {
		
		Rect rect = clippedSprite.ClipRect;
		rect.height = newVal;
		clippedSprite.ClipRect = rect;
	}
	void OnCompleteDecreaseBar()
	{
		completedAction();
	}


	
	public override void Exit ()
	{
		animating = false;
		base.Hide();
	}

	public override bool HasTween {
		get {
			return true;
		}
	}
	
	
	#endregion
	
	
	
}
