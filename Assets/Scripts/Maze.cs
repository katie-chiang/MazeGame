using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

	//the game objects that are used in the game
	public GameObject wall;
	public GameObject floor;
	public GameObject playerOne;
	public GameObject endFloor;

	//the size of the maze, which can be decided by user input
	public int colSize = 5;
	public int rowSize = 5;

	//array that keeps track of all the cells
	//making it public because i need to access it in the maze solving algorithm
	public Cell[] cells;

	//stack that keeps track of the traveled cells for back tracking
	private Stack<int> traveledCells;

	//these game object are parents
	public GameObject wallHolder;
	public GameObject floorHolder;

	//have this game object to destroy later on when going to the next level
	public GameObject tempPlayer;

	public void Start () {

		//create the parents
		floorHolder = new GameObject();
		floorHolder.name = "Floor";

		wallHolder = new GameObject();
		wallHolder.name = "Walls";

		//start creating the maze
		createWalls ();
	}

	//use this function to create a grid of walls
	public void createWalls(){

		GameObject tempWall;
		GameObject tempFloor;

		Vector3 initialPos;
		Vector3 currentPos;

		//the initial position for the start of the creation of the walls is the bottom left
		initialPos = new Vector3(-colSize/2 + 0.5f, 0.0f, -rowSize/2 + 0.5f);

		//create the walls for the columns
		for (int i = 0; i < rowSize; i++) {
			
			for (int j = 0; j <= colSize; j++) {

				//get the current position (starts from bottom left and goes to top right)
				currentPos = new Vector3 (initialPos.x + j - 0.5f, 0.0f, initialPos.z + i - 0.5f);

				//create the wall
				tempWall = Instantiate(wall, currentPos, Quaternion.identity) as GameObject;

				//assign the wall to the parent
				tempWall.transform.parent = wallHolder.transform;
			}
		}

		//create the walls for the rows
		for (int i = 0; i <= rowSize; i++) {
			
			for (int j = 0; j < colSize; j++) {

				//get the current position (starts from bottom left and goes to top right)
				currentPos = new Vector3 (initialPos.x + j, 0.0f, initialPos.z + i - 1);
				tempWall = Instantiate(wall, currentPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
				tempWall.transform.parent = wallHolder.transform;

			}
		}

		//create the floor
		for (int i = 0; i < rowSize; i++){
			for (int j = 0; j < colSize; j++){

				currentPos = new Vector3(initialPos.x + j, -0.5f, initialPos.z + i - 0.5f);

				//if it's the first tile or the last tile, then make the floor a different material
				if ((i == 0 && j == 0)) {
					
					tempFloor = Instantiate(endFloor, currentPos, Quaternion.identity) as GameObject;

				} else if((i == rowSize - 1 && j == colSize - 1)){
					//put a tag on the last tile for collision detection
					tempFloor = Instantiate(endFloor, currentPos, Quaternion.identity) as GameObject;
					tempFloor.tag = "end";
				}else{
					tempFloor = Instantiate(floor, currentPos, Quaternion.identity) as GameObject;
				}

				tempFloor.transform.parent = floorHolder.transform;
			}
		}

		//initialize position of player to the bottom left of the maze
		currentPos = new Vector3(initialPos.x , 0, initialPos.z - 0.5f);
		tempPlayer = Instantiate (playerOne, currentPos, Quaternion.identity);

		//function that assign walls to cells
		createCells ();

	}

	void createCells(){

		//an array that will keep track of the walls
		GameObject[] allWalls;

		//get the number of walls
		int numWalls = wallHolder.transform.childCount;

		//initialize size of the array
		allWalls = new GameObject[numWalls];

		//initialize the cells array to the amount of cells in the maze
		cells = new Cell[colSize * rowSize];

		//populate the walls array
		for (int i = 0; i < numWalls; i++) {
			allWalls [i] = wallHolder.transform.GetChild (i).gameObject;
		}

		//calculate the pattern for which wall direction it would be
		int southWall = (colSize + 1) * colSize;
		int northWall = ((colSize + 1) * rowSize) + colSize;
		int rowNum = 0;

		//assigns walls to the cells and number the cells
		for (int i = 0; i < cells.Length; i++) {

			cells [i] = new Cell ();

			if (i % colSize == 0 && i != 0) {
				rowNum++;
			}
				
			cells[i].north = allWalls[i + northWall];
			cells[i].south = allWalls[i + southWall];
			cells[i].east = allWalls[i + rowNum + 1];
			cells[i].west = allWalls[i + rowNum];
		}﻿

		//after the cells have been created, now transform grid into maze
		createMaze(); 

	}

	//creates the maze using Depth First Search algorithm
	void createMaze(){

		traveledCells = new Stack<int>();
		traveledCells.Clear();

		int totalCells = colSize * rowSize;
		int numCellsVisited = 0;

		int currentCell = 0;
		int currentNeighbour = 0;

		//start from cell 0
		//(the cells start from 0 at bottom left and goes to top right)
		currentCell = 0;
		cells [currentCell].visited = true;
		numCellsVisited++;

		while (numCellsVisited < totalCells){

				//function that finds the neighbour cell to travel to
				int wallToBreak = findNeighbour (ref currentCell, ref currentNeighbour);

				if (cells [currentNeighbour].visited == false && cells [currentCell].visited == true) {

					//function to break the wall between current and neighbour
					breakWall (wallToBreak, currentCell);
					cells [currentNeighbour].visited = true;
					numCellsVisited++;
					traveledCells.Push (currentCell);
					currentCell = currentNeighbour;

				}

		}
	}

	//function that deletes the wall
	void breakWall(int wallToBreak, int currentCell){

		switch (wallToBreak) {
		case 1:
			cells [currentCell].northExists = false;
			Destroy (cells [currentCell].north, 0.0f);
			break;
		case 2:
			cells [currentCell].southExists = false;
			Destroy (cells [currentCell].south, 0.0f);
			break;
		case 3:
			cells [currentCell].eastExists = false;
			Destroy (cells [currentCell].east, 0.0f);
			break;
		case 4:
			cells [currentCell].westExists = false;
			Destroy (cells [currentCell].west, 0.0f);
			break;

		}
	}

	int findNeighbour(ref int currentCell, ref int currentNeighbour){
		
		int totalCells = colSize * rowSize;
		int numNeighbours = 0;
		int[] neighboursCells = new int[4];
		int[] wallDirection = new int[4];


		//find north and not yet travelled neighbour
		if ((currentCell < totalCells - colSize) && !(cells[currentCell + colSize].visited)) {
			neighboursCells[numNeighbours] = currentCell + colSize;
			wallDirection [numNeighbours] = 1;
			numNeighbours++;
		}

		//find south and not yet travelled neighbour
		if ((currentCell >= colSize) && !(cells[currentCell - colSize].visited)) {	
			neighboursCells[numNeighbours] = currentCell - colSize;
			wallDirection [numNeighbours] = 2;
			numNeighbours++;
		}

		//find east and not yet travelled neighbour
		if ((currentCell + 1) % colSize != 0 && !(cells[currentCell + 1].visited)) {
			neighboursCells[numNeighbours] = currentCell + 1;
			wallDirection [numNeighbours] = 3;
			numNeighbours++;
		}

		//find west and not yet travelled neighbour
		if (currentCell % colSize != 0 && currentCell != 0 && !(cells[currentCell - 1].visited)) {
			neighboursCells[numNeighbours] = currentCell - 1;
			wallDirection [numNeighbours] = 4;
			numNeighbours++;
		}

		if (numNeighbours != 0) {
			//choose a random neigbour to go to
			int randomCell = Random.Range (0, numNeighbours);
			currentNeighbour = neighboursCells [randomCell];
			int wallToBreak = wallDirection [randomCell];
			return wallToBreak;

		} else {

			if(traveledCells.Count > 0){
				//if there are no neighbours, then start back tracking
				currentCell = traveledCells.Pop();	
			}
				
			return 0;

		}

	}

	// Update is called once per frame
	//is not used for the creation of the maze
	void Update () {

	}
}
