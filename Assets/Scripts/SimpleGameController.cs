using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;    
//using UnityEditor;
using SimpleJSON;

public class SimpleGameController : MonoBehaviour 
{	
	// The prefab we want to spawn
	public GameObject agent;
	public int nAgent = 1;

	public float startWait = 0f;
	public float changeWait = 5f;

	int nMarkets = 3;
	int t = 0;

	// Container for controllers of agents (scripts components)
	List<AgentMovement> agents_controllers = new List<AgentMovement> ();

	// Container for market positions
	List<Vector3> marketsPositions = new List<Vector3> ();

	List<int> GoalsMemory = new List<int> ();

	void SetDestinations(int t) {

		for (int i = 0; i < nAgent; i++) {

			// The new goal is the following market
			int goal_idx = (GoalsMemory [i] + 1) % 3;

			// Stock in memory the new goal
			GoalsMemory [i] = goal_idx;

			// Set the goal	
			agents_controllers [i].SetGoal(marketsPositions [goal_idx]);
			Debug.Log ("Goal of agent " + i + " is now Market " + goal_idx + ".");
		}
	}

	void GetMarketPositions() {
		for (int i = 0; i < nMarkets; i ++) {

			Vector3 marketPosition = GameObject.FindGameObjectWithTag (string.Concat ("Village_", i)).transform.position;
			Debug.Log ("Market position of market " + i + " is " + marketPosition + ".");
			marketsPositions.Add (marketPosition);
		}

	}

	void GetAgentsControl() {
		for (int i = 0; i < nAgent; i++) {
			Quaternion agent_initial_rotation = Quaternion.identity; 
			Vector3 agent_initial_position = new Vector3 (490, 27, 480);
			// Maybe it could be random at start using something like Random.Range (-spawnValues.x, spawnValues.x)
			GameObject ind = Instantiate(agent, agent_initial_position, agent_initial_rotation);
			agents_controllers.Add (ind.GetComponent<AgentMovement> ());
		}
	}

	void GetMemoryOfGoals() {
		for (int i = 0; i < nAgent; i++) {
			GoalsMemory.Add (Random.Range(0, 3));
		}
	}	

	void Start() {
		Debug.Log ("Prepare stuff.");

		GetMarketPositions ();
		GetAgentsControl ();

		GetMemoryOfGoals ();

		Debug.Log("Stuff prepared.");
		StartCoroutine (ChangeGoalsRegularly ());
	}	

	IEnumerator ChangeGoalsRegularly () {
		yield return new WaitForSeconds (startWait);
		while (true) {  // No end to this particulary interesting game!
			Debug.Log ("t: " + t);
			SetDestinations (t);
			t++;
			yield return new WaitForSeconds (changeWait);
		}
	}
}
