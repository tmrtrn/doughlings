using UnityEngine;
using System.Collections;
using System;

public class GunnerBehaviour : MorphStateBehaviour {

	public GameObject goGunBulletPrefab;
	public Transform trBulletExistLeftAnchor;
	public Transform trBulletExitsRightAnchor;
	public Transform trBulletExistMoveLeftAnchor;
	public Transform trBulletExistMoveRightAnchor;

	#region implemented abstract members of MorphStateBehaviour

	public override bool Enter (string enterGlobalStatus, System.Action enterAction)
	{
		GUIController.OnPausePressed += HandlePause;
		GUIController.OnResumePressed += HandleResume;

		GameController.Instance.morphController.GetComponent<BoxCollider2D>().size = ColliderSize;
		State enterState = State.None;
		if(enterGlobalStatus == MorphController.STATUS_MORPHFROM) {
			enterState = State.gunnerMorphFrom;
		}
		Wake();
		if(enterState != State.None) {
			ForceInternalState(enterState, State.gunnerIdle, ()=>{
				
				if(enterAction != null)
					enterAction();
				GameInputController.TapScreenSides += OnTapScreenSides;
				GameInputController.OneActionFingerTap += HandleOneActionFingerTap;
			});
		}
		else {
			
			if(enterAction != null)
				enterAction();
			GameInputController.TapScreenSides += OnTapScreenSides;
			GameInputController.OneActionFingerTap += HandleOneActionFingerTap;
		}
		
		return true;
	}



	public override void Exit (string exitStatus, System.Action exitAction)
	{
		GUIController.OnPausePressed -= HandlePause;
		GUIController.OnResumePressed -= HandleResume;

		Time.timeScale = 1;
		SetTimeScaleIgnore(false);
		GameController.Instance.gameHUDScreen.morphIndicator.components.FindAll( (obj)=> obj.HasTween).ForEach((obj)=> {
			iTween tween = obj.gameObject.GetComponent<iTween>();
			if(tween != null) {
				tween.useRealTime = false;
			}
		});

		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;
		GameInputController.OneActionFingerTap -= HandleOneActionFingerTap;
		
		
		State exitState = State.None;
		
		if(exitStatus == MorphController.STATUS_MORPHTO) {
			//Exit from smash while smashTo animation
			exitState = State.gunnerMorphTo;
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
			return "gunner";
		}
	}

	public override MorphState GetState {
		get {
			return MorphState.Gunner;
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
		InternalState = State.gunnerIdle;
	}

	public override float GetIndicatorDuration {
		get {
			return 15f;
		}
	}

	void HandlePause()
	{
		GameController.Instance.gameHUDScreen.morphIndicator.components.FindAll( (obj)=> obj.HasTween).ForEach((obj)=> {
			iTween tween = obj.gameObject.GetComponent<iTween>();
			if(tween != null) {
				tween.useRealTime = false;
			}
		});
	}

	void HandleResume()
	{
		GameController.Instance.gameHUDScreen.morphIndicator.components.FindAll( (obj)=> obj.HasTween).ForEach((obj)=> {
			iTween tween = obj.gameObject.GetComponent<iTween>();
			if(tween != null) {
				tween.useRealTime = true;
			}
		});
	}



	public override void DoShowOff ()
	{
		SetTimeScaleIgnore(true);
		((GameHUDScreen)(GUIController.Instance.currentScreen)).morphIndicator.components.FindAll( (obj)=> obj.HasTween).ForEach((obj)=> {
			iTween tween = obj.gameObject.GetComponent<iTween>();
			if(tween != null) {
				tween.useRealTime = true;
			}
		});

		Time.timeScale = 0.3f;
	}

	#endregion

	public override event System.Action<GameInputController.TouchArea> OnCharacterStartedMoving;
	public override event System.Action OnCharacterEndedMoving;
	public override event System.Action OnCharacterMoving;


	public enum State{
		None,
		gunnerIdle,
		gunnerMorphFrom,
		gunnerMorphTo,
		gunnerMoveRight,
		gunnerMoveLeft,
		gunnerMoveRightReverse,
		gunnerMoveLeftReverse,
		gunnerDamageIdle,
		gunnerDamageMoveLeft,
		gunnerDamageMoveRight,
		gunnerShootMoveLeft,
		gunnerShootMoveRight,
		gunnerShootMoveLeftReverse,
		gunnerShootMoveRightReverse,
		gunnerShootOdd,
		gunnerShootEven
	}
	protected State internalState = State.gunnerIdle;
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
			
			
			if(internalState == State.gunnerMoveLeftReverse || internalState == State.gunnerMoveRightReverse) {
				currentView.OnAnimationEnded -= OnReverseMoveCompleted;
			}
			else if(internalState == State.gunnerDamageIdle || internalState == State.gunnerDamageMoveLeft || internalState == State.gunnerDamageMoveRight) {
				currentView.OnAnimationEnded -= OnDamageAnimEnded;
			}
			else if(internalState == State.gunnerShootOdd || internalState == State.gunnerShootEven || internalState == State.gunnerShootMoveLeft || internalState == State.gunnerShootMoveRight) {
				currentView.OnAnimationEnded -= OnShootAnimEnded;
			}
			else if(internalState == State.gunnerShootMoveLeftReverse || internalState == State.gunnerShootMoveRightReverse) {
				currentView.OnAnimationEnded -= OnReverseShootCompleted;
			}

			
			previousInternalState = internalState;
			internalState = value;
			
			currentView = null;
			currentView = GetAnimView(internalState.ToString());


			
			if(internalState == State.gunnerMoveLeftReverse || internalState == State.gunnerMoveRightReverse) {
				currentView.OnAnimationEnded += OnReverseMoveCompleted;
			}
			else if(internalState == State.gunnerDamageIdle || internalState == State.gunnerDamageMoveLeft || internalState == State.gunnerDamageMoveRight) {
				currentView.OnAnimationEnded += OnDamageAnimEnded;
			}
			else if(internalState == State.gunnerShootOdd || internalState == State.gunnerShootEven || internalState == State.gunnerShootMoveLeft || internalState == State.gunnerShootMoveRight) {
				currentView.OnAnimationEnded += OnShootAnimEnded;
			}
			else if(internalState == State.gunnerShootMoveLeftReverse || internalState == State.gunnerShootMoveRightReverse) {
				currentView.OnAnimationEnded += OnReverseShootCompleted;
			}
			
			currentView.SetActiveGameObject(true);
			currentView.Play();
		}
	}
	
	
	void OnMorphToAnimEnded()
	{
		InternalState = State.gunnerIdle;
	}
	
	void OnReverseMoveCompleted()
	{
		//Reverse olurken "input began"'i ignore ettigimiz icin simdi check ediyoruz.
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.gunnerMoveLeft;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.gunnerMoveRight;
		}
		else
			InternalState = State.gunnerIdle;
		
	}
	
	public override void ReceivedDamage ()
	{
		
		
		if(InternalState == State.gunnerMoveLeft)
			InternalState = State.gunnerDamageMoveLeft;
		else if(InternalState == State.gunnerMoveRight)
			InternalState = State.gunnerDamageMoveRight;
		else 
			InternalState = State.gunnerDamageIdle;
		
		
	}
	
	void OnDamageAnimEnded()
	{
		if(previousInternalState == State.gunnerMoveLeft || previousInternalState == State.gunnerMoveLeftReverse
		   || previousInternalState == State.gunnerMoveRight || previousInternalState == State.gunnerMoveRightReverse) {
			GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
			if(currentTouchArea == GameInputController.TouchArea.Left) {
				InternalState = State.gunnerMoveLeft;
			}
			else if(currentTouchArea == GameInputController.TouchArea.Right) {
				InternalState = State.gunnerMoveRight;
			}
			else {
				InternalState = State.gunnerIdle;
			}
		}
		else {
			InternalState = State.gunnerIdle;
		}
		
	}

	void OnShootAnimEnded()
	{
		IsShooting = false;
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.gunnerShootMoveLeftReverse;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.gunnerMoveRightReverse;
		}
		else {
			InternalState = State.gunnerIdle;
		}
//		if(previousInternalState == State.gunnerMoveLeft || previousInternalState == State.gunnerMoveLeftReverse
//		   || previousInternalState == State.gunnerMoveRight || previousInternalState == State.gunnerMoveRightReverse) {
//			GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
//			if(currentTouchArea == GameInputController.TouchArea.Left) {
//				InternalState = State.gunnerShootMoveLeftReverse;
//			}
//			else if(currentTouchArea == GameInputController.TouchArea.Right) {
//				InternalState = State.gunnerMoveRightReverse;
//			}
//			else {
//				InternalState = State.gunnerIdle;
//			}
//		}
//		else {
//			InternalState = State.gunnerIdle;
//		}

	}

	void OnReverseShootCompleted()
	{
		//Reverse olurken "input began"'i ignore ettigimiz icin simdi check ediyoruz.
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.gunnerMoveLeft;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.gunnerMoveRight;
		}
		else
			InternalState = State.gunnerIdle;
		
	}
	
	
	
	public void ForceInternalState(GunnerBehaviour.State forcedState,GunnerBehaviour.State playAfterForcedState, Action end = null) {
		
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

	bool IsShooting = false;
	bool IsOdd = false;

	void HandleOneActionFingerTap (Vector2 obj)
	{
		IsShooting = true;

		Vector3 anchorPosition = Vector3.zero;
		float bulletFireDelay = 0;

//		Debug.Log(InternalState);

		if(InternalState == State.gunnerMoveLeft) {
			InternalState = State.gunnerShootMoveLeft;
		}
			
		else if(InternalState == State.gunnerMoveRight) {
			InternalState = State.gunnerShootMoveRight;
		}
			
		else if(InternalState != State.gunnerShootMoveRight && InternalState != State.gunnerShootMoveLeft) {
			InternalState =  IsOdd ? State.gunnerShootOdd : State.gunnerShootEven;
			IsOdd = !IsOdd;
			bulletFireDelay = 0.1f;
		}
		else if(InternalState == State.gunnerShootMoveRight ) {
			InternalState = State.gunnerShootMoveRight;
		}
		else if(InternalState == State.gunnerShootMoveLeft) {
			InternalState = State.gunnerShootMoveLeft;
		}
			
		else {

		}

		if(internalState == State.gunnerShootOdd)
			anchorPosition = trBulletExistLeftAnchor.position;
		else if(InternalState == State.gunnerShootEven)
			anchorPosition = trBulletExitsRightAnchor.position;
		else if(InternalState == State.gunnerShootMoveRight)
			anchorPosition = trBulletExistMoveRightAnchor.position;
		else if(InternalState == State.gunnerShootMoveLeft)
			anchorPosition = trBulletExistMoveLeftAnchor.position;

		if(anchorPosition != Vector3.zero) {
			GameObject goBullet = GameObject.Instantiate(goGunBulletPrefab , anchorPosition, new Quaternion()) as GameObject;
			goBullet.transform.parent = GameController.Instance.trBaseGameAnchor;
			goBullet.GetComponent<GunnerBullet>().Fire(bulletFireDelay,false);
		}



	}
	
	
	void OnTapScreenSides(FingerGestures.FingerPhase phase, GameInputController.TouchArea touchedSide)
	{
		if(InternalState == State.gunnerDamageMoveLeft || InternalState == State.gunnerDamageMoveLeft || InternalState == State.gunnerDamageMoveRight)
			return;
		//Ayni anda iki "input began" gelmez.Reverse oluyorsa aktif yonu animasyon bitince check edecek ve o yone donecek
		if(phase == FingerGestures.FingerPhase.Began && (InternalState != State.gunnerMoveLeftReverse && InternalState != State.gunnerMoveRightReverse)) {
			if(touchedSide == GameInputController.TouchArea.Left && (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.leftCharacterRestriction))
			{
				InternalState =  State.gunnerMoveLeft;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else if(touchedSide == GameInputController.TouchArea.Right && (GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.rightCharacterRestriction))
			{
				InternalState = State.gunnerMoveRight;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else {
				if(InternalState != State.gunnerIdle)
					InternalState = State.gunnerIdle;
			}
		}
		else if(phase == FingerGestures.FingerPhase.Ended) {
			if(InternalState == State.gunnerIdle)
				return;
			State nextState = (InternalState == State.gunnerMoveRight) ? State.gunnerMoveRightReverse : State.gunnerMoveLeftReverse;
			if(InternalState == State.gunnerShootMoveRight)
				nextState = State.gunnerShootMoveRightReverse;
			else if(InternalState == State.gunnerShootMoveLeft)
				nextState = State.gunnerShootMoveLeftReverse;

			InternalState = nextState;
			if(OnCharacterEndedMoving != null)
				OnCharacterEndedMoving();
		}
	}
	
	void Update()
	{
		if(InternalState == State.gunnerMoveLeft || InternalState == State.gunnerShootMoveLeft){
			if((GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.leftCharacterRestriction)){
				if(InternalState != State.gunnerIdle)
					InternalState = State.gunnerIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}

		//	GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition-0.05f , GameController.Instance.gameModel.characterSpeed,true);
		}
		else if(InternalState == State.gunnerMoveRight || InternalState == State.gunnerShootMoveRight) {
			if (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.rightCharacterRestriction) {
				if(InternalState != State.gunnerIdle)
					InternalState = State.gunnerIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
			
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition+0.05f , GameController.Instance.gameModel.characterSpeed,true);
			if(OnCharacterMoving != null)
				OnCharacterMoving();
		}


	}



}
