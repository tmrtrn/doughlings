using UnityEngine;
using System;
using System.Collections;

public enum MapItemStatus {None, OnlyHasItem, CanControl}

public class MapItem : MonoBehaviour {

	public GameObject contaigonPrefab;
	public GameObject excitementPrefab;
	public GameObject gatherFxPrefab;

	private Transform trItem;
	private LevelCellAbstract levelCellAbstract;
	private MapItemModel activeModel;
	private MapItemModel prevActiveModel;

	tk2dSpriteAnimator spriteAnimator;
	float winkTimer = 0;
	float winkTimeValue = 0;
	bool CanPlayWink = false;

	public LevelCell.HoldingLinkDirection holding;
	public int row = -1;
	public int column = -1;

	public Transform cTransform
	{
		get{
			if(trItem == null)
				trItem = transform;
			return trItem;
		}
	}

	public void InstantinatedFromEditor(LevelCellAbstract levelCellAbstract)
	{
		this.levelCellAbstract = levelCellAbstract;
		hookEvents();
		ActiveModel = MapItemModel.GetItemViewPropertyByType(levelCellAbstract.GetItemType);
		row = Row;
		column = Column;
	}

	public int Row
	{
		get{
			return levelCellAbstract.GetRowId;
		}
	}

	public int Column
	{
		get{
			return levelCellAbstract.GetColumnId;
		}
	}

	public bool IsMapAsset
	{
		get{
			return ActiveModel.IsMapAsset;
		}
	}

	public bool HasControlFromBoard
	{
		get{
			return levelCellAbstract.HasControlFromBoard;
		}
	}


	void hookEvents()
	{
		levelCellAbstract.ItemDestroyedAction += HandleItemDestroyedAction;
		levelCellAbstract.OnHoldingChanged += HandleOnHoldingChanged;
		levelCellAbstract.OnItemTypeChanged += HandleOnItemTypeChanged;
		levelCellAbstract.OnDamageReceived += OnDamageReceived;
	}

	void undelegateEvents()
	{
		levelCellAbstract.ItemDestroyedAction -= HandleItemDestroyedAction;
		levelCellAbstract.OnHoldingChanged -= HandleOnHoldingChanged;
		levelCellAbstract.OnItemTypeChanged -= HandleOnItemTypeChanged;
		levelCellAbstract.OnDamageReceived -= OnDamageReceived;
	}

	void HandleOnItemTypeChanged (MapItemType obj)
	{
		ActiveModel = MapItemModel.GetItemViewPropertyByType(obj);
	}

	void HandleOnHoldingChanged (LevelCell.HoldingLinkDirection obj)
	{
		holding = obj;
//		if(row == 11 && column == 2)
//			Debug.Log(" "+obj);
	}

	void HandleItemDestroyedAction (LevelCell arg1, DestroyType arg2)
	{

	}

	IEnumerator Wait(float time, Action endAction) {
		yield return new WaitForSeconds(time);
		endAction();
	}

	public MapItemModel ActiveModel
	{
		get{
			return activeModel;
		}
		set{
			//Exit from current type if new type different
			if(activeModel == null){
				//init view
				activeModel = value;
				prevActiveModel = activeModel;
				spriteAnimator = GetComponent<tk2dSpriteAnimator>();
				ClipProperty clipProperty = activeModel.GetDefaultClipProperty();
				if(activeModel.IsMapAsset) {
					GetComponent<tk2dSprite>().SetSprite(clipProperty.defaultSpriteName);
					StartCoroutine(Wait(UnityEngine.Random.Range(0,1f), ()=>{
						PlayClip(activeModel.GetDefaultClipProperty());
					}));
					
					return;
				}
				GetComponent<tk2dSprite>().SetSprite(clipProperty.defaultSpriteName);

				if( clipProperty.playType == ClipProperty.PlayType.RandomTime ) {
					
					winkTimeValue = UnityEngine.Random.Range(2,10);
					winkTimer = Time.time;
					CanPlayWink = true;
				}
				return;
			}
			prevActiveModel = activeModel;
			activeModel = value;

			if(prevActiveModel.HasTurnInToClip) {
				//Only Damage - This is only Grren, Red, Yellow doughlings
				PlayClip(prevActiveModel.GetTurnInToClipProperty());
			}
			
			if((prevActiveModel.Type == MapItemType.Gray) && (levelCellAbstract.GetItemType != MapItemType.Blank))
				return;
			if(prevActiveModel.ContagionRatio > UnityEngine.Random.Range(0,1f))
				Instantiate(contaigonPrefab, cTransform.position, cTransform.rotation);

		}
	}


	bool PlayClip(ClipProperty clipProperty, Action endAction = null)
	{
		tk2dSpriteAnimationClip clip = spriteAnimator.GetClipByName(clipProperty.name);
		if( !spriteAnimator.Playing || (spriteAnimator.Playing && (clipProperty.priority >= clip.priority))){
			clip.priority = clipProperty.priority;
			spriteAnimator.AnimationCompleted += (anmatorArg, clipArg) =>{
				if(endAction != null)
					endAction();
			};
			
			spriteAnimator.Play(clip);
			return true;
		}
		return false;
	}

	public bool IgnoreTimeScale
	{
		get {
			return spriteAnimator.timescaleIndependant;
		}
		set {
			spriteAnimator.timescaleIndependant = value;
		}
	}

	void Destroy()
	{

		undelegateEvents();
	}

	/// <summary>
	/// Destroy the specified destroyType.OnDamageReceived, OnTypeChanged and Destroy calls respectively.
	/// </summary>
	/// <param name='destroyType'>
	/// Destroy type.
	/// </param>
	
	public void Destroy(DestroyType destroyType)
	{
		//		Debug.Log("Destroy "+destroyType);
		undelegateEvents();
		CanPlayWink = false;
		cTransform.localPosition += new Vector3(0,0,-1);


		
		if(destroyType == DestroyType.Damage) {
			if(prevActiveModel.HasDestroyClip) {
				cTransform.parent = GameController.Instance.trInstantiatedEffectsParent;
				PlayClip(prevActiveModel.GetDestroyClipProperty(), () => {
					levelCellAbstract.DestroyedFromPlayer();
					if(prevActiveModel.Type == MapItemType.Blue) {
						//Start to play healed animation


						String healingAnimName = UnityEngine.Random.Range(0,2) == 0 ? "healedBaloonMorph" : "healedRocketMorph";
						ClipProperty clipHealed = new ClipProperty(healingAnimName);
						clipHealed.priority = AnimationPriority.High;
						PlayClip(clipHealed);
						//Start animation to upwards
						float target = -1080;
						float duration =Mathf.Abs((Math.Abs(target) + Math.Abs(transform.localPosition.y)*100)) * GameController.Instance.gameModel.healingDoughlingDuration / Math.Abs(target);//Mathf.Abs(target - cTransform.localPosition.y)  / target * GameController.Instance.gameModel.healingDoughlingDuration;
						//duration = duration * 100 / 1080;
						float delay = GameController.Instance.gameModel.healingDoughlingStartDelay;
						Action<GameObject> endedAction = (go)=>{
							Destroy(gameObject);	
						};


						
						iTween.MoveTo(gameObject , iTween.Hash("y", -target/100, "islocal",true, "time",duration, "delay",delay,  "easeType", iTween.EaseType.linear, 
						                                       "dontusesendmessage",true, "endedaction", endedAction, "onComplete","changedWithAction"));
						
						GameObject excitement = GameObject.Instantiate(excitementPrefab) as GameObject;
						excitement.transform.position = transform.position;
                        			excitement.transform.parent = GameController.Instance.trInstantiatedEffectsParent;//transform.parent;
						
					}
					else {
						//Destroy Gray
						Destroy(gameObject);
					}
				});
			}
		}
		else if(destroyType == DestroyType.Force){
			//Destroy immediatly
			Destroy(gameObject);
		}
		/*	else if(destroyType == DestroyType.Force_FadeOut){
			iTween.ValueTo(gameObject, iTween.Hash(
           		 "from", 1f, "to", 0f,
            	"time", 0.5f, "easetype", iTween.EaseType.linear,
            	"onupdate", "OnUpdateForceDestroyFadeOut",
				"oncomplete","OnCompleteForceDestroyFadeOut")); 
		}
		 */
	}

	void Update()
	{
		if(CanPlayWink && ( (winkTimer + winkTimeValue)< Time.time )){
			//type is doughling
			CanPlayWink = false;
			
			if(levelCellAbstract.GetItemType == MapItemType.Blank)
				return; // Now Destroying anims is playing
			
			ClipProperty winkClipProperty = activeModel.GetWinkClipProperty();
			PlayClip(winkClipProperty);
			winkTimeValue = UnityEngine.Random.Range(3,10);
			winkTimer = Time.time;
			CanPlayWink = true;

		}
	}

	void OnCollisionEnter2D(Collision2D otherCollision)
	{
	//	Debug.Log("MAPITEM OnCollisionEnter2D "+otherCollision.gameObject.name);
	}

	/// <summary>
	/// Raises the damage received event.
	/// Firstly this and "OnItemTypeChanged" functions call respectively
	/// </summary>
	/// <param name='damageType'>
	/// Damage type.
	/// </param>
	void OnDamageReceived(DamageType damageType)
	{
		if(ActiveModel.HasDamageClip && (int)damageType < 4) {
			PlayClip(ActiveModel.GetDamageClipProperty(damageType));
		}
	}


	void OnTriggerEnter2D(Collider2D otherCollider)
	{

		if(otherCollider.gameObject.tag == "Player")
		{
			levelCellAbstract.DestroyedFromPlayer();
			Destroy(GetComponent<Collider2D>());
			iTween.Stop(gameObject);
			if(IsMapAsset) {
				MorphState morphState = ActiveModel.GetMorphTo;

				if(morphState != MorphState.None && levelCellAbstract.IsStriked) {

					//GameController.Instance.morphController.SetState(morphState, MorphController.STATUS_MORPHFROM);

					Debug.Log("OnTriggerEnter2D");
					if(GameController.Instance.morphController.CurrentState != morphState) {
						GameController.Instance.morphController.SetState(MorphState.Morpheus, MorphController.STATUS_MORPHTO, ()=>{
							GameController.Instance.morphController.SetState(morphState, MorphController.STATUS_MORPHFROM);
						});
					}
					else {
						GameController.Instance.morphController.SetState(morphState, MorphController.STATUS_MORPHFROM);
					}


				}
                if (ActiveModel.Type == MapItemType.Life)
                    GameController.Instance.levelMaintenance.LifeCount += 1;
                else if (ActiveModel.Type == MapItemType.Star) {
                    GameController.Instance.levelMaintenance.StarCount += 1;
                    GameController.Instance.levelMaintenance.Score += 10;
                }
                else if (activeModel.Type == MapItemType.MorphGunner || activeModel.Type == MapItemType.MorphRobo || activeModel.Type == MapItemType.MorphSmash || activeModel.Type == MapItemType.MorphSpider)
                    GameController.Instance.levelMaintenance.MorphCount += 1;

				GameController.Instance.levelMaintenance.Score += ActiveModel.GetCharacterCollideScore;



				
				GameObject goGatherFx = GameObject.Instantiate(gatherFxPrefab, cTransform.position, cTransform.rotation) as GameObject;
                goGatherFx.transform.parent = GameController.Instance.trInstantiatedEffectsParent;
				goGatherFx.GetComponent<tk2dSpriteAnimator>().AnimationCompleted += (arg1,arg2) => { Destroy(goGatherFx); };
				goGatherFx.GetComponent<tk2dSpriteAnimator>().Play("gatherFx");
				
				iTween.ValueTo(gameObject, iTween.Hash(
					"from", 1f, "to", 0f,
					"time", 0.5f, "easetype", iTween.EaseType.linear,
					"onupdate", "OnUpdateForceDestroyFadeOut",
					"oncomplete","OnCompleteForceDestroyFadeOut")); 
				
			}
			else {
				GameController.Instance.levelMaintenance.Score -= prevActiveModel.GetBallCollideScore;
				GameController.Instance.morphController.Damage();
				Destroy(gameObject);
			}


		}
	}
	
	public void OnUpdateForceDestroyFadeOut(float newAlpha) {
		
		Color color = gameObject.GetComponent<tk2dBaseSprite>().color;
		color.a = newAlpha;
		gameObject.GetComponent<tk2dBaseSprite>().color = color;
		
	}
	public void OnCompleteForceDestroyFadeOut()
	{
		Destroy(gameObject);
	}

	public bool CanPlayMimicAnimation()
	{
		if(ActiveModel == null){
			// if current type model is null object will be destroyed. ignore mimic anim
			return false;
		}
		
		return ActiveModel.HasMimicAnimation() && !spriteAnimator.Playing && levelCellAbstract.GetItemType != MapItemType.Blank && levelCellAbstract.HasControlFromBoard;
	}

	public void PlayMimicAnimation()
	{
		PlayClip(ActiveModel.GetMimicClipProperty());
	}

	void OnMouseDown()
	{
		GameController.Instance.levelMaintenance.SetStrike(Row, Column,StrikeType.Strike_ColorBall);
	}


}
