using UnityEngine;
using System.Collections;

public class GunnerBullet : MonoBehaviour {

	public tk2dSpriteAnimator animator;
	public float duration = 2;

	public void Fire(float delay, bool ignoreTimeScale)
	{
		animator.timescaleIndependant = ignoreTimeScale;
		animator.Play("gunBulletExist");

		iTween.MoveTo(gameObject , iTween.Hash("y", 10.8f, "islocal",true, "time",duration, "delay",delay,"ignoretimescale",ignoreTimeScale, "easeType", iTween.EaseType.linear,"oncomplete","OnFireEnd"));
	}

	void OnFireEnd()
	{
		iTween.Stop(gameObject);
		animator.AnimationCompleted += (tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2) => {
			Destroy(gameObject);
		};
		animator.Play("gunBulletDestroy");
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{

		if(otherCollider.transform.tag !="MapItem")
			return;

		MapItem collidedItem = otherCollider.transform.GetComponent<MapItem>();

		if(!collidedItem.IsMapAsset || (collidedItem.HasControlFromBoard && collidedItem.IsMapAsset) ) {
			OnFireEnd();
			GameController.Instance.levelMaintenance.SetStrike(collidedItem.Row, collidedItem.Column, StrikeType.Strike_1X);
		}

	}

}
