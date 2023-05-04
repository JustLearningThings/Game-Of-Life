using Unity.VisualScripting;
using UnityEngine;


public class Game : MonoBehaviour {
	private          bool    _guiShown    = true;
	private const    int     ScreenWidth  = 64;
	private const    int     ScreenHeight = 48;
	private          float   _timer;
	private          bool    _lmbPressed;
	private          bool    _rmbPressed;
	private readonly Cell[,] _grid;
	private          Camera  _camera;
	private          bool    IsCameraNotNull => _camera != null;

	public  AudioSource audioSource;
	private Canvas      _gui;

	private bool _isDrawingZone;
	private int  _zoneX1 = -1;
	private int  _zoneX2 = -1;

	private int _zoneY1 = -1;
	private int _zoneY2 = -1;

	public Game() {
		_grid = new Cell[ScreenWidth, ScreenHeight];
	}

	// Initialization of game
	private void Start() {
		_camera = Camera.main;
		_gui    = GameObject.Find("GUI").GetComponent<Canvas>();

		PlaceCells();

		audioSource.mute = GameData.Muted;
	}

	// Run every frame (60 fps)
	private void Update() {
		UserInput();

		if (_isDrawingZone && Input.GetMouseButtonUp((int)MouseButton.Left)) {
			Vector2 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
			int     x          = Mathf.RoundToInt(mousePoint.x);
			int     y          = Mathf.RoundToInt(mousePoint.y);

			_isDrawingZone = false;
			_zoneX2        = x;
			_zoneY2        = y;
			Debug.Log($"Game Zone created at ({_zoneX1},{_zoneY1})-({_zoneX2},{_zoneY2})");
		}

		if ((_lmbPressed || _rmbPressed) && !_guiShown) {
			Vector2 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
			int     x          = Mathf.RoundToInt(mousePoint.x);
			int     y          = Mathf.RoundToInt(mousePoint.y);

			if (_lmbPressed && Input.GetKeyDown(KeyCode.LeftShift)) {
				_zoneX1        = x;
				_zoneY1        = y;
				_isDrawingZone = true;
			}

			if (
				x is >= 0 and < ScreenWidth
				&& y is >= 0 and < ScreenHeight
			) {
				_grid[x, y].Alive = _lmbPressed;
			}
		}

		if (GameData.Paused) {
			return;
		}

		if (_timer >= 1.05 - GameData.GameSpeed) {
			_timer = 0;

			ComputeTickLogic();
		}
		else {
			_timer += Time.deltaTime;
		}
	}

	// Process keyboard and mouse events
	private void UserInput() {
		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}

		if (Input.GetMouseButtonDown((int)MouseButton.Left) && IsCameraNotNull) {
			_lmbPressed = true;
		}
		else if (Input.GetMouseButtonUp((int)MouseButton.Left) && IsCameraNotNull) {
			_lmbPressed = false;
		}

		if (Input.GetMouseButtonDown((int)MouseButton.Right) && IsCameraNotNull) {
			_rmbPressed = true;
		}
		else if (Input.GetMouseButtonUp((int)MouseButton.Right) && IsCameraNotNull) {
			_rmbPressed = false;
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			GameData.Paused = !GameData.Paused;
		}

		if (Input.GetKeyDown(KeyCode.F)) {
			_guiShown    = !_guiShown;
			_gui.enabled = _guiShown;
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			ResetCells();
		}
	}

	// Init grid
	private void PlaceCells() {
		GameObject cellObject = new() {
			transform = {
				position = Vector3.zero,
				rotation = Quaternion.identity
			},
			name = "Cell"
		};

		SpriteRenderer sr = cellObject.AddComponent<SpriteRenderer>();
		sr.sprite = Resources.Load<Sprite>("Prefabs/Cell");

		Cell cell = cellObject.AddComponent<Cell>();
		cell.X            = 0;
		cell.Y            = 0;
		cell.Alive        = false;
		cell.NeighborsNum = 0;

		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				if (x == 0 && y == 0) {
					_grid[0, 0] = cell;
				}
				else {
					_grid[x, y] = Instantiate(cell, new Vector3(x, y, 0), Quaternion.identity);
				}
			}
		}
	}

	// Check cell in x, y if in special zone
	private void CheckSpecialZone(int x, int y) {
		Cell cell = _grid[x, y];
		cell.IsInSpecialZone = (
			cell.X >= _zoneX1 && cell.X <= _zoneX2 &&
			cell.Y >= _zoneY1 && cell.Y <= _zoneY2
		);
	}

	// Count neighbours of cell at x, y
	private void CountNeighbours(int x, int y) {
		var numNeighbors = 0;

		if (y + 1 < ScreenHeight && _grid[x, y + 1].Alive) {
			numNeighbors++;
		}

		if (x + 1 < ScreenWidth && _grid[x + 1, y].Alive) {
			numNeighbors++;
		}

		if (y - 1 >= 0 && _grid[x, y - 1].Alive) {
			numNeighbors++;
		}

		if (x - 1 >= 0 && _grid[x - 1, y].Alive) {
			numNeighbors++;
		}

		if (x + 1 < ScreenWidth && y + 1 < ScreenHeight && _grid[x + 1, y + 1].Alive) {
			numNeighbors++;
		}

		if (x - 1 >= 0 && y + 1 < ScreenHeight && _grid[x - 1, y + 1].Alive) {
			numNeighbors++;
		}

		if (x + 1 < ScreenWidth && y - 1 >= 0 && _grid[x + 1, y - 1].Alive) {
			numNeighbors++;
		}

		if (x - 1 >= 0 && y - 1 >= 0 && _grid[x - 1, y - 1].Alive) {
			numNeighbors++;
		}

		_grid[x, y].NeighborsNum = numNeighbors;
	}

	private void ControlLife(int x, int y) {
		Cell cell = _grid[x, y];

		// friendly neighbourhood specialized zone (cells don't die from underpopulation)
		// if (cell.Alive && cell.NeighborsNum < 2 && cell.IsInSpecialZone) {
		// 	cell.Alive = false;
		// }

		cell.Alive = cell.NeighborsNum == 2 && cell.Alive || cell.NeighborsNum == 3;
	}

	// Check each cell if should die/revive
	private void ComputeTickLogic() {
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				CountNeighbours(x, y);
				CheckSpecialZone(x, y);
				ControlLife(x, y);
			}
		}
	}

	private void ResetCells() {
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				Cell cell = _grid[x, y];
				cell.Alive           = false;
				cell.IsInSpecialZone = false;
			}
		}

		_zoneX1 = _zoneX2 = _zoneY1 = _zoneY2 = -1;
	}
}
