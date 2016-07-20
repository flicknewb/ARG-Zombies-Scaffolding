﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using LitJson;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public bool gameDataInitialized = false;
	public int daysSurvived, supply, ammo, reportedSupply, reportedWater, reportedFood, reportedTotalSurvivor, reportedActiveSurvivor, playerCurrentStamina, playerMaxStamina, zombiesToFight, foodCount, waterCount;
	public DateTime timeCharacterStarted;
	public float homebaseLat, homebaseLong;
//	public bool[] buildingToggleStatusArray;
	public string equippedWeaponID;
	[SerializeField]
	private GameObject[] weaponOptionsArray;

	public List <GameObject> survivorCardList = new List<GameObject>();
	public List <GameObject> weaponCardList = new List<GameObject>();
	public GameObject survivorCardHolder;
	public GameObject weaponCardHolder;

	private Scene activeScene;
	//made this public while working on the server "cleared list" data retention. it should go back to private
	public string activeBldg;
	public string locationJsonText, clearedBldgJsonText;

	public string userId;
	public string userFirstName;
	public string userLastName;

	private string startNewCharURL = "http://www.argzombie.com/ARGZ_SERVER/StartNewCharacter.php";
	private string resumeCharacterUrl = "http://www.argzombie.com/ARGZ_SERVER/ResumeCharacter.php";
	private string updateAllStatsURL = "http://www.argzombie.com/ARGZ_SERVER/UpdateAllPlayerStats.php";
	private string buildingClearedURL = "http://www.argzombie.com/ARGZ_SERVER/NewBuildingCleared.php";
	private string clearedBuildingDataURL = "http://www.argzombie.com/ARGZ_SERVER/ClearedBuildingData.php";
	private string fetchSurvivorDataURL = "http://www.argzombie.com/ARGZ_SERVER/FetchSurvivorData.php";
	private string fetchWeaponDataURL = "http://www.argzombie.com/ARGZ_SERVER/FetchWeaponData.php";
	private string clearSurvivorDataURL = "http://www.argzombie.com/ARGZ_SERVER/DeleteMySurvivorData.php";

	private bool eatDrikCounterIsOn;

	private static SurvivorPlayCard survivorPlayCardPrefab;
	private static WeaponPlayCard weaponPlayCardPrefab;

	void Awake () {
		MakeSingleton();
		StartCoroutine (StartLocationServices());

		eatDrikCounterIsOn = false;
		survivorPlayCardPrefab = Resources.Load<SurvivorPlayCard>("Prefabs/SurvivorPlayCard");
		weaponPlayCardPrefab = Resources.Load<WeaponPlayCard>("Prefabs/WeaponPlayCardPrefab");

		//ResetAllBuildings();
	}

	void OnLevelWasLoaded () {
		//this is a catch all to slave the long term memory to the active GameManager.instance object- each load will update long term memory.


		activeScene = SceneManager.GetActiveScene();
		if (activeScene.name.ToString() == "02a Map Level"){
			Debug.Log ("Time character started set to: " + timeCharacterStarted);
			SetDaysSurvived();
			
			MapLevelManager mapManager = FindObjectOfType<MapLevelManager>();
			mapManager.UpdateTheUI();

		} else if (activeScene.name.ToString() == "01a Login") {

			LoginManager loginMgr = FindObjectOfType<LoginManager>();
			if (FB.IsLoggedIn == true) {
				loginMgr.loggedInPanel.SetActive(true);
			}

		} else if (activeScene.name.ToString() == "01b Start") {
			if (eatDrikCounterIsOn == false) {
				InvokeRepeating ( "CheckEatingAndDrinking", 1.0f, 30.0f);
				eatDrikCounterIsOn = true;
			}
			//in the future this will have to be done on the server, and an update pulled periodically.
		}
	}

	void MakeSingleton() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

//	JsonData CurrentPlayerDataIntoJson () {
//		string[] playerData = new string[] () ;
//
//	}

	public void UpdateAllStatsToGameMemory () {
		//this is a big nono, hence it's disabled

		//StartCoroutine(UpdateGameManagerToGameServer());
	/*
		GamePreferences.SetShivCount(shivCount);
		GamePreferences.SetClubCount(clubCount);
		GamePreferences.SetGunCount(gunCount);
		GamePreferences.SetShivDurability(shivDurability);
		GamePreferences.SetClubDurability(clubDurability);
		GamePreferences.SetSupply(supply);
		GamePreferences.SetTotalSurvivors (totalSurvivors);
		GamePreferences.SetActiveSurvivors (survivorsActive);
		GamePreferences.SetWaterCount (waterCount);
		GamePreferences.SetFoodCount (foodCount);
		GamePreferences.SetMealsCount (mealCount);
		SetPublicPlayerHealth (this.playerCurrentHealth);
		GamePreferences.SetHomebaseLattitude (homebaseLat);
		GamePreferences.SetHomebaseLongitude (homebaseLong);
		*/
		//removed to update the server
	}

//	IEnumerator UpdateGameManagerToGameServer() {
////		JsonData playerJsonData = CurrentPlayerDataIntoJson();
////		String playerJsonString = File.ReadAllText(Application.dataPath + "/Resources/Player.json");
//
//
//		WWWForm form = new WWWForm();
//		form.AddField("id", GameManager.instance.userId );
//		form.AddField("first_name", GameManager.instance.userFirstName);
//		form.AddField("last_name", GameManager.instance.userLastName);
//		form.AddField("curr_stamina", GameManager.instance.playerCurrentStamina);
//		form.AddField("supply", GameManager.instance.supply);
//		form.AddField("food", GameManager.instance.foodCount);
//		form.AddField("water", GameManager.instance.waterCount);
//		form.AddField("knife_durability", GameManager.instance.shivDurability);
//		form.AddField("club_durability", GameManager.instance.clubDurability);
//		form.AddField("home_lat", GameManager.instance.homebaseLat.ToString());
//		form.AddField("home_lon", GameManager.instance.homebaseLong.ToString());
//		form.AddField("char_created_DateTime", GameManager.instance.timeCharacterStarted.ToString());
//
//		WWW www = new WWW(updateAllStatsURL, form);
//		yield return www;
//
//		if (www.error == null) {
//			
//			Debug.Log ("Server successfully updated " + www.text);
//
//			yield break;
//		} else {
//			Debug.Log("WWW error "+ www.error);
//		}
//	}

	IEnumerator NewCharacterUpdateServer () {
		WWWForm form1 = new WWWForm();
		form1.AddField("id", GameManager.instance.userId);

		//this is now handled in the start new character php script
		WWW www1 = new WWW(clearSurvivorDataURL, form1);
		yield return www1;
		Debug.Log (www1.text);
		
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId );
		form.AddField("first_name", GameManager.instance.userFirstName);
		form.AddField("last_name", GameManager.instance.userLastName);
		form.AddField("supply", GameManager.instance.supply);
		form.AddField("food", GameManager.instance.foodCount);
		form.AddField("water", GameManager.instance.waterCount);
		form.AddField("ammo", GameManager.instance.ammo);
		form.AddField("char_created_DateTime", GameManager.instance.timeCharacterStarted.ToString());

		WWW www = new WWW(startNewCharURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			
			Debug.Log ("New character successfully started on the server" + www.text);
			//SceneManager.LoadScene("02a Map Level");
			yield break;
		} else {
			Debug.Log("WWW error "+ www.error);
		}
	}


	/// <summary>
	/// Fetchs the resume player data.
	/// </summary>
	/// <returns>The resume player data.</returns>
	IEnumerator FetchResumePlayerData () {
		WWWForm form = new WWWForm();
		if (FB.IsLoggedIn == true) {
			form.AddField("id", GameManager.instance.userId);
		} else {
			GameManager.instance.userId = "10154194346243928";
			form.AddField("id", GameManager.instance.userId);
		}

		WWW www = new WWW(resumeCharacterUrl, form);
		yield return www;

		if (www.error == null) {

			Debug.Log ("resuming character, server returned raw json string of: " + www.text);

			//write the raw WWW return to a .json file 
			//File.WriteAllText(Application.dataPath + "/Resources/Player.json", www.text.ToString());

			//read that text out into a string object, and map that to a json object
			string playerJsonString = www.text.ToString();
			JsonData playerJson = JsonMapper.ToObject(playerJsonString);

			if (playerJson[0].ToString() == "Success"){
				//update the GameManager.instance with all dataum
				GameManager.instance.userFirstName = playerJson[1]["first_name"].ToString() ;
				GameManager.instance.userLastName = playerJson[1]["last_name"].ToString();
				GameManager.instance.playerCurrentStamina = (int)playerJson[1]["curr_stamina"];
				GameManager.instance.playerMaxStamina = (int)playerJson[1]["max_stamina"];
				int sup = Convert.ToInt32(playerJson[1]["supply"].ToString());
				GameManager.instance.supply = sup;
				int wat = Convert.ToInt32(playerJson[1]["water"].ToString());
				GameManager.instance.waterCount = wat;
				int fud = Convert.ToInt32(playerJson[1]["food"].ToString());
				GameManager.instance.foodCount = fud;
				float homeLat = (float)Convert.ToDouble(playerJson[1]["homebase_lat"].ToString());
				GameManager.instance.homebaseLat = homeLat;
				float homeLon = (float)Convert.ToDouble(playerJson[1]["homebase_lon"].ToString());
				GameManager.instance.homebaseLong = homeLon;
				Debug.Log ("server returned a date time string of: " + playerJson[1]["char_created_DateTime"]);
				DateTime oDate = Convert.ToDateTime(playerJson[1]["char_created_DateTime"].ToString());
				GameManager.instance.timeCharacterStarted = oDate;


			} else if (playerJson[0].ToString() == "Failed") {
				Debug.Log(playerJson[1].ToString());
			}


			yield break;
		} else {
			Debug.Log ("WWW error" + www.error);
		}

	}

	IEnumerator FetchWeaponData () {
		//wipe all old data clean.
		GameObject[] oldWeapons = GameObject.FindGameObjectsWithTag("weaponcard");
		foreach (GameObject weaponCard in oldWeapons) {
			int whereIsIt = GameManager.instance.weaponCardList.IndexOf (weaponCard);
			GameManager.instance.weaponCardList.RemoveAt(whereIsIt);
			Destroy(weaponCard.gameObject);
		}

		//get the data from the server
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		WWW www = new WWW(fetchWeaponDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData weaponJson = JsonMapper.ToObject(www.text);

			if (weaponJson[0].ToString() == "Success") {
				//parse through the entries to create new game objects, and add them to the list, and child them to the weapon card holder
				for (int i=0; i < weaponJson[1].Count; i++) {
					WeaponPlayCard instance = Instantiate(weaponPlayCardPrefab);
					instance.transform.SetParent(weaponCardHolder.gameObject.transform);
					instance.weapon_id = (int)weaponJson[1][i]["weapon_id"];
					instance.equipped_id = (int)weaponJson[1][i]["equipped_id"];
					instance.gameObject.name = weaponJson[1][i]["name"].ToString();
					instance.weapon.topDmg = (int)weaponJson[1][i]["top_dmg"];
					instance.weapon.botDmg = (int)weaponJson[1][i]["bot_dmg"];
					instance.weapon.stamCost = (int)weaponJson[1][i]["stam_cost"];
					instance.weapon.durability = (int)weaponJson[1][i]["durability"];

					if (weaponJson[1][i]["type"].ToString() == "knife") {
						instance.weapon.weaponType = BaseWeapon.WeaponType.KNIFE;
					} else if (weaponJson[1][i]["type"].ToString() == "club") {
						instance.weapon.weaponType = BaseWeapon.WeaponType.CLUB;
					} else if (weaponJson[1][i]["type"].ToString() == "gun") {
						instance.weapon.weaponType = BaseWeapon.WeaponType.GUN;
					}


				}
				weaponCardList.AddRange (GameObject.FindGameObjectsWithTag("weaponcard"));
				Debug.Log("all weapons added to the scene");

				//We need a function to match the weapons to the correct players, and equip them- if they are equipped.
				MergeWeaponAndSurvivorRecords ();
			} else if (weaponJson[0].ToString() == "Failed") {
				Debug.Log(weaponJson[1].ToString());
			}


		} else {
			Debug.Log(www.error);
		}

	}

	void MergeWeaponAndSurvivorRecords () {
		//for each weapon equipped- compare it's equipped ID with active survivor ID's
		foreach (GameObject weapon in weaponCardList) {
			WeaponPlayCard WPC = weapon.GetComponent<WeaponPlayCard>();

			//now loop through the player cards, and find a matching player card ID.
			foreach (GameObject survivorCard in survivorCardList) {
				SurvivorPlayCard SPC = survivorCard.GetComponent<SurvivorPlayCard>();

				if (SPC.survivor_id == WPC.equipped_id) {
					//This weapon has been previously assigned to this player- add the game object to the card
					SPC.survivor.weaponEquipped = weapon.gameObject;
					break;
				} else {
					continue;
				}
			}
		}
	}


	/// <summary>
	/// Fetchs the survivor data.
	/// </summary>
	/// <returns>The survivor data.</returns>
	IEnumerator FetchSurvivorData () {
		//delete all previous data from the gamemanager
		GameObject[] oldSurvivorCards = GameObject.FindGameObjectsWithTag("survivorcard");
		foreach (GameObject survivorCard in oldSurvivorCards) {
			int whereIsIt = GameManager.instance.survivorCardList.IndexOf (survivorCard);
			GameManager.instance.survivorCardList.RemoveAt(whereIsIt);
			Destroy(survivorCard.gameObject);
		}

		//construct form
		WWWForm form = new WWWForm();
		if (FB.IsLoggedIn == true) {
			form.AddField("id", GameManager.instance.userId);
		} else {
			GameManager.instance.userId = "10154194346243929";
			form.AddField("id", GameManager.instance.userId);
		}
		//make www call
		WWW www = new WWW(fetchSurvivorDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			//encode json return
			string survivorJsonString = www.text;
			JsonData survivorJson = JsonMapper.ToObject(survivorJsonString);

			if (survivorJson[0].ToString() == "Success") {
				//parse through json creating "player cards" within gamemanager for each survivor found on the server.
				for (int i = 0; i < survivorJson[1].Count; i++) {
					SurvivorPlayCard instance = Instantiate(survivorPlayCardPrefab);
					instance.survivor.name = survivorJson[1][i]["name"].ToString();
					instance.gameObject.name = survivorJson[1][i]["name"].ToString();
					//instance.survivor.weaponEquipped.name = survivorJson[i]["weapon_equipped"].ToString();
					instance.survivor.baseAttack = (int)survivorJson[1][i]["base_attack"];
					instance.survivor.baseStamina = (int)survivorJson[1][i]["base_stam"];
					instance.survivor.curStamina = (int)survivorJson[1][i]["curr_stam"];
					instance.entry_id = (int)survivorJson[1][i]["entry_id"];
					instance.survivor_id = (int)survivorJson[1][i]["survivor_id"];
					instance.team_pos = (int)survivorJson[1][i]["team_pos"];

					if (survivorJson[1][i]["weapon_equipped"].ToString() == "knife") {
						instance.survivor.weaponEquipped = GameManager.instance.weaponOptionsArray[0];
					} else if (survivorJson[1][i]["weapon_equipped"].ToString() == "club") {
						instance.survivor.weaponEquipped = GameManager.instance.weaponOptionsArray[1];
					} else if (survivorJson[1][i]["weapon_equipped"].ToString() == "gun") {
						instance.survivor.weaponEquipped = GameManager.instance.weaponOptionsArray[2];
					}

					instance.transform.SetParent(GameManager.instance.survivorCardHolder.transform);
				}
			} else {
				Debug.Log(survivorJson[1].ToString());
			}

			survivorCardList.AddRange (GameObject.FindGameObjectsWithTag("survivorcard"));
			if (SceneManager.GetActiveScene().buildIndex != 2 ) {
				SceneManager.LoadScene("02a Map Level");
			}

		} else {
			Debug.LogWarning(www.error);
		}
	}


	
	public void SetDaysSurvived () {
		DateTime now = System.DateTime.Now;
		Double days = (now - timeCharacterStarted).TotalDays;
		daysSurvived = Convert.ToInt32(days);

		Debug.Log ("The SetDaysSurvived function has returned: " + days + " Days since character created");
	}

	public void StartNewCharacter () {
		//Record the date and time the character is created- will be compared to get Days alive later.
		timeCharacterStarted = System.DateTime.Now;

		if (FB.IsLoggedIn == false) {
			GameManager.instance.userId = "10154194346243929";
			GameManager.instance.userFirstName = "Tanderson";
			GameManager.instance.userLastName = "Flickinhausen";
		}


		//roll a random number of survivors left alive and set both active and alive to that number.

		GameManager.instance.supply = UnityEngine.Random.Range(20, 70);
		GameManager.instance.waterCount = UnityEngine.Random.Range(10, 20);
		GameManager.instance.foodCount = UnityEngine.Random.Range(15, 30);
		GameManager.instance.ammo = UnityEngine.Random.Range(0,20);
		GameManager.instance.playerMaxStamina = 100;
		GameManager.instance.playerCurrentStamina = 100;


		StartCoroutine (NewCharacterUpdateServer());


		//pass all the rolled info to the gamePreferences - aka permenent memory
//		GamePreferences.SetShivCount(shivCount);
//		GamePreferences.SetClubCount(clubCount);
//		GamePreferences.SetGunCount(gunCount);
//		GamePreferences.SetShivDurability(shivDurability);
//		GamePreferences.SetClubDurability(clubDurability);
//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors (totalSurvivors);
//		GamePreferences.SetActiveSurvivors (survivorsActive);
//		GamePreferences.SetWaterCount (waterCount);
//		GamePreferences.SetFoodCount (foodCount);
		//this.SetPublicPlayerHealth (100);
		//Debug.Log ("GameManager started a new character- food / water: " + foodCount +" / "+ waterCount );


		//Debug.Log ("Character started at: " + timeCharacterStarted);
	}

	public void ResumeCharacter () {

		StartCoroutine (FetchSurvivorData());
		StartCoroutine (FetchWeaponData());
		StartCoroutine (FetchResumePlayerData());

		//The coroutine should handle fetching and loading all the game data from server now.  No data should be stored in preferences anymore.

//		survivorsActive = GamePreferences.GetActiveSurvivors();
//		totalSurvivors = GamePreferences.GetTotalSurvivors();
//		if (totalSurvivors > survivorsActive) {
//			survivorsActive = totalSurvivors;
//			// if there are more active than total, make the active = the total. temp idiot check.
//		}
//
//		waterCount = GamePreferences.GetWaterCount();
//		foodCount = GamePreferences.GetFoodCount();
//		supply = GamePreferences.GetSupply();
//		timeCharacterStarted = Convert.ToDateTime(GamePreferences.GetDayTimeCharacterCreated());
//		SetDaysSurvived();
//		playerCurrentHealth = GamePreferences.GetLastPlayerCurrentHealth();
//
//		shivCount = GamePreferences.GetShivCount();
//		clubCount = GamePreferences.GetClubCount();
//		gunCount = GamePreferences.GetGunCount();
//
//		shivDurability = GamePreferences.GetShivDurability();
//		clubDurability = GamePreferences.GetClubDurability();

	}

	public void RestartTheGame () {
		StartNewCharacter ();
		SceneManager.LoadScene("02a Map Level");// consider storing these in a public static array that can just name locations, and possibly swap backgrounds.
	}

	public void PaidRestartOfTheGame () {
		int survivors = UnityEngine.Random.Range(4, 8);

		int newSupply = Mathf.RoundToInt(this.supply * 0.75f);
		this.supply = newSupply;
		int newFood = Mathf.RoundToInt(this.foodCount / 2);
		foodCount = newFood;
		int newWater = Mathf.RoundToInt(this.waterCount / 2);
		waterCount = newWater;


//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors (totalSurvivors);
//		GamePreferences.SetActiveSurvivors (survivorsActive);
//		GamePreferences.SetWaterCount (waterCount);
//		GamePreferences.SetFoodCount (foodCount);
		GameManager.instance.UpdateAllStatsToGameMemory();

		SceneManager.LoadScene("02a Map Level");
	}

	public void SetPublicPlayerHealth (int playerStamina) {
		playerCurrentStamina = playerStamina;

		GameManager.instance.UpdateAllStatsToGameMemory();
//		GamePreferences.SetLastPlayerCurrentHealth(playerHealth);
		//this is for external setting of the permenant game object GameManager.instance
	}

	public void SetHomebaseLocation (float lat, float lon) {
		this.homebaseLat = lat;
		this.homebaseLong = lon;

		UpdateAllStatsToGameMemory();
//		GamePreferences.SetHomebaseLattitude(lat);
//		GamePreferences.SetHomebaseLongitude(lon);
	}

	public void LoadIntoCombat (int zombies, string bldg) {
		activeBldg = bldg;
		zombiesToFight = zombies;
		SceneManager.LoadScene ("02c Combat-5");
	}

	public void AddTimePlayed () {
		timeCharacterStarted  = timeCharacterStarted.AddHours(-1.0);
//		GamePreferences.SetDayTimeCharacterCreated (timeCharacterStarted.ToString());

		//right now this will not save to permenant memory- this should be handled in its own function/script- so it's not accidentally changed.
		UpdateAllStatsToGameMemory();
		SetDaysSurvived ();
		Debug.Log ("1 Hour added to time started. New Datetime is: " + timeCharacterStarted.ToString() );
	}

	public IEnumerator DeactivateClearedBuildings () {
		WWWForm myForm = new WWWForm();
		myForm.AddField("id", GameManager.instance.userId);

    	WWW www = new WWW(clearedBuildingDataURL, myForm);
    	yield return www;
    	Debug.Log(www.text);

    	if (www.error == null) {
    		//Debug.Log ("the cleared building call returned raw text of: "+www.text);
    		GameManager.instance.clearedBldgJsonText = www.text;
			//File.WriteAllText(Application.dataPath + "/Resources/clearedBldg.json", www.text.ToString());
    	} else {
    		Debug.Log (www.error);
    	}

    	JsonData clearedJson = JsonMapper.ToObject(GameManager.instance.clearedBldgJsonText);

    	//ensure there are more than 0 buildings returned
    	if (clearedJson.Count > 0) {
	    	for (int i = 0; i < clearedJson.Count; i++) {
	    		//if the building is still considered inactive by the server
	    		if (clearedJson[i]["active"].ToString() == "0") {
	    			//Debug.Log ("Coroutine has found "+ clearedJson[i]["bldg_name"].ToString()+" to be inactive");
					GameObject thisBuilding = GameObject.Find(clearedJson[i]["bldg_name"].ToString());
					if (thisBuilding != null) {
	    				PopulatedBuilding populatedBldg = thisBuilding.GetComponent<PopulatedBuilding>();
						//Debug.Log ("GameManager is attempting to deactivate "+populatedBldg.gameObject.name);
						populatedBldg.DeactivateMe();
	    			} else {
	    				continue;
	    			}

	    		} else if (clearedJson[i]["active"].ToString() == "1") {
					//Debug.Log (clearedJson[i]["bldg_name"].ToString()+" has been reactivated by the server, but remains on player DB. Last cleared DateTime: "+clearedJson[i]["time_cleared"].ToString());
	    		}
	    	}

    	} else {
    		Debug.Log ("Player has not cleared any buildings yet");
    	}
    }


//	public void ResetAllBuildings () {
//		for (int i = 0 ; i < buildingToggleStatusArray.Length ; i++ ){
//			buildingToggleStatusArray[i] = false;
//		}
//		Building[] arrayOfBuildings = FindObjectsOfType<Building>();
//		for (int i = 0; i < arrayOfBuildings.Length; i++) {
//			Debug.Log("Sending reactivation message to " + arrayOfBuildings[i].name );
//			arrayOfBuildings[i].ReactivateMe();
//		}
//	}

	public void BuildingIsCleared (int sup, int water, int food, int foundTotalSurvivors, int foundAbleBodiedSurvivors) {
		//local updates for the running game variables
		reportedSupply = sup;
		supply += sup;
		reportedWater = water;
		waterCount += water;
		reportedFood = food;
		foodCount += food;
		reportedTotalSurvivor = foundTotalSurvivors;
		reportedActiveSurvivor = foundAbleBodiedSurvivors;

		//this updates the long term memory, and will need to be changed to update the PHP server.  This is essentially saving the check-in.
//		GamePreferences.SetFoodCount(foodCount);
//		GamePreferences.SetWaterCount(waterCount);
//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors(totalSurvivors);
//		GamePreferences.SetActiveSurvivors(survivorsActive);
		GameManager.instance.UpdateAllStatsToGameMemory();

		StartCoroutine(SendClearedBuilding());
	}

	IEnumerator SendClearedBuilding () {

		//This code was for the mock-up. using an array and strings to store data on 4 test buildings.
		/*
		if (activeBldg == "Building01") {
			buildingToggleStatusArray[0]=true;
		} else if (activeBldg == "Building02") {
			buildingToggleStatusArray[1]=true;
		} else if (activeBldg == "Building03") {
			buildingToggleStatusArray[2]=true;
		} else if (activeBldg == "Building04") {
			buildingToggleStatusArray[3]=true;
		}
		*/
		string jsonString = GameManager.instance.locationJsonText;
		JsonData bldgJson = JsonMapper.ToObject(jsonString);
		string bldg_id = "";

		for (int i = 0; i < bldgJson["results"].Count; i++) {
			if (bldgJson["results"][i]["name"].ToString() == GameManager.instance.activeBldg) {
				bldg_id = bldgJson["results"][i]["id"].ToString();
			}
		}

		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("id", GameManager.instance.userId);
		wwwForm.AddField("bldg_name", GameManager.instance.activeBldg);
		wwwForm.AddField("bldg_id", bldg_id);
		wwwForm.AddField("supply", GameManager.instance.reportedSupply);
		wwwForm.AddField("food" , GameManager.instance.reportedFood);
		wwwForm.AddField("water", GameManager.instance.reportedWater);

		Debug.Log ("sending cleared building message to the server- bldg_name: "+GameManager.instance.activeBldg+" and id: "+bldg_id);
		WWW www = new WWW(buildingClearedURL, wwwForm);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
			StartCoroutine(FetchResumePlayerData());
		} else {
			Debug.Log(www.error);
		}

		StartCoroutine(GameManager.instance.DeactivateClearedBuildings());
	}


//	public void PlayerAttemptingPurchaseFullHealth () {
//		if (playerCurrentHealth < 100) {
//			if (supply >= 20) {
//				supply -= 20;
//				SetPublicPlayerHealth(100);
//				//updating server side stats is called in the public player health function.
//
//				MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
//				mapLevelManager.UpdateTheUI();
//			} else {
//				Debug.Log ("Player does not have enough supply to make the purchase");
//			}
//		} else {
//			Debug.Log ("Player Health already full");
//		}
//	}

//	public void PlayerAttemptingPurchaseNewSurvivor () {
//		if (supply >= 50) {
//			if (survivorsActive < totalSurvivors) {
//				//subtract the cost.
//				supply -= 50;
////				GamePreferences.SetSupply(supply);
//
//				//add the survivor
//				survivorsActive ++;
////				GamePreferences.SetActiveSurvivors(survivorsActive);
////				GamePreferences.SetTotalSurvivors(totalSurvivors);
//				GameManager.instance.UpdateAllStatsToGameMemory();
//
//				//this can only be called from inventory on map level- so get that lvl manager, and update the UI elements.
//				MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
//				mapLevelManager.UpdateTheUI();
//			} else {
//				Debug.Log("There are no inactive players to train");
//			}
//		} else {
//			Debug.Log ("Player does not have enough supply to make the purchase");
//		}
//	}
//
//	public void PlayerAttemptingPurchaseShiv () {
//		if ( supply >= 5 ) {
//
//			supply -= 5;
////			GamePreferences.SetSupply(this.supply);
//
//			shivCount ++;
////			GamePreferences.SetShivCount(shivCount);
//
//			GameManager.instance.UpdateAllStatsToGameMemory();
//
//			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
//			mapLevelManager.UpdateTheUI();
//		}
//	}
//
//	public void PlayerAttemtpingPurchaseClub () {
//		if (supply >= 15) {
//			supply -= 15;
////			GamePreferences.SetSupply(supply);
//
//			clubCount ++;
////			GamePreferences.SetClubCount(clubCount);
//
//			GameManager.instance.UpdateAllStatsToGameMemory();
//
//			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
//			mapLevelManager.UpdateTheUI();
//		}
//	}
//
//	public void PlayterAttemtptingPurchaseGun () {
//		if (supply >= 25) {
//			supply -= 25;
////			GamePreferences.SetSupply(supply);
//
//			gunCount += 10;
////			GamePreferences.SetGunCount(gunCount);
//
//			GameManager.instance.UpdateAllStatsToGameMemory();
//
//			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
//			mapLevelManager.UpdateTheUI();
//		}
//	}
//
//	public void ProcessDurability () {
//		if (weaponEquipped == "shiv") {
//			shivDurability --;
//
//			if (shivDurability <= 0) {
//				shivCount --;
//				shivDurability = 50;
////				GamePreferences.SetShivCount (shivCount);
////				GamePreferences.SetShivDurability (shivDurability);
//			} else {
////				GamePreferences.SetShivDurability (shivDurability);
//			}
//
//			Debug.Log ("Shiv has successfully processed durability, it's now got " + shivDurability + " durability, and total shivs: " +shivCount);
//		} else if (weaponEquipped == "club") {
//			clubDurability --;
//
//			if (clubDurability <= 0) {
//				clubCount --;
//				clubDurability = 50;
////				GamePreferences.SetClubCount (clubCount);
////				GamePreferences.SetClubDurability (clubDurability);
//			} else {
////				GamePreferences.SetClubDurability (clubDurability);
//			}
//
//			Debug.Log ("Club has successfully processed durability, it's now got " + clubDurability + " durability, and total clubs: " + clubCount);
//		} else if (weaponEquipped == "gun"){
//			gunCount --;
//
////			GamePreferences.SetGunCount(gunCount);
//
//			Debug.Log ("Gun has successfully processed ammo spent, player now has " + gunCount +" amunition left");
//		} else {
//			Debug.Log ("Durability function failed to execute");
//		}
//	}
//
//	public void CheckEatingAndDrinking () {
//		StartCoroutine ( UpdatePlayersEatingAndDrinking () );
//	}
//
//	// the idea with this coroutine is to invokerepeating on awake, every 15-30, and execute the 'meal' as soon as it's time.
//	IEnumerator UpdatePlayersEatingAndDrinking () {
//		//Debug.Log ("Checking to update food / Water / Meal counts");
//
//		//if we have fewer meals than expected.
//		DateTime now = System.DateTime.Now;
//		Double days = (now - timeCharacterStarted).TotalDays;
//		int expected = (int)Mathf.Floor( (float)days * 2 );
//
//		if (mealCount < expected) {
//			mealCount ++;
//			foodCount = foodCount - totalSurvivors;
//			waterCount = waterCount - totalSurvivors;
//
//			Debug.Log ("a meal has been processed. Total meals eaten: " + mealCount + " Food Count: " + foodCount + " Water Count: "+ waterCount);
//
////			GamePreferences.SetWaterCount(waterCount);
////			GamePreferences.SetFoodCount(foodCount);
////			GamePreferences.SetMealsCount(mealCount);
//
//			UpdateAllStatsToGameMemory();
//
//			//we need to restart the coroutine to ensure that we don't need to process multiple meals.  
//			if (mealCount != expected) {
//				StartCoroutine( UpdatePlayersEatingAndDrinking () );
//			} else {
//				yield break;
//			}
//		} else {
//			Debug.Log ("not time to eat or drink yet... but coroutine is checking @ days played: " + days + " and an expected meal count of " + expected);
//			yield break;
//		}
//	}


	public void PublicStartLocationServices () {
		StartCoroutine(StartLocationServices());
	}


	IEnumerator StartLocationServices () {
		if (!Input.location.isEnabledByUser){
			Debug.Log ("location services not enabled by user");
            yield break;
        }

		Input.location.Start(10f, 10f);

		//wait until Service initializes, or 20 seconds.
		int maxWait = 20;
		while (Input.location.status ==  LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		// Service did not initialize within 20 seconds
		if (maxWait < 1) {
			print ("Location initialization timed out");
			yield break;
		} 

		// connection failed to initialize
		if (Input.location.status == LocationServiceStatus.Failed) {
			print ("Unable to determine location");
			yield break;
		} else if (Input.location.status == LocationServiceStatus.Running) {
			//access granted and location values can be retireved
			Debug.Log ("Location Services report running successfully");
			yield return Input.location.lastData;
			print ("location is: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
		}

	}
}
