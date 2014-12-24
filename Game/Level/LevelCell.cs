using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelCell : LevelCellAbstract {

	public MapItem mapItem;

	public readonly int row;
	public readonly int column;
	private readonly float left;
	private readonly float top;
	private MapItemType itemType;
	public bool inner = false;
	private MapItemStatus itemStatus = MapItemStatus.None;
	private bool CanHolder = false;
	private int receivedDamageCount;

	private HoldingLinkDirection holdingDirection = HoldingLinkDirection.None;

	public override event Action<LevelCell, DestroyType> ItemDestroyedAction;
	public override event Action<HoldingLinkDirection> OnHoldingChanged;
	public override event Action<MapItemType> OnItemTypeChanged;
	public override event Action<DamageType> OnDamageReceived;

	[Flags]
	public enum HoldingLinkDirection
	{
		None = 0,
		Right = 1,
		Left = 2,
		Top = 48,
		Top_Right = 16,
		Top_Left = 32,
		Bottom_Right = 4,
		Bottom_Left = 8,
		Bottom = 12
	}

	public Transform cTransform
	{
		get{
			return mapItem.cTransform;
		}
	}

	public LevelCell(int row, int column, float left, float top, MapItemType defaultItemType)
	{
		this.row = row; //+ 6; //force the increase row id .Cause first 6 row is empty
		this.column = column;
		this.left = left;
		this.top = top;
		this.inner = row % 2 == 1;
		SetType = defaultItemType;

	}

	public void Instantiate(GameObject goItemPrefab, Transform parent)
	{
		if(GetItemType == MapItemType.Blank)
			return;
		itemStatus = MapItemStatus.CanControl;
		GameObject goItem = GameObject.Instantiate(goItemPrefab) as GameObject;
		mapItem = goItem.GetComponent<MapItem>();
		mapItem.cTransform.parent = parent;
		mapItem.GetComponent<tk2dSprite>().SortingOrder = 2;
		mapItem.cTransform.localPosition = new Vector3(left/100, top/100,-1);
		mapItem.InstantinatedFromEditor(this);
		
	}

	public override int GetRowId {
		get {
			return row;
		}
	}

	public override int GetColumnId {
		get {
			return column;
		}
	}
	public override MapItemType GetItemType {
		get {

			return itemType;
		}
	}



	public void Translate(float target, float duration, Action<GameObject> endedAction,iTween.EaseType easyType, float delay = 0)
	{
		if(!HasMapItem){
			endedAction(null);
			return;
		}
		target /= 100;
		iTween.MoveTo(mapItem.cTransform.gameObject , iTween.Hash("y", target, "islocal",true, "time",duration, "delay",delay,  "easeType", easyType, 
		                                                    "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction"));
	}

	public bool HasMapItem
	{
		get{
			return itemStatus == MapItemStatus.OnlyHasItem || itemStatus == MapItemStatus.CanControl;
		}
	}

	public bool HasHolderFeature
	{
		get {
			return CanHolder && HasControlFromBoard ;
		}
	}



	
	public MapItemType SetType {
		set{
			itemType = value;
			CanHolder = ((int)itemType > 0 && (int)itemType<6);
			if(OnItemTypeChanged != null)
				OnItemTypeChanged(itemType);

		}
	}

	public override bool HasControlFromBoard
	{
		get{
			return itemStatus == MapItemStatus.CanControl;
		}
	}

	public MapItemStatus Status
	{
		get{
			//temprory code
			if(mapItem == null) {
				itemStatus = MapItemStatus.None;
			}

			return itemStatus;
		}
		set{
			itemStatus = value;
			if(itemStatus == MapItemStatus.OnlyHasItem && mapItem.IsMapAsset)
				cTransform.GetComponent<Collider2D>().isTrigger = true;
			else if(itemStatus == MapItemStatus.None)
				GameObject.Destroy(cTransform.GetComponent<Collider2D>());
		}
	}

	public HoldingLinkDirection HoldingDirection
	{
		get{
			return holdingDirection;
		}
		set{
			holdingDirection = value;
			if(OnHoldingChanged != null)
				OnHoldingChanged(holdingDirection);
		}
	}

	public static List<LevelCell> scannedSameTypeLinkedCells = new List<LevelCell>();

	public void ScanStrikeToLinkedCells(LevelCell selectedCell)
	{
		List<LevelCell> sortedCells = GameController.Instance.levelMaintenance.GetEditorCells.FindAll((obj) => obj.HasHolderFeature);
		sortedCells.Sort((x, y) => y.row.CompareTo(x.row));
		
		List<LevelCell> activeCells = sortedCells.FindAll( (obj)=> obj.HoldingDirection != LevelCell.HoldingLinkDirection.None &&  obj.GetItemType == selectedCell.GetItemType  );
		
		int row = selectedCell.row;
		int column = selectedCell.column;
		bool inner = selectedCell.inner;
		
		LevelCell[] linkedCells = new LevelCell[6];
		//Top Cells
		linkedCells[0] = inner ? activeCells.Find((obj)=> obj.row == selectedCell.row+1 && obj.column == column+1) : activeCells.Find((obj)=> obj.row == row+1 && obj.column == column-1);
		linkedCells[1] = activeCells.Find((obj)=> obj.row == row+1 && obj.column == column);
		//BottomCells
		linkedCells[2] = inner ? activeCells.Find((obj)=> obj.row == row-1 && obj.column == column+1) : activeCells.Find((obj)=> obj.row == row-1 && obj.column == column-1);
		linkedCells[3] = activeCells.Find((obj)=> obj.row == row-1 && obj.column == column);
		//Right Cell
		linkedCells[4] = activeCells.Find((obj)=> obj.row == row && obj.column == column+1 );
		//Left Cell
		linkedCells[5] = activeCells.Find((obj)=> obj.row == row && obj.column == column-1 );
		
		for(int i=0; i<linkedCells.Length; i++) {
			if(linkedCells[i] != null) {
				if(!scannedSameTypeLinkedCells.Contains(linkedCells[i])) {
					scannedSameTypeLinkedCells.Add(linkedCells[i]);
					linkedCells[i].ScanStrikeToLinkedCells(linkedCells[i]);
				}
			}
		}
		
		
	}

	public override bool IsStriked {
		get {
			return _isStriked;
		}
	}

	bool _isStriked = false;

	public void ApplyStrike(StrikeType strikeType)
	{
		if(GetItemType == MapItemType.Blank || Status == MapItemStatus.None)
			return;
		if(strikeType == StrikeType.Strike_ColorBall) {

		}
		
		receivedDamageCount += 1;
		if(OnDamageReceived != null)
			OnDamageReceived(((DamageType)receivedDamageCount));
		
		bool isMapAsset = mapItem.IsMapAsset;
		MapItemType nextItemType = isMapAsset ? itemType : MapItemModel.GetNextItemType(itemType);
		bool willBeDestroy = (nextItemType == MapItemType.Blank || (itemType == MapItemType.Gray && (DamageType)receivedDamageCount == DamageType.Damage_4X)) ? true : false;
		if(willBeDestroy){
			mapItem.cTransform.parent = cTransform;
			_isStriked = true;
		}
			
		SetType = willBeDestroy ? MapItemType.Blank : nextItemType;
		
		if(willBeDestroy && !isMapAsset) {
			Status = MapItemStatus.None;
			//			GameObject.Destroy(trItemObject.GetComponent<Collider>());
			ItemDestroyedAction(this, DestroyType.Damage);
			return;
		}
		else if(isMapAsset){
			_isStriked = true;
			//will be destroy end of the translation to downwards
			mapItem.cTransform.parent = GameController.Instance.trEditor;
			
			Status = MapItemStatus.OnlyHasItem;
			// trigger start translation to downwards
			mapItem.cTransform.localPosition += new Vector3(0,0, -1);
			float target = -1080f;
			float duration = Mathf.Abs((Math.Abs(target) - Math.Abs(mapItem.cTransform.localPosition.y)*100)) * GameController.Instance.gameModel.fallingMapItemDuration / Math.Abs(target); 
			float delay = GameController.Instance.gameModel.fallingMapItemStartDelay;
			Translate(target ,duration, (obj) => {
				ForceDestroy(DestroyType.Force);
			},iTween.EaseType.linear, delay);
		}
	}

	public void ForceDestroy (DestroyType destroyType)
	{

		ItemDestroyedAction(this, destroyType);
		Status = MapItemStatus.None;
		itemType = MapItemType.Blank;
		iTween.Stop(cTransform.gameObject);
	}

	public override void DestroyedFromPlayer ()
	{
		itemStatus = MapItemStatus.None;

	}

	

}
