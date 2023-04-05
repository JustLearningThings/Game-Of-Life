using UnityEngine;

public class Cell : MonoBehaviour {
	private bool _alive;

	public bool Alive {
		get => _alive;
		set {
			GetComponent<SpriteRenderer>().enabled = value;
			_alive                                 = value;
		}
	}


	public int NeighborsNum;
}
