using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotController : MonoBehaviour {

	public List<Transform> trSpots;
	public GameObject showOffBoard;

	void Awake () {
		
//		GameController.GameplayStarted += HandleGameplayStarted;
	
	}

	void OnEnable()
	{
		GameController.OnGameStateChanged += HandleGameStateChanged;
	}

	void OnDisable()
	{
		GameController.OnGameStateChanged -= HandleGameStateChanged;
	}

	void HandleGameStateChanged()
	{
		if(GameController.Instance.State != GameController.GameState.Running){
			showOffBoard.GetComponent<Button>().enabled = false;
		}
		else {
			showOffBoard.GetComponent<Button>().enabled = true;
		}
	}

	public void Reset ()
	{
		trSpots.ForEach((obj)=> {
			obj.gameObject.SetActive(false);
		});
		UpdateShowOff();
	}
	
	void UpdateShowOff()
	{

		if(trSpots.Exists( (obj)=> !obj.gameObject.activeInHierarchy)) {
			//Close Show off if open
			float target = -12.25f;
			iTween.MoveTo(showOffBoard , iTween.Hash("y", target, "islocal",true, "time",0.5f,   "easeType",iTween.EaseType.easeInBack, "ignoretimescale", true));
		}
		else{
			float target = -9.74f;
			iTween.MoveTo(showOffBoard , iTween.Hash("y", target, "islocal",true, "time",0.5f,   "easeType",iTween.EaseType.easeOutBack, "ignoretimescale", true));
		}
		
	}

	public void PressedShowOff()
	{
		if(trSpots.Exists( (obj)=> !obj.gameObject.activeInHierarchy)) {
			return;
		}
		Reset();
		GameController.Instance.DoShowOff();

	}

	public void ReceivedExcitement()
	{
		Shuffle<Transform>(trSpots);
		Transform trRandomLightOff = trSpots.Find((obj)=> !obj.gameObject.activeInHierarchy);
		if(trRandomLightOff != null) {
			trRandomLightOff.gameObject.SetActive(true);
		} 

		UpdateShowOff();
		GameController.Instance.levelMaintenance.SpotCount += 1;
	}

	public void ReceivedContagion()
	{
		Transform trRandomLightOn = trSpots.Find((obj)=> obj.gameObject.activeInHierarchy);
		if(trRandomLightOn != null) {
			trRandomLightOn.gameObject.SetActive(false);
		}
		UpdateShowOff();
		GameController.Instance.levelMaintenance.SpotCount -= 1;
	}

	public IList<T> Shuffle<T>(this IList<T> input) {

		for(int i=0; i<input.Count; i++) {
			var swap = UnityEngine.Random.Range(0,i);
			T tmp = input[i];
			input[i] = input[swap];
			input[swap] = tmp;
		}
		return input;
	}


}
