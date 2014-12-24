using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public enum GameState{ Running = 0, Paused = 1, Over = 2, Tutorial = 3 }

//	[System.NonSerialized]
	public GameState _state = GameState.Running;

	public static event System.Action GameplayStarted;
	public static event Action OnGameStateChanged;

	public LevelMaintenance levelMaintenance;
	public MorphController morphController;
	public SpotController spotController;
	public GameModel gameModel;
	public GameHUDScreen gameHUDScreen;
	public GamePauseScreen gamePauseScreen;
	public GameOverVictoryScreen gameOverVictoryScreen;
    	public GameOverFailedScreen gameOverFailedScreen;
	public Transform trBaseGameAnchor;
    	public Transform trInstantiatedEffectsParent;
    	public Transform trEditor;
	public GameObject ballPrefab;


	public bool IsDebug = true;
	public List<BallTest> activeBalls = new List<BallTest>();



	static GameController instance;
	public static GameController Instance {
		get {
			if(instance == null) {
				instance = FindObjectOfType(typeof(GameController)) as GameController;
			}
			return instance;
		}
	}

	void Awake() {
		if(instance == null) {
			instance = this;
		}
	}

	public GameState State
	{
		get{
			return _state;
		}
		set{
			_state = value;
			if(OnGameStateChanged != null)
				OnGameStateChanged();
		}
	}

	void Start()
	{
        MorphController.OnMorphStateChanged += HandleMorphStateChanged;

		GUIController.Instance.SetScreen(gameHUDScreen);
		DataStorage.Instance.ReadXmlDB();
		morphController.LoadMorphObjects();
		levelMaintenance.ActiveMapId = 2;
		StartGameplay();


        	levelMaintenance.StarCount = 0;
        	levelMaintenance.Score = 0;
        	levelMaintenance.MorphCount = 0;
		levelMaintenance.SpotCount = 0;

//		spotController.Reset();
	}

	public void StartGameplay() {

		previousTimeScale = 1;

		GUIController.Instance.InputEnabled = true;

		ResumeGame(false);
		if(GameplayStarted != null) {
			GameplayStarted();
		}

		activeBalls.ForEach( (obj)=> {
			obj.DestroyBall();
		});
		activeBalls.Clear();

        	iTween.Stop(morphController.cTransform.gameObject);
		morphController.SetInitialPosition();

		morphController.SetState(MorphState.Morpheus, "", ()=>{
			((MorpheusBehaviour)(morphController.CurrentBehavior)).ForceInternalState(MorpheusBehaviour.State.born, MorpheusBehaviour.State.idle, ()=>{
				BallTest activeBall = null;
				activeBall = GameController.Instance.morphController.CreateBall(BallTest.BallType.Normal, ()=> {
					((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(activeBall, true);
					morphController.ForceEnableInputs(true);
				}, ballPrefab);
				activeBall.IsMainBall = true;
				activeBalls.Add(activeBall);
			});
		}); 
	
//		morphController.SetState(MorphState.Spider,"", ()=>{
//			((SpiderBehaviour)(morphController.CurrentBehavior)).ForceInternalState(SpiderBehaviour.State.spiderIdle, SpiderBehaviour.State.None);
//		});


	}

	float previousTimeScale = 1;

	public void PauseGame() {

		if (State == GameState.Running)
		{
			morphController.ForceEnableInputs(false);

			previousTimeScale = Time.timeScale;
			State = GameState.Paused;
		}
		if(GUIController.Instance.currentScreen != gamePauseScreen) {
			GUIController.Instance.SetScreen(gamePauseScreen,true);	
			Time.timeScale = 0;
		}
	}

	public void ResumeGame(bool onlyChangeScreen) {

		if(onlyChangeScreen) {
			if(GUIController.Instance.currentScreen != gameHUDScreen) {
				GUIController.Instance.SetScreen(gameHUDScreen,true);
			}
			State = GameState.Running;
			return;
		}
		Debug.Log("previousTimeScale "+previousTimeScale);
		Time.timeScale = previousTimeScale;

		if(State == GameState.Paused){
			morphController.ForceEnableInputs(true);
		}
		
		if(GUIController.Instance.currentScreen != gameHUDScreen) {
			GUIController.Instance.SetScreen(gameHUDScreen,true);
		}

		levelMaintenance.timeCounter = Time.time;
		State = /*tutorialController.enabled ? GameState.Tutorial : */ GameState.Running;
	}

	public void Restart() {
		State = GameState.Over;
		GUIController.Instance.FadeOutBlackCurtain();
		morphController.CurrentBehavior.Exit("",()=>{
			morphController.Reset();
			GUIController.Instance.SetScreen(gameHUDScreen,true);
			foreach (Transform child in trInstantiatedEffectsParent)
			{
				Destroy(child.gameObject);
			}
			
			levelMaintenance.ActiveMapId = levelMaintenance.validMapId;
			levelMaintenance.StarCount = 0;
			levelMaintenance.Score = 0;
			levelMaintenance.MorphCount = 0;
			levelMaintenance.SpotCount = 0;
			
			
			StartGameplay();
		});

		spotController.Reset();

	}



	public IEnumerator LevelVictory()
	{
		if(State == GameState.Over)
			yield break;
		State = GameState.Over;
		GUIController.Instance.InputEnabled = false;
		activeBalls.ForEach( (obj)=> {
			obj.DestroyBall();
		});
		activeBalls.Clear();


		Debug.Log("LevelVictory");

		morphController.ForceEnableInputs(false);
		morphController.SetVictory( ()=>{

			StartCoroutine(Wait(2.5f, ()=>{
				foreach (Transform child in trInstantiatedEffectsParent)
				{
					Destroy(child.gameObject);
				}
				morphController.CurrentBehavior.Exit("",null);
				levelMaintenance.Score += (levelMaintenance.MorphCount * 10);
				levelMaintenance.Score += (levelMaintenance.LifeCount * 5);
				GUIController.Instance.SetScreen(gameOverVictoryScreen,true);
				GUIController.Instance.InputEnabled = true;
			}));

		});


//		morphController.ForceEnableInputs(false);
//		morphController.SetState(MorphState.Morpheus, MorphController.STATUS_MORPHFROM, ()=>{
//			morphController.ForceEnableInputs(false);
//
//			((MorpheusBehaviour)(GameController.Instance.morphController.CurrentBehavior)).ForceInternalState(MorpheusBehaviour.State.victoryStart, MorpheusBehaviour.State.victory);
//			StartCoroutine(Wait(2.5f, ()=>{
//				foreach (Transform child in trInstantiatedEffectsParent)
//				{
//					Destroy(child.gameObject);
//				}
//				morphController.CurrentBehavior.Exit("",null);
//				levelMaintenance.Score += (levelMaintenance.MorphCount * 10);
//				levelMaintenance.Score += (levelMaintenance.LifeCount * 5);
//				GUIController.Instance.SetScreen(gameOverVictoryScreen,true);
//				GUIController.Instance.InputEnabled = true;
//			}));
//
//		});


	}

	IEnumerator Wait(float waitTime, Action endedAction) {
		yield return new WaitForSeconds(waitTime);
		endedAction();
	}

	public IEnumerator BallCollideWithBottomCollider(BallTest collidedBall)
	{
		if(!IsDebug) 
		{
            if (morphController.CurrentState == MorphState.Spider) {
                collidedBall.DestroyBall();
                activeBalls.Remove(collidedBall);

                yield return new WaitForSeconds(3);
                if (activeBalls.Count > 0) {
                    yield break;
                }
            }
			if(collidedBall.ballType == BallTest.BallType.ColorBall && morphController.CurrentState == MorphState.Morpheus) {

				if(((MorpheusBehaviour)morphController.CurrentBehavior).InstantiateColorBall()) {
					Debug.Log("instantinated ball "+((MorpheusBehaviour)morphController.CurrentBehavior).colorBallRight);
					yield break;
				}
			}

			State = GameState.Over;

			GUIController.Instance.InputEnabled = false;
			activeBalls.ForEach( (obj)=> {
				obj.DestroyBall();
			});
			activeBalls.Clear();
			((GameHUDScreen)(GUIController.Instance.currentScreen)).ballTargetIndicator.CancelIndicator();

			Debug.Log("BallCollideWithBottomCollider");


			morphController.ForceEnableInputs(false);
			morphController.SetTimeScaleIgnore(true); 

			int updatedLifeCount = levelMaintenance.LifeCount - levelMaintenance.NeedToCountineLifeCount;

			if(updatedLifeCount > 0) {
				GUIController.Instance.ShowCountineGamePopUp( ()=>{

					Action<GameObject> endedAction = (go)=>{
						morphController.SetTimeScaleIgnore(false);
						levelMaintenance.LifeCount = updatedLifeCount;
						StartGameplay();
					};

					iTween.MoveTo(morphController.cTransform.gameObject, iTween.Hash("y", -1, "time", 0.2f, "delay", 0f,"ignoretimescale", true, "easeType", iTween.EaseType.easeInSine,
					                                                                  "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction"));

				});
			}



			morphController.SetDeath( ()=>{

				Time.timeScale = 0;

	//		morphController.SetState(MorphState.Morpheus, MorphController.STATUS_MORPHTO, ()=>{
	//			morphController.CurrentBehavior.SetDeath( ()=>{


					if(updatedLifeCount < 0) {
						foreach (Transform child in trInstantiatedEffectsParent)
						{
							Destroy(child.gameObject);
						}
						levelMaintenance.LifeCount = 0;
						
						levelMaintenance.ActiveMapId = -1;
						GUIController.Instance.SetScreen(gameOverFailedScreen, true);
					}
					else {

					}
					GUIController.Instance.InputEnabled = true;
	//			});
				
	//		});

			});
		//	Time.timeScale = 0;
			Debug.Log("timescale is zero");
		}
        yield break;
	}


    void HandleMorphStateChanged(MorphStateBehaviour newState)
    {
        if (morphController.previousMorphState != null) {

//            Debug.Log("prev state " + morphController.previousMorphState + "  curr " + morphController.CurrentState);

            if (morphController.previousMorphState == MorphState.Spider && morphController.CurrentState == MorphState.Morpheus) {
                activeBalls.ForEach((obj) => obj.DestroyBall());
                activeBalls.Clear();

                BallTest activeBall = null;
                activeBall = GameController.Instance.morphController.CreateBall(BallTest.BallType.Normal, () =>
                {
                    ((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(activeBall, true);
                  
                }, ballPrefab);
                activeBall.IsMainBall = true;
                activeBalls.Add(activeBall);
            }
		if(morphController.previousMorphState == MorphState.Morpheus) {
			BallTest colorBall = activeBalls.Find((obj)=> obj.ballType == BallTest.BallType.ColorBall) ;
			if(colorBall != null ){
				activeBalls.Remove(colorBall);
				colorBall.DestroyBall();
			}
				if(activeBalls.Count == 0 && morphController.CurrentState != MorphState.Spider) {

					BallTest activeBall = null;
					activeBall = GameController.Instance.morphController.CreateBall(BallTest.BallType.Normal, () =>
					                                                                {
						((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(activeBall, true);
						
					}, ballPrefab);
					activeBall.IsMainBall = true;
					activeBalls.Add(activeBall);
				}
		} 
        }
    }


	public void DoShowOff()
	{
		morphController.CurrentBehavior.DoShowOff();
	}

	public BallTest GetMainBall
	{
		get{
			return activeBalls.Find( (obj)=> obj.IsMainBall);
		}
	}
	


}
