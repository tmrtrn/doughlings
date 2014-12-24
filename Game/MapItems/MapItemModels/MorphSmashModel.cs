using UnityEngine;
using System.Collections;

public class MorphSmashModel : MapItemModel {

	public MorphSmashModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty clip = new ClipProperty("morphSmash");
		clip.loop = true;
		clip.playType = ClipProperty.PlayType.AutoStart;
		clip.defaultSpriteName = "morphSmash00";
		clip.priority = AnimationPriority.Medium;
		
		return clip;
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
			return MorphState.Smash;
		}
	}
}
