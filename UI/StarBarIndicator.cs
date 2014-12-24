using UnityEngine;
using System.Collections;

public class StarBarIndicator  : MonoBehaviour{

	public float slicedCount = 3;
	public tk2dClippedSprite clippedSprite;

	void Awake()
	{
		SetStar(0);
	}

	public void SetStar(int starCount)
	{
		if(clippedSprite == null) {
			clippedSprite = GetComponent<tk2dClippedSprite>();
		}
		Rect rect = clippedSprite.ClipRect ;
		rect.height = starCount/slicedCount;
		clippedSprite.ClipRect = rect;
	}

	public void ChangeRatio(float ratio){
		float target =  ratio;
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", clippedSprite.ClipRect.height, "to", target ,
			"time", 0.5f, "easetype", iTween.EaseType.linear,
			"onupdate", "OnUpdateBar")); 
	}

	void OnUpdateBar(float newVal) {
		
		Rect rect = clippedSprite.ClipRect;
		rect.height = newVal;
		clippedSprite.ClipRect = rect;
	}

}
