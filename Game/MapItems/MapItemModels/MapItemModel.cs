using UnityEngine;
using System.Collections;

public enum MapItemType
{
	Blank = 0,
	Gray = 1,
	Blue = 2,
	Green = 3,
	Red = 4,
	Yellow = 5,
	Star = 7,
	Life = 8,
	MorphGunner = 9,
	MorphRobo = 10,
	MorphSmash = 11,
	MorphSpider = 12
}

public enum AnimationPriority
{
	Low = 0,
	Medium = 1,
	High = 2
}

public enum DamageType
{
	Damage_1X = 1,
	Damage_2X = 2,
	Damage_3X = 3,
	Damage_4X = 4
}

public enum StrikeType
{
	Strike_1X,
	Strike_ColorBall
}


public enum DestroyType
{
	Damage,
	Force,
	//	Force_FadeOut
}

public abstract class MapItemModel  {
	
	
	
	protected MapItemType type;
	
	public MapItemModel(MapItemType type)
	{
		this.type = type;
	}
	
	public abstract ClipProperty GetDefaultClipProperty();
	public abstract bool IsMapAsset{get;}
	
	public MapItemType Type
	{
		get{
			return this.type;
		}
	}
	
	public virtual ClipProperty GetWinkClipProperty()
	{
		return GetDefaultClipProperty();
	}
	
	public virtual ClipProperty GetMimicClipProperty()
	{
		return null;
	}
	
	public virtual bool HasTurnInToClip
	{
		get{
			return false;
		}
	}
	
	public virtual float ContagionRatio
	{
		get {
			return 0;
		}
	}
	
	public virtual bool HasDamageClip
	{
		get{
			return false;
		}
	}
	
	public virtual bool HasDestroyClip
	{
		get{
			return false;
		}
	}
	
	public virtual ClipProperty GetTurnInToClipProperty()
	{
		ClipProperty clip = new ClipProperty(type.ToString().ToLower()+"TurnInTo");
		clip.priority = AnimationPriority.High;
		return clip;
	}
	
	public virtual ClipProperty GetDamageClipProperty(DamageType damageType = DamageType.Damage_1X)
	{
		ClipProperty clip = new ClipProperty(type.ToString().ToLower()+"Damage");
		clip.priority = AnimationPriority.High;
		return clip;
	}
	
	public virtual ClipProperty GetDestroyClipProperty()
	{
		ClipProperty clip = new ClipProperty(type.ToString().ToLower()+"Destroy");
		clip.priority = AnimationPriority.High;
		return clip;
	}
	
	public virtual bool HasMimicAnimation()
	{
		return false;
	}
	
	public virtual int GetBallCollideScore
	{
		get{
			return 0;
		}
	}
	
	public virtual int GetCharacterCollideScore
	{
		get{return 0;}
	}
	
	public virtual MorphState GetMorphTo
	{
		get{
			return MorphState.None;
		}
	}
	
	
	
	public static MapItemModel GetItemViewPropertyByType(MapItemType type)
	{
		
		if(type == MapItemType.Gray)
			return new GrayItemModel(type);
		else if(type == MapItemType.Blue)
			return new BlueItemModel(type);
		else if(type == MapItemType.Green)
			return new GreenItemModel(type);
		else if(type == MapItemType.Red)
			return new RedItemModel(type);
		else if(type == MapItemType.Yellow)
			return new YellowItemModel(type);
//		else if(type == TodoMapItemType.Shapeshift)
//			return new ShapeshiftItemView(type);
		else if(type == MapItemType.Star)
			return new StarItemModel(type);
		else if(type == MapItemType.Life)
			return new LifeItemModel(type);
		else if(type == MapItemType.MorphGunner)
			return new MorphGunnerModel(type);
		else if(type == MapItemType.MorphRobo)
			return new MorphRoboModel(type);
		else if(type == MapItemType.MorphSmash)
			return new MorphSmashModel(type);
		else if(type == MapItemType.MorphSpider)
			return new MorphSpiderModel(type);
		
		
		return null; 
	}

	public static MapItemType GetNextItemType(MapItemType currentType)
	{
		if(currentType == MapItemType.Blue)
			return MapItemType.Blank;
		if(currentType == MapItemType.Green)
			return MapItemType.Blue;
		if(currentType == MapItemType.Yellow)
			return MapItemType.Green;
		if(currentType == MapItemType.Red)
			return MapItemType.Yellow;
		if(currentType == MapItemType.Gray)
			return MapItemType.Gray;
		//		if(currentType == Shapeshift)
		//			return Blank;
		if(currentType == MapItemType.Star)
			return MapItemType.Blank;
		if(currentType == MapItemType.Life)
			return MapItemType.Blank;
		if(currentType == MapItemType.MorphGunner || currentType == MapItemType.MorphRobo || currentType == MapItemType.MorphSmash || currentType == MapItemType.MorphSpider)
			return MapItemType.Blank;
		return MapItemType.Blank;
	}
	
}

public class ClipProperty
{

	public enum PlayType{ByOrder, AutoStart, RandomTime}
	public ClipProperty(string name)
	{
		this.name = name;
	}
	public readonly string name = "";
	public PlayType playType = PlayType.ByOrder;
	public bool loop = false;
	public string defaultSpriteName = "";
	public AnimationPriority priority = AnimationPriority.Low;
}