using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SmashBehaviour : MorphStateBehaviour {
	#region implemented abstract members of MorphStateBehaviour

	public override bool Enter (string enterGlobalStatus, Action enterAction)
	{
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().size = ColliderSize;
		State enterState = State.None;
		if(enterGlobalStatus == MorphController.STATUS_MORPHFROM) {
			enterState = State.smashMorphFrom;
		}
		Wake();
		if(enterState != State.None) {
			ForceInternalState(enterState, State.smashIdle, ()=>{

				if(enterAction != null)
					enterAction();
				GameInputController.TapScreenSides += OnTapScreenSides;
			});
		}
		else {

			if(enterAction != null)
				enterAction();
			GameInputController.TapScreenSides += OnTapScreenSides;
		}


		return true;
	}

	Action _exitAction = null;

	public override void Exit(string exitStatus,Action exitAction)
	{
		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;

		_exitAction = null;
		_exitAction = exitAction;

		State exitState = State.None;
		
		if(exitStatus == MorphController.STATUS_MORPHTO) {
			//Exit from smash while smashTo animation
			exitState = State.smashMorphTo;
		}

		AnimView currentView = GetAnimView(InternalState.ToString());
		currentView.Stop();
		currentView.SetActiveGameObject(false);
		
		if(exitState != State.None) {
			ForceInternalState(exitState, State.None, ()=>{
				Sleep();
				if(_exitAction != null)
					_exitAction();
			});
		}
		else {
			if(exitAction != null)
				exitAction();
			Sleep();
		}

	}
	
	public override void Wake ()
	{
		base.Wake ();
	}
	
	public override void Sleep ()
	{
		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;
		base.Sleep ();
	}
	public override string GetNameOnStorage {
		get {
			return "smash";
		}
	}
	public override Vector2 ColliderSize {
		get {
			return new Vector2(3.84f,1.2f);
		}
	}

	public override MorphState GetState {
		get {
			return MorphState.Smash;
		}
	}

	public override float GetIndicatorDuration {
		get {
			return 15f;
		}
	}
    public override bool CanWinScoreBallCollideWithItem
    {
        get
        {
            return true;
        }
    }
	public override void PlayIdle ()
	{
		InternalState = State.smashIdle;
	}

	public override void ForceEnableInputs (bool enable)
	{
		if(!enable){
			GameInputController.ResetInputs();
			GameInputController.TapScreenSides -= OnTapScreenSides;
			return;
		}
		GameInputController.TapScreenSides += OnTapScreenSides;
	}

	public void ForceInternalState(SmashBehaviour.State forcedState,SmashBehaviour.State playAfterForcedState, Action end = null) {
		
		AnimView forcedStateView = GetAnimView(forcedState.ToString());

		forcedStateView.OnAnimationEnded += ()=>{
			if(InternalState == forcedState) {
				if(playAfterForcedState != State.None)
					InternalState = playAfterForcedState;
				if(end != null)
					end(); //forced animasyon çalışırken başka bir animasyonla kesilirse, burası çalışmayabilir.
			}
		};

		InternalState = forcedState;

	}

	public override void DoShowOff ()
	{
		Time.timeScale = 0;
		SetTimeScaleIgnore(true);
		ForceInternalState(State.smashThunderClap, State.smashIdle, ()=> {
			Time.timeScale = 1;
			StartCoroutine(StartStrikeShowOff());
			SetTimeScaleIgnore(false);
		});

	}

	IEnumerator StartStrikeShowOff()
	{
		Debug.Log("StartStrikeShowOff");

		

		List<LevelCell> cells = GameController.Instance.levelMaintenance.GetEditorCells.FindAll((obj)=> obj.HasControlFromBoard);
		cells.Sort((x, y) => y.row.CompareTo(x.row));
		int iNum = 0;
		while(iNum < cells.Count) {
			cells[iNum].ApplyStrike(StrikeType.Strike_1X);
			iNum += 1;
		}

		yield break;
	}


	public override void SetDeath (System.Action ended)
	{
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().enabled = false;
//		GameController.Instance.morphController.State = MorphState.Morpheus;
//		((MorpheusBehaviour)(GameController.Instance.morphController.CurrentBehavior)).ForceInternalState(MorpheusBehaviour.State.morphFrom , MorpheusBehaviour.State.None, ()=>{
//			Debug.Log("morphFrom ended");
//			((MorpheusBehaviour)(GameController.Instance.morphController.CurrentBehavior)).SetDeath(ended);
//		});
	}

	public override event System.Action<GameInputController.TouchArea> OnCharacterStartedMoving;
	public override event System.Action OnCharacterEndedMoving;
	public override event System.Action OnCharacterMoving;

	#endregion

	public enum State{
		None,
		smashMorphFrom,
		smashMorphTo,
		smashIdle,
		smashMoveRight,
		smashMoveLeft,
		smashMoveRightReverse,
		smashMoveLeftReverse,
		smashThunderClap
	}
	protected State internalState = State.smashIdle;
	protected State previousInternalState;


	protected State InternalState {
		get {
			return internalState;
		}
		set{
			Debug.Log("InternalState "+value);
			//Exit from current state
			AnimView currentView = GetAnimView(internalState.ToString());
			currentView.Stop();
			currentView.SetActiveGameObject(false);
			

			if(internalState == State.smashMoveLeftReverse || internalState == State.smashMoveRightReverse) {
				currentView.OnAnimationEnded -= OnReverseMoveCompleted;
			}
			
			
			previousInternalState = internalState;
			internalState = value;
			
			currentView = null;
			currentView = GetAnimView(internalState.ToString());
			

			if(internalState == State.smashMoveLeftReverse || internalState == State.smashMoveRightReverse) {
				currentView.OnAnimationEnded += OnReverseMoveCompleted;
			}
			
			currentView.SetActiveGameObject(true);
			currentView.Play();
		}
	}
	
	#region OnAnimationEnded
	
	
	void OnMorphToAnimEnded()
	{
		InternalState = State.smashIdle;
	}
	
	void OnReverseMoveCompleted()
	{
		//Reverse olurken "input began"'i ignore ettigimiz icin simdi check ediyoruz.
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.smashMoveLeft;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.smashMoveRight;
		}
		else
			InternalState = State.smashIdle;
		
	}
	
	#endregion
	
	
	
	void OnTapScreenSides(FingerGestures.FingerPhase phase, GameInputController.TouchArea touchedSide)
	{
		//Ayni anda iki "input began" gelmez.Reverse oluyorsa aktif yonu animasyon bitince check edecek ve o yone donecek
		if(phase == FingerGestures.FingerPhase.Began && (InternalState != State.smashMoveLeftReverse && InternalState != State.smashMoveRightReverse)) {
			if(touchedSide == GameInputController.TouchArea.Left && (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.leftCharacterRestriction))
			{
				InternalState =  State.smashMoveLeft;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else if(touchedSide == GameInputController.TouchArea.Right && (GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.rightCharacterRestriction))
			{
				InternalState = State.smashMoveRight;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else {
				if(InternalState != State.smashIdle)
					InternalState = State.smashIdle;
			}
		}
		else if(phase == FingerGestures.FingerPhase.Ended) {
			if(InternalState == State.smashIdle)
				return;
			InternalState = InternalState == State.smashMoveRight ? State.smashMoveRightReverse : State.smashMoveLeftReverse;
			if(OnCharacterEndedMoving != null)
				OnCharacterEndedMoving();
		}
	}
	
	
	void Update()
	{
		if(InternalState == State.smashMoveLeft){
			if((GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.leftCharacterRestriction)){
				if(InternalState != State.smashIdle)
					InternalState = State.smashIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition-0.05f , GameController.Instance.gameModel.characterSpeed);
		}
		else if(InternalState == State.smashMoveRight) {
			if (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.rightCharacterRestriction) {
				if(InternalState != State.smashIdle)
					InternalState = State.smashIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
			
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition+0.05f , GameController.Instance.gameModel.characterSpeed);
			if(OnCharacterMoving != null)
				OnCharacterMoving();
		}
	}


}
