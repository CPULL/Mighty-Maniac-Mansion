using TMPro;
using UnityEngine;

public class Confirm : MonoBehaviour {
  public Canvas canvas;
  public TextMeshProUGUI Message;
  GameStatus previous;
  int mode;

  private void Awake() {
    GD.confirm = this;
  }

  public static void Show(string msg, int mode) {
    GD.confirm.Message.text = msg;
    GD.confirm.previous = GD.status;
    GD.status = GameStatus.Confirming;
    GD.confirm.canvas.enabled = true;
    GD.confirm.mode = mode;
  }

  public void Yes() {
    if (mode==0) // Quit game
      Application.Quit();
    else if (mode == 1) // Restart same chars
      GD.Restart(GD.RestartFrom.Game);
    else if (mode == 2) // Restart new chars
      GD.Restart(GD.RestartFrom.CharSel);
    else if (mode == 3) // Restart from intro
      GD.Restart(GD.RestartFrom.Intro);
  }

  public void No() {
    GD.confirm.canvas.enabled = false;
    GD.status = GD.confirm.previous;
  }
}
