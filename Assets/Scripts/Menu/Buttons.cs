using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {
	public AudioClip buttonPressSound;

	public void OnButtonHover() {
		AudioSource audioSource =
			gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

		audioSource.clip = buttonPressSound;
		audioSource.Play();
	}

	public void StartGame() {
		SceneManager.LoadScene("Game");
	}

	public void QuitGame() {
		Application.Quit();
	}
}
