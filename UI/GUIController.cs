using UnityEngine;
using System.Collections;
using System;

public class GUIController : MonoBehaviour {

	bool inputEnabled = true;
	float rayDistance = float.MaxValue;
	public GameObject popUpPrefab;
	public GameObject goBlackCurtain;

	GameObject goPopUpLife; 

	public static Action OnPausePressed;
	public static Action OnResumePressed;

	public bool InputEnabled {
		get{
			return inputEnabled;
		}
		set{
			inputEnabled = value;
		}
	}

	[System.NonSerialized] public GameScreen currentScreen;
	[System.NonSerialized] public GameScreen previousScreen;
//	public GameScreen firstScreen;
	
	
	static GUIController instance;
	
	public static GUIController Instance {
		get{
			if(instance == null){
				instance = FindObjectOfType(typeof(GUIController)) as GUIController;
				if(instance == null)
					instance = new GameObject("GUIController").AddComponent<GUIController>();
			}
			return instance;
		}
	}
	
	Camera m_GUICamera;
	public Camera GUICamera {
		get {
			if(m_GUICamera == null) {
				m_GUICamera = GameObject.FindWithTag("GUICamera").camera;
			}
			return m_GUICamera;
		}
	}
	
	void Awake()
	{
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Destroy(gameObject);
			return;
		}
//		if(firstScreen != null)
//			SetScreen(firstScreen);
	}
	
	void Start()
	{
		
	}
	
	void OnDestroy() {
		if(instance == this) {
			instance = null;
		}
	}

	/*
	string selRow = "-1";
	string selColumn ="-1";
	void OnGUI()
	{
		selRow = GUI.TextArea(new Rect(100,0,80,80),selRow);
		selColumn = GUI.TextArea(new Rect(100,100,80,80),selColumn);
		if(GUI.Button(new Rect(0,0,80,80)," strike")){
//			TodoGamePipeline.instance.SetStrike(int.Parse(selRow), int.Parse(selColumn), TodoStrikeType.Strike_1X);
		}
	} */
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetScreen(GameScreen newScreen){
		SetScreen(newScreen,true);
	}
	public void SetScreen(GameScreen newScreen, bool doTransitions) {
		AnimateGoToScreen(newScreen, doTransitions);
	}

	void AnimateGoToScreen(GameScreen newScreen, bool doTransitions) {
		InputEnabled = false;

		if(currentScreen != null) {
			if(currentScreen.hasTransition && doTransitions) {
				currentScreen.TransitionOut();
			}
			else {
				currentScreen.Hide();
			}


		}
		previousScreen = currentScreen;
		currentScreen = newScreen;

		if(currentScreen != null) {
			if(currentScreen.hasTransition && doTransitions) {
				currentScreen.TransitionIn();
			}
			else {
				currentScreen.Show();
			}

		}
		InputEnabled = true;


	}

	public void ShowCountineGamePopUp(System.Action pressedCountineAction)
	{
		
		goPopUpLife = Instantiate(popUpPrefab) as GameObject;
		goPopUpLife.transform.parent = GameController.Instance.trBaseGameAnchor;
		goPopUpLife.transform.localPosition = new Vector3(9.6f,2.5f,-15);
		iTween.MoveTo(goPopUpLife, iTween.Hash("y", -5.4f, "islocal",true, "ignoretimescale", true,"time", 0.5f, "easeType", iTween.EaseType.easeOutBack));
		PopupView popUpView = goPopUpLife.GetComponent<PopupView>();
		PopupView.PopupData data = new PopupView.PopupData();
		data.btn1Pressed += ()=>{
			System.Action<GameObject> ScaleEndedAction = (go)=>{
				Destroy(goPopUpLife);
				goBlackCurtain.SetActive(false);
				pressedCountineAction();
			};

			iTween.MoveTo(goPopUpLife, iTween.Hash("y", 2.5f,"islocal",true, "ignoretimescale", true,"time", 0.5f, "easeType", iTween.EaseType.easeInBack,
			                                        "dontusesendmessage",true, "endedaction",ScaleEndedAction , "onComplete","changedWithAction"));

			iTween.ValueTo(gameObject, iTween.Hash(
				"ignoretimescale", true,
				"from", 1f, "to", 0f,
				"time", 0.5f, "easetype", iTween.EaseType.linear,
				"onupdate", "OnUpdateFade",
				"oncomplete","OnCompleteFadeOut")); 


		};
		data.keyTitle = "popupTitle";
		data.keyContext = "popupContext";
		data.keyBtn1 = "x"+GameController.Instance.levelMaintenance.NeedToCountineLifeCount;
		popUpView.Render(data);

		goBlackCurtain.SetActive(true);
		goBlackCurtain.GetComponent<tk2dTiledSprite>().SortingOrder = 2;

		iTween.ValueTo(gameObject, iTween.Hash(
			"ignoretimescale", true,
			"from", 0f, "to", 1f,
			"time", 0.5f, "easetype", iTween.EaseType.linear,
			"onupdate", "OnUpdateFade",
			"oncomplete","OnCompleteFadeIn")); 
		
	}
	public void OnUpdateFade(float newAlpha) {
		Color color = goBlackCurtain.GetComponent<tk2dTiledSprite>().color;
		color.a = newAlpha;
		goBlackCurtain.GetComponent<tk2dTiledSprite>().color = color;
		
	}
	public void OnCompleteFadeIn()
	{
		goBlackCurtain.SetActive(true);
	}
	
	public void OnCompleteFadeOut()
	{
		goBlackCurtain.SetActive(false);
	}

	
	
	public bool DoesTapHitGUI(Vector2 mousePosition)
	{
		return GetHitButton(GUICamera, mousePosition) != null;
	}
	
	Button GetHitButton(Camera cam, Vector3 mousePosition)
	{
		Ray ray = cam.ScreenPointToRay(mousePosition);
		RaycastHit hit = new RaycastHit();
		return Physics.Raycast(ray,out hit, rayDistance, cam.cullingMask) ? hit.transform.GetComponent<Button>() : null;
	}


	#region GameHud Sceen buttonActions
	
	void PauseButtonAction() {
		Debug.Log("Pressed pause InputEnabled " +InputEnabled+ "        "+(GameController.Instance.State != GameController.GameState.Running));
		if(!InputEnabled /*|| GameController.Instance.state != GameController.GameState.Running*/)
			return;

		if(OnPausePressed != null)
			OnPausePressed();

		GameController.Instance.PauseGame();
	//	StartCoroutine(GameController.Instance.LevelVictory());
	}

	void ResumeButtonAction()
	{
		if(!InputEnabled)
			return;

		GameController.Instance.ResumeGame(/*goPopUpLife != null*/GameObject.FindGameObjectWithTag("POPUP") != null);
		if(OnResumePressed != null)
			OnResumePressed();

	}
	
	void RestartButtonAction()
	{
		if(!InputEnabled)
			return;
		if(goPopUpLife != null) {
			Destroy(goPopUpLife);
		}
		GameController.Instance.Restart();
	}
	
	#endregion


	public void FadeOutBlackCurtain()
	{
		GameObject goFoundBlackCurtain = GameObject.FindGameObjectWithTag("GUIBlackCurtain");
		if(goFoundBlackCurtain != null) {
			iTween.ValueTo(gameObject, iTween.Hash(
				"ignoretimescale", true,
				"from", 1f, "to", 0f,
				"time", 0.5f, "easetype", iTween.EaseType.linear,
				"onupdate", "OnUpdateFade",
				"oncomplete","OnCompleteFadeOut")); 
		}
	}




}
