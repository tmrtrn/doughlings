using UnityEngine;
using System.Collections;

public class GrayItemModel : MapItemModel {
	
	public GrayItemModel(MapItemType type) : base(type){}

	public override ClipProperty GetDefaultClipProperty ()
	{
		ClipProperty grayClip = new ClipProperty("grayDamageAnim1");
		grayClip.loop = false;
		grayClip.playType = ClipProperty.PlayType.ByOrder;
		grayClip.defaultSpriteName = "gray00";
		grayClip.priority = AnimationPriority.Medium;
		
		return grayClip;
	}
	
	public override bool IsMapAsset {
		get {
			return false;
		}
	}
	
	public override bool HasDestroyClip {
		get {
			return true;
		}
	}
	
	public override bool HasDamageClip {
		get {
			return true;
		}
	}
	
	public override float ContagionRatio {
		get {
			return 0.1f;
		}
	}

    public override int GetBallCollideScore
    {
        get
        {
            return 1;
        }
    }
	
	public override ClipProperty GetDestroyClipProperty ()
	{
		return base.GetDestroyClipProperty ();
	}
	
	public override ClipProperty GetDamageClipProperty (DamageType damageType)
	{
//		ClipProperty clip = new ClipProperty( damageType == DamageType.Damage_4X ? "grayDestroy" :  "grayDamageAnim"+(int)damageType);
		ClipProperty clip = new ClipProperty("grayDamageAnim"+(int)damageType);
		clip.priority = AnimationPriority.High;
		return clip;
	}
}
