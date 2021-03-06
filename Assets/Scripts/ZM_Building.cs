﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZM_Building : MonoBehaviour {

	private ZombieModeManager zombieMapLevlMgr;

	public string buildingID, photo_reference;
	public float myLattitude, myLongitude;

	void Start () {
		zombieMapLevlMgr = FindObjectOfType<ZombieModeManager> ();
	}

	public void SetUpTheBuilding (string name, string bld_id, float lat, float lng, string photo_ref) {
		gameObject.name = name;
		buildingID = bld_id;
		myLattitude = lat;
		myLongitude = lng;
		photo_reference = photo_ref;
	}

	public void ClickedOn () {
		Debug.Log (this.gameObject.name+" is making with the clicky clicky!! ID: "+buildingID);
		this.gameObject.tag = "Untagged";
		zombieMapLevlMgr.ActivateBuildingInspector (gameObject.GetComponent<ZM_Building>());
	}

	public void ThisBuildingIsBaited () {
		Debug.Log("this building is found to have been baited already: "+ buildingID +" || " + this.name);
		Button myButton = this.GetComponent<Button>();
		myButton.interactable = false;
	}

}
