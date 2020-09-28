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
    if (gs != null)
      Destroy(this.gameObject);

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
}
