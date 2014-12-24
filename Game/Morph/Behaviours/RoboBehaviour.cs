using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoboBehaviour : MorphStateBehaviour {
	#region implemented abstract members of MorphStateBehaviour



	public override bool Enter (string enterGlobalStatus, System.Action enterAction)
	{
		GameController.Instance.morphController.GetComponent<BoxCollider2D>().size = ColliderSize;
		State enterState = State.None;
		if(enterGlobalStatus == MorphController.STATUS_MORPHFROM) {
			enterState = State.roboMorphFrom;
		}
		Wake();
		if(enterState != State.None) {
			ForceInternalState(enterState, State.roboIdle, ()=>{
				
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
	public override void Exit (string exitStatus, System.Action exitAction)
	{
		GameController.Instance.levelMaintenance.forceRoboShowoffPause = false;
		isPlayingShowOff = false;

		if(goLaserLayer != null) {
			Destroy(goLaserLayer);
		}

		if(GameController.Instance.activeBalls.Count == 0) {
			BallTest ball = null;
			ball = GameController.Instance.morphController.CreateBall(BallTest.BallType.Normal, () => {
				((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(ball, true);
			}, GameController.Instance.ballPrefab);
			GameController.Instance.activeBalls.Add(ball);
		}


		GameInputController.ResetInputs();
		GameInputController.TapScreenSides -= OnTapScreenSides;
		
		
		
		State exitState = State.None;
		
		if(exitStatus == MorphController.STATUS_MORPHTO) {
			//Exit from smash while smashTo animation
			exitState = State.roboMorphTo;
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
			return "robo";
		}
	}
	public override MorphState GetState {
		get {
			return MorphState.Robo;
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
		InternalState = State.roboIdle;
	}

	bool isPlayingShowOff = false;
	GameObject goLaserLayer;

	public override void DoShowOff ()
	{
		((GameHUDScreen)(GUIController.Instance.currentScreen)).ballTargetIndicator.CancelIndicator();
		GameController.Instance.activeBalls.ForEach( (obj)=> obj.DestroyBall());
		GameController.Instance.activeBalls.Clear();

		GameController.Instance.levelMaintenance.forceRoboShowoffPause = true;
		//x = -0.25, y = 0.28 parent roboLaserBlast
		isPlayingShowOff = true;
		ForceInternalState(State.roboLaserBlast, State.roboIdle, ()=>{

			BallTest ball = null;
			ball = GameController.Instance.morphController.CreateBall(BallTest.BallType.Normal, () => {
				((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(ball, true);
			}, GameController.Instance.ballPrefab);
			GameController.Instance.activeBalls.Add(ball);

		});
		goLaserLayer = Instantiate(GameController.Instance.morphController.goRoboLaserPrefab) as GameObject;
		goLaserLayer.transform.parent = GetAnimView(State.roboLaserBlast.ToString()).gameObject.transform;
		GetAnimView(State.roboLaserBlast.ToString()).limbViews.ForEach( (obj)=> obj.GetComponent<tk2dSprite>().SortingOrder = 0);
		goLaserLayer.transform.localPosition = new Vector3(-0.25f, 0.28f, -3);

		goLaserLayer.GetComponent<tk2dSpriteAnimator>().AnimationEventTriggered += UpdateShowoffLayerAnimation;
		goLaserLayer.GetComponent<tk2dSpriteAnimator>().AnimationCompleted += EndShowOffLayerAnimation;
		tk2dSpriteAnimator[] animators = goLaserLayer.GetComponentsInChildren<tk2dSpriteAnimator>();
		foreach (tk2dSpriteAnimator blastAnimator in animators){
			blastAnimator.Play();
		}

	}

	void UpdateShowoffLayerAnimation(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int currentFrame)
	{
		Debug.Log("UpdateShowoffLayerAnimation "+currentFrame+"   : "+clip.GetFrame(currentFrame).eventInt);
		if(currentFrame > 2 && currentFrame<28) {

		}
		int receivedMessage = clip.GetFrame(currentFrame).eventInt;
		if(receivedMessage == 1) {
			//Set Strike
			CalculateItemsOnLaserBounds();
		}
		else if(receivedMessage == 0) {
			//Cancel Strike
			GameController.Instance.levelMaintenance.forceRoboShowoffPause = false;
			StartCoroutine(GameController.Instance.levelMaintenance.levelEditor.ScanRows());
		}
	}

	void EndShowOffLayerAnimation(tk2dSpriteAnimator caller, tk2dSpriteAnimationClip currentClip) {
		if(goLaserLayer != null) {
			Destroy(goLaserLayer);
		}
		isPlayingShowOff = false;
	}

	//TODO: 
	void CalculateItemsOnLaserBounds()
	{
		Debug.Log("CalculateItemsOnLaserBounds "+goLaserLayer.renderer.bounds);

		List<LevelCell> intersectedItems = new List<LevelCell>();

		Vector3 laserBoundsCenter = goLaserLayer.renderer.bounds.center;
		laserBoundsCenter.z = 11;
		Bounds boundLaser = goLaserLayer.renderer.bounds;
		boundLaser.center = laserBoundsCenter;

//		GameController.Instance.levelMaintenance.GetEditorCells.FindAll((obj)=>obj.HasControlFromBoard)
//			.ForEach((obj)=>Debug.Log(+obj.column+" : "+obj.row+" => "+obj.cTransform.renderer.bounds+"  :  "+boundLaser
//		                                                                                                                         +"  INTERSECT "+
//			                          IsIntersects(boundLaser,obj.cTransform.renderer.bounds)));


		intersectedItems = GameController.Instance.levelMaintenance.GetEditorCells.FindAll((obj)=> obj.Status != MapItemStatus.None && IsIntersects(boundLaser,obj.cTransform.renderer.bounds) );

		intersectedItems.ForEach((obj)=> {
			Debug.Log(obj.column+" : "+obj.row);
			GameController.Instance.levelMaintenance.levelEditor.ApplyStrike(obj.row, obj.column, StrikeType.Strike_1X, true);
		});
	}

	bool IsIntersects(Bounds arg1, Bounds arg2)
	{
		Rect rect1 = new Rect(arg1.center.x - arg1.extents.x, arg1.center.y - arg1.extents.y, arg1.size.x, arg1.size.y);
		Rect rect2 = new Rect(arg2.center.x - arg2.extents.x, arg2.center.y - arg2.extents.y, arg2.size.x, arg2.size.y);
		return Intersect(rect1,rect2);
	}

	public static bool Intersect( Rect a, Rect b ) {
		
		FlipNegative( ref a );
		
		FlipNegative( ref b );
		
		bool c1 = a.xMin < b.xMax;
		
		bool c2 = a.xMax > b.xMin;
		
		bool c3 = a.yMin < b.yMax;
		
		bool c4 = a.yMax > b.yMin;
		
		return c1 && c2 && c3 && c4;
		
	}
	
	
	
	public static void FlipNegative(ref Rect r) {
		
		if( r.width < 0 ) 
			
			r.x -= ( r.width *= -1 );
		
		if( r.height < 0 )
			
			r.y -= ( r.height *= -1 );
		
	}

	#endregion

	public override event System.Action<GameInputController.TouchArea> OnCharacterStartedMoving;
	public override event System.Action OnCharacterEndedMoving;
	public override event System.Action OnCharacterMoving;

	public enum State{
		None,
		roboIdle,
		roboMorphFrom,
		roboMorphTo,
		roboMoveRight,
		roboMoveLeft,
		roboMoveRightReverse,
		roboMoveLeftReverse,
		roboDamageIdle,
		roboDamageMoveLeft,
		roboDamageMoveRight,
		roboLaserBlast
	}
	protected State internalState = State.roboIdle;
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
			
			
			if(internalState == State.roboMoveLeftReverse || internalState == State.roboMoveRightReverse) {
				currentView.OnAnimationEnded -= OnReverseMoveCompleted;
			}
			else if(internalState == State.roboDamageIdle || internalState == State.roboDamageMoveLeft || internalState == State.roboDamageMoveRight) {
				currentView.OnAnimationEnded -= OnDamageAnimEnded;
			}
			
			
			previousInternalState = internalState;
			internalState = value;
			
			currentView = null;
			currentView = GetAnimView(internalState.ToString());
			
			
			if(internalState == State.roboMoveLeftReverse || internalState == State.roboMoveRightReverse) {
				currentView.OnAnimationEnded += OnReverseMoveCompleted;
			}
			else if(internalState == State.roboDamageIdle || internalState == State.roboDamageMoveLeft || internalState == State.roboDamageMoveRight) {
				currentView.OnAnimationEnded += OnDamageAnimEnded;
			}
			
			currentView.SetActiveGameObject(true);
			currentView.Play();
		}
	}


	void OnMorphToAnimEnded()
	{
		InternalState = State.roboIdle;
	}
	
	void OnReverseMoveCompleted()
	{
		if(isPlayingShowOff)
		{
			return;
		}
		//Reverse olurken "input began"'i ignore ettigimiz icin simdi check ediyoruz.
		GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
		if(currentTouchArea == GameInputController.TouchArea.Left) {
			InternalState = State.roboMoveLeft;
		}
		else if(currentTouchArea == GameInputController.TouchArea.Right) {
			InternalState = State.roboMoveRight;
		}
		else
			InternalState = State.roboIdle;
		
	}

	public override void ReceivedDamage ()
	{
		if(isPlayingShowOff)
		{
			return;
		}
		
		if(InternalState == State.roboMoveLeft)
			InternalState = State.roboDamageMoveLeft;
		else if(InternalState == State.roboMoveRight)
			InternalState = State.roboDamageMoveRight;
		else 
			InternalState = State.roboDamageIdle;
		
		
	}
	
	void OnDamageAnimEnded()
	{
		if(isPlayingShowOff)
		{
			return;
		}

		if(previousInternalState == State.roboMoveLeft || previousInternalState == State.roboMoveLeftReverse
		   || previousInternalState == State.roboMoveRight || previousInternalState == State.roboMoveRightReverse) {
			GameInputController.TouchArea currentTouchArea = GameInputController.GetCurrentTouchArea();
			if(currentTouchArea == GameInputController.TouchArea.Left) {
				InternalState = State.roboMoveLeft;
			}
			else if(currentTouchArea == GameInputController.TouchArea.Right) {
				InternalState = State.roboMoveRight;
			}
			else {
				InternalState = State.roboIdle;
			}
		}
		else {
			InternalState = State.roboIdle;
		}
		
	}



	public void ForceInternalState(RoboBehaviour.State forcedState,RoboBehaviour.State playAfterForcedState, Action end = null) {
		
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
		if(isPlayingShowOff)
		{
			return;
		}

		if(InternalState == State.roboDamageMoveLeft || InternalState == State.roboDamageMoveLeft || InternalState == State.roboDamageMoveRight)
			return;
		//Ayni anda iki "input began" gelmez.Reverse oluyorsa aktif yonu animasyon bitince check edecek ve o yone donecek
		if(phase == FingerGestures.FingerPhase.Began && (InternalState != State.roboMoveLeftReverse && InternalState != State.roboMoveRightReverse)) {
			if(touchedSide == GameInputController.TouchArea.Left && (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.leftCharacterRestriction))
			{
				InternalState =  State.roboMoveLeft;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else if(touchedSide == GameInputController.TouchArea.Right && (GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.rightCharacterRestriction))
			{
				InternalState = State.roboMoveRight;
				if(OnCharacterStartedMoving != null)
					OnCharacterStartedMoving(touchedSide);
			}	
			else {
				if(InternalState != State.roboIdle)
					InternalState = State.roboIdle;
			}
		}
		else if(phase == FingerGestures.FingerPhase.Ended) {
			if(InternalState == State.roboIdle)
				return;
			InternalState = InternalState == State.roboMoveRight ? State.roboMoveRightReverse : State.roboMoveLeftReverse;
			if(OnCharacterEndedMoving != null)
				OnCharacterEndedMoving();
		}
	}

	void Update()
	{
		if(isPlayingShowOff)
		{
			return;
		}
		if(InternalState == State.roboMoveLeft){
			if((GameController.Instance.morphController.CurrentXPosition < GameController.Instance.gameModel.leftCharacterRestriction)){
				if(InternalState != State.roboIdle)
					InternalState = State.roboIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition-0.05f , GameController.Instance.gameModel.characterSpeed);
		}
		else if(InternalState == State.roboMoveRight) {
			if (GameController.Instance.morphController.CurrentXPosition > GameController.Instance.gameModel.rightCharacterRestriction) {
				if(InternalState != State.roboIdle)
					InternalState = State.roboIdle;
				if(OnCharacterEndedMoving != null)
					OnCharacterEndedMoving();
			}
			
	//		GameController.Instance.morphController.CurrentXPosition = iTween.FloatUpdate( GameController.Instance.morphController.CurrentXPosition, GameController.Instance.morphController.CurrentXPosition+0.05f , GameController.Instance.gameModel.characterSpeed);
			if(OnCharacterMoving != null)
				OnCharacterMoving();
		}
	}

}
