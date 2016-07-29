using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsController : MonoBehaviour {
	private bool showStats;
	GameObject statsCanvas;
	GameObject xpBar, mpBar, hpBar;
	GameObject player;
	//Bars
	Transform xpText, mpText, hpText;
	Transform xpLine, mpLine, hpLine;
	float filledWidth;
	Stat xp,mp,hp;
	//Main Bars
	Transform mainHP, mainMP, mainXP, mainClass;
	GameObject mainName;
	//Upgradables
	Transform charVit, charDex, charStr, charInt, charPoints;
	CharacterStats stats;
	//Class
	Transform charClass, classIcon;
	//Nickname
	Transform charName;
	//Level
	Transform charLevel;
	//Calculated Stats
	Transform physAtk, magAtk,critRate,accuracy, charSpeed,physDef,magDef,critDmg,evasion,charLuck;
	// Use this for initialization
	void Start () {
		statsCanvas = GameObject.Find ("StatCanvas");
		player = GameObject.FindGameObjectWithTag ("Player");
		xpBar = GameObject.Find ("XP Panel");
		mpBar = GameObject.Find ("MP Panel");
		hpBar = GameObject.Find ("HP Panel");
		//Main Bars (Always Visible)
		mainHP = GameObject.Find ("HP main").transform.Find ("Bar");
		mainMP = GameObject.Find ("MP main").transform.Find ("Bar");
		mainXP = GameObject.Find ("XP main").transform.Find ("Bar");
		mainName = GameObject.Find ("Main Character Name");
		mainName.GetComponent<Text>().text = player.GetComponent<CharacterStats> ().nickName.ToString();
		mainClass = GameObject.Find ("Main Stats Panel/ClassBG").transform.Find ("Main Class");
		mainClass.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Sprites/ClassIcons/" + player.GetComponent<CharacterStats> ().curClass.ToString ());
		
		if (!hpBar || !mpBar || !hpBar || !player) {
			Debug.Log ("Something wrong!");
		} //error checker for ME
		else {
			xpText = xpBar.transform.Find("Value");
			mpText = mpBar.transform.Find("Value");
			hpText = hpBar.transform.Find("Value");

			xpLine = xpBar.transform.Find("EmptyBar/Bar");
			mpLine = mpBar.transform.Find("EmptyBar/Bar");
			hpLine = hpBar.transform.Find("EmptyBar/Bar");
			filledWidth = xpLine.GetComponent<Image>().rectTransform.sizeDelta.x; //get full width of bar
			//structs 
			xp = new Stat(0,0);
			mp = new Stat(0,0);
			hp = new Stat(0,0);
			stats =  player.GetComponent<CharacterStats> ();
		}
	  //Upgradables
		charStr = GameObject.Find("Upgrades Panel/Strength").transform.Find("Value");
		charDex = GameObject.Find("Upgrades Panel/Dextenity").transform.Find("Value");
		charVit = GameObject.Find("Upgrades Panel/Vitality").transform.Find("Value");
		charInt = GameObject.Find("Upgrades Panel/Intelligence").transform.Find("Value");
		charPoints = GameObject.Find ("Upgrades Panel").transform.Find ("PointsVal");
		if (!charStr || !charDex || !charVit || !charInt ) {
			Debug.Log ("Something wrong!");
		} 
		//Class
		charClass = GameObject.Find ("Lvl and Class Panel").transform.Find ("Class Value");
		charClass.GetComponent<Text> ().text = stats.curClass.ToString (); //set name
		classIcon = GameObject.Find("Lvl and Class Panel").transform.Find("Class Image");
		classIcon.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/ClassIcons/" + stats.curClass.ToString ());
		//Name
		charName = GameObject.Find ("Stats Panel").transform.Find ("Character Name");
		charName.GetComponent<Text>().text = stats.nickName.ToString();
		//Level
		charLevel = GameObject.Find ("Lvl and Class Panel").transform.Find ("Lvl Value");
		//Calculated Stats
		physAtk = GameObject.Find("Info Panel/PhysAtk").transform.Find("Value");
		magAtk = GameObject.Find("Info Panel/MagAtk").transform.Find("Value");
		critRate = GameObject.Find("Info Panel/CritRate").transform.Find("Value");
		accuracy = GameObject.Find("Info Panel/Accuracy").transform.Find("Value");
		charSpeed = GameObject.Find("Info Panel/Speed").transform.Find("Value");
		physDef = GameObject.Find("Info Panel/PhysDef").transform.Find("Value");
		magDef = GameObject.Find("Info Panel/MagDef").transform.Find("Value");
		critDmg = GameObject.Find("Info Panel/CritDmg").transform.Find("Value");
		evasion = GameObject.Find("Info Panel/Evasion").transform.Find("Value");
		charLuck = GameObject.Find("Info Panel/Luck").transform.Find("Value");

	}
	
	// Update is called once per frame
	void Update () {
		//Keys Combinations
		if (Input.GetButtonDown("Stats") && PlayerPrefs.GetInt("drag")==0)
		{
			showStats = !showStats;
			statsCanvas.SetActive(showStats);
		}
		if(Input.GetKeyDown(KeyCode.Q))
		{
			this.IncreaseLevel();
		}
		//Main Bar always active	
		//update structs
		xp.Set(stats.CurXP,stats.MaxXP);
		mp.Set(stats.CurMP,stats.MaxMP);
		hp.Set(stats.CurHP,stats.MaxHP);

		UpdateStat(mainXP,xp);
		UpdateStat(mainMP,mp);
		UpdateStat(mainHP,hp);
		//Handle stats
		if (showStats) 
		{
			//Stats
			stats =  player.GetComponent<CharacterStats> ();
			//Get stats
			hpText.GetComponent<Text>().text = stats.CurHP + "/" + stats.MaxHP;
			mpText.GetComponent<Text>().text = stats.CurMP + "/" + stats.MaxMP;
			xpText.GetComponent<Text>().text = stats.CurXP + "/" + stats.MaxXP;
			//Resize
			UpdateStat(xpLine,xp);
			UpdateStat(mpLine,mp);
			UpdateStat(hpLine,hp);
			//Show Upgradable values
			charStr.GetComponent<Text>().text = stats.tmpStr.ToString();
			charDex.GetComponent<Text>().text = stats.tmpDex.ToString();
			charVit.GetComponent<Text>().text = stats.tmpVit.ToString();
			charInt.GetComponent<Text>().text = stats.tmpInt.ToString();
			charPoints.GetComponent<Text>().text = stats.tmpUpgCnt.ToString();
			//Level
			charLevel.GetComponent<Text> ().text = stats.Level.ToString();
			//Calculated stats formulas
			stats.calcStats.PhysAtk = (float)(100 + (0.9 * stats.Level) + 12 * stats.Str); //TODO: create formula
			stats.calcStats.MagAtk =  (float)(100 + (0.75* stats.Level) + 13*stats.Int); 
			stats.calcStats.CritRate = (float)(2+ stats.Dex / 100);
			stats.calcStats.Accuracy = (float)(100 + (0.6 * stats.Level) + 12.75 * stats.Dex);
			stats.calcStats.Speed = (float)(5 + (0.1 * stats.Dex)); 
			stats.calcStats.PhysDef = (float)(100 + (0.8 * stats.Level) + 17 * stats.Vit + 5*stats.Str);
			stats.calcStats.MagDef = (float)(100 + (0.77 * stats.Level) + 14 * stats.Int);
			stats.calcStats.CritDmg = (float)(100 + (0.2 * stats.Dex));
			stats.calcStats.Evasion = (float)(100 + (0.13 * stats.Level) + 5 * stats.Dex);
			stats.calcStats.Luck = (float)(10 + (0.3 * stats.Level));
			//Calculated stats show
			physAtk.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.PhysAtk).ToString();
			magAtk.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.MagAtk).ToString(); 
			critRate.GetComponent<Text>().text = System.Math.Round(stats.calcStats.CritRate,2).ToString() + " %";
			accuracy.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.Accuracy).ToString();
			charSpeed.GetComponent<Text>().text =  System.Math.Round(stats.calcStats.Speed,2).ToString();
			physDef.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.PhysDef).ToString();
			magDef.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.MagDef).ToString();
			critDmg.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.CritDmg).ToString() + " %";
			evasion.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.Evasion).ToString();
			charLuck.GetComponent<Text>().text = Mathf.RoundToInt(stats.calcStats.Luck).ToString();
		}
	}
	//Bars helpers
	private struct Stat
	{
		public int cur;
		public int max;
		//constructor
		public Stat(int _cur, int _max)
		{
			cur = _cur;
			max = _max;
		}
		//Set
		public void Set(int _cur, int _max)
		{
			cur = _cur;
			max = _max;
		}
	}

	void UpdateStat(Transform trans, Stat stat)
	{
		float deltXp = GetStatPercent(stat.cur,stat.max);
		double offX = (0 - filledWidth / 2) + (trans.GetComponent<Image> ().rectTransform.sizeDelta.x / 2); //(filledWidth / 2) - 1.5 * (trans as RectTransform).rect.width;
		trans.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(filledWidth*deltXp, trans.GetComponent<Image>().rectTransform.sizeDelta.y);
		trans.localPosition = new Vector2((float)(offX+1f), trans.localPosition.y); 

	}

	float GetStatPercent(int cur, int max)
	{
		float result = (float)cur / (float)max;
		return result;
	}

	//Upgrades TODO: MOVE ALL BELOW TO stat class!
	public void IncrStat(string stat)
	{
		if(!(stats.tmpUpgCnt>0)){return;}
		switch (stat) {
		case "Str":
			stats.tmpStr++;
			break;
		case "Dex":
			stats.tmpDex++;
			break;
		case "Int":
			stats.tmpInt++;
			break;
		case "Vit":
			stats.tmpVit++;
			break;
		}
		stats.tmpUpgCnt--;
	}

	public void DecrStat(string stat)
	{
		switch (stat) {
		case "Str":
			if (stats.tmpStr==stats.Str) {return;}
			stats.tmpStr--;
			break;
		case "Dex":
			if (stats.tmpDex==stats.Dex) {return;}
			stats.tmpDex--;
			break;
		case "Int":
			if (stats.tmpInt==stats.Int) {return;}
			stats.tmpInt--;
			break;
		case "Vit":
			if (stats.tmpVit==stats.Vit) {return;}
			stats.tmpVit--;
			break;
		}
		stats.tmpUpgCnt++;
	}

	public void ResetStats()
	{
		stats.tmpStr = stats.Str;
		stats.tmpDex = stats.Dex;
		stats.tmpInt = stats.Int;
		stats.tmpVit = stats.Vit;
		stats.tmpUpgCnt = stats.UpgCnt;
	}

	public void SaveStats()
	{
		stats.Str = stats.tmpStr;
		stats.Dex = stats.tmpDex;
		stats.Int = stats.tmpInt;
		stats.Vit = stats.tmpVit;
		stats.UpgCnt = stats.tmpUpgCnt;
	}

	public void IncreaseLevel()
	{
		stats.Level++;
		stats.UpgCnt += 5;
		stats.tmpUpgCnt += 5;
	}
}
