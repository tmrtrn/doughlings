using UnityEngine;
using System.Collections;

public class Excitement : FallingObject {
	
	
	
	public override void Start()
	{
		base.Start();
		GetComponent<tk2dSpriteAnimator>().Play("excitement");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public override void OnTriggerEnter2D (Collider2D collision)
	{
		if(collision.tag == "Player") {
			
			GameController.Instance.levelMaintenance.Score += 5;
			GameController.Instance.spotController.ReceivedExcitement();
			
			base.OnTriggerEnter2D (collision);
			
		}
		
		
	} 
}
