using UnityEngine;
using System.Collections;

public class GamePauseScreen : GameScreen {

	public GameObject goBlackCurtain;

	GameObject goFoundBlackCurtain;

	public override void TransitionIn ()
	{

		base.TransitionIn();
		goFoundBlackCurtain = GameObject.FindGameObjectWithTag("GUIBlackCurtain");
		if(goFoundBlackCurtain != null) {
			if(goFoundBlackCurtain.activeInHierarchy) {
				goFoundBlackCurtain.GetComponent<tk2dTiledSprite>().SortingOrder = 3;
				return;
			}
		}
		goBlackCurtain.GetComponent<tk2dTiledSprite>().SortingOrder = 3;

		Color color = goBlackCurtain.GetComponent<tk2dTiledSprite>().color;
		color.a = 0;
		goBlackCurtain.GetComponent<tk2dTiledSprite>().color = color;

		goBlackCurtain.SetActive(true);
		Debug.Log("ValueTO");
		iTween.ValueTo(gameObject, iTween.Hash(
			"ignoretimescale", true,
			"from", 0f, "to", 1f,
			"time", 0.5f, "easetype", iTween.EaseType.linear,
			"onupdate", "OnUpdateFade",
			"oncomplete","OnCompleteFadeIn")); 

	}

	public void OnUpdateFade(float newAlpha) {

		Color color = goBlackCurtain.GetComponent<tk2dTiledSprite>().color;
		color.a = newAlpha;
		goBlackCurtain.GetComponent<tk2dTiledSprite>().color = color;
		
	}
	public void OnCompleteFadeIn()
	{
		goBlackCurtain.SetActive(true);
	}

	public void OnCompleteFadeOut()
	{
		goBlackCurtain.SetActive(false);
	}

	public override void TransitionOut ()
	{
		base.TransitionOut ();
	
		if(goFoundBlackCurtain != null) {
		//	goBlackCurtain = GameObject.FindGameObjectWithTag("GUIBlackCurtain");
			goFoundBlackCurtain.GetComponent<tk2dTiledSprite>().SortingOrder = 2;
		}
		else {
			Debug.Log("TransitionOut goFoundBlackCurtain is NULL");
			iTween.ValueTo(gameObject, iTween.Hash(
				"ignoretimescale", true,
				"from", 1f, "to", 0f,
				"time", 0.5f, "easetype", iTween.EaseType.linear,
				"onupdate", "OnUpdateFade",
				"oncomplete","OnCompleteFadeOut")); 
		}
	}

	public override void Show ()
	{
		base.Show ();
	}

	public override void Hide ()
	{
		base.Hide ();
	}

	public override ScreenType Type {
		get {
			return ScreenType.PAUSE;
		}
	}

}
