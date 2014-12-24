using UnityEngine;
using System.Collections;
using System;

public class SpiderBehaviour : MorphStateBehaviour {

	private GameObject goShowOffWeb;

	public override bool Enter (string enterGlobalStatus, System.Action enterAction)
	{
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().size = ColliderSize;
		State enterState = State.None;
		if(enterGlobalStatus == MorphController.STATUS_MORPHFROM) {
			enterState = State.spiderMorphFrom;
		}
		Wake();
		if(enterState != State.None) {
			ForceInternalState(enterState, State.spiderIdle, ()=>{
				
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

		GameController.Instance.activeBalls.ForEach( (obj)=> obj.DestroyBall());
		GameController.Instance.activeBalls.Clear();



		return true;
	}
	public override void Exit (string exitStatus, System.Action exitAction)
	{
		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;

		if(goShowOffWeb != null)
			Destroy(goShowOffWeb);
		goShowOffWeb = null;
		
		
		State exitState = State.None;
		
		if(exitStatus == MorphController.STATUS_MORPHTO) {
			//Exit from smash while smashTo animation
			exitState = State.spiderMorphTo;
		}
		
		AnimView currentView = GetAnimView(InternalState.ToString());
		currentView.Stop();
		currentView.SetActiveGameObject(false);
		
		
		if(exitState != State.None) {
			ForceInternalState(exitState, State.None, ()=>{
				Sleep();
				if(exitAction != null)
					exitAction();

                

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

   
	public override void SetDeath (System.Action ended)
	{
		throw new System.NotImplementedException ();
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
	
	public override string GetNameOnStorage {
		get {
			return "spider";
		}
	}
	public override MorphState GetState {
		get {
			return MorphState.Spider;
		}
	}
	
	public override float GetIndicatorDuration {
		get {
			return 25f;
		}
	}
	
	public override bool CanWinScoreBallCollideWithItem
	{
		get
		{
			return true;
		}
	}
	
	public override Vector2 ColliderSize {
		get {
			return new Vector2(3.84f,1.2f);
		}
	}
	public override void PlayIdle ()
	{
		InternalState = State.spiderIdle;
	}

	public override void DoShowOff ()
	{
		ForceInternalState(State.spiderWebCreate, State.spiderIdle);
		goShowOffWeb = Instantiate(GameController.Instance.morphController.goSpiderWebPrefab) as GameObject;
		goShowOffWeb.transform.parent = GameController.Instance.trBaseGameAnchor;
		goShowOffWeb.transform.localPosition = new Vector3(9.6f, -10.42f, -2);

	}

	public override event System.Action<GameInputController.TouchArea> OnCharacterStartedMoving;
	public override event System.Action OnCharacterEndedMoving;
	public override event System.Action OnCharacterMoving;
	
	public enum State{
		None,
		spiderIdle,
		spiderMorphFrom,
		spiderMorphTo,
		spiderMoveRight,
		spiderMoveLeft,
		spiderMoveRightReverse,
		spiderMoveLeftReverse,
		spiderDamageIdle,
		spiderDamageMoveLeft,
		spiderDamageMoveRight,
		spiderBallCreate,
		spiderWebCreate
	}
	protected State internalState = State.None;
	protected State previousInternalState;
	
	
	protected State InternalState {
		get {
			return internalState;
		}
		set{
			
			//Exit from current state
			AnimView currentView = GetAnimView(internalState.ToString());
            if (currentView != null) {

                currentView.Stop();
                currentView.SetActiveGameObject(false);


                if (internalState == State.spiderMoveLeftReverse || internalState == State.spiderMoveRightReverse)
                {
                    currentView.OnAnimationEnded -= OnReverseMoveCompleted;
                }
                else if (internalState == State.spiderDamageIdle || internalState == State.spiderDamageMoveLeft || internalState == State.spiderDamageMoveRight)
                {
                    currentView.OnAnimationEnded -= OnDamageAnimEnded;
                }

            }

			
			
			
			previousInternalState = internalState;
			internalState = value;
			
			currentView = null;
			currentView = GetAnimView(internalState.ToString());
			
			
			if(internalState == State.spiderMoveLeftReverse || internalState == State.spiderMoveRightReverse) {
				currentView.OnAnimationEnded += OnReverseMoveCompleted;
			}
			else if(internalState == State.spiderDamageIdle || internalState == State.spiderDamageMoveLeft || internalState == State.spiderDamageMoveRight) {
				currentView.OnAnimationEnded += OnDamageAnimEnded;
			}
			
			currentView.SetActiveGameObject(true);
			currentView.Play();
		}
	}
	
	
	void OnMorphToAnimEnded()
	{
		InternalState = State.spiderIdle;
	}
	
	void OnReverseMoveCompleted()
	{
		//Reverse olurken "input began"'i ignore ettigimiz icin simdi check ediyoruz.
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.spiderMoveLeft;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.spiderMoveRight;
		}
		else
			InternalState = State.spiderIdle;
		
	}
	
	public override void ReceivedDamage ()
	{
		
		
		if(InternalState == State.spiderMoveLeft)
			InternalState = State.spiderDamageMoveLeft;
		else if(InternalState == State.spiderMoveRight)
			InternalState = State.spiderDamageMoveRight;
		else 
			InternalState = State.spiderDamageIdle;
		
		
	}
	
	void OnDamageAnimEnded()
	{
		if(previousInternalState == State.spiderMoveLeft || previousInternalState == State.spiderMoveLeftReverse
		   || previousInternalState == State.spiderMoveRight || previousInternalState == State.spiderMoveRightReverse) {
			GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
			if(currentTouchArea == GameInputController.TouchArea.Left) {
				InternalState = State.spiderMoveLeft;
			}
			else if(currentTouchArea == GameInputController.TouchArea.Right) {
				InternalState = State.spiderMoveRight;
			}
			else {
				InternalState = State.spiderIdle;
			}
		}
		else {
			InternalState = State.spiderIdle;
		}
		
	}
	
	
	
	public void ForceInternalState(SpiderBehaviour.State forcedState,SpiderBehaviour.State playAfterForcedState, Action end = null) {
		
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
	
	
	
	void OnTapScreenSides(FingerGestures.FingerPhase phase, GameInputController.TouchArea touchedSide)
	{
		if(InternalState == State.spiderDamageMoveLeft || InternalState == State.spiderDamageMoveLeft || InternalState == State.spiderDamageMoveRight)
			return;
		//Ayni anda iki "input began" gelmez.Reverse oluyorsa aktif yonu animasyon bitince check edecek ve o yone donecek
		if(phase == FingerGestures.FingerPhase.Began && (InternalState != State.spiderMoveLeftReverse && InternalState != State.spiderMoveRightReverse)) {
			if(touchedSide == GameInputController.TouchArea.Left && (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.leftCharacterRestriction))
			{
				InternalState =  State.spiderMoveLeft;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else if(touchedSide == GameInputController.TouchArea.Right && (GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.rightCharacterRestriction))
			{
				InternalState = State.spiderMoveRight;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else {
				if(InternalState != State.spiderIdle)
					InternalState = State.spiderIdle;
			}
		}
		else if(phase == FingerGestures.FingerPhase.Ended) {
			if(InternalState == State.spiderIdle)
				return;
			InternalState = InternalState == State.spiderMoveRight ? State.spiderMoveRightReverse : State.spiderMoveLeftReverse;
			if(OnCharacterEndedMoving != null)
				OnCharacterEndedMoving();
		}
	}
	
	void Update()
	{
		if(InternalState == State.spiderMoveLeft){
			if((GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.leftCharacterRestriction)){
				if(InternalState != State.spiderIdle)
					InternalState = State.spiderIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
//			GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition-0.05f , GameController.Instance.gameModel.characterSpeed);
		}
		else if(InternalState == State.spiderMoveRight) {
			if (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.rightCharacterRestriction) {
				if(InternalState != State.spiderIdle)
					InternalState = State.spiderIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
			
//			GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition+0.05f , GameController.Instance.gameModel.characterSpeed);
			if(OnCharacterMoving != null)
				OnCharacterMoving();
		}

        if (InternalState == State.spiderIdle && GameController.Instance.activeBalls.Count < (goShowOffWeb != null ? 4 : 2) && GameController.Instance.activeBalls.Find((obj)=> !obj.throwed) == null)
        {
            ForceInternalState(State.spiderBallCreate, State.spiderIdle);
            BallTest ball = null;
            ball = GameController.Instance.morphController.CreateBall(BallTest.BallType.SpiderBall, () => {
                ((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(ball, true);
            }, GameController.Instance.ballPrefab);
            GameController.Instance.activeBalls.Add(ball);
        }
        

	}

}
