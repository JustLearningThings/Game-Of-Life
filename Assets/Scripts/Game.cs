using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour {
	private             bool    _guiShown    = true;
	private const       int     ScreenWidth  = 64;
	private const       int     ScreenHeight = 48;
	private             float   _timer;
	private             bool    _lmbPressed;
	private             bool    _rmbPressed;
	[CanBeNull] private Cell    _prevCellDrawn;
	private readonly    Cell[,] _grid;
	private             Camera  _camera;
	private             bool    IsCameraNotNull => _camera != null;

	public  Cell        cellObject;
	public  AudioSource audioSource;
	private Canvas      _gui;

	private bool GuiShown {
		get => _guiShown;
		set {
			_gui.enabled = value;
			_guiShown                                   = value;
		}
	}

	public Game() {
		_grid = new Cell[ScreenWidth, ScreenHeight];
	}

	private void Start() {
		_camera = Camera.main;
		_gui    = GameObject.Find("GUI").GetComponent<Canvas>();

		PlaceCells();

		audioSource.mute = GameData.Muted;
	}

	private void Update() {
		UserInput();

		if (_lmbPressed || _rmbPressed) {
			Vector2 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
			int     x          = Mathf.RoundToInt(mousePoint.x);
			int     y          = Mathf.RoundToInt(mousePoint.y);

			if (
				x is >= 0 and < ScreenWidth
				&& y is >= 0 and < ScreenHeight
				&& _prevCellDrawn != _grid[x, y]
			) {
				_grid[x, y].Alive = _lmbPressed;
				_prevCellDrawn    = _grid[x, y];
			}
		}

		if (GameData.Paused) {
			return;
		}

		if (_timer >= 1.1 - GameData.GameSpeed) {
			_timer = 0;

			CountNeighbors();
			PopulationControl();
		}
		else {
			_timer += Time.deltaTime;
		}
	}

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
			GuiShown = !GuiShown;
		}
	}

	private void PlaceCells() {
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				Cell cell = Instantiate(cellObject, new Vector2(x, y), Quaternion.identity);
				cell.X     = x;
				cell.Y     = y;
				cell.Alive = false;

				_grid[x, y] = cell;
			}
		}
	}

	private void CountNeighbors() {
		int numNeighbors;
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				numNeighbors = 0;

				if (y + 1 < ScreenHeight) {
					if (_grid[x, y + 1].Alive) {
						numNeighbors++;
					}
				}

				if (x + 1 < ScreenWidth) {
					if (_grid[x + 1, y].Alive) {
						numNeighbors++;
					}
				}

				if (y - 1 >= 0) {
					if (_grid[x, y - 1].Alive) {
						numNeighbors++;
					}
				}

				if (x - 1 >= 0) {
					if (_grid[x - 1, y].Alive) {
						numNeighbors++;
					}
				}

				if (x + 1 < ScreenWidth && y + 1 < ScreenHeight) {
					if (_grid[x + 1, y + 1].Alive) {
						numNeighbors++;
					}
				}

				if (x - 1 >= 0 && y + 1 < ScreenHeight) {
					if (_grid[x - 1, y + 1].Alive) {
						numNeighbors++;
					}
				}

				if (x + 1 < ScreenWidth && y - 1 >= 0) {
					if (_grid[x + 1, y - 1].Alive) {
						numNeighbors++;
					}
				}

				if (x - 1 >= 0 && y - 1 >= 0) {
					if (_grid[x - 1, y - 1].Alive) {
						numNeighbors++;
					}
				}

				_grid[x, y].NeighborsNum = numNeighbors;
			}
		}
	}

	private void PopulationControl() {
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				if (_grid[x, y].NeighborsNum == 2 && _grid[x, y].Alive || _grid[x, y].NeighborsNum == 3) {
					_grid[x, y].Alive = true;
				}
				else {
					_grid[x, y].Alive = false;
				}
			}
		}
	}

	private static bool RandomAliveCell() => Random.Range(0, 100) > 75;
}
