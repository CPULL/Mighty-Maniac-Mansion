using UnityEngine;

public class Confirm : MonoBehaviour {
  public Canvas canvas;
  GameStatus previous;

  private void Awake() {
    GD.confirm = this;
  }

  public static void Show() {
    GD.confirm.previous = GD.status;
    GD.status = GameStatus.Confirming;
    GD.confirm.canvas.enabled = true;
  }

  public void Yes() {
    Application.Quit();
  }

  public void No() {
    GD.confirm.canvas.enabled = false;
    GD.status = GD.confirm.previous;
  }
}
