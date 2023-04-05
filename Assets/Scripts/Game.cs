using UnityEngine;

public class Game : MonoBehaviour {
	private const int   ScreenWidth  = 64;
	private const int   ScreenHeight = 48;
	private       float _timer;

	public Cell  cellObject;
	public float speed = .1f;
	public bool  simulationEnabled;

	private readonly Cell[,] _grid;
	private          Camera  _camera;
	private          bool    IsCameraNotNull => _camera != null;

	public AudioSource audioSource;

	public Game() {
		_grid = new Cell[ScreenWidth, ScreenHeight];
	}

	private void Start() {
		_camera      = Camera.main;
		PlaceCells(1);

		Debug.Log(StaticData.IsMuted);
		audioSource.mute = StaticData.IsMuted;
	}

	private void Update() {
		UserInput();
		if (!simulationEnabled) {
			return;
		}

		if (_timer >= speed) {
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

		if (Input.GetMouseButtonDown(0)) {
			if (IsCameraNotNull) {
				Vector2 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
				int     x          = Mathf.RoundToInt(mousePoint.x);
				int     y          = Mathf.RoundToInt(mousePoint.y);

				if (x >= 0 && y >= 0 && x < ScreenWidth && y < ScreenHeight) {
					_grid[x, y].Alive = !_grid[x, y].Alive;
				}
			}
		}

		if (Input.GetKey(KeyCode.P)) {
			simulationEnabled = false;
		}
		else if (Input.GetKey(KeyCode.B)) {
			simulationEnabled = true;
		}
	}

	void PlaceCells(int type) {
		if (type == 1) {
			for (int y = 0; y < ScreenHeight; y++)
			for (int x = 0; x < ScreenWidth; x++) {
				Cell cell = Instantiate(cellObject, new Vector3(x, y, 0), Quaternion.identity);

				_grid[x, y]       = cell;
				_grid[x, y].Alive = false;
			}

			for (int y = 21; y < 24; y++)
			for (int x = 31; x < 38; x++) {
				if (x != 34) {
					if (y == 21 || y == 23) {
						_grid[x, y].Alive = true;
					}
					else if (y == 22 && ((x != 32) && (x != 36))) {
						_grid[x, y].Alive = true;
					}
				}
			}
		}
		else if (type == 2) {
			for (int y = 0; y < ScreenHeight; y++)
			for (int x = 0; x < ScreenWidth; x++) {
				Cell cell = Instantiate(cellObject, new Vector3(x, y, 0), Quaternion.identity);

				_grid[x, y]       = cell;
				_grid[x, y].Alive = RandomAliveCell();
			}
		}
	}

	private void CountNeighbors() {
		for (int x = 0; x < ScreenWidth; x++) {
			for (int y = 0; y < ScreenHeight; y++) {
				int numNeighbors = 0;

				if (y + 1 < ScreenHeight) {
					if (_grid[x, y + 1].Alive)
						numNeighbors++;
				}

				if (x + 1 < ScreenWidth) {
					if (_grid[x + 1, y].Alive)
						numNeighbors++;
				}

				if (y - 1 >= 0) {
					if (_grid[x, y - 1].Alive)
						numNeighbors++;
				}

				if (x - 1 >= 0) {
					if (_grid[x - 1, y].Alive)
						numNeighbors++;
				}

				if (x + 1 < ScreenWidth && y + 1 < ScreenHeight) {
					if (_grid[x + 1, y + 1].Alive)
						numNeighbors++;
				}

				if (x - 1 >= 0 && y + 1 < ScreenHeight) {
					if (_grid[x - 1, y + 1].Alive)
						numNeighbors++;
				}

				if (x + 1 < ScreenWidth && y - 1 >= 0) {
					if (_grid[x + 1, y - 1].Alive)
						numNeighbors++;
				}

				if (x - 1 >= 0 && y - 1 >= 0) {
					if (_grid[x - 1, y - 1].Alive)
						numNeighbors++;
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
