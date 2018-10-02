using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float moveSpeed;
	private float maxspeed = 20;

	private Rigidbody playerRigid;

	private Maze mazeScript;
	private GameObject theMaze;

	private Vector3 input; 

	//get the rigid body component of object and also information of the maze
	void Start () {
		
		moveSpeed = 10f;
		playerRigid = GetComponent<Rigidbody> ();

		theMaze = GameObject.Find ("MazeGenerator");
		mazeScript = theMaze.GetComponent<Maze> ();
	}

	void Update () {

		//get the user input
		input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		//control the player speed to under a max speed
		if (playerRigid.velocity.magnitude < maxspeed) {

			//use the rigid body function to move the player
			playerRigid.AddForce (input * moveSpeed);

		}

	}

	//collision detection for player reaching the goal
	void OnCollisionEnter(Collision col){

		if (col.gameObject.tag == "end") {

			//make the dimensions of the maze bigger
			mazeScript.colSize++;
			mazeScript.rowSize++;

			//Destroy the old player
			Destroy (mazeScript.tempPlayer);

			//Destroy the old walls
			for (int i = 0; i < mazeScript.wallHolder.transform.childCount; i++) {
				Destroy(mazeScript.wallHolder.transform.GetChild (i).gameObject, 0.0f);
			}

			//detach all children
			mazeScript.wallHolder.transform.DetachChildren ();

			//Destroy the old floors
			for (int i = 0; i < mazeScript.floorHolder.transform.childCount; i++) {
				Destroy(mazeScript.floorHolder.transform.GetChild (i).gameObject, 0.0f);
			}

			//detach all children
			mazeScript.floorHolder.transform.DetachChildren ();

			//create new maze with bigger dimensions
			mazeScript.createWalls();
		}
	
	}

}
