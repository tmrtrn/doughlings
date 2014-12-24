using UnityEngine;
using System.Collections;
using System;

public class FallingObject : MonoBehaviour {
	
	
	
	public virtual void Start()
	{
		
		float target = -1080;
		float duration = Mathf.Abs((Math.Abs(target) - Math.Abs(transform.localPosition.y)*100)) * GameController.Instance.gameModel.fallingObjectsFallingDuration / Math.Abs(target); // Mathf.Abs((target - transform.localPosition.y) / target) * GameController.Instance.gameModel.fallingObjectsFallingDuration;
		float delay = GameController.Instance.gameModel.fallingObjectsFallingStartDelay;
		
		Action<GameObject> TranslateEndedAction = (go)=>{
			
			Destroy(gameObject);
		};
		
		
		iTween.MoveTo(gameObject , iTween.Hash("y", target/100, "islocal",true, "time",duration, "delay",delay,  "easeType", iTween.EaseType.linear, 
				"dontusesendmessage",true, "endedaction",TranslateEndedAction , "onComplete","changedWithAction"));
	}
	
	
	
	public virtual void OnTriggerEnter2D(Collider2D collision){
		
		iTween.Stop(gameObject);
		Destroy(gameObject);
		
	}
	
	
		
	
}
