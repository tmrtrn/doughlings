using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum MoveDirection
{
	None,
	Left,
	Right
}


public class MorphController : MonoBehaviour {



	public const string STATUS_MORPHTO = "morphTo";
	public const string STATUS_MORPHFROM = "morphFrom";

	public tk2dSpriteCollectionData morphSpriteCollectionData;
	public List<MorphStateBehaviour> behaviours = new List<MorphStateBehaviour>();
	private Vector3 initialPosition = new Vector3(9.6f,0.64f, -6);
	public static event Action<MorphStateBehaviour> OnMorphStateChanged;
	public GameObject goSpiderWebPrefab;
	public GameObject goRoboLaserPrefab;
	public MoveDirection _moveDirection = MoveDirection.None;

	public List<MorphStateBehaviour> Behaviours {
		get{
			return behaviours;
		}
	}


	public void LoadMorphObjects()
	{
		DataStorage.Instance.WriteMorphDataTo2DToolkitAnimation(morphSpriteCollectionData);
		Behaviours.ForEach( (behaviour)=>{
			behaviour.InstantinateAnimations(DataStorage.Instance.GetMorphAnimationsProperty.FindAll( (obj) => obj.type == behaviour.GetNameOnStorage));
		});
		Behaviours.ForEach((obj) => obj.Sleep());
	}

	public MorphState startingMorphState = MorphState.Morpheus;
	MorphState morphState;
	[System.NonSerialized]
	public MorphState previousMorphState;
	
	public MorphState CurrentState
	{
		get{
			return morphState;
		}
	}

	public MorphStateBehaviour CurrentBehavior {
		get{
			return GetBehaviorByState(morphState);
		}
	} 
	
	public MorphStateBehaviour GetBehaviorByState(MorphState state)
	{
		return Behaviours.Find((obj) => obj.GetState == state);
	}

	List<MorphProperty> listTransMorphs = new List<MorphProperty>();

	void Update()
	{

		if(GameController.Instance.State == GameController.GameState.Running) {

			if(_moveDirection == MoveDirection.Left && CurrentXPosition > GameController.Instance.gameModel.leftCharacterRestriction) {
				CurrentXPosition = iTween.FloatUpdate( CurrentXPosition, CurrentXPosition - 0.05f , GameController.Instance.gameModel.characterSpeed);
			}
			else if(_moveDirection == MoveDirection.Right && CurrentXPosition < GameController.Instance.gameModel.rightCharacterRestriction) {
				CurrentXPosition = iTween.FloatUpdate( CurrentXPosition, CurrentXPosition + 0.05f , GameController.Instance.gameModel.characterSpeed);
			}

		}

		if(listTransMorphs.Count == 0)
			return;
		listTransMorphs.RemoveAll( (obj) => obj.processEnded);
		if(listTransMorphs.Count == 0)
			return;

		if(GameController.Instance.State == GameController.GameState.Running) {
			MorphProperty processingProperty = listTransMorphs.Find((obj)=> obj.IsProcessing);
			if(processingProperty == null) {
				listTransMorphs[0].IsProcessing = true;
				StartCoroutine( "ProcessState",listTransMorphs[0]);
			}

		}

	}

	protected MoveDirection Move
	{
		get{
			return _moveDirection;
		}
		set {
			_moveDirection = value;
		}
	}

	void OnTapScreenSides(FingerGestures.FingerPhase phase, GameInputController.TouchArea touchedSide)
	{
		if(phase == FingerGestures.FingerPhase.Began) {
			if(touchedSide == GameInputController.TouchArea.Left)
				Move = MoveDirection.Left;
			else if(touchedSide == GameInputController.TouchArea.Right) {
				Move = MoveDirection.Right;
			}
			else {
				Move = MoveDirection.None;
			}
		}
		else if(phase == FingerGestures.FingerPhase.Ended) {
			Move = MoveDirection.None;
		}
	}

	public void ClearWaitingMorphList()
	{

		listTransMorphs.ForEach( (obj)=>{
			obj.ProcessEnded();
		});
		listTransMorphs.Clear();
	}

	public void Reset()
	{
		listTransMorphs.Clear();
		morphState = MorphState.None;
		GameInputController.TapScreenSides -= OnTapScreenSides;
	}


	public void SetDeath(Action completed)
	{
		GameInputController.TapScreenSides -= OnTapScreenSides;

		Action stateChangedAction = ()=>{
			CurrentBehavior.SetDeath( ()=>{
				ClearWaitingMorphList();
				completed();
			});
		};

		MorphProperty newProperty = new MorphProperty();
		newProperty.id = listTransMorphs.Count;
		newProperty.state = MorphState.Morpheus;
		newProperty.globalStatus = MorphController.STATUS_MORPHTO;
		newProperty.SetStateChanged(stateChangedAction);

		StartCoroutine("ProcessState",newProperty);

	}

	public void SetVictory(Action completed)
	{
		Action stateChangedAction = ()=>{
			ForceEnableInputs(false);
			((MorpheusBehaviour)(CurrentBehavior)).ForceInternalState(MorpheusBehaviour.State.victoryStart, MorpheusBehaviour.State.victory);
			completed();
		};

		MorphProperty newProperty = new MorphProperty();
		newProperty.id = listTransMorphs.Count;
		newProperty.state = MorphState.Morpheus;
		newProperty.globalStatus = MorphController.STATUS_MORPHFROM;
		newProperty.SetStateChanged(stateChangedAction);
		
		StartCoroutine("ProcessState",newProperty);
	}

	IEnumerator ProcessState(MorphProperty currentProperty) {

		MorphStateBehaviour currentBehavior = GetBehaviorByState(morphState);
//		if(currentBehavior != null)
//			Debug.Log(currentBehavior.name +"  new "+GetBehaviorByState(currentProperty.state).name);
		if(currentBehavior == GetBehaviorByState(currentProperty.state)) {
			if(currentProperty.state == MorphState.Smash)
				((GameHUDScreen)(GUIController.Instance.currentScreen)).morphIndicator.SetState(MorphState.Smash, currentBehavior.GetIndicatorDuration);
			if(currentProperty.state == MorphState.Robo)
				((GameHUDScreen)(GUIController.Instance.currentScreen)).morphIndicator.SetState(MorphState.Robo, currentBehavior.GetIndicatorDuration);
			if(currentProperty.state == MorphState.Gunner)
				((GameHUDScreen)(GUIController.Instance.currentScreen)).morphIndicator.SetState(MorphState.Gunner, currentBehavior.GetIndicatorDuration);
			if(currentProperty.state == MorphState.Spider)
				((GameHUDScreen)(GUIController.Instance.currentScreen)).morphIndicator.SetState(MorphState.Spider, currentBehavior.GetIndicatorDuration);

			currentProperty.ProcessEnded();

			yield break;
		}
		
		
		
		
		Action exitCurrentBehaviourCompleted = ()=>{

			GetBehaviorByState(currentProperty.state).Enter(currentProperty.globalStatus, ()=> {
				
				morphState = currentProperty.state;
				currentProperty.ProcessEnded();

				if(OnMorphStateChanged != null) 
					OnMorphStateChanged(GetBehaviorByState(morphState));
			});
			
			
			
		};

        	previousMorphState = morphState;
		
		
		if(currentBehavior != null) {

	//		ForceEnableInputs(false);

			currentBehavior.Exit(currentProperty.globalStatus, ()=>{

				exitCurrentBehaviourCompleted();
			});
           
			morphState = currentProperty.state;
		}
		else {
			morphState = currentProperty.state;
			exitCurrentBehaviourCompleted();
			
		}

		yield return null;
	}

	public void SetState(MorphState state,string globalStatus, Action stateChanged = null) {

		if(GameController.Instance.State == GameController.GameState.Over)
			return;
		
		MorphProperty newProperty = new MorphProperty();
		newProperty.id = listTransMorphs.Count;
		newProperty.state = state;
		newProperty.globalStatus = globalStatus;
		newProperty.SetStateChanged(stateChanged);

		listTransMorphs.Add(newProperty);
	}



	public MorphState State {
		get{
			return morphState;
		}
//		set {
			

			
//			if(GetBehaviorByState(value).Enter()) {
//				previousMorphState = morphState;
//				morphState = value;
//				Debug.Log("OnMorphStateChanged "+(OnMorphStateChanged != null));
//				if(OnMorphStateChanged != null) 
//					OnMorphStateChanged(GetBehaviorByState(morphState));
//			}
//			else {
//				Debug.Log("Already running "+value);
//			}
//		}
	}


	void HandleMoving(GameInputController.TouchArea direction)
	{
		iTween.Stop(gameObject);
		if(direction == GameInputController.TouchArea.Left) {

		}

	}

	iTween _tween;
	public iTween tween
	{
		get{
			_tween = GetComponent<iTween>();
			return _tween;
		}
	}

	[System.NonSerialized]
	private Transform thisTransform;
	public static bool CanTranslate = true;
	
	public float CurrentXPosition
	{
		get{
			return cTransform.position.x;
		}
		set{
			if(!CanTranslate)
				return;
			Vector3 vec = thisTransform.position;
			vec.x = value;
			cTransform.position = vec;
		}
	}
	
	public float CurrentYPosition
	{
		get{
			return cTransform.position.y;
		}
		set{
			Vector3 vec = thisTransform.position;
			vec.y = value;
			cTransform.position = vec;
		}
	}

	public Transform cTransform
	{
		get{
			if(thisTransform == null)
				thisTransform = transform;
			return thisTransform;
		}
	}

	public void ForceEnableInputs(bool enable){
		if(CurrentBehavior != null) {
			CurrentBehavior.ForceEnableInputs(enable);
		}
	}

    public void SetTimeScaleIgnore(bool ignore)
    {
	behaviours.ForEach((obj)=>{
		obj.SetTimeScaleIgnore(ignore);
	});
        
        
    }


	public void SetInitialPosition()
	{
		if(thisTransform == null)
			thisTransform = transform;
		thisTransform.position = initialPosition;

		GameInputController.TapScreenSides += OnTapScreenSides;
	}

	public void Damage()
	{
		CurrentBehavior.ReceivedDamage();
	}

	public Vector3 GetBallInstantinateAnchorPosition {
		get {
			return new Vector3(cTransform.position.x , cTransform.position.y + 0.87f, -3); 
		}
	}


	public BallTest CreateBall (BallTest.BallType ballType, Action created, GameObject ballPrefab)
	{
		GameObject goBall = Instantiate(ballPrefab) as GameObject;
		goBall.transform.position = GetBallInstantinateAnchorPosition + new Vector3(0,0, 0);
		goBall.SetActive(false);
		
		goBall.transform.parent = transform;
		
		
//		Utility.Instance.Wait( 0.7f, () => {
			goBall.SetActive(true);
			goBall.transform.parent = cTransform;
			goBall.GetComponent<BallTest>().PlayBallLoadingAnim(ballType, (BallTest) =>{
			//	goBall.transform.parent = GameController.Instance.trBaseGameAnchor;
				created();
			} );
			
//		});
		return goBall.GetComponent<BallTest>();
	}


	protected class MorphProperty
	{
		public int id = -1;
		public MorphState state;
		public string globalStatus;
		Action stateChanged = null;
		public bool IsProcessing = false;
		public bool processEnded = false;

		public void SetStateChanged(Action stateChanged)
		{
			this.stateChanged = stateChanged;
		}

		public void ProcessEnded()
		{
			IsProcessing = false;
			if(stateChanged != null) {
				stateChanged();
			}
			processEnded = true;
		}


		
	}

}
