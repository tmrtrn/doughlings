using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameInputController : MonoBehaviour {
	
	public enum TouchArea{None,Left,Right,Middle}
    	public static event Action<Vector2> OneActionFingerTap;
	public static event Action OnTouchedScreen;
	//Used to translate character
	public static event Action<FingerGestures.FingerPhase, TouchArea> TapScreenSides;

	
	static List<TouchedFinger> listTouched = new List<TouchedFinger>();
	
	class TouchedFinger
	{
		int fingerId = -1;
		bool isActive = false;
		TouchArea chargeWithTouchArea;
		FingerGestures.Finger finger;
		public bool forceTrueForTest = false;
		bool isKeyboard = false;
		
		public TouchedFinger(TouchArea chargeWithTouchArea, FingerGestures.Finger finger, bool isKeyboard = false)
		{
			this.chargeWithTouchArea = chargeWithTouchArea;
			this.finger = finger;
			isActive = false;
			this.isKeyboard = isKeyboard;
		}
		
		public bool Active
		{
			get{
				return isActive;
			}
			set{
				isActive = value;
				if(TapScreenSides == null)
					return;
				if(isActive)
					TapScreenSides(FingerGestures.FingerPhase.Began , chargeWithTouchArea);
				else
					TapScreenSides(FingerGestures.FingerPhase.Ended , chargeWithTouchArea);
			}
		}
		

		public bool CheckFingerUp()
		{
			if(forceTrueForTest)
				return true;
			if(isKeyboard)
				return false;
			if(finger.Phase == FingerGestures.FingerPhase.Ended || finger.Phase == FingerGestures.FingerPhase.None)
				return true;
			if(GetTouchedArea(finger.Position) != chargeWithTouchArea){
				return true;
			}
			return false;
		}
		
		public TouchArea ChargeWithTouchArea{
			get{
				return chargeWithTouchArea;
			}
		}
	}

    void OnEnable()
    {
        FingerGestures.OnFingerDown  += OnFingerDown;
		FingerGestures.OnTap += OnTap;
    }
	
	void OnDisable()
	{
		listTouched.Clear();
		FingerGestures.OnFingerDown  -= OnFingerDown;
		FingerGestures.OnTap -= OnTap;
	}
	
	public static TouchArea GetTouchedArea(Vector2 fingerPos)
	{	
		if(fingerPos.x < Screen.width * 0.25f) {
			return TouchArea.Left;
		}
		else if(fingerPos.x >= Screen.width * 0.25f && fingerPos.x <= Screen.width * 0.75f) {
			return TouchArea.Middle;
		}
		else if(fingerPos.x > Screen.width * 0.75f) {
			return TouchArea.Right;
		}
		return TouchArea.None;
	}
	
	bool touched = false;
	
	
	
	void OnFingerDown(int fingerIndex, Vector2 fingerPos )
	{
		if(GUIController.Instance.DoesTapHitGUI(fingerPos))
			return;
		
		TouchArea pressedTouchArea = GetTouchedArea(fingerPos);
		if(pressedTouchArea == TouchArea.Left || pressedTouchArea == TouchArea.Right) {
			TouchedFinger touchedFinger = new TouchedFinger(pressedTouchArea, FingerGestures.GetFinger(fingerIndex));
			listTouched.Add(touchedFinger);
		}
		
	}
	/*
	void OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		if(touched)
			UpdateActions(FingerGestures.FingerPhase.Ended, fingerPos);
		touched = false;
	}
	
	*/
    void OnTap(Vector2 fingerPos) { 
		
		if(GUIController.Instance.DoesTapHitGUI(fingerPos))
			return;
		if(fingerPos.x > Screen.width* 0.25f  && fingerPos.x < Screen.width*0.75f ) {
			if(OneActionFingerTap != null)
				OneActionFingerTap(fingerPos);
		}
		if(OnTouchedScreen != null)
			OnTouchedScreen();
    }


	public static void ResetInputs()
	{
		TouchedFinger touchedFinger = listTouched.Find((obj) => obj.Active == true);
		if(touchedFinger == null)
			return;
		listTouched.Clear();
		touchedFinger.Active = false;

	}
	
	public static TouchArea GetCurrentTouchArea()
	{
		TouchedFinger touchedFinger = listTouched.Find((obj) => obj.Active == true);
		if(touchedFinger == null)
			return TouchArea.None;
		return touchedFinger.Active ? touchedFinger.ChargeWithTouchArea : TouchArea.None;
	}
	
	TouchedFinger touchedFingerLeft;
	TouchedFinger touchedFingerRight;
	
	void Update()
	{

		TouchedFinger activeTouch = listTouched.Find((obj) => obj.Active == true);
		if(activeTouch == null) {
			if(listTouched.Count > 0){
				activeTouch = listTouched[0];
				listTouched[0].Active = true;
			}
				
		}
		if(activeTouch != null) {
			if(activeTouch.CheckFingerUp()) {
				listTouched.Remove(activeTouch);
				activeTouch.Active = false;
			}
		}
		
		
		
		if(Application.isEditor && Application.isPlaying) {
			if(Input.GetKeyDown(KeyCode.LeftArrow)){
				if(TapScreenSides != null){
					touchedFingerLeft = new TouchedFinger(TouchArea.Left, null, true);
					listTouched.Add(touchedFingerLeft);
				}
					//TapScreenSides(FingerGestures.FingerPhase.Began , TouchArea.Left);
			}
			else if(Input.GetKeyUp(KeyCode.LeftArrow)) {
				if(TapScreenSides != null){
					touchedFingerLeft.forceTrueForTest = true;
				}
			//		TapScreenSides(FingerGestures.FingerPhase.Ended , TouchArea.Left);
			}
			
			if(Input.GetKeyDown(KeyCode.RightArrow)){
				if(TapScreenSides != null) {
					touchedFingerRight = new TouchedFinger(TouchArea.Right, null, true);
					listTouched.Add(touchedFingerRight);
				}
			//		TapScreenSides(FingerGestures.FingerPhase.Began , TouchArea.Right);
			}
			else if(Input.GetKeyUp(KeyCode.RightArrow)) {
				if(TapScreenSides != null)
					touchedFingerRight.forceTrueForTest = true;
			//		TapScreenSides(FingerGestures.FingerPhase.Ended , TouchArea.Right);
			}
		}
		
	}
	
	void LateUpdate()
	{
		
	}
}
