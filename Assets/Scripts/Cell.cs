using UnityEngine;

public class Cell : MonoBehaviour {
	private bool _alive;
	public  bool IsInSpecialZone;

	public int X;
	public int Y;

	public bool Alive {
		get => _alive;
		set {
			GetComponent<SpriteRenderer>().enabled = value;
			_alive                                 = value;
		}
	}

	public void AddState(bool state) {
		
	}

	public int NeighborsNum;
}
