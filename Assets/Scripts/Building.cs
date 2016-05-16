﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Building : MonoBehaviour {

	public int zombiePopulation;

	[SerializeField]
	private Text myText;

	private bool buildingClear = false;
	
	// Use this for initialization
	void Start () {
		 	CheckIfThisBuildingIsClear();
			
	}

	private void updateTheText () {
		myText.text = this.zombiePopulation.ToString();
	}

	void OnLevelWasLoaded () {
		GenerateZombies();
		updateTheText();
	}
	
	public void BuildingPressed () {
		Debug.Log ("Building "+ gameObject.name +" has been triggered");
		if (buildingClear == false) { //only load combat if the building is not clear
			//combatManager.SetZombiesEncountered (zombiePopulation);
			GameManager.instance.LoadIntoCombat(zombiePopulation, this.gameObject.name);
		} else {
			Debug.Log("Building thinks it's already been cleared");
		}
	}

	private void CheckIfThisBuildingIsClear () {
		if (this.gameObject.name == "Building01" && GameManager.instance.buildingToggleStatusArray[0] == true) {
			DeactivateMe();
		} else if (this.gameObject.name == "Building02" && GameManager.instance.buildingToggleStatusArray[1] == true) {
			DeactivateMe();
		} else if (this.gameObject.name == "Building03" && GameManager.instance.buildingToggleStatusArray[2] == true) {
			DeactivateMe();
		} else if (this.gameObject.name == "Building04" && GameManager.instance.buildingToggleStatusArray[3] == true) {
			DeactivateMe();
		}
	}

	public void DeactivateMe () {
		this.zombiePopulation = 0;
		myText.text = "0";
		this.buildingClear = true;
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<Image>().color = Color.gray;

		Debug.Log ("Deactivate function has completed for " + this.gameObject.name + " and currently has " + this.zombiePopulation.ToString() + " zombies");
		//still need to write the code to change appearance, turn on transparent panel? indicate that it's clear.
	}

	public void ReactivateMe () {
		GenerateZombies();
		this.buildingClear = false;
		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<Image>().color = Color.white;
		updateTheText();
		Debug.Log (this.gameObject.name.ToString() + " has completed the reactivation instructions");
	}

	void GenerateZombies () {
		int zombies = Random.Range ( 1, 10);
		zombiePopulation = zombies;
	}

	void Update () {
		
	}
}