using UnityEngine;
using System.Collections;

public class GreenItemModel : MapItemModel {
	
	public GreenItemModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty greenClip = new ClipProperty("greenWink");
		greenClip.loop = false;
		greenClip.playType = ClipProperty.PlayType.RandomTime;
		greenClip.defaultSpriteName = "green00";
		greenClip.priority =  AnimationPriority.Low;
		
		return greenClip;
	}
	
	public override bool HasMimicAnimation ()
	{
		return true;
	}
	
	public override ClipProperty GetMimicClipProperty ()
	{
		ClipProperty greenClip = new ClipProperty("greenMimic");
		greenClip.loop = false;
		greenClip.playType = ClipProperty.PlayType.ByOrder;
		greenClip.defaultSpriteName = "green00";
		greenClip.priority =  AnimationPriority.Low;
		
		return greenClip;
	}
	
	public override bool HasTurnInToClip {
		get {
			return true;
		}
	}
	
	public override float ContagionRatio {
		get {
			return 0.01f;
		}
	}
	
	public override bool IsMapAsset {
		get {
			return false;
		}
	}
	
	public override int GetBallCollideScore {
		get {
			return 4;
		}
	}
}
