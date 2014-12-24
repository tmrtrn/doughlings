using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelMaintenance : MonoBehaviour {

	public LevelEditor levelEditor;


	[Flags]
	public enum MapState {None = 0 , Loading = 1, Loaded =2 , CanDrop = 4, Dropping = 8, Ended = 16  }

	private int currentMapId = -1;
    	public int validMapId = -1;
	public MapState _mapState = MapState.None;
	public float timeCounter;
	public static event Action<MapState> MapStateChangedAction;
	float nextLoadingRowTimeValue;

	public int ActiveMapId
	{
		get{return currentMapId;}
		set{
			forceRoboShowoffPause = false;

			//Destroy current map
			levelEditor.OnMapItemDestroyed -= HandleOnMapItemDestroyed;
			levelEditor.Reset();
			_mapState = MapState.None;

            currentMapId = value;

            if (currentMapId == -1)
            {

                return;
            }
            validMapId = currentMapId;
			nextLoadingRowTimeValue = GameController.Instance.gameModel.intervalFalling;
		

			StartCoroutine(LoadMap());
			levelEditor.OnMapItemDestroyed += HandleOnMapItemDestroyed;

			LifeCount = ActiveMap.DefaultLife;
		}
	}



	TodoMap ActiveMap
	{
		get{
			return DataStorage.Instance.GetMapById(currentMapId);
		}
	}

	public MapState SelectedMapState
	{
		get { return _mapState; }
		set {
			//			if((_mapState & value) != value){
			_mapState = value;
			if (MapStateChangedAction != null)
				MapStateChangedAction(_mapState);
			//			}
		}
	}

	public List<LevelCell> GetEditorCells
	{
		get{
			return levelEditor.GetCells;
		}
	}

	IEnumerator LoadMap()
	{


		
		int createdRowCount = 0;
		int createdColumnId = 0;
		bool uCanLoadNewOne = false;
		
		if(ActiveMap.mapCells.Count > 0)
		{
			createdRowCount += 1;
			uCanLoadNewOne = true;
			SelectedMapState = MapState.Loading;
		}
		else
		{
			uCanLoadNewOne = false;
			SelectedMapState = MapState.None;
			yield return null;
		}
		
		ActiveMap.mapCells.Sort((x, y) => y.row.CompareTo(x.row));
		if(ActiveMap.mapCells[0] == null){
			Debug.Log("Empty map !!!");
			return false;
		}
		
		Action checkCompleted = () => {
			if(ActiveMap.mapCells[0].row  > createdRowCount && createdRowCount < GameController.Instance.gameModel.maxRowCountOnMapLoaded )
			{
				createdRowCount += 1;
				uCanLoadNewOne = true;
			}
			else
			{
				uCanLoadNewOne = false;
				SelectedMapState = MapState.Loaded;
				Debug.Log("ALL ROWS LOADED");
				StartDropping();
				StartCoroutine(levelEditor.ScanRows());
	//			board.ScanRows();
			}
			
		};
		
		while((SelectedMapState & MapState.Loading) == MapState.Loading)
		{
			if(uCanLoadNewOne)
			{
				uCanLoadNewOne = false;
				LoadOneRow(true,ActiveMap.mapCells.FindAll( (obj) => obj.row == createdRowCount -1), checkCompleted);
			}
			yield return null;
			
		}
		yield return null;
	}



	private void LoadOneRow(bool isMapLoading, List<TodoMapCell> rowCells, Action end)
	{
		
		bool loadedToTopRow = false;
		bool droppedOthers = false;
		List<LevelCell> instCells = new List<LevelCell>();
		
		float duration_translate_all_row = isMapLoading ? 0.02f : GameController.Instance.gameModel.droDownDuration;
		float delay_translate_all_row = isMapLoading ? 0 : GameController.Instance.gameModel.dropDownStartDelay;
		iTween.EaseType ease_translate_all_row = isMapLoading ? iTween.EaseType.linear : GameController.Instance.gameModel.dropDownEase;
		
		
		levelEditor.TranslateAllToOneRow(duration_translate_all_row, delay_translate_all_row, ease_translate_all_row, () => {
			droppedOthers = true;
			if(loadedToTopRow){
				end();
			}
			
		});
		
		rowCells.ForEach((obj) => 
		                 {
			instCells.Add(levelEditor.InstantinateCell(obj));
		});
		
		float duration_translate_top_row = isMapLoading ? 0.02f : GameController.Instance.gameModel.fallingDuration;
		float delay_perItem_translate_top_row = isMapLoading ? 0.01f : GameController.Instance.gameModel.fallingDelayPerItem;
		iTween.EaseType ease_translate_top_row = isMapLoading ? iTween.EaseType.linear : GameController.Instance.gameModel.fallingEaseType;
		float delay_start_translate_top_row = isMapLoading ? 0: GameController.Instance.gameModel.fallingStartDelay;
		
		
		levelEditor.TranslateToTopRow(instCells, duration_translate_top_row, delay_perItem_translate_top_row, delay_start_translate_top_row, ease_translate_top_row, () => {
			loadedToTopRow = true;
			if(droppedOthers){
				end();
			}
		});
	}

	void StartDropping()
	{
		timeCounter = Time.time;
		SelectedMapState = SelectedMapState | MapState.CanDrop;
	}

	void LoadRowToGame()
	{
		SelectedMapState = SelectedMapState | MapState.Dropping;
		LoadOneRow(false, ActiveMap.mapCells.FindAll( (obj) => obj.row == levelEditor.InstantinatedLastRowId +1), () => {
			SelectedMapState = SelectedMapState & ~MapState.Dropping;
			timeCounter = Time.time;
			ActiveMap.mapCells.Sort((x, y) => y.row.CompareTo(x.row));
			if(ActiveMap.mapCells[0].row < levelEditor.InstantinatedLastRowId+1 && (SelectedMapState & MapState.Ended) != MapState.Ended ){
				SelectedMapState |= MapState.Ended;
				nextLoadingRowTimeValue = nextLoadingRowTimeValue * 6;
				return;
			}
			else if((SelectedMapState & MapState.Ended) == MapState.Ended ) {
				//The end of the nextLoadingRowTimeValue * 6;
				SelectedMapState = SelectedMapState & ~MapState.CanDrop;
				levelEditor.ForceFallAllCells(()=>{

				});
				return;
			}
			
			
			levelEditor.ForceFallCells();
			StartCoroutine(levelEditor.ScanRows());


		});
	}

	void HandleOnMapItemDestroyed (LevelCellAbstract arg1, DestroyType arg2)
	{
		if((SelectedMapState & MapState.Loaded) == MapState.Loaded && levelEditor.GetCells.FindAll((obj)=> obj.HasControlFromBoard).Count < 24
		   && (SelectedMapState & MapState.Ended) != MapState.Ended && (SelectedMapState & MapState.Dropping) != MapState.Dropping){
			timeCounter = Time.time; // Delay next dropDown
			LoadRowToGame();
		}
	}
	

	public bool forceRoboShowoffPause = false;
	void Update() {

		if(forceRoboShowoffPause)
			return;
		
		if (GameController.Instance.State == GameController.GameState.Running && (SelectedMapState & MapState.CanDrop) == MapState.CanDrop && 
		    (SelectedMapState & MapState.Dropping) != MapState.Dropping)
		{
			if (timeUpdate(Time.time)) {
				LoadRowToGame();
			}
		}

		if((SelectedMapState & MapState.Ended) == MapState.Ended) {
            		if (levelEditor.GetCells.FindAll((obj) => (obj.Status == MapItemStatus.CanControl)).Count == 0) {
                		if (GameController.Instance.trEditor.childCount == 0 && GameController.Instance.trInstantiatedEffectsParent.childCount == 0) {
                    			_mapState = MapState.None;
                   			 StartCoroutine(GameController.Instance.LevelVictory());
                		}
            		}

		}
	
	}

	bool timeUpdate(float time)
	{
		if (timeCounter + nextLoadingRowTimeValue < time) {
			timeCounter = time;
			return true;
		}
		return false;
	}

	public void SetStrike(int row, int column, StrikeType type)
	{
		levelEditor.ApplyStrike(row, column, type);
		StartCoroutine(levelEditor.ScanRows());
	}

	#region Score - Life Actions
	int score = 0;
	public static event Action<int> OnScoreChanged;
	int lifeCount = 0;
	public static event Action<int> OnLifeCountChanged;
	int unlockedStarCount = 0;
	int collectedStarCount = 0;
	int spotCount = 0;
	public static event Action<int> OnUnlockedStarCountChanged;
	public static event Action<float> OnStarCountChanged;
    int morphCount = 0;

	public int Score
	{
		get{return score;}
		set{
			if(score != value){
				
				score = value;
				if(score < 0)
					score = 0;
				if(OnScoreChanged != null)
					OnScoreChanged(score);
			}
			
		}
	}
	
	public int LifeCount
	{
		get{return lifeCount;}
		set{
			if(value != lifeCount)
			{
				lifeCount = value;
				if(OnLifeCountChanged != null)
					OnLifeCountChanged(lifeCount);
			}
		}
	}

    public int MorphCount
    {
        get {
            return morphCount;
        }
        set {
            morphCount = value;
        }
    }

	public int UnlockedStarCount {
		get{
			return unlockedStarCount;
		}
	}
	
	public int StarCount
	{
		get{return collectedStarCount;}
		set{
			int totalStarCount = ActiveMap.mapCells.FindAll( (obj)=> obj.itemType == MapItemType.Star).Count;
			
			collectedStarCount = value;
			float ratio = (float)collectedStarCount/ totalStarCount;
			if(OnStarCountChanged != null)
				OnStarCountChanged(ratio);

				int ratioUnlock = ActiveMap.mapCells.FindAll( (obj)=> obj.itemType == MapItemType.Star).Count / 3;
                unlockedStarCount = collectedStarCount / ratioUnlock;
//				if(collectedStarCount >= ratioUnlock * (unlockedStarCount+1)) {
//					unlockedStarCount += 1;
//					if(OnUnlockedStarCountChanged != null)
//						OnUnlockedStarCountChanged(unlockedStarCount);
//				}


		}
	}

	public int SpotCount
	{
		get{
			return spotCount;
		}
		set {
			spotCount = value;
			if(spotCount < 0)
				spotCount = 0;
		}
	}

	public int NeedToCountineLifeCount
	{
		get{
			return levelEditor.InstantinatedLastRowId -11 ;
		}
	}
	
	#endregion
}
