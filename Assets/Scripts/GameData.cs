using System;

public static class GameData {
	public static bool  Muted     = false;
	public static float GameSpeed = 0.75f;
	public static bool  Paused    = true;

	public static string GetGameSpeedPercentage() =>
		Convert.ToInt32(GameSpeed * 100).ToString();
}
