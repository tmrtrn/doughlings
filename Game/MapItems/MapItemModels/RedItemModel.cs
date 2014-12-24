using UnityEngine;
using System.Collections;

public class RedItemModel : MapItemModel {
	
	public RedItemModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty redClip = new ClipProperty("redWink");
		redClip.loop = false;
		redClip.playType = ClipProperty.PlayType.RandomTime;
		redClip.defaultSpriteName = "red00";
		redClip.priority =  AnimationPriority.Low;
		
		return redClip;
	}
	
	public override bool HasMimicAnimation ()
	{
		return true;
	}
	
	public override ClipProperty GetMimicClipProperty ()
	{
		ClipProperty redClip = new ClipProperty("redMimic");
		redClip.loop = false;
		redClip.playType = ClipProperty.PlayType.ByOrder;
		redClip.defaultSpriteName = "red00";
		redClip.priority =  AnimationPriority.Low;
		
		return redClip;
	}
	
	public override bool HasTurnInToClip {
		get {
			return true;
		}
	}
	
	public override float ContagionRatio {
		get {
			return 0.05f;
		}
	}
	
	public override bool IsMapAsset {
		get {
			return false;
		}
	}
	
	public override int GetBallCollideScore {
		get {
			return 2;
		}
	}
}
