using UnityEngine;

//create a cell class to keep track of the walls that make up the cell
public class Cell{

	public bool visited = false;

	//number the walls
	public GameObject north; //1
	public GameObject south; //2
	public GameObject east;  //3
	public GameObject west; //4

	//record which walls have been destroyed, for the maze solving algorithm
	public bool northExists = true;
	public bool southExists = true;
	public bool eastExists = true;
	public bool westExists = true;

}