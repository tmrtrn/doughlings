using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Utility : MonoBehaviour {
	
	private static Utility _instance;
	
	void Awake(){
		if(_instance == null ) {
			_instance = this;
		}
	}
	void Start()
	{
		
	}

	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
	
	public static Utility Instance {
		get {
			if(_instance == null)
				_instance = FindObjectOfType(typeof(Utility)) as Utility;
			return _instance;
		}
	}
	
	#region Timer Events
	
	List<TimerProperty> intervals = new List<TimerProperty>();
	
	public void Wait(float time, Action endAction){
		TimerProperty property = new TimerProperty();
		StartCoroutine(property.OnlyWait(time,endAction));
	}
	
	
	
	public int AddTimerEvent(float interval, Action updatedAction,float delay = 0)
	{
		int id = 0;
		
		while( intervals.Find((obj) => obj.id == id) != null){
			id +=1;
		}
		TimerProperty property = new TimerProperty();
		property.id = id;
		property.interval = interval;
		property.updatedAction = updatedAction;
		intervals.Add(property);
		
		property.isPaused = false;
		property.running = true;
		StartCoroutine(property.StartTimerLoop(delay));
		
		return id;
	}
	
	public void ChangeTimerInterval(int id, float updatedValue)
	{
		intervals.Find((obj) => obj.id == id).interval = updatedValue;
	}
	
	public void RemoveTimerEvent(int id)
	{
		Debug.Log("RemoveTimerEvent "+id);
		TimerProperty selected =  intervals.Find((obj) => obj.id == id);
		selected.Stop();
		intervals.Remove(selected);
	}
	
	
	public void PauseAllTimer()
	{
		intervals.ForEach((obj) => obj.Pause());
	}
	
	public void StopAllTimer()
	{
		
		intervals.ForEach((obj) => obj.Stop());
		intervals.Clear();
	}
	
	
	
	public class TimerProperty
	{
		public int id;
		public float interval;
		public Action updatedAction;
		public bool isPaused = false;
		public bool running = false;
		
		
		public void Pause()
		{
			isPaused = true;
		}
		
		public void Stop()
		{
			running = false;
			updatedAction = null;
		}
		
		public IEnumerator StartTimerLoop(float delay =0)
		{
			bool delayed = delay > 0 ? false : true;
			while(running){
				if(!delayed) {
					yield return new WaitForSeconds(delay);
					delayed = true;
				}
				while (isPaused && running) 
				{
					yield return new WaitForFixedUpdate();	
				}
				if(!isPaused && running){
					yield return new WaitForSeconds(interval);
					if(updatedAction != null)
						updatedAction();
				}
				
			}
			yield return null;
		}
		
		public IEnumerator OnlyWait(float time, Action endAction)
		{
			yield return new WaitForSeconds(time);
			endAction();
			yield return null;
		}
	}
	
	#endregion
}