using UnityEngine;
using System.Collections;

public class UnlockStarIndicator : MonoBehaviour {

	public GameObject goStar1;
	public GameObject goStar2;
	public GameObject goStar3;

	void OnEnable()
	{
		goStar1.SetActive(false);
		goStar2.SetActive(false);
		goStar3.SetActive(false);
	}

	public void SetStar(int unlockedStarCount) {
		StopCoroutine("StartIndicator");
		if(unlockedStarCount == 0)
			return;
		else if(unlockedStarCount == 2) {
			goStar1.SetActive(true);
			goStar2.SetActive(true);
			return;
		}
		else if(unlockedStarCount == 3) {
			goStar1.SetActive(true);
			goStar2.SetActive(true);
			goStar3.SetActive(true);
			return;
		}

	}


	public IEnumerator StartIndicator(int unlockedStarCount, System.Action endAction) {

		if(unlockedStarCount == 0)
			endAction();
		if(unlockedStarCount == 1){
			goStar1.SetActive(true);
		}
		else if(unlockedStarCount == 2) {
			goStar1.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			goStar2.SetActive(true);
		}
		else if(unlockedStarCount == 3) {
			goStar1.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			goStar2.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			goStar3.SetActive(true);
		}
		if(endAction != null)
			endAction();

	}
}
