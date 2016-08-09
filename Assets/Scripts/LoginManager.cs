using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;

public class LoginManager : MonoBehaviour {

	[SerializeField]
	private Text loginPasswordText, loginEmailText, registerEmail, registerPassword, registerPassword2;
	private int survivorsDrafted = 0;

	public GameObject registrationPanel, loggedInPanel, survivorDraftPanel;
	public IGraphResult fbFriendsResult;
	public SurvivorPlayCard[] survivorDraftCardArray;

//	private string registerUrl = "http://localhost/ARGZ_SERVER/register.php";
//	private string playerDataUrl = "http://localhost/ARGZ_SERVER/PlayerData.php";
//	private string loginUrl = "http://localhost/ARGZ_SERVER/login.php";

	private string newSurvivorUrl = "http://argzombie.com/ARGZ_SERVER/create_new_survivor.php";
	private string findUserAcctURL = "http://argzombie.com/ARGZ_SERV";
	
	// Use this for initialization
	void Start () { 
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
        //Handle FB.Init
            FB.Init(SetInit, OnHideUnity);
        }
        
        
    }

    void OnLevelWasLoaded () {
			//if the game data hasn't been loaded, then attempt to resume character automatically.
            if (GameManager.instance.gameDataInitialized) {
				Debug.Log("Why didn't this register as true?!?");
            } else {
            	if (FB.IsLoggedIn) {
            		Debug.Log("Automatically attempting to resume game");
            		ResumeCharacter();
				}
            }
    }
    
    void SetInit () {
        FB.ActivateApp();
        if (FB.IsLoggedIn) {
            Debug.Log ("FB is logged in");

            //fetch the name and ID from the FB API.
			FB.API ("/me?fields=id", HttpMethod.GET, UpdateUserId);
		    FB.API ("/me?fields=first_name", HttpMethod.GET, UpdateUserFirstName);
		    FB.API ("/me?fields=last_name", HttpMethod.GET, UpdateUserLastName);
		    FB.API ("/me", HttpMethod.GET, UpdateUserName);

            loggedInPanel.SetActive (true);

        } else {
            Debug.Log ("FB is not logged in");
            loggedInPanel.SetActive (false);
        }
        
    }
    
    void OnHideUnity (bool isGameShown) {
        
        if (!isGameShown) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
    
    public void FBlogin ()  {
        
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("user_friends");
        permissions.Add("email");
        
        
        FB.LogInWithReadPermissions (permissions, AuthCallBack);
        
    }
    
    void AuthCallBack (IResult result) {
        
        if (result.Error != null) {
            Debug.Log (result.Error);
        } else {
            
            if (FB.IsLoggedIn) {
                Debug.Log ("FB is logged in");
                loggedInPanel.SetActive (true);
                FB.API ("/me?fields=id", HttpMethod.GET, UpdateUserId);
		        FB.API ("/me?fields=first_name", HttpMethod.GET, UpdateUserFirstName);
		        FB.API ("/me?fields=last_name", HttpMethod.GET, UpdateUserLastName);
				FB.API ("/me", HttpMethod.GET, UpdateUserName);
            } else {
                Debug.Log ("FB is NOT logged in");
                loggedInPanel.SetActive (false);
            }
            
        }
        
    }
    
	private void UpdateUserId (IResult result) {
		if (result.Error == null) {
            GameManager.instance.userId = result.ResultDictionary["id"].ToString();
        } else {
            Debug.Log (result.Error);
        }
	}

	private void UpdateUserFirstName(IResult result) {
		if (result.Error == null) {
			GameManager.instance.userFirstName = result.ResultDictionary["first_name"].ToString();
		} else {
			Debug.Log (result.Error);
		}
	}

	private void UpdateUserLastName (IResult result) {
		if (result.Error == null) {
			GameManager.instance.userLastName = result.ResultDictionary["last_name"].ToString();
		}else{
			Debug.Log(result.Error);
		}
	}

	private void UpdateUserName (IResult result) {
		if(result.Error == null) {
			GameManager.instance.userName = result.ResultDictionary["name"].ToString();
		}else {
			Debug.Log(result.Error);
		}
	}

	private void UpdateSurvivorDraftWindow (IGraphResult result) {
		if (result.Error == null) {
			//store the data object for later use in next friend being updated.
			fbFriendsResult = result;
			Debug.Log(result.ToString());
			string data = result.RawResult as string;
			Debug.Log(data);
			JsonData jsonData = JsonMapper.ToJson(data);

			//fill the player data into the play card objects on the draft window.
			for (int i=0; i<3; i++) {
				//set the name from the result.
				survivorDraftCardArray[i].survivor.name = jsonData[i]["name"].ToString();
				// roll and load random stats
				int stam = Random.Range(90, 140);
				survivorDraftCardArray[i].survivor.baseStamina = stam;
				survivorDraftCardArray[i].survivor.curStamina = stam;
				int attk = Random.Range(9, 25);
				survivorDraftCardArray[i].survivor.baseAttack = attk;

				//get and update the photo
				Image survivorPic = survivorDraftCardArray[i].profilePic;
				string imgUrl = jsonData[i]["picture"]["data"]["url"].ToString();
				WWW www = new WWW(imgUrl);
				survivorPic.sprite = Sprite.Create(www.texture, new Rect(0,0,200,200), new Vector2());

				//update the text field.
				string myText = "";
				myText += "name: " + survivorDraftCardArray[i].survivor.name.ToString()+"\n";
				myText += "stamina: " + survivorDraftCardArray[i].survivor.baseStamina.ToString()+"\n";
				myText += "attack: " +survivorDraftCardArray[i].survivor.baseAttack.ToString(); 
				survivorDraftCardArray[i].displayText.text = myText;
			}
		}else{
			Debug.Log(result.Error);
		}
	}

	//this is a temporary function to test sending characters to the server.  eventually these choices will be auto-populated from friends, and cycle choices on each pick- creating a Zombie Apocalypse Draft.
	public void ChooseSurvivorToSend (int choice) {

		
		if (survivorsDrafted <= 4){
			if (choice == 1) {
				int teamPos = 4 - survivorsDrafted;
				StartCoroutine(SendNewSurvivorToServer("Bill", 140, 8, teamPos));
			} else if (choice == 2) {
				int teamPos = 4 - survivorsDrafted;
				StartCoroutine(SendNewSurvivorToServer("Sally", 100, 10, teamPos));
			} else if (choice == 3) {
				int teamPos = 4 - survivorsDrafted;
				StartCoroutine(SendNewSurvivorToServer("Jacob", 90, 12, teamPos));
			}
			survivorsDrafted ++;
			if (survivorsDrafted == 4) {
				GameManager.instance.ResumeCharacter();
			}
		} else {
			GameManager.instance.ResumeCharacter();

		}
	}

	IEnumerator SendNewSurvivorToServer (string name, int stamina, int attack, int teamPosition) {
		WWWForm form = new WWWForm();
		form.AddField("owner_id", GameManager.instance.userId);
		form.AddField("team_position", teamPosition); //this will need to actually pull
		form.AddField("name", name);
		form.AddField("base_stam", stamina);
		form.AddField("curr_stam", stamina);
		form.AddField("base_attack", attack);
		form.AddField("weapon_equipped", "none");

		WWW www = new WWW(newSurvivorUrl, form);
		yield return www;
		if (www.error == null) {
			Debug.Log(www.text);
			string jsonReturn = www.text.ToString();
			JsonData jsonResult = JsonMapper.ToObject(jsonReturn);

			Debug.Log (jsonResult[0].ToString());

			//at some point the client will need to recieve the json from the server and report a failed creation.
//			if (jsonResult[0].ToString() == "Success") {
//				Debug.Log(jsonResult[1].ToString());
//			} else {
//				Debug.LogError ("new survivor not added to server error: "+ jsonResult[1].ToString());
//				survivorsDrafted --;
//			}

		}else{
			survivorsDrafted --;
			Debug.Log(www.error);
		}

	}


/*
	public void RegisterAccount () {
		if (registerPassword.ToString() != registerPassword2.ToString()) {
			Debug.Log ("Passwords do not match- not submitting to server");
		} else {
			StartCoroutine(Register());
		}
	}

	IEnumerator Register () {
		WWWForm form = new WWWForm();
		//form.AddField("register", "register");
		form.AddField("email", registerEmail.text);
		form.AddField("password", registerPassword.text);
		WWW www = new WWW(registerUrl, form);
		yield return www;

		//error check
		if (www.error == null) {
			Debug.Log ("WWW OK! result: " + www.text);

			if (www.text == "email not valid") {
				//server returned invalid email
			} else if (www.text == "email address is already registered") {
				//server returned email already registered
			} else if (www.text == "account successfully created") {
				Debug.Log ("server successfully created new account");
				ToggleRegistrationPanel();
				//will also have to log user in
			}
		}else{
			Debug.Log ("WWW error: " + www.error);
		}
	}

	public void LoginCheck () {
		StartCoroutine(Login());
	}

	IEnumerator Login () {
		WWWForm form = new WWWForm();
		//form.AddField("register", "register");
		form.AddField("email", loginEmailText.text);
		form.AddField("password", loginPasswordText.text);
		WWW www = new WWW(loginUrl, form);
		yield return www;

		if (www.error == null) {
			Debug.Log ("WWW OK! result: " + www.text);

			if (www.text == "email not valid") {
				//server returned invalid email
			} else if (www.text == "Incorrect email or password") {
				//server returned email already registered
			} else if (www.text == "successfully logged in") {
				Debug.Log ("You have successfully logged in");
				FakeLoggedInSuccess();//this just activates the panel for character new/continue
			}
		}else{
			Debug.Log ("WWW error: " + www.error);
		}
	}
*/
	
	public void ToggleRegistrationPanel () {
		if (registrationPanel.activeInHierarchy == false) {
			registrationPanel.SetActive(true);
		} else if (registrationPanel.activeInHierarchy == true) {
			registrationPanel.SetActive(false);
		}
	}

	public void FakeLoggedInSuccess () {
		if (loggedInPanel.activeInHierarchy == false){
			loggedInPanel.gameObject.SetActive(true);

			if (FB.IsLoggedIn == true) {
				GameManager.instance.ResumeCharacter();
			}

		} else {
			loggedInPanel.gameObject.SetActive(false);
		}

	}

	public void ResumeCharacter () {
			GameManager.instance.ResumeCharacter();
			//SceneManager.LoadScene ("02a Map Level");
	}

	public void StartNewCharacter () {
		survivorDraftPanel.SetActive(true);
		FB.API("me/friends?fields=name,picture.width(200).height(200)", HttpMethod.GET, UpdateSurvivorDraftWindow);
		GameManager.instance.StartNewCharacter();
			
	}
}
