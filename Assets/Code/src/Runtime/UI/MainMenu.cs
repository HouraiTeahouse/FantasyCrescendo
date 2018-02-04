using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
		
public class MainMenu : MonoBehaviour {

  public GameMode CurrentGameMode;

  public void SetGameMode(GameMode gameMode) => CurrentGameMode = gameMode;

  public void QuitGame() => Application.Quit();

  public void OpenUrl(string url) => Application.OpenURL(url);

}

}