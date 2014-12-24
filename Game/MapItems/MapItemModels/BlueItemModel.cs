using UnityEngine;
using System.Collections;

public class BlueItemModel : MapItemModel {
	
	public BlueItemModel(MapItemType type) : base(type){}
	
	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty blueClip = new ClipProperty("blueWink");
		blueClip.loop = false;
		blueClip.playType = ClipProperty.PlayType.RandomTime;
		blueClip.defaultSpriteName = "blue00";
		blueClip.priority = AnimationPriority.Low;
		
		return blueClip;
	}
	
	public override bool HasMimicAnimation ()
	{
		return true;
	}
	
	public override ClipProperty GetMimicClipProperty ()
	{
		ClipProperty blueClip = new ClipProperty("blueMimic");
		blueClip.loop = false;
		blueClip.playType = ClipProperty.PlayType.ByOrder;
		blueClip.defaultSpriteName = "blue00";
		blueClip.priority = AnimationPriority.Low;
		
		return blueClip;
	}
	
	public override bool HasTurnInToClip {
		get {
			return false;
		}
	}
	
	public override bool HasDestroyClip {
		get {
			return true;
		}
	}

	public override bool IsMapAsset {
		get {
			return false;
		}
	}
	
	public override ClipProperty GetDestroyClipProperty ()
	{
		return base.GetDestroyClipProperty ();
	}
	
	public override int GetBallCollideScore {
		get {
			return 5;
		}
	}
	
}
