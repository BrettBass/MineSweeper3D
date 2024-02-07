using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box
{
    public int Width  { get; private set; }
    public int Height { get; private set; }
    public int Depth  { get; private set; }

    //private Cell [,,] voxels;
    private Dictionary<Vector3Int, Cell> voxels = new Dictionary<Vector3Int,Cell>();
    private HashSet<Vector3Int> minePositions   = new HashSet<Vector3Int>();

    public Box(int width, int height, int depth)
    {
        Width  = width;
        Height = height;
        Depth  = depth;


    }

    public bool Surface(int x, int y, int z)
    {
        return x == 0 || x == Width  - 1 ||
               y == 0 || y == Height - 1 ||
               z == 0 || z == Depth  - 1;
    }

    private void CreateCells()
    {
        // iterate through all voxels of a cube
        // only accept voxels along cube's surface area
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                for (int k = 0; k < Depth; k++)
                    if (Surface(i,j,k))
                    {
                        Cell cell    = new Cell();
                        cell.type    = Cell.Type.Empty;
                        cell.showing = false;

                        voxels.Add(new Vector3Int(i,j,k), cell);
                    }
    }
    private void CreateMines(int numMines)
    {
        while(minePositions.Count < numMines)
        {
            Vector3Int rand = new Vector3Int(
                    Random.Range(0,Width ),
                    Random.Range(0,Height),
                    Random.Range(0,Depth));

            minePositions.Add(rand);
        }
    }
    private void CreateNumbers()
    {
        foreach (Vector3Int mine in minePositions)
        {
        }
    }


   // public Box(int height, int width, int depth)
   // {
   //     Height = height;
   //     Width  = width;
   //     Depth  = depth;

   //     voxels = new Cell[Height,Width,Depth];
   //     HashSet<(int,int,int)> nullSpace = new HashSet<(int,int,int)>();


   //     // tranform to hollow box by subtracting null space (inner box)
   //     for (int i = 1; i < height - 1; i++)
   //         for (int j = 1; j < width - 1; j++)
   //             for (int k = 1; k < depth - 1; k++)
   //                 nullSpace.Add((i,j,k));

   //     // Create solid box
   //     for (int i = 0; i < height; i++)
   //         for (int j = 0; j < width; j++)
   //             for (int k = 0; k < depth; k++)
   //                 if ( !nullSpace.Contains((i,j,k)) )
   //                     voxels[i,j,k] = new Cell();
   // }
}