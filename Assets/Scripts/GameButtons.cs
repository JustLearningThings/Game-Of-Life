using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour {
	private TextMeshProUGUI _gameSpeedText;
	private AudioSource     _musicSource;

	private void Start() {
		Slider slider = GameObject
		                .Find("Game Speed Slider")
		                .GetComponent<Slider>();

		slider
			.onValueChanged
			.AddListener((value) => GameData.GameSpeed = value);

		slider.value = GameData.GameSpeed;
		
		Toggle muteToggle = GameObject.Find("Mute Toggle").GetComponent<Toggle>();

		_musicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();

		muteToggle.onValueChanged.AddListener((value) => {
			GameData.Muted    = value;
			_musicSource.mute = value;
		});

		muteToggle.isOn = GameData.Muted;
		
		_gameSpeedText = GameObject.Find("Game Speed").GetComponent<TextMeshProUGUI>();
	}

	private void Update() {
		_gameSpeedText.text = Convert.ToInt32(GameData.GameSpeed * 100).ToString();
	}
}
