using UnityEngine;
using System;
using System.Collections;

public class ScoreCounter : MonoBehaviour {

	public tk2dTextMesh textMesh;
	public float speed = 2;
	public float increaseInterval = 2; 

	int targetScore;
	Action endAction;

	void OnEnable()
	{
		textMesh.text = "0";
		textMesh.Commit();
	}

	public void SetScoreCounter(int score) {
		iTween.Stop(gameObject);
		this.targetScore = score;
		endAction = null;
		textMesh.text = ((int)targetScore).ToString();
		textMesh.Commit();
	}

	public void StartScoreCounter(int score, Action endAction){
	
		this.targetScore = score;
		this.endAction = endAction;
		if(score == 0)
			endAction();

		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 0, "to", score,
			"time", (speed), "easetype", iTween.EaseType.linear,
			"onupdate", "OnUpdateCounter",
			"oncomplete","OnCompleteCounter")); 
	}
	void OnUpdateCounter(float newVal) {

		textMesh.text = ((int)newVal).ToString();
		textMesh.Commit();
		if(((int)newVal) == targetScore){
			if(endAction != null)
				endAction();
			iTween.Stop(gameObject);
		}
	}

	void OnCompleteCounter()
	{
		textMesh.text = ((int)targetScore).ToString();
		textMesh.Commit();
		if(endAction != null)
			endAction();
	}

}
