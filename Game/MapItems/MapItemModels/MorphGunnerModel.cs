using UnityEngine;
using System.Collections;

public class MorphGunnerModel : MapItemModel {

	public MorphGunnerModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty clip = new ClipProperty("morphGunner");
		clip.loop = true;
		clip.playType = ClipProperty.PlayType.AutoStart;
		clip.defaultSpriteName = "morphGunner00";
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
			return MorphState.Gunner;
		}
	}
	
}
