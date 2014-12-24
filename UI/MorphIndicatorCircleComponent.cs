using UnityEngine;
using System.Collections;

public class MorphIndicatorCircleComponent : MorphIndicatorComponent {
	#region implemented abstract members of MorphIndicatorComponent
	public override bool Enter (MorphState activeState, float duration, System.Action completedAction)
	{
		if(activeState == MorphState.Morpheus || activeState == MorphState.None) {
			base.Hide();
			return false;
		}
		base.Show();
		GetComponent<tk2dSprite>().SetSprite("morphIndicator"+activeState.ToString());
		return true;
	}
	public override void Exit ()
	{
		base.Hide();
	}
	#endregion


}
