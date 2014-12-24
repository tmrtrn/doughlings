using UnityEngine;
using System.Collections;

public class BallTest : MonoBehaviour {

	public enum BallType
	{
		Normal,
		SpiderBall,
		ColorBall
	}

	public GameObject StrikeAnimPrefab;
	public LayerMask paddleMask;

	public bool IsMainBall = false;
	public bool throwed = false;
	[System.NonSerialized]
	public BallType ballType = BallType.Normal;

	// Use this for initialization
	void Start () {
	//	rigidbody2D.velocity = new Vector2(-5,5);
		collider2D.isTrigger = true;
	}

    float dX = 0;
    float dY = 0;

	public void ThrowBall(Vector2 direction) {


        	dX = direction.normalized.x;
		dY = direction.normalized.y;
		
		float _dX = direction.normalized.x * GameController.Instance.gameModel.ballSpeed ;
		float _dY = direction.normalized.y * GameController.Instance.gameModel.ballSpeed ;
		rigidbody2D.velocity = new Vector2(_dX,_dY);
		throwed = true;
		collider2D.isTrigger = false;
	}

	float normRayDistance = 0.5f;
	float currentRayDistance = 0.3f;
	float roboRayDistance = 1f;

    void FixedUpdate()
    {
    //   cTransform.position += new Vector3(dX, dY, 0) * GameController.Instance.gameModel.ballSpeed/100;
		if(!throwed)
			return;



		RaycastHit2D hit = Physics2D.Raycast(cTransform.position, rigidbody2D.velocity.normalized, currentRayDistance, paddleMask);
		if(hit.collider != null) {

			if(GameController.Instance.morphController.CurrentState == MorphState.Robo) {
				rigidbody2D.velocity = Vector2.zero;

				((GameHUDScreen)(GUIController.Instance.currentScreen)).StartBallTargetIndicator(this, true);

				return;
			}

			var heading = hit.transform.position - cTransform.position;
			var distance = heading.sqrMagnitude;
			var direction =  heading / distance;

			float sum = Mathf.Abs(direction.x) + Mathf.Abs(direction.y);
			direction.x = (1/sum) * direction.x;
			direction.y = (1/sum) * direction.y;


			float _dX = direction.normalized.x * GameController.Instance.gameModel.ballSpeed;
			float _dY = direction.normalized.y * GameController.Instance.gameModel.ballSpeed;
	//		Debug.Log("FixedUpdate "+direction.x+" "+direction.y+" X,Y  "+_dX+" "+_dY+"  normalize : "+direction.normalized.x+" "+direction.normalized.y+" mag "+direction.magnitude);
			Vector3 normal = hit.normal;
		//	direction.x *= -1;
		
		//	rigidbody2D.velocity = Vector3.Reflect(direction,normal).normalized* GameController.Instance.gameModel.ballSpeed ;//new Vector2(_dX,_dY);
			rigidbody2D.velocity = new Vector2(-_dX, -_dY);
			if(true)
				return;


		
//			Debug.DrawRay(hit.point, hit.normal*3, Color.blue);  
//			Vector2 sizeOfPlayerCollider = GameController.Instance.morphController.CurrentBehavior.ColliderSize;
//			float ratioDistance = (hit.collider.transform.position.x - hit.point.x)/(sizeOfPlayerCollider.x/2);
//			ratioDistance = Mathf.Clamp(ratioDistance, -1f, 1f);
//			float newVelX =  -ratioDistance;
//
//
//			Vector2 buVel = rigidbody2D.velocity;
//			float sum = Mathf.Abs(buVel.x) + Mathf.Abs(buVel.y);
//			float left = sum*(newVelX);
//
//			Vector2 updatedVel = new Vector2(left , sum - left).normalized; 
//			updatedVel.x = Mathf.Clamp(updatedVel.x, -0.7f, 0.7f);
//			updatedVel.y = 1- Mathf.Abs(updatedVel.x);
//
//			
//			rigidbody2D.velocity = updatedVel * GameController.Instance.gameModel.ballSpeed;
//			buVel = rigidbody2D.velocity;
//			sum = Mathf.Abs(buVel.x) + Mathf.Abs(buVel.y);
//
//

		
		}
    }

	Transform _trBall;

	public Transform cTransform
	{
		get{
			if(_trBall == null)
				_trBall = transform;
			return _trBall;
		}
	}

	public BallTest PlayBallLoadingAnim(BallType typeOfBall, System.Action<BallTest> animEndedAction)
	{
		if(GameController.Instance.morphController.State == MorphState.Robo)
			currentRayDistance = roboRayDistance;
		else
			currentRayDistance = normRayDistance;

		rigidbody2D.velocity = Vector2.zero;
		this.ballType = typeOfBall;

		string animName = "ballEnter";
		if(typeOfBall == BallType.SpiderBall){
			animName = "ballEnterSpider";
		}
		else if(typeOfBall == BallType.ColorBall)
			animName = "ballEnterColor";

		GetComponent<tk2dSpriteAnimator>().AnimationCompleted += (arg1,arg2) => {
            if(typeOfBall == BallType.SpiderBall)
                GetComponent<tk2dSpriteAnimator>().Play("ballSpiderLoop");
		else if(typeOfBall == BallType.ColorBall)
		  GetComponent<tk2dSpriteAnimator>().Play("ballColorLoop");
            animEndedAction(this);
        };
		GetComponent<tk2dSpriteAnimator>().Play(animName);
		return this;
	}


	void OnTriggerEnter2D(Collider2D otherCollider)
	{

		if(otherCollider.gameObject.tag == "Player") {
			return;

			Vector2 backupVel = rigidbody2D.velocity;
			rigidbody2D.velocity = Vector2.zero;
			Vector2 sizeOfPlayerCollider = GameController.Instance.morphController.CurrentBehavior.ColliderSize;
			Vector2 positionOfCollider = new Vector2(GameController.Instance.morphController.CurrentXPosition ,GameController.Instance.morphController.CurrentYPosition);
			float maxReflectionAngle = 90;
			float distanceToColliderCenter = cTransform.position.x - positionOfCollider.x;
			float distanceRatio = distanceToColliderCenter / (sizeOfPlayerCollider.x/2);
			float reflectionAngle = maxReflectionAngle * distanceRatio;
			distanceRatio = Mathf.Clamp(Mathf.Abs(distanceRatio), 0.2f ,0.7f);
			float newVelX =  Mathf.Sin(distanceRatio * 90 * Mathf.PI / 180);
			float newVelY =   Mathf.Cos(distanceRatio * 90 * Mathf.PI / 180);
			if((distanceToColliderCenter / (sizeOfPlayerCollider.x/2)) < 0)
				newVelX *= -1;
			Vector2 updatedVel = new Vector2(newVelX, newVelY);
            		updatedVel.Normalize();


			float sum = Mathf.Abs(dX) + Mathf.Abs(dY);
			float sumNew = Mathf.Abs(updatedVel.x) + Mathf.Abs(updatedVel.y);
			float oran = Mathf.Abs(sum / sumNew);
			updatedVel.x *= oran;
			updatedVel.y *= oran;

			updatedVel.x *= (newVelX < 0 ? -1 : 1);
			updatedVel.y *= (newVelY < 0 ? -1 : 1);

           

			rigidbody2D.velocity = updatedVel.normalized * GameController.Instance.gameModel.ballSpeed;
            		dX = updatedVel.x;
            		dY = updatedVel.y;
		
		}
	}

    float collisionTestTimer = 0;

	void OnCollisionEnter2D(Collision2D collision)
	{
        
        
		if(collision.gameObject.tag =="BottomCollider" && !GameController.Instance.IsDebug) {
            		StartCoroutine(GameController.Instance.BallCollideWithBottomCollider(this));
			return;
		}
		
		if(collision.gameObject.tag == "Player")
		{
			ballCollideItemCounter = -1;
			

			
		}
		else{

		}

//        if (collisionTestTimer + 0.05f > Time.time)
//        {
//			return;
//        }
//        else {
//            foreach (ContactPoint2D contact in collision.contacts) //Find collision point
//            {
//
//                //Find the BOUNCE of the object
//
//                Vector2 velocity = (2 * (Vector3.Dot(new Vector3(dX, dY, 0), Vector3.Normalize(contact.normal))) * Vector3.Normalize(contact.normal)) - new Vector3(dX, dY, 0); //Following formula  v' = 2 * (v . n) * n - v
//
//                velocity.x *= -1; //Had to multiply everything by -1. Don't know why, but it was all backwards.
//                velocity.y *= -1;
//           //     Debug.Log("dX " + dX + " dY " + dY + "   newVel " + velocity.x + "  " + velocity.y);
//                dX = velocity.x;
//                dY = velocity.y;
//                cTransform.position += new Vector3(dX, dY, 0) * GameController.Instance.gameModel.ballSpeed / 80;
//                //         Vector2 velocity = Vector3.Reflect(new Vector3(dX, dY, 0), collision.contacts[0].normal);
//
//                
//                //          dX = velocity.x;
//                //          dY = velocity.y;
//                break;
//            }
//            collisionTestTimer = Time.time;
//
//        }
//       
        
        

		
		
		string animName = "ballCollide";
		Vector3 point = collision.contacts[0].point;
		point.z -= 2;
		if(collision.transform.tag == "MapItem"){
			MapItem itemView = collision.gameObject.GetComponent<MapItem>();
			if(itemView.IsMapAsset)
				animName = "hitFx";
			else
				point.z += 2;
			BallCollidedWithMapItem(itemView.ActiveModel);

			MapItemAnimationController.Instance.BallCollidedMapItem(itemView.Row, itemView.Column);

			StrikeType strikeType = StrikeType.Strike_1X;
			if(ballType == BallType.ColorBall)
				strikeType = StrikeType.Strike_ColorBall;

			GameController.Instance.levelMaintenance.SetStrike(itemView.Row, itemView.Column,strikeType);
		}
		
		GameObject goCollide = Instantiate(StrikeAnimPrefab, point, cTransform.localRotation) as GameObject;
        goCollide.transform.parent = GameController.Instance.trInstantiatedEffectsParent;
		goCollide.GetComponent<tk2dSpriteAnimator>().AnimationCompleted += (arg1,arg2) => { Destroy(goCollide); };
		goCollide.GetComponent<tk2dSpriteAnimator>().Play(animName);
		
	}

	
	public void DestroyBall()
	{
		Destroy(gameObject);
	}

	Vector2 buVelocity;

	public void PauseBall()
	{
		buVelocity = rigidbody2D.velocity;
		rigidbody2D.velocity = Vector2.zero;
	}

	public void ResumeBall()
	{
		rigidbody2D.velocity = buVelocity;
	}


    int ballCollideItemCounter = -1;

    void BallCollidedWithMapItem(MapItemModel itemModel)
    {
        int score = 0;
        score += itemModel.GetBallCollideScore;

        if (GameController.Instance.morphController.CurrentBehavior.CanWinScoreBallCollideWithItem)
        {
            ballCollideItemCounter += 1;
            score += ballCollideItemCounter;
            if (ballCollideItemCounter == 8)
                ballCollideItemCounter = -1;
        }
        else {
            ballCollideItemCounter = -1;
        }
        GameController.Instance.levelMaintenance.Score += score;
    }

}
