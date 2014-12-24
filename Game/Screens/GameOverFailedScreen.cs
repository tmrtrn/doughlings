using UnityEngine;
using System.Collections;

public class GameOverFailedScreen : GameScreen {

	public GameObject goBlackCurtain;
	public GameObject spots;
    public tk2dTextMesh levelFailedText;

	public override void TransitionIn ()
	{
		
		goBlackCurtain.SetActive(false);
		base.TransitionIn();
		spots.SetActive(false);
		goBlackCurtain.SetActive(true);
        levelFailedText.text = LangController.Instance.GetLangDataByKey("level_failed").value;
        levelFailedText.Commit();
	}

	public override void TransitionOut ()
	{
		base.TransitionOut ();
		goBlackCurtain.SetActive(false);
	}

	public override ScreenType Type {
		get {
			return ScreenType.GAMEOVERFAILED;
		}
	}

}
