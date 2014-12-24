using UnityEngine;
using System.Collections;

public class StarItemModel : MapItemModel {
	
	public StarItemModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty starClip = new ClipProperty("starRotate");
		starClip.loop = true;
		starClip.playType = ClipProperty.PlayType.AutoStart;
		starClip.defaultSpriteName = "star00";
		starClip.priority = AnimationPriority.Medium;
		
		return starClip;
	}
	public override bool IsMapAsset {
		get {
			return true;
		}
	}
}
