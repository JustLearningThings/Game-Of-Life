using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour {
	private const       int     ScreenWidth  = 64;
	private const       int     ScreenHeight = 48;
	private             float   _timer;
	private             bool    _lmbPressed;
	private             bool    _rmbPressed;
	[CanBeNull] private Cell    _prevCellDrawn;
	private readonly    Cell[,] _grid;
	private             Camera  _camera;
	private             bool    IsCameraNotNull => _camera != null;

	public Cell        cellObject;
	public AudioSource audioSource;

	public Game() {
		_grid = new Cell[ScreenWidth, ScreenHeight];
	}

	private void Start() {
		_camera = Camera.main;
		PlaceCells(1);

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

		if (_timer >= GameData.GameSpeed) {
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
	}

	private void PlaceCells(int type) {
		switch (type) {
			case 1: {
				for (int y = 0; y < ScreenHeight; y++) {
					for (int x = 0; x < ScreenWidth; x++) {
						Cell cell = Instantiate(cellObject, new Vector3(x, y, 0), Quaternion.identity);

						_grid[x, y]       = cell;
						_grid[x, y].Alive = false;
					}
				}

				for (int x = 31; x < 38; x++) {
					for (int y = 21; y < 24; y++) {
						if (x != 34) {
							switch (y) {
								case 21 or 23:
								case 22 when ((x != 32) && (x != 36)):
									_grid[x, y].Alive = true;
									break;
							}
						}
					}
				}

				break;
			}
			case 2: {
				for (int x = 0; x < ScreenWidth; x++) {
					for (int y = 0; y < ScreenHeight; y++) {
						Cell cell = Instantiate(cellObject, new Vector3(x, y, 0), Quaternion.identity);

						_grid[x, y]       = cell;
						_grid[x, y].Alive = RandomAliveCell();
					}
				}

				break;
			}
		}
	}

	private void CountNeighbors() {
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				int numNeighbors = 0;

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
			for (int y = 0; y < ScreenHeight; y++)
				if (_grid[x, y].Alive) {
					if (_grid[x, y].NeighborsNum is not 2 or 3)
						_grid[x, y].Alive = false;
				}
				else if (_grid[x, y].NeighborsNum == 3) {
					_grid[x, y].Alive = true;
				}
		}
	}

	private static bool RandomAliveCell() => Random.Range(0, 100) > 75;
}
