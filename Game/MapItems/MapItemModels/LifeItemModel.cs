using UnityEngine;
using System.Collections;

public class LifeItemModel : MapItemModel {
	
	public LifeItemModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty lifeClip = new ClipProperty("lifeAnim");
		lifeClip.loop = true;
		lifeClip.playType = ClipProperty.PlayType.AutoStart;
		lifeClip.defaultSpriteName ="life00";
		lifeClip.priority =  AnimationPriority.Medium;
		
		return lifeClip;
	}
	
	public override bool IsMapAsset {
		get {
			return true;
		}
	}
}
