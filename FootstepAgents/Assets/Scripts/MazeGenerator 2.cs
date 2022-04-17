using UnityEngine;
using System.Collections;

/// <summary>
/// Maze generator with density and complexity.
/// 
/// """This algorithm works by creating n (density) islands of length p (complexity). 
/// An island is created by choosing a random starting point with odd coordinates, 
/// then a random direction is chosen. If the cell two steps in the direction is free, 
/// then a wall is added at both one step and two steps in this direction. 
/// The processus is iterated for n steps for this island. p islands are created. 
/// n and p are expressed as float to apapt them to the size of the maze. 
/// With a low complexity, islands are very small and the maze is easy to solve. 
/// With low density, the maze has more "big empty rooms"."""
/// Adapted from the last python example one this page:
/// https://en.wikipedia.org/wiki/Maze_generation_algorithm
/// </summary>
public class MazeGenerator
{
	/// <summary>
	/// The maze.
	/// </summary>
	private int[,] mazey;


	/// <summary>
	/// Generate a max (maze?) given the specified width, height, complexity and density.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	/// <param name="complexity">Complexity.</param>
	/// <param name="density">Density.</param>
	public int[,] maze (int width, int height, float complexity, float density) {
		// Only odd shapes
		int[] shape = {(height / 2) * 2 + 1, (width / 2) * 2 + 1};
		// Adjust complexity and density relative to maze size
		complexity = (complexity * (5 * (shape[0] + shape[1])));
		density    = (density * (shape[0] / 2 * shape[1] / 2));

		// Build the maze
		int[,] Z = new int[shape[0],shape[1]];

		// # Fill borders
		for (int i = 0; i < shape[1]; i++) {
			Z[0,i] = Z[shape[0]-1,i] = 1;	
		}
		for (int i = 0; i < shape[0]; i++) {
			Z[i,0] = Z[i,shape[1]-1] = 1;	
		}

		// Make isles
		for (int i = 0; i < density ;i++) {
			int x = (int) Random.Range(0, shape[1] / 2) * 2;
			int y = (int) Random.Range(0, shape[0] / 2) * 2;
			Z[y, x] = 1;

			for (int j = 0; j < complexity; j++) {
				ArrayList neighbours = new ArrayList();

				if (x > 1) {
					ArrayList tempList = new ArrayList();
					tempList.Add(y);
					tempList.Add(x-2);
					neighbours.Add(tempList);
				}
				if (x < shape[1] - 2) {
					ArrayList tempList = new ArrayList();
					tempList.Add(y);
					tempList.Add(x+2);
					neighbours.Add(tempList);	
				}
				if (y > 1) {
					ArrayList tempList = new ArrayList();
					tempList.Add(y-2);
					tempList.Add(x);
					neighbours.Add(tempList);	
				}
				if (y < shape[0] - 2) {
					ArrayList tempList = new ArrayList();
					tempList.Add(y+2);
					tempList.Add(x);
					neighbours.Add(tempList);	
				}

				if (neighbours.Count > 0) {
					ArrayList tempList = (ArrayList) neighbours[Random.Range(0, neighbours.Count - 1)];
					int y_ = (int) tempList[0];
					int x_ = (int) tempList[1];

					if (Z[y_, x_] == 0) {
						Z[y_, x_] = 1;
						Z[y_ + (y - y_) / 2, x_ + (x - x_) / 2] = 1;
						x = x_;
						y = y_;
					}
				}
			}
		}

		this.mazey = Z;
		return Z;
	}


	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	/// <filterpriority>2</filterpriority>
	public override string ToString()
	{
		string sout = "[";

		for (int i = 0; i < this.mazey.GetLength(0); i++)
		{
			sout = sout + "[";
			for ( int j = 0; j < this.mazey.GetLength(1); j++)
			{
				sout = sout + this.mazey[i,j];
			}
			sout = sout + "]\n";
		}

		sout = sout + "]";
		return sout;
	}
}