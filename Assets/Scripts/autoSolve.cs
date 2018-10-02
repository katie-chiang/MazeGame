using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using depth first search algorithm to allow the player to solve the maze by itself
public class autoSolve : MonoBehaviour {

	private float speed;

	private Maze mazeScript;
	private GameObject theMaze;

	private Cell[] cells;

	private Stack<int> traveledCells;
	private Stack<int> travelingDirection;
	private Stack<int> playerRoute;

	private Vector3 currentPos;
	private Vector3 endPos;

	private bool foundPath = false;

	//get the maze script to get the information of the maze
	void Start () {

		speed = 5;
			
		theMaze = GameObject.Find ("MazeGenerator");
		mazeScript = theMaze.GetComponent<Maze> ();

		//get the cells array which has the "graph" that I need to traverse
		cells = mazeScript.cells;

		//get the initial position of the player
		currentPos = transform.position;
		endPos = transform.position;

		//start searching through the maze
		searchMaze ();
	}

	//after route is found, move the player
	void Update(){

		if (foundPath) {

			//playerRoute is the stack that contains the direction in which the player should turn
			if (playerRoute.Count >= 0) {

				if (transform.position != endPos) {
					Vector3 pos = Vector3.MoveTowards (transform.position, endPos, speed * Time.deltaTime);
					GetComponent<Rigidbody> ().MovePosition (pos);
				} else {
					currentPos = endPos;
					//update end pos
					if (playerRoute.Count != 0) {
						int dir = playerRoute.Pop ();
						movePlayer (dir);
					}
				}	
			}
		}
	}

	//function that updates the end position by taking the current position of the player
	//and moving it in the direction desired
	void movePlayer(int travelTo){

		switch (travelTo) {
		case 1:
			endPos = new Vector3 (currentPos.x, currentPos.y, currentPos.z + 1);
			break;
		case 2:
			endPos = new Vector3 (currentPos.x, currentPos.y, currentPos.z - 1);
			break;
		case 3:
			endPos = new Vector3 (currentPos.x + 1, currentPos.y, currentPos.z);
			break;
		case 4:
			endPos = new Vector3 (currentPos.x - 1, currentPos.y, currentPos.z);
			break;

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

	//use depth first search to solve the maze
	//right now in the cell array all the visited are marked as true
	//so in this algorithm, reverse the logic for visited to check for visited
	void searchMaze(){

		int currentCell = 0;
		int currentNeighbour = 0;

		//stack that keeps track of the cell numbers
		traveledCells = new Stack<int>();
		traveledCells.Clear();

		//stack that keeps track of the directions traveled
		travelingDirection = new Stack<int> ();
		travelingDirection.Clear ();

		int totalCells = mazeScript.colSize * mazeScript.rowSize;
		int numCellsVisited = 0;
		int travelTo;

		//start at cell 0
		cells [currentCell].visited = false;
		numCellsVisited++;

		while (numCellsVisited < totalCells) {

			//break out of the loop if reached destination (top right corner)
			if (currentCell == totalCells - 1) {
				break;
			}

			//travelTo has the direction to trun in
			travelTo = checkNeighbour (ref currentCell, ref currentNeighbour);

			//again, reversing the boolean logic from previous maze-making algo
			if (cells [currentNeighbour].visited == true && cells [currentCell].visited == false) {

				//have a stack that keeps track of the direction to turn in
				travelingDirection.Push (travelTo);

				cells [currentNeighbour].visited = false;
				numCellsVisited++;
				traveledCells.Push (currentCell);
				currentCell = currentNeighbour;

			}
		
		}

		//after reached destination and broke out of loop, now reverse the order of the directions with another stack
		playerRoute = new Stack<int> ();
		playerRoute.Clear ();

		while (travelingDirection.Count != 0) {
			int dir = travelingDirection.Pop ();
			playerRoute.Push (dir);
		}

		//set found path to true so player can start moving
		foundPath = true;

	}
		
	int checkNeighbour(ref int currentCell, ref int currentNeighbour){

		int totalCells = mazeScript.colSize * mazeScript.rowSize;
		int numNeighbours = 0;
		int[] neighboursCells = new int[4];
		int[] travelDirection = new int[4];

		//for north, check if out of bounds or if cell exists and if it was visited
		if ((currentCell < totalCells - mazeScript.colSize) && cells[currentCell + mazeScript.colSize].visited) {

			//check if a wall exists, only counts as neighbour if no wall exists
			if (!cells [currentCell].northExists) {
				neighboursCells[numNeighbours] = currentCell + mazeScript.colSize;
				travelDirection [numNeighbours] = 1;
				numNeighbours++;
			}

		}

		//for south
		if ((currentCell >= mazeScript.colSize) && cells[currentCell - mazeScript.colSize].visited) {	

			if (!cells [currentCell].southExists) {
				neighboursCells[numNeighbours] = currentCell - mazeScript.colSize;
				travelDirection [numNeighbours] = 2;
				numNeighbours++;
			}

		}

		//for east
		if (((currentCell + 1) % mazeScript.colSize != 0) && cells[currentCell + 1].visited) {

			if (!cells [currentCell].eastExists) {
				neighboursCells[numNeighbours] = currentCell + 1;
				travelDirection [numNeighbours] = 3;
				numNeighbours++;
			}
	
		}

		//for west
		if ((currentCell % mazeScript.colSize != 0) && currentCell != 0 && cells[currentCell - 1].visited) {

			if (!cells [currentCell].westExists) {
				neighboursCells[numNeighbours] = currentCell - 1;
				travelDirection [numNeighbours] = 4;
				numNeighbours++;
			}
		}

		if (numNeighbours != 0) {
			
			//choose a random neigbour to go to
			int randomCell = Random.Range (0, numNeighbours);
			currentNeighbour = neighboursCells [randomCell];
			int travelTo = travelDirection [randomCell];
			return travelTo;

		} else {
			
			if(traveledCells.Count > 0){

				//if there are no neighbours, then start back tracking
				currentCell = traveledCells.Pop();

				//also pop the wrong directions traveled
				travelingDirection.Pop();

			}

			return 0;

		}

	}

}
