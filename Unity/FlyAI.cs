using UnityEngine;
using System.Collections;

public class FlyAI : MonoBehaviour {
	
	public float speed = 7f;
	public float size = 1;
	public float cooldown;
	public float FlyTimer;
	//public Transform target;
	float direction = -1f; //??
	
	private Animator anim;
	//private Transform myTransform;
	bool right;
	void Awake(){
		//myTransform = transform;
	}
	// Use this for initialization
	void Start () {
		//GameObject go = GameObject.FindGameObjectWithTag ("Player");
		//target = go.transform;
		FlyTimer = 0;
		
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D>().velocity = new Vector2 ( speed * (-direction), GetComponent<Rigidbody2D>().velocity.y);
		transform.localScale = new Vector3 (direction*size, size, 1);
		EnemyHealth eh = (EnemyHealth)GetComponent("EnemyHealth");
		if (eh.curHealth <= 0) {
			speed = 0;
			anim.SetBool("isDead",true);		
		}
		
		if (FlyTimer > 0) {
			FlyTimer -= Time.deltaTime;
			if (FlyTimer < 0){FlyTimer = 0;}
		}
		if (FlyTimer == 0 && eh.curHealth > 0) {
			direction *= (-1);
			FlyTimer = cooldown;
		}
	}
	//столкновение
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "GroundItem")
			direction *= -1f;
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if ((col.gameObject.name == "forEnemys") || (col.gameObject.name == "Hero"))
			direction *= -1f;
	}
	void death()
	{
		Destroy (gameObject);
	}
	public bool Rdir()
	{
		if (direction < 0) {return false;} 
		else {return true;}
	}
	public AudioSource src;
	void playSFX(AudioClip aud)
	{
		src.clip = aud;
		src.Play ();
	}
}
