using UnityEngine;
using System;
using System.Collections;

public class MorpheusBehaviour : MorphStateBehaviour {

	public override bool Enter (string enterGlobalStatus, Action enterAction)
	{
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().size = ColliderSize;
		State enterState = State.None;

		if(enterGlobalStatus == MorphController.STATUS_MORPHTO) {
			enterState = State.morphFrom;
		}


		Wake();

		if(enterState != State.None) {
			ForceInternalState(enterState, State.None, ()=>{

				if(enterAction != null)
					enterAction();

			});
		}
		else {

			if(enterAction != null)
				enterAction();

		}


		return true;
	}



	public override void Exit(string enterGlobalStatus, Action exitAction)
	{
		colorBallRight = 0;
		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;

		State exitState = State.None;

		if(enterGlobalStatus == MorphController.STATUS_MORPHFROM) {
			//Exit from morpheus while morphTo animation
			exitState = State.morphTo;
		}

		AnimView currentView = GetAnimView(internalState.ToString());
		currentView.Stop();


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

	public override void Sleep ()
	{
		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;
		base.Sleep ();
	}

	public override void PlayIdle ()
	{
		InternalState = State.idle;
	}


	public override string GetNameOnStorage {
		get {
			return "morpheus";
		}
	}
	public override bool CanWinScoreBallCollideWithItem {
		get {
			return true;
		}
	}

	public override Vector2 ColliderSize {
		get {
			return new Vector2(2,0.8f);
		}
	}

	public override MorphState GetState {
		get {
			return MorphState.Morpheus;
		}
	}

	public int colorBallRight = 0;

	public override void DoShowOff ()
	{		
	   	colorBallRight += 3;
		if(GameController.Instance.activeBalls.Find((obj) => obj.ballType == BallTest.BallType.ColorBall) == null) {
			InstantiateColorBall();
		}

	}

	public bool InstantiateColorBall()
	{

		if(colorBallRight == 0) {

			return false;
		}
		GameController.Instance.activeBalls.ForEach((obj) => obj.DestroyBall());
		GameController.Instance.activeBalls.Clear();

		colorBallRight -= 1;
		BallTest activeBall = null;

		InternalState = State.ballCreate;

		activeBall = GameController.Instance.morphController.CreateBall(BallTest.BallType.ColorBall, () =>
		                                                                {
			((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(activeBall, true);
			
		}, GameController.Instance.ballPrefab);
		GameController.Instance.activeBalls.Add(activeBall);
		return true;
	}
   
	

	public override void DestroyBehaviourObject ()
	{
		base.DestroyBehaviourObject ();
	}

	public override void ForceEnableInputs (bool enable)
	{
		if(!enable){
			GameInputController.ResetInputs();
			GameInputController.TapScreenSides -= OnTapScreenSides;
			return;
		}
		else
		       GameInputController.TapScreenSides += OnTapScreenSides;
	}


	Action deathAnimCompleted = null;

	public override void SetDeath (Action ended)
	{
		SetTimeScaleIgnore(true);
		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().enabled = false;
		deathAnimCompleted = ended;
		InternalState = State.death;
	}


	public enum State{None,ballCreate, born, damageIdle, damageMoveLeft, damageMoveRight, death, idle,
		morphFrom, morphTo, moveLeft, moveRight, moveLeftReverse, moveRightReverse, victoryStart, victory }
	protected State internalState = State.ballCreate;
	protected State previousInternalState;
	
	
	protected State InternalState {
		get {
			return internalState;
		}
		set{
			//Exit from current state
			AnimView currentView = GetAnimView(internalState.ToString());
			currentView.Stop();
			currentView.SetActiveGameObject(false);
			
			
			
			if(internalState == State.born) {
				currentView.OnAnimationEnded -= OnBornAnimCompleted;
			}
			else if(internalState == State.ballCreate) {
				currentView.OnAnimationEnded -= OnBallCreateAnimCompleted;
			}
			
			else if(internalState == State.death) {
				currentView.OnAnimationEnded -= OnDeathAnimationEnded;
			}
			else if(internalState == State.moveLeftReverse || internalState == State.moveRightReverse) {
				currentView.OnAnimationEnded -= OnReverseMoveCompleted;
			}
			else if(internalState == State.damageIdle || internalState == State.damageMoveLeft || internalState == State.damageMoveRight) {
				currentView.OnAnimationEnded -= OnDamageAnimEnded;
			}
			
			
			previousInternalState = internalState;
			internalState = value;
			
			currentView = null;
			currentView = GetAnimView(internalState.ToString());
			
			
			if(internalState == State.born) {
				currentView.OnAnimationEnded += OnBornAnimCompleted;
			}
			else if(internalState == State.ballCreate) {
				currentView.OnAnimationEnded += OnBallCreateAnimCompleted;
			}
			else if(internalState == State.death) {
				currentView.OnAnimationEnded += OnDeathAnimationEnded;
			}
			else if(internalState == State.moveLeftReverse || internalState == State.moveRightReverse) {
				currentView.OnAnimationEnded += OnReverseMoveCompleted;
			}
			else if(internalState == State.damageIdle || internalState == State.damageMoveLeft || internalState == State.damageMoveRight) {
				currentView.OnAnimationEnded += OnDamageAnimEnded;
			}
			

			currentView.SetActiveGameObject(true);
			currentView.Play();
		}
	}


	public void ForceInternalState(MorpheusBehaviour.State forcedState,MorpheusBehaviour.State playAfterForcedState, Action end = null) {

		GetAnimView(InternalState.ToString()).Stop();

		AnimView forcedStateView = GetAnimView(forcedState.ToString());
		forcedStateView.OnAnimationEnded += ()=>{
			if(InternalState == forcedState) {
				if(end != null)
					end(); //forced animasyon çalışırken başka bir animasyonla kesilirse, burası çalışmayabilir.
				if(playAfterForcedState != State.None)
					InternalState = playAfterForcedState;
			}
		};
		InternalState = forcedState;
		
	}


	public override void ReceivedDamage ()
	{
		
		
		if(InternalState == State.moveLeft)
			InternalState = State.damageMoveLeft;
		else if(InternalState == State.moveRight)
			InternalState = State.damageMoveRight;
		else 
			InternalState = State.damageIdle;
		
		
	}

	void OnDamageAnimEnded()
	{
		if(previousInternalState == State.moveLeft || previousInternalState == State.moveLeftReverse
		   || previousInternalState == State.moveRight || previousInternalState == State.moveRightReverse) {
			GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
			if(currentTouchArea == GameInputController.TouchArea.Left) {
				InternalState = State.moveLeft;
			}
			else if(currentTouchArea == GameInputController.TouchArea.Right) {
				InternalState = State.moveRight;
			}
			else {
				InternalState = State.idle;
			}
		}
		else {
			InternalState = State.idle;
		}
		
	}
	
	
	void OnBornAnimCompleted()
	{
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().enabled = true;
		InternalState = State.idle;
	}
	
	void OnBallCreateAnimCompleted()
	{
		InternalState = State.idle;
	}
	
	void OnReverseMoveCompleted()
	{
		//Reverse olurken "input began"'i ignore ettigimiz icin simdi check ediyoruz.
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.moveLeft;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.moveRight;
		}
		else
			InternalState = State.idle;
		
	}
	
	
	void OnDeathAnimationEnded()
	{
		if(deathAnimCompleted != null) {
			deathAnimCompleted();
			deathAnimCompleted = null;
		}
	//	Sleep(); //Exit();
	}


	
	
	public override event Action<GameInputController.TouchArea> OnCharacterStartedMoving;
	public override event Action OnCharacterEndedMoving;
	public override event Action OnCharacterMoving;
	
	
	void OnTapScreenSides(FingerGestures.FingerPhase phase, GameInputController.TouchArea touchedSide)
	{
		
		if(InternalState == State.damageMoveLeft || InternalState == State.damageMoveLeft || InternalState == State.damageMoveRight)
			return;
		//Ayni anda iki "input began" gelmez.Reverse oluyorsa aktif yonu animasyon bitince check edecek ve o yone donecek
		if(phase == FingerGestures.FingerPhase.Began && (InternalState != State.moveLeftReverse && InternalState != State.moveRightReverse)) {
			if(touchedSide == GameInputController.TouchArea.Left && (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.leftCharacterRestriction))
			{
				InternalState =  State.moveLeft;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else if(touchedSide == GameInputController.TouchArea.Right && (GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.rightCharacterRestriction))
			{
				InternalState = State.moveRight;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else {
				if(InternalState != State.idle)
					InternalState = State.idle;
			}
		}
		else if(phase == FingerGestures.FingerPhase.Ended) {
			if(InternalState == State.idle)
				return;
			InternalState = InternalState == State.moveRight ? State.moveRightReverse : State.moveLeftReverse;
			if(OnCharacterEndedMoving != null)
				OnCharacterEndedMoving();
		}
	}
	
	void Update()
	{

		if(InternalState == State.moveLeft){
			if((GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.leftCharacterRestriction)){
				if(InternalState != State.idle)
					InternalState = State.idle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition-0.05f , GameController.Instance.gameModel.characterSpeed);
		}
		else if(InternalState == State.moveRight) {
			if (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.rightCharacterRestriction) {
				if(InternalState != State.idle)
					InternalState = State.idle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
			
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition+0.05f , GameController.Instance.gameModel.characterSpeed);
			if(OnCharacterMoving != null)
				OnCharacterMoving();
		}
	}




}
