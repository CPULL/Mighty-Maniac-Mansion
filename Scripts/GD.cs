using System.IO;
using UnityEngine;

public class GD : MonoBehaviour {
  public static GD gs;
  public static GameStatus status = GameStatus.NotYetLoaded;
  public static Chars actor1;
  public static Chars actor2;
  public static Chars actor3;
  public static Chars kidnapped;

  void Awake() {
    if (gs != null) {
      Destroy(this.gameObject);
      return;
    }

    gs = this;
    DontDestroyOnLoad(this.gameObject);
  }



  public static void Log(string s) {
    string path = Path.Combine(Application.dataPath, "error.log");
    StreamWriter sw = new StreamWriter(path, append: true);
    sw.WriteLine(s);
    sw.Flush();
    sw.Close();
  }


  // Static instances
  public static CharSelection charSel;
  public static Intro intro;
  public static Controller c;
  public static Balloon b;
  public static Options opts;
  public static Confirm confirm;
  public static Sounds s;
  public RestartFrom restartFrom = RestartFrom.NotStarted;

  public enum RestartFrom { NotStarted, Intro, CharSel, Game }

  public static void Restart(GD.RestartFrom from) {
    gs.restartFrom = from;
    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
  }

  public static void ReadyToStart() {
    switch (gs.restartFrom) {
      case RestartFrom.NotStarted:
        status = GameStatus.IntroVideo;
        break;
      case RestartFrom.Intro:
        status = GameStatus.IntroVideo;
        break;
      case RestartFrom.CharSel:
        status = GameStatus.CharSelection;
        break;
      case RestartFrom.Game:
        status = GameStatus.StartGame;
        break;
    }
  }
}
