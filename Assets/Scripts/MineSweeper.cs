using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Minesweeper : MonoBehaviour
{
    #region MaterialTypes
    public Material TileUnknown;
    public Material TileFlag;
    public Material TileExploded;
    public Material TileEmpty;
    public Material TileMine;
    public Material[] NumberTiles;
    #endregion

    [SerializeField] private TextMeshProUGUI gameStatusText = null;
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private TextMeshProUGUI timerText = null;

    private readonly int HOUR = 3600;
    public float currentTimeInSeconds;
    public int Width = 16, Height = 16, Depth = 16;

    private int bufferDistance = 5;

    private VoxelGrid gameGrid;
    private Vector3 center = Vector3.zero;

    RaycastHit hitInfo;
    private int remainingCellCount;
    private bool isGameOver = false;


    private void OnEnable()
    {
        UIModifier.OnChangeSize += UIManager_OnChanngSize;
    }

    private void UpdateGameStatus(string status)
    {
        gameStatusText.text = status;
    }

    private void UIManager_OnChanngSize(int x, int y, int z)
    {
        Debug.Log("AT UI Manager");
        Width = x;
        Height = y;
        Depth = z;
        gameGrid.Cleanup();
        newGame();
    }

    private void newGame()
    {
        gameGrid = new VoxelGrid(Width, Height, Depth);
        center = new Vector3(Width / 2f, Height / 2f, Depth / 2f);
        bufferDistance = Mathf.Max(Width, Height, Depth) / 4;

        SetupCamera();

        CreateVoxelBox();
        remainingCellCount = gameGrid.voxels.Count - gameGrid.getMineCount();
        UpdateGameStatus(remainingCellCount.ToString());
        isGameOver = false;
        ResetTimer();
    }

    // Start is called before the first frame update
    void Start()
    {
        newGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingCellCount <= 0)
            HandleGameEnd("Winner", TileFlag);

        else if (!isGameOver)
        {
            UpdateTimer();

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                HandleMouseInput();
        }

        HandleCameraKeyboardInput();
    }

    void HandleGameEnd(string status, Material material)
    {
        Debug.Log(status);
        gameGrid.MarkMines(material);
        UpdateGameStatus(status);
        isGameOver = true;
    }

    void UpdateTimer()
    {
        currentTimeInSeconds += Time.deltaTime;
        if (currentTimeInSeconds >= HOUR) // an hour
        {
            ResetTimer();
        }
        DisplayTime(currentTimeInSeconds);
    }

    void HandleMouseInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 100))
        {
            Vector3Int position = Vector3Int.CeilToInt(hitInfo.transform.position);
            Cell cell = gameGrid.voxels[position].GetComponent<Cell>();
            Renderer renderer = gameGrid.voxels[position].GetComponent<Renderer>();

            if (Input.GetMouseButtonUp(0))
                HandleLeftClick(cell, renderer, position);

            else if (Input.GetMouseButtonUp(1))
                HandleRightClick(cell, renderer);

        }
    }

    void HandleLeftClick(Cell cell, Renderer renderer, Vector3Int position)
    {
        if (!cell.showing && !cell.flagged)
        {
            if (cell.type == Cell.Type.Mine)
                HandleGameEnd("Loser", TileExploded);

            else
                Reveal(position);
        }
    }

    void HandleRightClick(Cell cell, Renderer renderer)
    {
        if (renderer.sharedMaterial == TileUnknown)
            renderer.material = TileFlag;

        else if (renderer.sharedMaterial == TileFlag)
            renderer.material = TileUnknown;

        cell.flagged = !cell.flagged;
    }

    private void SetupCamera()
    {
        mainCamera.transform.rotation = Quaternion.LookRotation(new Vector3Int(0, 0, 0));
        mainCamera.transform.position = new Vector3(Width / 2f - bufferDistance, Height / 2f, -1 * Mathf.Sqrt(Mathf.Pow(Width, 2) + Mathf.Pow(Height, 2)));
    }

    public void Reveal(Vector3Int voxel)
    {
        Renderer renderer = gameGrid.voxels[voxel].GetComponent<Renderer>();
        Cell cell = gameGrid.voxels[voxel].GetComponent<Cell>();

        if (cell.type == Cell.Type.Empty)
            RecursiveReveal(voxel);

        if (!cell.showing)
            remainingCellCount--;

        cell.showing = true;

        // Change material to display cell value
        hitInfo.transform.GetComponent<Renderer>().material = GetCube(cell);

        // Update how many cells are left
        UpdateGameStatus(remainingCellCount.ToString());
    }

    private void RecursiveReveal(Vector3Int voxel)
    {
        Cell cell = gameGrid.voxels[voxel].GetComponent<Cell>();

        if (cell.showing || cell.type == Cell.Type.Mine) return;
        //if (cell.type == Cell.Type.Mine) return;

        Renderer renderer = gameGrid.voxels[voxel].GetComponent<Renderer>();

        cell.showing = true;
        remainingCellCount--;
        renderer.material = GetCube(cell);

        if (cell.type == Cell.Type.Empty)
        {
            foreach (var neighbor in gameGrid.GetNeighbors(voxel))
                RecursiveReveal(neighbor);
        }

    }

    void CreateVoxelBox()
    {
        foreach (KeyValuePair<Vector3Int, GameObject> currentVoxel in gameGrid.voxels)
            currentVoxel.Value.transform.GetComponent<Renderer>().material = GetCube(currentVoxel.Value.GetComponent<Cell>());
    }

    private Material GetCube(Cell cell)
    {
        if (cell.showing) return GetTileType(cell);
        if (cell.flagged) return TileFlag;

        return TileUnknown;

    }

    private Material GetTileType(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Mine: return TileMine;
            case Cell.Type.Empty: return TileEmpty;
            case Cell.Type.Number: return GetTileNumber(cell);

            default: return null;
        }
    }

    private Material GetTileNumber(Cell cell)
    {
        if (cell.num >= 1 && cell.num <= 8)
            return NumberTiles[cell.num - 1]; // Adjust index to match array

        return null;
    }

    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ResetTimer()
    {
        currentTimeInSeconds = 0;
    }

    private void HandleCameraKeyboardInput()
    {
        if (Input.GetKey(KeyCode.W))
            RotateCamera(Vector3.right);

        else if (Input.GetKey(KeyCode.S))
            RotateCamera(Vector3.left);

        else if (Input.GetKey(KeyCode.A))
            RotateCamera(Vector3.up);

        else if (Input.GetKey(KeyCode.D))
            RotateCamera(Vector3.down);

        else if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        else if (Input.GetKey(KeyCode.Space))
            SetupCamera();

    }

    private void RotateCamera(Vector3 direction)
    {
        mainCamera.transform.RotateAround(center, direction, 60 * Time.deltaTime);
    }

}
