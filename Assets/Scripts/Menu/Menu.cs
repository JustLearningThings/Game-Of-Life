using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	private GameObject      _gameSpeedSlider;
	private TextMeshProUGUI _gameSpeedText;

	public AudioSource audioSource;

	private void Start() {
		_gameSpeedSlider = GameObject.Find("Game Speed Slider");
		_gameSpeedText = GameObject
		                 .Find("Game Speed")
		                 .GetComponent<TextMeshProUGUI>();

		_gameSpeedText.text = GameData.GetGameSpeedPercentage();

		Slider slider = _gameSpeedSlider
			.GetComponent<Slider>();

		slider.value = GameData.GameSpeed;

		slider
			.onValueChanged
			.AddListener(value => GameData.GameSpeed = value);
	}

	private void Update() {
		_gameSpeedText.text = GameData.GetGameSpeedPercentage();

		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	public void ToggleMute() {
		audioSource.mute = GameData.Muted = !GameData.Muted;
	}
}
