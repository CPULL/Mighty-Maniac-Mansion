using SimpleJSON;
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
    LoadScenes();
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
  public static AllObjects a;
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



  private void LoadScenes() {
    string path = Application.dataPath + "/Actions/";

    foreach (string file in Directory.GetFiles(path, "*.json", SearchOption.AllDirectories)) {
      try {
        Debug.Log("GD: " + file);
        string json = File.ReadAllText(file);
        JSONNode js = JSON.Parse(json);


        // 	"id":"", "name":"","type":"cutscene|actor|item","AppliesTo":null,
        GameScene seq = new GameScene(js["id"].Value, js["name"].Value, js["type"].Value);
        if (js["condition"].IsArray) {
          JSONNode conditions = js["condition"];
          for (int i = 0; i < conditions.AsArray.Count; i++) {
            JSONNode condition = conditions[i];
            seq.condition.Add(new Condition(condition["type"].Value, condition["id"].AsInt, condition["id"].Value, condition["iv"].AsInt, condition["bv"].AsBool, condition["sv"].Value, condition["fv"].AsFloat));
          }
        }

        if (js["sequence"].IsArray) {
          JSONNode sequence = js["sequence"];
          for (int i = 0; i < sequence.AsArray.Count; i++) {
            JSONNode jstep = sequence[i];
            GameStep step = new GameStep(jstep["name"].Value);

            if (jstep["condition"].IsArray) {
              JSONNode conditions = jstep["condition"];
              for (int j = 0; j < conditions.AsArray.Count; j++) {
                JSONNode condition = conditions[j];
                step.conditions.Add(new Condition(condition["type"].Value, condition["id"].AsInt, condition["id"].Value, condition["iv"].AsInt, condition["bv"].AsBool, condition["sv"].Value, condition["fv"].AsFloat));
              }
            }

            if (jstep["action"].IsArray) {
              JSONNode actions = jstep["action"];
              for (int j = 0; j < actions.AsArray.Count; j++) {
                JSONNode action = actions[j];
                Vector2 vv = Vector2.zero;
                if (action["vv"].IsArray) {
                  vv.x = action["vv"][0].AsFloat;
                  vv.y = action["vv"][1].AsFloat;
                }
                step.actions.Add(
                  new GameAction(action["type"].Value, action["rep"].IsNull ? true : actions["rep"].AsBool, action["del"].AsFloat, action["id1"].Value, action["id2"].Value, action["sv"].Value, action["iv"].AsInt, action["dv"].Value, vv)
                );
              }
            }


            seq.steps.Add(step);

          }
        }

        continue;


      
      
      */
      } catch (System.Exception e) {
        Debug.Log("Main ERROR reading " + file + ": " + e.Message);
        // FIXME here we need a better message
      }
    }
  }


}
