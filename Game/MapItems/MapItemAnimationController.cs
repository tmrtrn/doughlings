using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapItemAnimationController : MonoBehaviour {

	static MapItemAnimationController instance;
	bool working = false;
	
	public static MapItemAnimationController Instance {
		get{
			if(instance == null){
				instance = FindObjectOfType(typeof(MapItemAnimationController)) as MapItemAnimationController;
				if(instance == null)
					instance = new GameObject("MapItemAnimationController").AddComponent<MapItemAnimationController>();
			}
			return instance;
		}
	}

	void Awake()
	{
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Destroy(gameObject);
			return;
		}
	}
	
	void OnDestroy() {
		if(instance == this) {
			instance = null;
		}
	}

	void OnEnable()
	{
		LevelMaintenance.MapStateChangedAction += HandleMapStateChangedAction;
	}

	void OnDisable()
	{
		LevelMaintenance.MapStateChangedAction -= HandleMapStateChangedAction;
		working = false;
	}

	void HandleMapStateChangedAction (LevelMaintenance.MapState state)
	{
		if(((state & LevelMaintenance.MapState.Loaded) == LevelMaintenance.MapState.Loaded) && !IsPlaying) {
			StartCoroutine(StartController());
		}
		else if(((state & LevelMaintenance.MapState.Ended) == LevelMaintenance.MapState.Ended) && IsPlaying) {
			working = false;
		}
	}

	public bool IsPlaying
	{
		get{
			return working;
		}
	}

	Vector2 randomTimeBounds = new Vector2(2,5);

	IEnumerator StartController()
	{
		working = true;
		while(working) {

			yield return new WaitForSeconds(UnityEngine.Random.Range(randomTimeBounds.x , randomTimeBounds.y));
			List<LevelCell> cellsOnView = GameController.Instance.levelMaintenance.GetEditorCells.FindAll( (next)=> next.HasControlFromBoard);
			List<MapItem> mapItemsCanPlayMimic = new List<MapItem>();
			cellsOnView.ForEach( (next)=> {
				try{
					MapItem mapItem = next.cTransform.GetComponent<MapItem>();
					if(mapItem.CanPlayMimicAnimation()) {
						mapItemsCanPlayMimic.Add(mapItem);
					}
				}
				catch(Exception ex){

				}

			});

			int rand = UnityEngine.Random.Range(0,mapItemsCanPlayMimic.Count);
			if(rand < mapItemsCanPlayMimic.Count)
				mapItemsCanPlayMimic[rand].PlayMimicAnimation();
		}
	}

	public void BallCollidedMapItem(int row, int column)
	{
		LevelCell collidedCell = GameController.Instance.levelMaintenance.GetEditorCells.Find( (next)=> (next.row == row) && (next.column == column) );

		Vector2[] iaNeighboardRowColumn = new Vector2[6];
		iaNeighboardRowColumn[0] = new Vector2(row+1,column);
		iaNeighboardRowColumn[1] = new Vector2(row-1, column);
		if(collidedCell.inner){

			iaNeighboardRowColumn[2] = new Vector2(row+1, column+1);
			iaNeighboardRowColumn[3] = new Vector2(row-1, column+1);
		}
		else{
			iaNeighboardRowColumn[2] = new Vector2(row+1, column-1);
			iaNeighboardRowColumn[3] = new Vector2(row-1, column-1);
		}
		iaNeighboardRowColumn[4] = new Vector2(row,column-1);
		iaNeighboardRowColumn[4] = new Vector2(row,column+1);

		List<MapItem> neighboardsCollidedCell = new List<MapItem>();

		for(int i=0; i<iaNeighboardRowColumn.Length; i++) {
			LevelCell cell = GameController.Instance.levelMaintenance.GetEditorCells.Find( (next)=> (next.row == iaNeighboardRowColumn[i].x) && (next.column == iaNeighboardRowColumn[i].y) );
			if(cell != null) {
				if(cell.HasControlFromBoard){
					MapItem mapItem = cell.cTransform.GetComponent<MapItem>();
					if(mapItem.CanPlayMimicAnimation()) {
						neighboardsCollidedCell.Add(mapItem);
					}
				}
			}
		}
		int rand = UnityEngine.Random.Range(0,neighboardsCollidedCell.Count);
		if(rand < neighboardsCollidedCell.Count)
			neighboardsCollidedCell[rand].PlayMimicAnimation();
		
	}

}
