using UnityEngine;
using System.Collections;

public class MorphSpiderModel : MapItemModel {

	public MorphSpiderModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty clip = new ClipProperty("morphSpider");
		clip.loop = true;
		clip.playType = ClipProperty.PlayType.AutoStart;
		clip.defaultSpriteName = "morphSpider00";
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
			return MorphState.Spider;
		}
	}
}
