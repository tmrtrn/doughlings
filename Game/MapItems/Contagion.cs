using UnityEngine;
using System.Collections;

public class Contagion : FallingObject {

	public override void Start()
	{
		base.Start();
		GetComponent<tk2dSpriteAnimator>().Play("contagion");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public override void OnTriggerEnter2D (Collider2D collision)
	{
		if(collision.tag == "Player") {
			
			GameController.Instance.levelMaintenance.Score -= 5;
			GameController.Instance.spotController.ReceivedContagion();
			
			base.OnTriggerEnter2D (collision);
			
			GameController.Instance.morphController.Damage();
		}
		
		
	} 
}
