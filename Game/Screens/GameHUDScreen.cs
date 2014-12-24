using UnityEngine;
using System.Collections;

public class GameHUDScreen : GameScreen {


	
	public DigitControl scoreIndicator;
	public DigitControl lifeIndicator;
	public MorphIndicator morphIndicator;
	public StarBarIndicator starBarIndicator;
	public BallTargetIndicator ballTargetIndicator;

	public GameObject spots;

	public override ScreenType Type {
		get {
			return ScreenType.GAMEHUD;
		}
	}

	public override void TransitionIn ()
	{

		base.TransitionIn();
		spots.SetActive(true);
		//Show();

	}

	public override void TransitionOut ()
	{

		base.TransitionOut();
		//Hide();
	}
	
	public override void Show ()
	{


		Debug.Log("HandleOnCharacterStateChanged OK");

		ballTargetIndicator.Show();
		base.Show ();



	}

	void OnEnable()
	{
		LevelMaintenance.OnScoreChanged += OnScoreChanged;
		LevelMaintenance.OnLifeCountChanged += OnLifeCountChanged;
		MorphController.OnMorphStateChanged += HandleOnCharacterStateChanged;
		LevelMaintenance.OnStarCountChanged += HandleOnStarCountChanged;

	}

	void OnDisable()
	{
		LevelMaintenance.OnScoreChanged -= OnScoreChanged;
		LevelMaintenance.OnLifeCountChanged -= OnLifeCountChanged;
		MorphController.OnMorphStateChanged -= HandleOnCharacterStateChanged;
		LevelMaintenance.OnStarCountChanged -= HandleOnStarCountChanged;
	}

	
	
	public override void Hide ()
	{
		
		ballTargetIndicator.Hide();

		Debug.Log("HandleOnCharacterStateChanged Remove");

		base.Hide ();
	}
	
	void OnScoreChanged(int score)
	{
		StartCoroutine(scoreIndicator.SetNumber(score));
	}
	
	void OnLifeCountChanged(int lifeCount)
	{
		StartCoroutine(lifeIndicator.SetNumber(lifeCount));
	}
	
//	void OnUnlockedStarCountChanged(int starCount)
//	{
//		starBarIndicator.SetStar(starCount);
//	}

	void HandleOnStarCountChanged (float starChangeRatio)
	{
		starBarIndicator.ChangeRatio(starChangeRatio);
	}



	public void StartBallTargetIndicator(BallTest ballView, bool doTranslate)
	{
        
		StartCoroutine(ballTargetIndicator.PlayIndicator(ballView, doTranslate));
	} 
	void HandleOnCharacterStateChanged (MorphStateBehaviour obj)
	{
       
		morphIndicator.SetState(obj.GetState , obj.GetIndicatorDuration);
	}




	
	
}
