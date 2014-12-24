using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DigitControl : MonoBehaviour {
	
	//	public tk2dSpriteCollectionData collectionData;
	public List<DigitProperty> digits;
	
	public IEnumerator SetNumber(int number)
	{
		int count = 0;
		
		for(int i=0; i<digits.Count; i++) {
			int digit = number % 10;
			number = number/10;
			digits[i].sprite.SetSprite("digitSize"+digits[i].texture_size+"_"+digit);
		}
		yield break;
	}
}

[System.Serializable]
public class DigitProperty 
{
	public tk2dSprite sprite;
	public int texture_size = 1;
	
}