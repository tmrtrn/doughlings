using UnityEngine;
using System.Collections;

public class GameModel : MonoBehaviour {
	
	
	public int totalColumnCount = 12; // Default Value 12
	public int totalRowCount = 12;
	public int widthPerAsset = 78; // Default Value 78
	public int widthOffset = 2;
	public int heightPerAsset = 78;
	public int heightOffset = -8;
	public int topAnchorPoint = 100; // This should be change
	public int innerLeftAnchorPoint = 80;
	public int outerLeftAnchorPoint = 40;
	
	
	public int maxRowCountOnMapLoaded = 12;
	
	public float droDownDuration = 0.5f;
	public float dropDownStartDelay = 0;
	public iTween.EaseType dropDownEase = iTween.EaseType.linear;
	
	public float intervalFalling = 4;
	public float fallingDuration = 2;
	public float fallingDelayPerItem = 0.1f;
	public float fallingStartDelay = 0;
	public iTween.EaseType fallingEaseType = iTween.EaseType.linear;
	
	public float ballSpeed = 10;
	
	public float characterSpeed = 100;
	
	public float leftCharacterRestriction = 400;
	public float rightCharacterRestriction = 1500;
	
	public float scannedAndFallingDuration = 2;
	public float scannedAndFallingStartDelay = 1;
	
	//When applied strip
	public float fallingMapItemDuration = 2;
	public float fallingMapItemStartDelay = 0;
	
	//like excitement
	public float fallingObjectsFallingDuration = 2;
	public float fallingObjectsFallingStartDelay = 1;
	
	public float healingDoughlingDuration = 4;
	public float healingDoughlingStartDelay = 0;
	
	
}