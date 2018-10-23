using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
using System.Linq;
using System.IO;  

public class AgentMovement : MonoBehaviour
{
	int tolerance = 10;

	Animator anim;
	NavMeshAgent nav;

	Vector3 goal;
	bool isWalking = false;

	void Awake ()
	{	
		anim = GetComponent <Animator> ();
		nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
	}

	void Update ()
	{	
		if (isWalking) {

			if (IsArrived ()) {
				StopWalking ();
			} 
		}
	}
		
	// Check if player is in market perimeter
	bool IsArrived () {

		Vector3 playerPos = transform.position;
		bool cond1 = playerPos.x <= (goal.x + tolerance) & playerPos.x >= (goal.x - tolerance);
		bool cond2 = playerPos.z <= (goal.z + tolerance) & playerPos.z >= (goal.z - tolerance);
		return cond1 & cond2;
	}

	void Animating (bool walking)
	{	
		anim.SetBool ("Walking", walking);
	}

	void Walk () {
		nav.isStopped = false;
		anim.SetBool ("Walking", true);
		nav.SetDestination (goal);
		isWalking = true;
	}

	void StopWalking () {
		isWalking = false;
		nav.isStopped = true;
		anim.SetBool ("Walking", false);
	}
	 
	public void SetGoal (Vector3 position) {
		goal = position;
		Walk ();
	}

	public bool GetIsWalking() {
		return isWalking;
	}

	public void SetTolerance(int newTolerance) {
		tolerance = newTolerance;
	}
}

