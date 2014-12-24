using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour {

	public GameObject mapItemPrefab;

	Transform trLevelEditor;
	private List<LevelCell> cells = new List<LevelCell>();
	private int droppedCount = 0;
	public event Action<LevelCellAbstract, DestroyType> OnMapItemDestroyed;
	private int _instantiatedLastRowId = -1;

	public void Reset()
	{
		droppedCount = 0;

		cells.Clear();
		foreach (Transform child in cTransform) {
			Destroy(child.gameObject);
		}
	}

	public Transform cTransform
	{
		get{
			if(trLevelEditor == null)
				trLevelEditor = transform;
			return trLevelEditor;
		}
	}


	public int InstantinatedLastRowId { 
		get { 
			return _instantiatedLastRowId;
//			cells.Sort( (x, y) => y.row.CompareTo(x.row) );
//			if(cells.Count > 0)
//				return cells[0].row;
//			return -1;
		}
	}

	public List<LevelCell> GetCells
	{
		get{
			return cells;
		}
	}
	

	public LevelCell InstantinateCell(TodoMapCell mapCell)
	{
		int left = (mapCell.column *(GameController.Instance.gameModel.widthPerAsset + GameController.Instance.gameModel.widthOffset) ) + (mapCell.row % 2 == 1 ? GameController.Instance.gameModel.innerLeftAnchorPoint : GameController.Instance.gameModel.outerLeftAnchorPoint);
		int top =  GameController.Instance.gameModel.heightPerAsset / 2;
		LevelCell instCell = new LevelCell(mapCell.row, mapCell.column, left, top, mapCell.itemType);
		instCell.Instantiate(mapItemPrefab, cTransform);
		instCell.ItemDestroyedAction += OnItemDestroyed;
		cells.Add(instCell);
		_instantiatedLastRowId = mapCell.row;
		return instCell;
	}
	
	public void TranslateAllToOneRow(float duration,float delay,iTween.EaseType easeType, Action ended)
	{
		GameObject goCarrier = new GameObject("Carrier");
		goCarrier.transform.parent = cTransform;
		goCarrier.transform.localPosition = Vector3.zero;
		
		cells.FindAll((obj) => obj.HasControlFromBoard).ForEach( (obj) => {
			obj.cTransform.parent = goCarrier.transform;
		});
		Action<GameObject> translated = (go) => 
		{
			droppedCount += 1;
			cells.FindAll((obj) => obj.HasControlFromBoard).ForEach( (obj) => {
				obj.cTransform.parent = cTransform;
			});
			GameObject.Destroy(go); // Destroyed gocarrier object
			ended();
		};
		
		float target = -1 * (GameController.Instance.gameModel.heightPerAsset + GameController.Instance.gameModel.heightOffset);
		iTween.MoveTo(goCarrier , iTween.Hash("y", target/100 , "islocal",true, "time",duration, "delay",delay, "easeType", iTween.EaseType.linear, 
		                                      "dontusesendmessage",true, "endedaction", translated, "onComplete","changedWithAction"));
	}

	public void TranslateToTopRow(List<LevelCell> selCells,float duration,float delay_perItem, float delay_start, iTween.EaseType easyType, Action ended )
	{
		if(selCells.Count == 0)
			ended();
		int translatedCellCount = 0;
		Action<GameObject> translated = (go) => {
			translatedCellCount += 1;
			if(translatedCellCount == selCells.Count){
				ended();
			}
		};
		
		selCells.ForEach((obj) => obj.Translate(GameController.Instance.gameModel.topAnchorPoint, duration, translated,easyType, delay_start + (delay_perItem * obj.column)));
	}

	void OnItemDestroyed(LevelCell cell, DestroyType destroyType)
	{
	
		cells.Remove(cell);
		cell.mapItem.Destroy(destroyType);
		if(OnMapItemDestroyed != null)
			OnMapItemDestroyed(cell, destroyType);
		
	}

	public void ApplyStrike(int row, int column, StrikeType strikeType, bool ignoreScan = false)
	{
		LevelCell selectedCell = cells.Find((obj) => ((obj.row == row) && (obj.column == column)));

		if(strikeType == StrikeType.Strike_ColorBall) {

			LevelCell.scannedSameTypeLinkedCells.Add(selectedCell);
			selectedCell.ScanStrikeToLinkedCells(selectedCell);
			LevelCell.scannedSameTypeLinkedCells.ForEach((obj)=> obj.ApplyStrike(strikeType));
			LevelCell.scannedSameTypeLinkedCells.Clear();
		}
		else
			selectedCell.ApplyStrike(strikeType);

		if(!ignoreScan)
			StartCoroutine(ScanRows());
	}





	/// <summary>
	/// NOW WE DONT USE THIS FUNCTION !!! Forces the fall down cells.If 13. row is last row , The undermost row is starting to fall down.
	/// </summary>
	public void ForceFallCells()
	{
		if(droppedCount > 12){
			List<LevelCell> list = cells.FindAll((obj) => obj.row == (droppedCount - 13) && obj.HasControlFromBoard);
			if(list != null) {
				FallCells(list);
			}
		}
	}

	public void ForceFallAllCells(Action translationEnd)
	{
		List<LevelCell> forcedCells =  cells.FindAll((obj) => obj.HasControlFromBoard);
		forcedCells.ForEach( (obj) => obj.Status = MapItemStatus.OnlyHasItem);
		FallCells(forcedCells, translationEnd, true);
	}

	void FallCells(List<LevelCell> list, Action translationEnd = null, bool disableDelay = false)
	{
		GameObject goCarrier = new GameObject("CarrierScan");
		goCarrier.transform.parent = cTransform;
		goCarrier.transform.localPosition = new Vector3(0,0,-1);
		
		list.ForEach((obj) => {
			obj.cTransform.parent = goCarrier.transform;
			obj.Status = MapItemStatus.OnlyHasItem;
		});
		
		Action<GameObject> translated = (go) => 
		{
			foreach (Transform child in go.transform) {
				MapItem item = child.GetComponent<MapItem>();
				if(item != null) {
					item.transform.parent = null;
					LevelCell cell = cells.Find( (obj) => obj.GetRowId == item.Row && obj.GetColumnId == item.Column );
					if(cell != null){
						cell.ForceDestroy(DestroyType.Force);
					}
					else {
						Debug.Log("CANT FIND LEVELCELL !!!");
					}
				}

			}
			/*
			MapItem[] items = go.GetComponentsInChildren<MapItem>();
			if(items == null)
			return;
			for(int i=0; i<items.Length; i++)
			{
				items[i].transform.parent = null;
				cells.Find( (obj) => obj.GetRowId == items[i].Row && obj.GetColumnId == items[i].Column ).ForceDestroy(DestroyType.Force);
			}
			
			
			GameObject.Destroy(go); // Destroyed gocarrier object
			*/
			GameObject.Destroy(go); // Destroyed gocarrier object
			if(translationEnd != null)
				translationEnd();
		};
		
		
		
		float target = -1 * 1080;
		float duration = GameController.Instance.gameModel.scannedAndFallingDuration;
		float delay = GameController.Instance.gameModel.scannedAndFallingStartDelay;
        	if (disableDelay)
           		delay = 0;
		iTween.MoveTo(goCarrier , iTween.Hash("y", target /100 , "islocal",true, "time",duration,"delay",delay,  "easeType", iTween.EaseType.linear, 
		                                      "dontusesendmessage",true, "endedaction", translated, "onComplete","changedWithAction"));
	}

	public IEnumerator ScanRows()
	{


		cells.ForEach( (obj)=> obj.HoldingDirection= LevelCell.HoldingLinkDirection.None);
		cells.FindAll((obj) => obj.HasControlFromBoard).ForEach((obj) => {

			obj.HoldingDirection = (obj.row == InstantinatedLastRowId   ? LevelCell.HoldingLinkDirection.Top : LevelCell.HoldingLinkDirection.None);
		});
		
		//		cells.FindAll((obj) => obj.HasControlFromBoard && obj.row == InstantinatedLastRowId).ForEach( (obj) => 
		//		{
		//			obj.HoldingDirection = TodoBoardCell.HoldingLinkDirection.Top;
		//		});
		//		
		ScanTopToBottom();
		ScanLeftToRight();
		ScanRightToLeft();
		ScanBottomToTop();
		
		ScanTopToBottom();
		ScanRightToLeft();
		ScanLeftToRight();
		ScanBottomToTop();
		
		ScanBottomToTop();
		ScanRightToLeft();
		ScanLeftToRight();
		ScanTopToBottom();
		
		ScanBottomToTop();
		ScanLeftToRight();
		ScanRightToLeft();
		ScanTopToBottom();
		
		List<LevelCell> fallingCells = cells.FindAll((obj) => obj.HasControlFromBoard == true && obj.HoldingDirection == LevelCell.HoldingLinkDirection.None);
		if(fallingCells == null)
			yield break;
		if(fallingCells.Count == 0)
			yield break;
		
		FallCells(fallingCells);
		yield break;


	}



	void ScanTopToBottom()
	{
		List<LevelCell> sortedCells = cells.FindAll((obj) => obj.HasHolderFeature);
		sortedCells.Sort((x, y) => y.row.CompareTo(x.row));


		
		sortedCells.ForEach((obj) => {
			if(obj.HoldingDirection > 0) {
				
				LevelCell bottom_1 = obj.inner ? sortedCells.Find((arg) => arg.row == (obj.row -1) && arg.column == obj.column) : sortedCells.Find((arg) => arg.row == (obj.row - 1)  && arg.column == obj.column-1);
				LevelCell bottom_2 = obj.inner ? sortedCells.Find((arg) => arg.row == (obj.row -1) && arg.column == (obj.column+1)) : sortedCells.Find((arg) => arg.row == (obj.row - 1)  && arg.column == obj.column);
				if(bottom_1 != null) {
					bottom_1.HoldingDirection |= LevelCell.HoldingLinkDirection.Top;	
				}		
				if(bottom_2 != null){
					bottom_2.HoldingDirection |= LevelCell.HoldingLinkDirection.Top;
				}
				
				
			}
		});
		
	}
	
	void ScanLeftToRight()
	{
		List<LevelCell> sortedCells = cells.FindAll((obj) => obj.HasHolderFeature);
		sortedCells.Sort((x, y) => x.column.CompareTo(y.column));
		
		sortedCells.ForEach((obj) => {
			if(obj.HoldingDirection > 0) {
				LevelCell next = sortedCells.Find( (arg) => arg.row == obj.row && arg.column == (obj.column +1));
				if(next != null)
					next.HoldingDirection |= LevelCell.HoldingLinkDirection.Left;
				
			}
		});
	}
	
	void ScanRightToLeft()
	{
		List<LevelCell> sortedCells = cells.FindAll((obj) => obj.HasHolderFeature);
		sortedCells.Sort((x, y) => y.column.CompareTo(x.column));
		
		sortedCells.ForEach((obj) => {
			if(obj.HoldingDirection > 0) {
				LevelCell next = sortedCells.Find( (arg) => arg.row == obj.row && arg.column == (obj.column -1));
				if(next != null)
					next.HoldingDirection |= LevelCell.HoldingLinkDirection.Right;
				
			}
		});
	}
	
	void ScanBottomToTop()
	{
		List<LevelCell> sortedCells = cells.FindAll((obj) => obj.HasControlFromBoard);
		sortedCells.Sort((x, y) => x.row.CompareTo(y.row));
		
		sortedCells.ForEach((obj) => {
			if(obj.HoldingDirection >0){
				LevelCell bottom_1 = obj.inner ? sortedCells.Find((arg) => arg.row == (obj.row +1) && arg.column == obj.column) : sortedCells.Find((arg) => arg.row == (obj.row + 1)  && arg.column == obj.column-1);
				LevelCell bottom_2 = obj.inner ? sortedCells.Find((arg) => arg.row == (obj.row +1) && arg.column == (obj.column+1)) : sortedCells.Find((arg) => arg.row == (obj.row + 1)  && arg.column == obj.column);
				if(bottom_1 != null) {
					bottom_1.HoldingDirection |= LevelCell.HoldingLinkDirection.Bottom;	
				}		
				if(bottom_2 != null){
					bottom_2.HoldingDirection |= LevelCell.HoldingLinkDirection.Bottom;
				}
			}
		});
	}
	
	
	
}
