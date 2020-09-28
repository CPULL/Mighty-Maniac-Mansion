using UnityEngine;

public class GameData : MonoBehaviour {
  public static GameData gs;
  public static GameStatus status = GameStatus.Cutscene;
  public static Chars actor1;
  public static Chars actor2;
  public static Chars actor3;
  public static Chars kidnapped;


  void Awake() {
    if (gs != null && gs != this)
      DontDestroyOnLoad(this.gameObject);

    gs = this;
    DontDestroyOnLoad(this.gameObject);
  }
}
