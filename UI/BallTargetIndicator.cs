using UnityEngine;
using System.Collections;
using System;

public class BallTargetIndicator : MonoBehaviour {

	void Awake()
	{
		gameObject.SetActive(false);
	}

	BallTest ballScript;
	Transform thisTransform;
	float percentsPerSecond = 0.06f; 
	float currentPathPercent = 0.0f; 

	void OnEnable()
	{

	}

	void OnDisable()
	{
        GameInputController.OneActionFingerTap -= HandleGameInputControllerOneActionFingerTap; 
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Show()
	{
		if(animationPlaying)
			gameObject.SetActive(true);
	}

	Transform cTransform
	{
		get{
			if(thisTransform == null)
				thisTransform = transform;
			return thisTransform;
		}
	}



	bool animationPlaying = false;
	bool indicatorMoving = false;
	public AnimationCurve curve;

	public IEnumerator PlayIndicator(BallTest ballScript, bool doTranslate){
		
		MorphController.CanTranslate = false;

		this.ballScript = ballScript;
		if(animationPlaying) {
			MorphController.CanTranslate = true;
			yield break;
		}
			
		ballScript.cTransform.parent = GameController.Instance.morphController.cTransform;
		cTransform.parent = GameController.Instance.morphController.cTransform;

		indicatorMoving = false;
		float x = 0;
		float y = 0;

		Action<GameObject> TranslateEndedAction = (go)=>{
			cTransform.position = ballScript.cTransform.position;
			x = cTransform.localPosition.x;
			y = cTransform.localPosition.y ;
			gameObject.SetActive(true);
			indicatorMoving = true;
			GameInputController.OneActionFingerTap += HandleGameInputControllerOneActionFingerTap; 
			MorphController.CanTranslate = true;
		};


		if(doTranslate) {
			Vector3 target = GameController.Instance.morphController.GetBallInstantinateAnchorPosition;
			Vector3 current = ballScript.cTransform.position;
		
			/*
			Vector3[] v3Path =  new Vector3[30];
			for(int i=0 ; i<v3Path.Length; i++) {
				float dX = (target.x - current.x) - (((target.x - current.x)/v3Path.Length) * (i+1));
				float dY = (target.y - current.y) - (((target.y - current.y)/v3Path.Length) * (i+1));


				float degree = (float)i * 2 /((v3Path.Length - 1));
				if(degree > 1)
					degree = 2 - degree ;

				float cos_diff =  Mathf.Acos(degree)*180/Mathf.PI;
				float sin_diff = Mathf.Asin(degree)*180/Mathf.PI;
				float _x =  target.x - dX;
				float _y = target.y + dY;
				v3Path[i] = new Vector3( _x - sin_diff/100   ,_y + cos_diff/100 ,0) ;
				Debug.Log("degree "+degree+"   dX "+dX+" cos_diff "+cos_diff+"  sin_diff "+sin_diff+"  path x "+v3Path[i].x);

			}
			*/

			Debug.Log("doTranslate is MoveTo MorphController.CanTranslate "+MorphController.CanTranslate);
			iTween.MoveTo(ballScript.gameObject, iTween.Hash("x",target.x, "y",target.y,  "time",0.7f, "delay",0,  "easeType","islocal",true, iTween.EaseType.easeInOutBack, 
			                                                 "dontusesendmessage",true, "endedaction",TranslateEndedAction , "onComplete","changedWithAction"));
		}
		else {
			TranslateEndedAction(null);
			MorphController.CanTranslate = true;
		}

		float distanceX = 1.11f;
		float distanceY = 1.11f;
		


		animationPlaying = true;

		while(animationPlaying) {

			if(indicatorMoving) {

				float dX = Mathf.Sin(currentPathPercent * Mathf.PI / 1.80f) * distanceX;
				float dY = Mathf.Cos(currentPathPercent * Mathf.PI / 1.80f) * distanceY;
				
				cTransform.localPosition = new Vector3(x + dX, y+dY, -2);
				/*		Quaternion targetRotation = Quaternion.LookRotation(GameController.Instance.character.cTransform.position - trBallTargetIndicator.position, Vector3.up);
			float zRot = 90 -  targetRotation.eulerAngles.x ; 
			zRot *= Mathf.Sin((targetRotation.eulerAngles.y * Mathf.PI)/180);
			trBallTargetIndicator.eulerAngles = new Vector3(0, 0,zRot);
		*/
				
				currentPathPercent += percentsPerSecond;
				if(currentPathPercent > 0.60f){
					percentsPerSecond *= -1;
				}
				else if(currentPathPercent < -0.60f){
					percentsPerSecond *= -1;
				}
				
				yield return new WaitForSeconds(0.02f);

			}
			yield return null;

			
		}

	}

    public void CancelIndicator()
    {
        GameInputController.OneActionFingerTap -= HandleGameInputControllerOneActionFingerTap;
        animationPlaying = false;
        indicatorMoving = false;
        gameObject.SetActive(false);
    }

	void HandleGameInputControllerOneActionFingerTap (Vector2 obj)
	{
		if(GameController.Instance.State != GameController.GameState.Running || ballScript == null)
			return;
		GameInputController.OneActionFingerTap -= HandleGameInputControllerOneActionFingerTap; 
		animationPlaying = false;
		gameObject.SetActive(false);
		ballScript.cTransform.parent = GameController.Instance.trBaseGameAnchor;
		var heading = cTransform.position - ballScript.cTransform.position;
		var distance = heading.magnitude;
		var direction = heading / distance;
		ballScript.ThrowBall(direction);

	}


}
