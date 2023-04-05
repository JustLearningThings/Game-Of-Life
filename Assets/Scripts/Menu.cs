using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public AudioSource audioSource;

	private void Update() {
		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}
	}
	
	public void StartGame() {
		SceneManager.LoadScene("Game", LoadSceneMode.Single);
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void ToggleMute() {
		audioSource.mute = StaticData.IsMuted = !StaticData.IsMuted;
	}
}
