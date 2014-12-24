using UnityEngine;
using System.Collections;

public class YellowItemModel : MapItemModel {
	
	public YellowItemModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty yellowClip = new ClipProperty("yellowWink");
		yellowClip.loop = false;
		yellowClip.playType = ClipProperty.PlayType.RandomTime;
		yellowClip.defaultSpriteName = "yellow00";
		yellowClip.priority = AnimationPriority.Low;
		
		return yellowClip;
	}
	
	public override bool HasMimicAnimation ()
	{
		return true;
	}
	
	public override ClipProperty GetMimicClipProperty ()
	{
		ClipProperty yellowClip = new ClipProperty("yellowMimic");
		yellowClip.loop = false;
		yellowClip.playType = ClipProperty.PlayType.ByOrder;
		yellowClip.defaultSpriteName = "yellow00";
		yellowClip.priority = AnimationPriority.Low;
		return yellowClip;
	}
	
	public override bool HasTurnInToClip {
		get {
			return true;
		}
	}
	
	public override float ContagionRatio {
		get {
			return 0.02f;
		}
	}
	

	public override bool IsMapAsset {
		get {
			return false;
		}
	}
	
	public override int GetBallCollideScore {
		get {
			return 3;
		}
	}
}
