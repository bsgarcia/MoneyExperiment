using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using UnityEngine.AI; 
using System.Collections.Generic;
using System.IO;    
//using UnityEditor;
using SimpleJSON;


public class GameController : MonoBehaviour 
{	

	public int speed = 10;
	public int tolerance = 10;
	public bool capture = true;
	public int frameRate = 20;


	// The HUD texts and images
	public Text TimeCountingText;
	public Text TheEndText;

	public List<GameObject> labels = new List<GameObject>(4);

	// The prefabs we want to spawn
	public List<GameObject> agent = new List<GameObject>(3);

	// The parameters
	public int t = 0;

	// public Vector3 spawnValues;

	int nMarkets = 3;

	int nAgent;
	int tMax;

	bool end;

	List<int> typeList = new List<int>();

	// 1rst level: list created will contain a list per unit of time 
	// 2nd level: Each list indexed on time will contain list for each agent
	// 3rd level: Each list indexed on agent idx will contain 2 integers referencing the market where the agent goes
	List<List<List<int>>> marketList = new List<List<List<int>>> (); 

	// Container for controllers of agents (scripts components)
	List<AgentMovement> agents_controllers = new List<AgentMovement>();
	List<NavMeshAgent> agentsNavMesh = new List<NavMeshAgent>();

	// Container for market positions
	List<Vector3> marketsPositions = new List<Vector3>();

	string dataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) 
					  + "/AndroidXP/data.json";

	string captureFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) 
		+ "/AndroidXP/Capture";

	void ImportData (string file)
	{	
		Debug.Log(dataPath);
		string string_data = File.ReadAllText(file); 
		JSONNode data = SimpleJSON.JSON.Parse (string_data);

		// Get number of agent and number of time units
		nAgent = data ["p"].Count;
		tMax = data ["market_choice"].Count;

		for (int i = 0; i < nAgent; i++) {
			typeList.Add (data ["p"] [i].AsInt);
		}

		for (int i = 0; i < tMax; i++) {

			marketList.Add (new List<List<int>> ());

			for (int j = 0; j < nAgent; j++) {

				marketList [i].Add (new List<int> ());


				for (int k = 0; k < 2; k++) {
					marketList [i] [j].Add(data ["market_choice"] [i] [j] [k].AsInt);
				}
			}
		}
	}

	void GetMarketPositions() {
		for (int i = 0; i < nMarkets; i ++) {
			marketsPositions.Add (GameObject.FindGameObjectWithTag (string.Concat ("Village_", i)).transform.position);
		}

	}

	void GetAgentsControl() {

		// foreach (int i in typeList) {	
		for (int i = 0; i < nAgent; i++) { 

			// Define initial position and rotation 
			// Maybe it could be random at start using something like Random.Range (-spawnValues.x, spawnValues.x)
			Vector3 agent_initial_position = new Vector3 (490, 27, 480);
			Quaternion agent_initial_rotation = Quaternion.identity; 

			// Instantiate agent
			int agentType = typeList [i];
			GameObject ind = Instantiate(agent[agentType], agent_initial_position, agent_initial_rotation);

			// Get the script that control this particular agent
			agents_controllers.Add (ind.GetComponent<AgentMovement> ());

			agentsNavMesh.Add (ind.GetComponent<NavMeshAgent> ());
		}
	}

	void PrepareAgents() {
		for (int i = 0; i < nAgent; i++) {
			agentsNavMesh [i].speed = speed;
			agents_controllers [i].SetTolerance (tolerance);
		}
		
	}

	void SetDestinations(int t) {

		for (int i = 0; i < nAgent; i++) {	

			if (marketList [t] [i].Contains (0) & marketList [t] [i].Contains (1)) {
				agents_controllers [i].SetGoal (marketsPositions [0]);

			} else if (marketList [t] [i].Contains (1) & marketList [t] [i].Contains (2)) {
				agents_controllers [i].SetGoal (marketsPositions [1]);

			} else {
				agents_controllers [i].SetGoal (marketsPositions [2]);
			}
		}
	}

	void PrepareCapture() {
		// Set the playback framerate (real time will not relate to game time after this).
		Time.captureFramerate = frameRate;

		// Create the folder
		System.IO.Directory.CreateDirectory(captureFolder);

		// If this folder exits, delete all the files in it
		System.IO.DirectoryInfo di = new DirectoryInfo(captureFolder);
		foreach(System.IO.FileInfo file in di.GetFiles()) file.Delete();
	}

	void MakeScreenShots() {
//
//		if (Time.time > nextScreenShot) {
//			nextScreenShot = Time.time + frameRate;
		// Append filename to folder name (format is '0005 shot.png"')
		string name = string.Format ("{0}/{1:D04}shot.png", captureFolder, Time.frameCount);

		// Capture the screenshot to the specified file.
		ScreenCapture.CaptureScreenshot (name);
//		}
	}

	void Start() {
		Debug.Log ("Prepare stuff.");

		TheEndText.text = "";

		// Prepare some stuff
		ImportData(dataPath);

		GetMarketPositions ();
		GetAgentsControl ();

		PrepareAgents ();
		PrepareCapture ();

		Debug.Log("Stuff prepared.");

		// Launch
		TimeCountingText.text = t.ToString();
		SetDestinations(t);
	}	

	void Update() {

		MakeScreenShots();

		if (!end) {
			int sumArrived = 0;
			for (int i = 0; i < nAgent; i++) {
				if (!agents_controllers [i].GetIsWalking ()) {
					sumArrived += 1; 
				}
			}

			if (sumArrived == nAgent) {
				NewTimeStep();
			}
		}
	}

	void NewTimeStep() {
		t++;
		if (t >= tMax) {
			End ();
		} else {
			TimeCountingText.text = t.ToString();
			Debug.Log ("t: " + t);
			SetDestinations(t);
		}
	}

	void End() {
		Debug.Log ("Simulation ended.");
		end = true;
		TimeCountingText.text = "";
		TheEndText.text = "The End";
		foreach (GameObject i in labels) {
			i.SetActive(false);
		}
	}
//
//	public void SetSpeed(float newValue) {
//
//		for (int i = 0; i < nAgent; i++) {
//			agentsNavMesh [i].speed = (int) newValue;
//		}
//		Debug.Log ("Speed of agents is now: " + newValue + ".");
//	}

}
