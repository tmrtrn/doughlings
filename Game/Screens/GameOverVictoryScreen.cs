using UnityEngine;
using System.Collections;

public class GameOverVictoryScreen : GameScreen {

	public GameObject goBlackCurtain;
	public ScoreCounter scoreCounterLife;
	public ScoreCounter morphScoreCounter;
	public ScoreCounter showOffScoreCounter;
	public ScoreCounter totalScoreCounter;
	public UnlockStarIndicator unlockStarIndicator;
	public tk2dTextMesh levelCompletedText;

	public GameObject spots;
	
	public override void TransitionIn ()
	{

		goBlackCurtain.SetActive(false);
		base.TransitionIn();
		spots.SetActive(false);
		//Time.timeScale = 0;
		goBlackCurtain.SetActive(true);
		levelCompletedText.text = LangController.Instance.GetLangDataByKey("level_completed").value;
		levelCompletedText.Commit();
		StartScoreCounters();

		GameInputController.OnTouchedScreen += HandleOnTouchedScreen;

	}

	void HandleOnTouchedScreen ()
	{
		ForceSetScoreCounters();
	}
	
	public override void TransitionOut ()
	{
		Time.timeScale = 1;
		GameInputController.OnTouchedScreen -= HandleOnTouchedScreen;
		base.TransitionOut ();
		goBlackCurtain.SetActive(false);

	}

	void ForceSetScoreCounters()
	{
		morphScoreCounter.SetScoreCounter(0);
		showOffScoreCounter.SetScoreCounter(0);
		scoreCounterLife.SetScoreCounter(GameController.Instance.levelMaintenance.LifeCount);
		totalScoreCounter.SetScoreCounter(GameController.Instance.levelMaintenance.Score);
		unlockStarIndicator.SetStar(GameController.Instance.levelMaintenance.UnlockedStarCount);
	}

	void StartScoreCounters()
	{
		morphScoreCounter.StartScoreCounter(GameController.Instance.levelMaintenance.MorphCount, ()=>{
			showOffScoreCounter.StartScoreCounter(0, ()=>{
				scoreCounterLife.StartScoreCounter(GameController.Instance.levelMaintenance.LifeCount, ()=>{
					totalScoreCounter.StartScoreCounter(GameController.Instance.levelMaintenance.Score, ()=>{
						StartCoroutine(unlockStarIndicator.StartIndicator(GameController.Instance.levelMaintenance.UnlockedStarCount, ()=>{
							
						}));
					});
				});
			});
		});

	}
	
	public override void Show ()
	{
		base.Show ();
	}
	
	public override void Hide ()
	{
		base.Hide ();
	}

	public override ScreenType Type {
		get {
			return ScreenType.GAMEOVERVICTORY;
		}
	}

}
