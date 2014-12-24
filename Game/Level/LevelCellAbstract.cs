using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class LevelCellAbstract  {

	public abstract int GetRowId{get;}
	public abstract int GetColumnId{get;}
	public abstract MapItemType GetItemType{get;}
	public abstract bool HasControlFromBoard{get;}
	public abstract event Action<LevelCell, DestroyType> ItemDestroyedAction;
	public abstract event Action<LevelCell.HoldingLinkDirection> OnHoldingChanged;
	public abstract event Action<MapItemType> OnItemTypeChanged;
	public abstract event System.Action<DamageType> OnDamageReceived;
	public abstract void DestroyedFromPlayer();
	public abstract bool IsStriked{get;}
}
