using UnityEngine;

public class Cell : MonoBehaviour {
	private bool _alive;
	public  bool IsInSpecialZone;

	public int X;
	public int Y;

	public bool Alive {
		get => _alive;
		set => _alive = GetComponent<SpriteRenderer>().enabled = value;		
	}

	public int NeighborsNum;
}
