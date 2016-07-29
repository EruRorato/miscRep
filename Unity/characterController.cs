using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterController : MonoBehaviour {
	public enum State{Stay,Run,Jump,Atack,Atacked,Dead};
	public State state;
	public float maxSpeed = 10f;
	public float jumpForce = 700f;
	public float score=0;
	public float Keys=0;
	public Texture HealthTexture;
	public int health=5;
	public bool facingRight = true;
	public bool grounded = false;
	bool isSit = false;
	public bool isProtected = false;
	public Transform groundCheck;
	public float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	private Animator anim;
	public float move;
	private bool NotEnoughGems = false;
	public AudioSource src2;
	public AudioClip pause;
	public GameObject Cam;
	public GameObject Red,Blue;
	//Quaternion NormRot;
	// Use this for initialization
	void Start () 
	{
		Cursor.visible = false;
		if (Application.loadedLevelName == "ScMnu") 
		{
			PlayerPrefs.SetFloat("Player Score",0f);
			//PlayerPrefs.SetInt("Char",0);
		}

		if (Application.loadedLevelName != "ScMnu") 
		{
			if (PlayerPrefs.GetInt("Char")==0)
			{

				characterController ch = (characterController)Blue.GetComponent("characterController");
				ch.enabled = false;
				Destroy(Blue);
				if(Application.loadedLevelName!="Sc1"  && Application.loadedLevelName!="ScMini")
				{
					CameraController2D cc2 = (CameraController2D)Cam.GetComponent ("CameraController2D");
					cc2.SetTarget(Red.transform);
				}
				else
				{
					if(Application.loadedLevelName!="ScMini")
					{
						newcam nc = (newcam)Cam.GetComponent("newcam");
						nc.target = Red.transform;
					}
				}
				GameObject fr = GameObject.Find ("Fairy");
				UIFollowTarget fl = (UIFollowTarget)fr.GetComponent ("UIFollowTarget");
				fl.target = Red.transform;
				fr = GameObject.Find ("FairyTextForm");
				fl = (UIFollowTarget)fr.GetComponent ("UIFollowTarget");
				fl.target = Red.transform;
			}

			if (PlayerPrefs.GetInt("Char")==1)
			{
				characterController ch = (characterController)Red.GetComponent("characterController");
				ch.enabled = false;
				Destroy(Red);
				if(Application.loadedLevelName!="Sc1" && Application.loadedLevelName!="ScMini")
				{
					CameraController2D cc2 = (CameraController2D)Cam.GetComponent ("CameraController2D");
					cc2.SetTarget(Blue.transform);
				}
				else
				{
					if(Application.loadedLevelName!="ScMini")
					{
						newcam nc = (newcam)Cam.GetComponent("newcam");
						nc.target = Blue.transform;
					}
				}
				GameObject fr = GameObject.Find ("Fairy");
				UIFollowTarget fl = (UIFollowTarget)fr.GetComponent ("UIFollowTarget");
				fl.target = Blue.transform;
				fr = GameObject.Find ("FairyTextForm");
				fl = (UIFollowTarget)fr.GetComponent ("UIFollowTarget");
				fl.target = Blue.transform;
			}
		}

		//if (Application.loadedLevel != 1) 
		//{
			score = PlayerPrefs.GetFloat ("Player Score");
		//} 

		if (PlayerPrefs.GetFloat ("RespX") != 0 && PlayerPrefs.GetFloat ("RespX") != null && PlayerPrefs.GetInt("RespDead")==1) 
		{
			if(PlayerPrefs.GetInt("Char")==1) {Blue.gameObject.transform.position = new Vector3 (PlayerPrefs.GetFloat ("RespX"), PlayerPrefs.GetFloat ("RespY"), PlayerPrefs.GetFloat("myZ"));}
			if(PlayerPrefs.GetInt("Char")==0) {Red.gameObject.transform.position = new Vector3 (PlayerPrefs.GetFloat ("RespX"), PlayerPrefs.GetFloat ("RespY"), PlayerPrefs.GetFloat("myZ"));}
			PlayerPrefs.SetInt("RespDead",0);
		}
		//NormRot = transform.rotation;
		anim = GetComponent<Animator>();


	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
		move = Input.GetAxis ("Horizontal");
		anim.SetFloat("Speed", Mathf.Abs(move));
		anim.SetBool ("IsGround", grounded);
		anim.SetFloat ("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
		if (!grounded)
			return;
		
	}
	
	void Update()
	{
		//if (!grounded && transform.rotation.z!=0) {
			//transform.rotation = NormRot;		
		//}
		PlayerHealth ph=(PlayerHealth)GetComponent("PlayerHealth");
		//РїСЂРѕРІРµСЂРєР° Р¶РёРІ РёР»Рё РЅРµС‚
		if(ph.curHealth<=0){anim.SetBool("alive",false);state = State.Dead;PlayerPrefs.SetFloat("myZ",transform.position.z);}
		if (ph.curHealth > 0) {
						//РћР±СЂР°Р±РѕС‚РєР° РєР»Р°РІРёС€
			if (grounded && (Input.GetKeyDown (KeyCode.UpArrow))) {

					anim.SetBool ("IsGround", false);
					GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, jumpForce));
			}
			if (!isSit && anim.GetBool ("hitZ") == false && anim.GetBool ("hitX") == false && anim.GetBool ("hitC") == false && (anim.GetCurrentAnimatorStateInfo (0).IsName ("idle") || anim.GetCurrentAnimatorStateInfo (0).IsName ("run") || anim.GetCurrentAnimatorStateInfo (0).IsName ("Jump"))) {
					GetComponent<Rigidbody2D>().velocity = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			}
			if (grounded && (Input.GetKeyDown (KeyCode.DownArrow))) {
					anim.SetBool ("IsSit", true);
					isSit = true;
			}
			if (grounded && isSit && (Input.GetKeyUp (KeyCode.DownArrow))) {
					anim.SetBool ("IsSit", false);
					isSit = false;
			}
			//РћР±СЂР°С‚РѕРєР° СѓРґР°СЂРѕРІ
			if ((Input.GetKeyDown (KeyCode.Z))) {
					anim.SetBool ("hitZ", true);
					anim.SetBool ("isAtack", true);
			}
			if ((Input.GetKeyDown (KeyCode.X))) {
					anim.SetBool ("hitX", true);
					anim.SetBool ("isAtack", true);
			}
			if ((Input.GetKeyDown (KeyCode.C))) {
					anim.SetBool ("hitC", true);
					anim.SetBool ("isAtack", true);
			}
			//РѕС‚Р¶Р°С‚РёРµ РєР»Р°РІРёС€ СѓРґР°СЂРѕРІ
			if (Input.GetKeyUp (KeyCode.Z)) {
					anim.SetBool ("hitZ", false);
			}
			if (Input.GetKeyUp (KeyCode.X)) {
					anim.SetBool ("hitX", false);
			}
			if (Input.GetKeyUp (KeyCode.C)) {
					anim.SetBool ("hitC", false);
			}
			//РЈРїСЂР°РІР»РµРЅРёРµ РїРѕРІРѕСЂРѕС‚РѕРј Р°РЅРёРјР°С†РёРё
			if (move > 0 && !facingRight)
					Flip ();
			else if (move < 0 && facingRight)
					Flip ();

			//РїСЂРѕРІРµСЂРєР° СЃРѕСЃС‚РѕСЏРЅРёСЏ

				if(anim.GetBool("isAtacked")==true) //РјРѕР¶РµС‚ Р±С‹С‚СЊ РїСЂРёРіРѕРґРёС‚СЃСЏ РїРѕР·Р¶Рµ
				{
					state = State.Atacked;
					Debug.Log("Atacked!");
				}
				else
				{
				if(anim.GetBool("IsGround")==false){state = State.Jump;}
				else
				{
					if(move!=0){state = State.Run;}
					else
					{
						if(anim.GetBool("isAtack")==true)
						{
							state=State.Atack;
						}
						else{state = State.Stay;}
					}
				}
			}
			}
			//СѓРґР°Р»РёС‚СЊ
			if (Input.GetKey (KeyCode.Escape)) {
					Application.Quit();
			}

			if (Input.GetKey (KeyCode.R)) {
			PlayerPrefs.SetFloat("myZ",transform.position.z);
				PlayerPrefs.SetInt("RespDead",1);
				Application.LoadLevel (Application.loadedLevel);
			}
		if (Input.GetKey (KeyCode.O)) {Application.LoadLevel("ScSel");}
		if (Input.GetKey (KeyCode.P)) {Application.LoadLevel("ScMnu");}
		if (Input.GetKeyDown (KeyCode.Space)) {
			GameObject pauseObj = GameObject.Find("Paused");
			UILabel uil = (UILabel)pauseObj.GetComponent("UILabel");
				//if(Time.timeScale!=0){uil.enabled = true;Time.timeScale=0;}
			//else{Time.timeScale=1;uil.enabled = false;}
			AudioSource CamSnd = (AudioSource)Cam.GetComponent("AudioSource");
			if(Time.timeScale!=0){uil.enabled = true;Time.timeScale=0;src2.Stop();CamSnd.Pause();src.clip = pause;src.Play();}
			else{Time.timeScale=1;uil.enabled = false;src2.Play();src.clip = pause;src.Play();CamSnd.Play();}
		}


	}
	
	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}   
	//РџРµСЂРµСЃРµС‡РµРЅРёРµ СЃ С‚СЂРёРіРіРµСЂР°РјРё
	public AudioClip objectCollect;
	public AudioClip MainObjectCollect;
	void OnTriggerEnter2D(Collider2D col)
	{
		if ((col.gameObject.name == "dieCollider") || (col.gameObject.name == "SpinSaw")) 
		{

			Application.LoadLevel (Application.loadedLevel);
		}
		if (col.gameObject.name == "dieCollider2") 
		{
			PlayerHealth ph = (PlayerHealth)GetComponent("PlayerHealth");
			ph.AddjustCurrentHealth(-5);
		}
		if (col.gameObject.tag == "Respawn") 
		{
			PlayerPrefs.SetFloat("Player Score",score);
			PlayerPrefs.SetFloat("RespX",col.gameObject.transform.position.x);
			PlayerPrefs.SetFloat("RespY",col.gameObject.transform.position.y);
		}
		if (col.gameObject.name == "RedGem" || col.gameObject.name == "BlueGem") 
		{
			score+=50;
			src.clip=objectCollect;
			src.Play();
			Destroy (col.gameObject);
		}
		if (col.gameObject.name == "Key") 
		{
			Keys++;
			src.clip=objectCollect;
			src.Play();
			Destroy (col.gameObject);
		}
		if (col.gameObject.name == "Coin") 
		{
			score+=10;
			src.clip=MainObjectCollect;
			src.Play();
			Destroy (col.gameObject);
		}
		if (col.gameObject.name == "HealthPot") 
		{
			PlayerHealth ph = (PlayerHealth)GetComponent("PlayerHealth");
			if(ph.curHealth<5){ph.AddjustCurrentHealth(+1);}
			else{score+=100;}
			src.clip=MainObjectCollect;
			src.Play();
			Destroy (col.gameObject);
		}
		if (col.gameObject.name == "endLevel") {
				PlayerPrefs.SetFloat("Player Score",score);
				Application.LoadLevel ("Lvl1mini");
		}
		if (col.gameObject.name == "ToLvlMini") {
			PlayerPrefs.SetFloat("Player Score",score);
			Application.LoadLevel ("ScMini");
		}
		if (col.gameObject.name == "endMiniLevel") {
			PlayerPrefs.SetFloat("Player Score",score);
			GameObject drag = GameObject.Find("Dragon");
			DragonController dc = (DragonController)drag.GetComponent("DragonController");
			dc.enabled = true;
			BoxCollider2D coll = (BoxCollider2D)drag.GetComponent("BoxCollider2D");
			coll.enabled = true;
			AudioSource ac = (AudioSource)drag.GetComponent("AudioSource");
			ac.enabled = true;
			Destroy(gameObject);
			GameObject dest = GameObject.Find("Fairy");
			Destroy(dest.gameObject);
			dest = GameObject.Find("FairyTextForm");
			Destroy(dest.gameObject);
			dest = GameObject.Find("endMiniLevel");
			endMiniScript scr = (endMiniScript)dest.GetComponent("endMiniScript");
			scr.enabled = true;
			//Application.LoadLevel ("Sc3");
		}
		if(col.gameObject.name == "Restrt")
		{
			Application.LoadLevel (Application.loadedLevel);
		}
		/*if (col.gameObject.name == "showFairy") {
			showFairy SF = (showFairy)col.GetComponent("showFairy");
			SF.Spawn();
			Destroy(col.gameObject);
		}*/
		if (col.gameObject.name == "TextSpawn") {
			//GameObject root = GameObject.Find("UI Root");
			//root. = true;
			GameObject Target = GameObject.Find("Fairy");
			UI2DSprite sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = true;
			Target = GameObject.Find("FairyTextForm");
			sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = true;
			Target = GameObject.Find("FairyText");
			AppearFairyText ap = (AppearFairyText)col.gameObject.GetComponent("AppearFairyText");
			UILabel lable = (UILabel)Target.gameObject.GetComponent("UILabel");
			lable.text = ap.Text;
			lable.enabled = true;
			TypewriterEffect tw = (TypewriterEffect) Target.transform.GetComponent("TypewriterEffect");
			tw.enabled = true;
			Destroy(col.gameObject);

		}
		if (col.gameObject.name == "TextHide") {
			GameObject Target = GameObject.Find("Fairy");
			UI2DSprite sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = false;
			Target = GameObject.Find("FairyTextForm");
			sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = false;
			Target = GameObject.Find("FairyText");
			UILabel lable = (UILabel)Target.gameObject.GetComponent("UILabel");
			lable.enabled = false;
			TypewriterEffect tw = (TypewriterEffect) Target.transform.GetComponent("TypewriterEffect");
			tw.enabled = false;
			tw.ResetToBeginning();
			Destroy(col.gameObject);
		}
		if (col.gameObject.name == "DialogueSpawn") {
			GameObject Target = GameObject.Find("ChatBubble");
			UI2DSprite sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = true;
			Target = GameObject.Find("Conversation");
			UILabel sl = (UILabel)Target.gameObject.GetComponent("UILabel");
			sl.enabled = true;
			/*Target = GameObject.Find("Portrait");
			sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = true;
			Target = GameObject.Find("Portrait2");
			sp = (UI2DSprite)Target.gameObject.GetComponent("UI2DSprite");
			sp.enabled = true;*/
			Target = GameObject.Find("UI Root");
			ScDialogue sc = (ScDialogue)col.gameObject.GetComponent("ScDialogue");
			sc.enabled = true;
		}
		if (col.gameObject.name == "ImmortalGuard") 
		{
			if(score>200)//Р·РґРµСЃСЊ РїСЂРѕРІРµСЂРёС‚СЊ СЃРѕР±СЂР°РЅС‹ Р»Рё РІСЃРµ РєР»СЋС‡РµРІС‹Рµ РѕР±СЉРµРєС‚С‹, Р° РїРѕРєР° РґР»СЏ С‚РµСЃС‚Р° СЃС‡РµС‚
			{
				Immortal im = (Immortal)col.GetComponent("Immortal");
				im.anim.SetBool("alive",false);
			}
			else
			{
				PlayerHealth ph = (PlayerHealth)GetComponent("PlayerHealth");
				ph.AddjustCurrentHealth(-5);
			}
		}
		if (col.gameObject.name == "JumperWind") {
						GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, 100f));
						//move = 0;
						//Destroy (col.gameObject);
				} else {
			//rigidbody2D.AddForce (new Vector2 (0f, jumpForce));
				}
		ShotScript shot = col.gameObject.GetComponent<ShotScript> ();  //Shoot 
		if (shot != null) 
		{
			PlayerHealth ph = (PlayerHealth)GetComponent("PlayerHealth");
			ph.AddjustCurrentHealth(-1);
			MoveScript ms = (MoveScript)col.GetComponent("MoveScript");
			ms.speed.x=0;
			Animator an = (Animator)col.gameObject.GetComponent("Animator");
			an.SetTrigger("got");
			//Destroy(col.gameObject);
		}  // end shoot intercept
	}
	//РЎС‚РѕР»РєРЅРѕРІРµРЅРёСЏ
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.name == "Pecker" && transform.position.y > (col.transform.position.y+0.4)) 
		{
			EnemyHealth eh = (EnemyHealth) col.gameObject.GetComponent("EnemyHealth");
			eh.AddjustCurrentHealth(-100);
		}
		if (col.gameObject.tag == "Enemy") {
			if((transform.position.y-col.transform.position.y<0.7) && (transform.position.y-col.transform.position.y>0.59))
			{
				//Debug.Log(transform.position.y-(col.transform.position.y+0.6));
				GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, jumpForce/2));
				if(col.gameObject.name == "brownPig" || col.gameObject.name == "grayPig")
				{
					EnemyHealth eh = (EnemyHealth) col.gameObject.GetComponent("EnemyHealth");
					eh.AddjustCurrentHealth(-50);
				}
			}
			{
				//Debug.Log(transform.position.y-(col.transform.position.y));
				if((transform.position.y-col.transform.position.y<0.79) && (transform.position.y-col.transform.position.y>0.7))
				{
					GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, jumpForce/2));
					if(col.gameObject.name == "WoodMask" || col.gameObject.name == "IceDragon")
					{
						EnemyHealth eh = (EnemyHealth) col.gameObject.GetComponent("EnemyHealth");
						eh.AddjustCurrentHealth(-50);
					}
				}
			}

		}

		if (col.gameObject.name == "jumpMush" ) {
			//Debug.Log(transform.position.y-(col.transform.position.y+0.6));
			GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, ((jumpForce/10)*8)));
			Animator an = (Animator) col.gameObject.GetComponent("Animator");
			an.SetTrigger("act");
		}
		if (col.gameObject.name == "MovingPlatform") 
		{
			//grounded=true;
		}
	}
//		//Р±РµР· СѓРґР°СЂРѕРІ
//		//if (grounded) {
//		//				transform.rotation = col.gameObject.transform.rotation;
//		//} 
//		if (anim.GetBool ("hitZ") == false && anim.GetBool ("hitX") == false && anim.GetBool ("hitC") == false) {
//			if (col.gameObject.tag == "Enemy")
//				if (health > 0 && isProtected == false)
//			{
//				anim.SetBool("isAtacked",true);
//				//rigidbody2D.AddForce (new Vector2 (-950f, 90f));
//				health--;
//			}		
//		}
//		else
//		{
//			if(col.gameObject.tag == "Enemy"){col.gameObject.GetComponent<PigScript>().health--;}
//		}
//	}
	//РРЅС‚РµСЂС„РµР№СЃ
	public Font myFont;
	GUIStyle myStyle;

	void OnGUI()
	{	
		if (Application.loadedLevelName != "ScMnu"){

		if (myStyle == null) {
			myStyle = new GUIStyle(GUI.skin.label);	
			myStyle.font = myFont;
		}
		if(Application.loadedLevelName!="ScMini"){GUI.Box (new Rect (5, 18, 200, 150), "Score:" + score,myStyle);}
			else{FoodCollect fc = (FoodCollect)GetComponent("FoodCollect");GameObject lbl = GameObject.Find("ScoreLabel");UILabel lab = (UILabel)lbl.GetComponent("UILabel");lab.text ="Score:" + fc.FoodScore;lbl = GameObject.Find("TimeLabel");lab = (UILabel)lbl.GetComponent("UILabel");lab.text = "Time :" + System.Math.Round((double)fc.Timer,2);}
		PlayerHealth ph = (PlayerHealth)GetComponent("PlayerHealth");
		for (int i=0; i<ph.curHealth; i++) {
			GUI.DrawTexture(new Rect(5+14*i,5,14,15),HealthTexture,ScaleMode.ScaleToFit);		
		}
		}
	}
	public AudioSource src;
	void PlaySoundSFX(AudioClip aud)
	{
		src.clip = aud;
		src.Play ();
	}
	void PlaySound(AudioClip aud)
	{
		GetComponent<AudioSource>().clip = aud;
		GetComponent<AudioSource>().Play ();
	}
	//РљР°Рє С‚Рѕ СѓР№С‚Рё РѕС‚ СЌС‚РёС… С„СѓРЅРєС†РёР№
	void isatack()
	{
		anim.SetBool ("isAtack", false);
		PlayerAtack pl = (PlayerAtack)GetComponent("PlayerAtack");
		pl.switcher = true;
	}
	void EndAtacked()
	{
		anim.SetBool ("isAtacked", false);
	}
	void protectOff()
	{
		isProtected = false;
	}
	void protectOn()
	{
		isProtected = true;
	}
	void SwitchCollider()
	{
		if (transform.GetComponent<Collider2D>().enabled == true) 
		{
			transform.GetComponent<Collider2D>().enabled = false;
		} 
		else 
		{
			transform.GetComponent<Collider2D>().enabled = true;
		}
	}
	void OnCollider()
	{
		if (!transform.GetComponent<Collider2D>().enabled) {
			transform.GetComponent<Collider2D>().enabled = true;		
		}
	}
}