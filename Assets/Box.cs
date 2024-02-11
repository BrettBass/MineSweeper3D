using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Box
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Depth { get; private set; }

    //private Cell [,,] voxels;
    public Dictionary<Vector3Int, Cell> voxels = new Dictionary<Vector3Int, Cell>();
    private HashSet<Vector3Int> minePositions = new HashSet<Vector3Int>();



    // Define offsets for neighboring cells in each direction, including diagonals
    private static readonly Vector3Int[] offsets = new Vector3Int[]
    {
        // Neighbors in X and Y directions
        new Vector3Int(-1, 0, 0),   // Negative X
        new Vector3Int(1, 0, 0),    // Positive X
        new Vector3Int(0, -1, 0),   // Negative Y
        new Vector3Int(0, 1, 0),    // Positive Y
        // Diagonal neighbors in X and Y directions
        new Vector3Int(-1, -1, 0),  // Diagonal: Negative X, Negative Y
        new Vector3Int(-1, 1, 0),   // Diagonal: Negative X, Positive Y
        new Vector3Int(1, -1, 0),   // Diagonal: Positive X, Negative Y
        new Vector3Int(1, 1, 0),    // Diagonal: Positive X, Positive Y
        // Neighbors in Z direction
        new Vector3Int(0, 0, -1),   // Negative Z
        new Vector3Int(0, 0, 1),    // Positive Z
        // Diagonal neighbors in Z direction
        new Vector3Int(0, -1, -1),  // Diagonal: Negative Z, Negative Y
        new Vector3Int(0, -1, 1),   // Diagonal: Negative Z, Positive Y
        new Vector3Int(0, 1, -1),   // Diagonal: Positive Z, Negative Y
        new Vector3Int(0, 1, 1),    // Diagonal: Positive Z, Positive Y
        // Diagonal neighbors in X and Z directions
        new Vector3Int(-1, 0, -1),  // Diagonal: Negative X, Negative Z
        new Vector3Int(-1, 0, 1),   // Diagonal: Negative X, Positive Z
        new Vector3Int(1, 0, -1),   // Diagonal: Positive X, Negative Z
        new Vector3Int(1, 0, 1),    // Diagonal: Positive X, Positive Z
        //xyz combinations
        new Vector3Int(-1, -1, -1), // Negative X, Negative Y, Negative Z
        new Vector3Int(-1, -1, 1),  // Negative X, Negative Y, Positive Z
        new Vector3Int(-1, 1, -1),  // Negative X, Positive Y, Negative Z
        new Vector3Int(-1, 1, 1),   // Negative X, Positive Y, Positive Z
        new Vector3Int(1, -1, -1),  // Positive X, Negative Y, Negative Z
        new Vector3Int(1, -1, 1),   // Positive X, Negative Y, Positive Z
        new Vector3Int(1, 1, -1),   // Positive X, Positive Y, Negative Z
        new Vector3Int(1, 1, 1),    // Positive X, Positive Y, Positive Z
    };

    public Box(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;


        CreateCells();
        CreateMines((int)(0.3f * voxels.Count));
        CreateNumbers();
    }

    public bool Surface(int x, int y, int z)
    {
        return x == 0 || x == Width - 1 ||
               y == 0 || y == Height - 1 ||
               z == 0 || z == Depth - 1;
    }

    private void CreateCells()
    {
        // iterate through all voxels of a cube
        // only accept voxels along cube's surface area
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                for (int k = 0; k < Depth; k++)
                    if (Surface(i, j, k))
                    {
                        Cell cell = new Cell();
                        cell.type = Cell.Type.Empty;
                        cell.showing = false;

                        voxels.Add(new Vector3Int(i, j, k), cell);
                    }
    }
    private void CreateMines(int numMines)
    {
        while (minePositions.Count < numMines)
        {
            Vector3Int rand = new Vector3Int(
                    Random.Range(0, Width),
                    Random.Range(0, Height),
                    Random.Range(0, Depth));

            if (Surface(rand.x, rand.y, rand.z) && voxels.ContainsKey(rand))
            {
                minePositions.Add(rand);
                Debug.Log($"Added Min at POSITION: {rand}");
                voxels[rand].type = Cell.Type.Mine;
            }
        }
    }
    private void CreateNumbers()
    {
        Debug.Assert(minePositions.Count != 0);
        foreach (Vector3Int mine in minePositions)
        {
            foreach (Vector3Int neighbor in GetNeighbors(mine))
            {
                if (voxels[neighbor].type == Cell.Type.Empty)
                    voxels[neighbor].type = Cell.Type.Number;

                voxels[neighbor].num++;
                Debug.Log($"VOXEL NUMBER: {voxels[neighbor].num}");
            }
        }
    }
    // max possible neighbors = 8
    //
    public List<Vector3Int> GetNeighbors(Vector3Int currentPosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        bool[] planes = CheckPlanes(currentPosition);

        foreach (Vector3Int offset in offsets)
        {
            Vector3Int potentialNeighbor = currentPosition + offset;
            bool[] neihborsPlanes = CheckPlanes(potentialNeighbor);
            for (int i = 0; i < planes.Length; i++)
            {
                if (planes[i] && neihborsPlanes[i])
                {
                    if (voxels.ContainsKey(potentialNeighbor))
                    {
                        neighbors.Add(potentialNeighbor);
                        break;
                    }
                }
            }
        }
        return neighbors;
    }

    private bool[] CheckPlanes(Vector3Int voxel)
    {
        return new bool[]{voxel.x == 0, voxel.x == Width  - 1,
                          voxel.y == 0, voxel.y == Height - 1,
                          voxel.z == 0, voxel.z == Depth  - 1};
    }
}
