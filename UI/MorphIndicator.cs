using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MorphIndicator : MonoBehaviour {
	
	public enum IndicatorType{
		None,
		Smash,
		Robo
	}
	
	public List<MorphIndicatorComponent> components;
	private MorphState currentMorphType;
	
	
	// Use this for initialization
	void Awake () {
		components.ForEach( (obj)=> obj.Exit());
	}
	
	public MorphState GetState()
	{
		return currentMorphType;
	}
	public void SetState(MorphState newMorphState, float duration)
	{
		if(currentMorphType != null)
			components.ForEach((obj)=> obj.Exit());
		currentMorphType = newMorphState;
		components.ForEach((obj)=> obj.Enter(newMorphState, duration, StateEnded));
	}
	
	void StateEnded()
	{

		GameController.Instance.morphController.SetState(MorphState.Morpheus, MorphController.STATUS_MORPHTO, ()=>{
			((MorpheusBehaviour)(GameController.Instance.morphController.CurrentBehavior)).ForceEnableInputs(true);
			((MorpheusBehaviour)(GameController.Instance.morphController.CurrentBehavior)).ForceInternalState(MorpheusBehaviour.State.idle, MorpheusBehaviour.State.None);
		});
	}
	
	
}
