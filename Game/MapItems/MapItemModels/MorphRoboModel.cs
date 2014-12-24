using UnityEngine;
using System.Collections;

public class MorphRoboModel : MapItemModel {

	public MorphRoboModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty starClip = new ClipProperty("morphRobo");
		starClip.loop = true;
		starClip.playType = ClipProperty.PlayType.AutoStart;
		starClip.defaultSpriteName = "morphRobo00";
		starClip.priority = AnimationPriority.Medium;
		
		return starClip;
	}
	public override bool IsMapAsset {
		get {
			return true;
		}
	}
	
	public override int GetCharacterCollideScore {
		get {
			return 10;
		}
	}

	public override MorphState GetMorphTo {
		get {
			return MorphState.Robo;
		}
	}
}
