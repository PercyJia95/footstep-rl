using UnityEngine;
using System.Collections;


/// <summary>
/// Maze viewer.  Constructs a maze and then Instantiates in the scene.
/// </summary>
public class MazeCreator : MonoBehaviour 
{
	/// <summary>
	/// The data folder.
	/// </summary>
	private const string DATA_FOLDER = "Data";

	/// <summary>
	/// The default name for the file (prefix).
	/// </summary>
	private const string DEFAULT_NAME = "maze";

	/// <summary>
	/// The wall tag name.
	/// </summary>
	private const string WALL_TAG = "wall";

	/// <summary>
	/// The wall prefab.
	/// </summary>
	public Transform wallPrefab;

	/// <summary>
	/// The floor.  Should be of an integer width and length (X & Z values).
	/// </summary>
	public Transform floor;


	public Transform generatedEnvironmentParent;

	/// <summary>
	/// The maze complexity.
	/// </summary>
	public float complexity;

	/// <summary>
	/// The maze density.
	/// </summary>
	public float density;

	/// <summary>
	/// The height of the wall.
	/// </summary>
	public float wallHeight;

	/// <summary>
	/// The width of the wall.
	/// </summary>
	public float wallWidth;

	/// <summary>
	/// The height ofset of the wall.
	/// </summary>
	public float heightOffset;

	private MazeGenerator mG;


	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		mG = new MazeGenerator();
		GenerateNewEnvironment ();
	}


	/// <summary>
	/// Instantiates the maze from array.
	/// This particular method draws the walls as stretched cubes of wall width by the 
	/// longest contiguous occupised cells, in two passes - one for horizontal the other for vertical.
	/// </summary>
	/// <returns>The maze from array.</returns>
	/// <param name="twoDMaze">Two D maze.</param>
	public uint InstantiateMazeFromArray(int[,] twoDMaze)
	{
		uint count = 0;
		// First pass draws vertical walls
		for(uint i = 0; i < twoDMaze.GetLength(0); i++)
		{
			for (uint j = 0; j < twoDMaze.GetLength(1) ; j++)
			{
				if ( twoDMaze[i,j] == 1 && j + 1 < twoDMaze.GetLength(1) && twoDMaze[i,j + 1] == 1)
				{
					uint startPoint = j;
					while (j < twoDMaze.GetLength (1) && twoDMaze [i, j] == 1)
						j++;

					float[] obstacle_data = new float[4];
					obstacle_data[0] = i;
					obstacle_data[1] = i + 1;
					obstacle_data[2] = startPoint;
					obstacle_data[3] = j;

					//Make a unit cube
					Transform cube = Instantiate(wallPrefab) as Transform;

					Vector3 tmp = new Vector3(); 
					tmp.x = (obstacle_data[1] - obstacle_data[0]) * wallWidth;
					tmp.y = wallHeight;
					tmp.z = (obstacle_data[3] - obstacle_data[2]) - (1f - (wallWidth));
					cube.transform.localScale = tmp;

					//Translate it
					tmp.x = (obstacle_data[0] + (obstacle_data[1] - obstacle_data[0])/2) - (floor.localScale.x / 2);
					tmp.y = wallHeight / 2f - heightOffset;
					tmp.z = (obstacle_data[2] + (obstacle_data[3] - obstacle_data[2])/2) - (floor.localScale.z / 2);
					cube.transform.localPosition = tmp;
					count++;

					cube.gameObject.name = "Wall";
					cube.gameObject.tag = "Wall";
					cube.transform.parent = generatedEnvironmentParent;
				}
			}
		}

		// Second pass draws horizontal walls
		for (uint j = 0; j < twoDMaze.GetLength (1); j++) {
			for (uint i = 0; i < twoDMaze.GetLength (0); i++) {
				if (twoDMaze [i, j] == 1 && i + 1 < twoDMaze.GetLength (1) && twoDMaze [i + 1, j] == 1) {
					uint startPoint = i;
					while (i < twoDMaze.GetLength (1) && twoDMaze [i, j] == 1)
						i++;


					float[] obstacle_data = new float[4];
					obstacle_data [0] = startPoint;
					obstacle_data [1] = i;
					obstacle_data [2] = j;
					obstacle_data [3] = j + 1;

					//Make a unit cube
					Transform cube = Instantiate (wallPrefab) as Transform;

					Vector3 tmp = new Vector3 (); 
					tmp.x = (obstacle_data [1] - obstacle_data [0]) - (1f - (wallWidth));
					tmp.y = wallHeight;
					tmp.z = (obstacle_data [3] - obstacle_data [2]) * wallWidth;
					cube.transform.localScale = tmp;

					//Translate it
					tmp.x = (obstacle_data [0] + (obstacle_data [1] - obstacle_data [0]) / 2) - (floor.localScale.x / 2);
					tmp.y = wallHeight / 2f - heightOffset;
					tmp.z = (obstacle_data [2] + (obstacle_data [3] - obstacle_data [2]) / 2) - (floor.localScale.z / 2);
					cube.transform.localPosition = tmp;
					count++;

					cube.gameObject.name = "Wall";
					cube.gameObject.tag = "Wall";
					cube.transform.parent = generatedEnvironmentParent;
				}
			}
		}

		return count;
	}


	/// <summary>
	/// Saves the maze.
	/// </summary>
	public void SaveMaze () {
		// SaveWalls saveWalls = new SaveWalls();
		// saveWalls.pathToOutput = DATA_FOLDER;            
		// saveWalls.env_name = DEFAULT_NAME;
		// saveWalls.save(GameObject.FindGameObjectsWithTag(WALL_TAG));
	}

	public void ClearEnvironment () {
		foreach(Transform child in generatedEnvironmentParent.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	public void GenerateNewEnvironment() {
		// floor.localScale = floor.localScale * 5;

		int[,] mazey = mG.maze((int)floor.localScale.x, (int)floor.localScale.z, complexity, density);

		//Debug.Log(mG.ToString());

		InstantiateMazeFromArray(mazey);
	}
}