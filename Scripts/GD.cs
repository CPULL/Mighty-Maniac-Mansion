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
  public Material MatNormal;
  public Material MatBackground;
  public Material MatFlashLight;
  public Material MatLightOff;
  public Material MatOutline; // Used by actors <- normal
  public Material MatLightOffOutline; // Used by actors <- Lightsoff

  void Awake() {
    if (gs != null) {
      Destroy(this.gameObject);
      return;
    }

    gs = this;
    DontDestroyOnLoad(this.gameObject);
  }



  public static Material Normal() { return gs.MatNormal; }
  public static Material Outline() { return gs.MatOutline; }
  public static Material LightOff() { return gs.MatLightOff; }
  public static Material FlashLight() { return gs.MatFlashLight; }
  public static Material LightOffOutline() { return gs.MatLightOffOutline; }


  public static void Log(string s) {
    string path = Path.Combine(Application.dataPath, "../error.log");
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

  public static void LoadGameScenes() {
    gs.LoadScenes();
  }

  private void LoadScenes() {
    string path = Application.dataPath + "/Actions/";

    string[] jsons = null;
    try {
      jsons = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
      if (jsons == null || jsons.Length == 0) {
        Log("Path for the actions and cutscene not found!");
        Log("Expected: " + path);
        Application.Quit();
      }
    } catch(System.Exception) {
      Log("Path for the actions and cutscene not found!");
      Log("Expected: " + path);
      Application.Quit();
    }


    foreach (string file in jsons) {
      try {
//FIXME        Debug.Log("GD: " + file);
        string json = File.ReadAllText(file);
        JSONNode js = JSON.Parse(json);

        GameScene seq = new GameScene(js["id"].Value, js["name"].Value, js["type"].Value, js["AppliesTo"].Value);
        if (js["condition"].IsArray) {
          JSONNode conditions = js["condition"];
          for (int i = 0; i < conditions.AsArray.Count; i++) {
            JSONNode condition = conditions[i];
            seq.globalCondition.Add(new Condition(condition["type"].Value, condition["id"].AsInt, condition["id"].Value, condition["iv"].AsInt, condition["bv"].AsBool, condition["sv"].Value, condition["fv"].AsFloat));
          }
        }

        if (js["startup"].IsArray && js["startup"].AsArray.Count > 0) {
          JSONNode startup = js["startup"];
          for (int i = 0; i < startup.AsArray.Count; i++) {
            try {
              JSONNode action = startup[i];
              Vector2 vv = Vector2.zero;
              if (action["vv"].IsArray) {
                vv.x = action["vv"][0].AsFloat;
                vv.y = action["vv"][1].AsFloat;
              }
              string a = action["type"].Value;

              float c = action["del"].AsFloat;
              string d = action["id1"].Value;
              string e = action["id2"].Value;
              string f = action["sv"].Value;
              int g = action["iv"].AsInt;
              string h = action["dv"].Value;
              seq.startup.Add(new GameAction(a, c, d, e, f, g, h, vv));
            } catch (System.Exception e) {
              Debug.Log("Action ERROR in " + file + ", startup #" + i + ": " + e.Message);
            }
          }
        }

        if (js["shutdown"].IsArray && js["shutdown"].AsArray.Count > 0) {
          JSONNode shutdown = js["shutdown"];
          for (int i = 0; i < shutdown.AsArray.Count; i++) {
            try {
              JSONNode action = shutdown[i];
              Vector2 vv = Vector2.zero;
              if (action["vv"].IsArray) {
                vv.x = action["vv"][0].AsFloat;
                vv.y = action["vv"][1].AsFloat;
              }
              string a = action["type"].Value;

              float c = action["del"].AsFloat;
              string d = action["id1"].Value;
              string e = action["id2"].Value;
              string f = action["sv"].Value;
              int g = action["iv"].AsInt;
              string h = action["dv"].Value;
              seq.shutdown.Add(new GameAction(a, c, d, e, f, g, h, vv));
            } catch (System.Exception e) {
              Debug.Log("Action ERROR in " + file + ", shutdown #" + i + ": " + e.Message);
            }
          }
        }

        if (js["sequence"].IsArray) {
          JSONNode sequence = js["sequence"];
          for (int i = 0; i < sequence.AsArray.Count; i++) {
            JSONNode jstep = sequence[i];
            GameStep step = new GameStep(jstep["name"].Value, jstep["skippable"].AsBool);

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
                try {
                  JSONNode action = actions[j];
                  Vector2 vv = Vector2.zero;
                  if (action["vv"].IsArray) {
                    vv.x = action["vv"][0].AsFloat;
                    vv.y = action["vv"][1].AsFloat;
                  }
                  string a = action["type"].Value;

                  float c = action["del"].AsFloat;
                  string d = action["id1"].Value;
                  string e = action["id2"].Value;
                  string f = action["sv"].Value;
                  int g = action["iv"].AsInt;
                  string h = action["dv"].Value;
                  GameAction ga = new GameAction(a, c, d, e, f, g, h, vv);
                  step.actions.Add(
                    ga
                  );
                } catch(System.Exception e) {
                  GD.Log("Action ERROR in " + file + ", action #" + j +": " + e.Message);
                  Debug.Log("Action ERROR in " + file + ", action #" + j +": " + e.Message);
                }
              }
            }

            seq.steps.Add(step);
          }
        }

        if (seq.Type == GameSceneType.Cutscene || seq.Type == GameSceneType.Unique || seq.Type == GameSceneType.SetOfActions) {
          a.cutscenes.Add(seq);
        }
        else if (seq.Type == GameSceneType.ActorBehavior) {
          string at = js["AppliesTo"].Value;
          Chars ch = (Chars)System.Enum.Parse(typeof(Chars), at, true);
          if (!System.Enum.IsDefined(typeof(Chars), ch)) {
            GD.Log("Unknown Actor: \"" + at + "\" in file " + file);
            Debug.LogError("Unknown Actor: \"" + at + "\" in file " + file);
            continue;
          }
          Actor actor = Controller.GetActor(ch);
          actor.behaviors.Add(seq);
        }

        if (seq.steps.Count == 0) {
          GD.Log("Scene without steps: " + file);
          Debug.LogError("Scene without steps: " + file);
        }
        else {
          for (int i = 0; i < seq.steps.Count; i++) {
            if (seq.steps[i].actions.Count == 0) {
              GD.Log("Scene with steps(" + i + ") without actions: " + file);
              Debug.LogError("Scene with steps(" + i + ") without actions: " + file);
            }
          }
        }

      } catch (System.Exception e) {
        GD.Log("Main ERROR reading " + file + ": " + e.Message);
        Debug.Log("Main ERROR reading " + file + ": " + e.Message);
        // FIXME here we need a better message
      }

    }
  }

  internal void SetMaterial(Material mat, int colors, int pixels, int outlinesize, float outlinestr, int scanlines, float slfreq, float slspeed, float slnoise, float slstr) {
    // Colors
    mat.SetFloat("_UseC64Cols", colors != 0 ? 1 : 0);
    mat.SetFloat("_UseExC64Cols", colors == 2 ? 1 : 0);

    // Pixelize
    if (pixels == 0) pixels = 0;
    else if (pixels == 1) pixels = 640;
    else if (pixels == 2) pixels = 480;
    else if (pixels == 3) pixels = 320;
    else if (pixels == 4) pixels = 256;
    else if (pixels == 5) pixels = 160;
    mat.SetFloat("_Res", pixels);

    // Outline
    mat.SetFloat("_UseOutline", outlinesize != 0 ? 1 : 0);
    mat.SetFloat("_OutlineSize", outlinesize);
    mat.SetFloat("_OutlineStrenght", outlinestr);

    // Scanlines
    mat.SetFloat("_CRT", (scanlines & 1) == 1 ? 1 : 0);
    mat.SetFloat("_CRTDir", (scanlines & 2) == 2 ? 1 : 0);
    mat.SetFloat("_CRTInternalce", (scanlines & 4) == 4 ? 1 : 0);
    mat.SetFloat("_CRTStrenght", slstr);
    mat.SetFloat("_CRTFreq", slfreq);
    mat.SetFloat("_CRTSpeed", slspeed);
    mat.SetFloat("_CRTNoise", slnoise);
  }

  internal static void SetC64Mode(int colors, int pixels, int outlinesize, float outlinestr, int scanlines, float slfreq, float slspeed, float slnoise, float slstr) {
    gs.SetMaterial(gs.MatNormal, colors, pixels, outlinesize, outlinestr, scanlines, slfreq, slspeed, slnoise, slstr);
    gs.SetMaterial(gs.MatBackground, colors, pixels, outlinesize, outlinestr, scanlines, slfreq, slspeed, slnoise, slstr);
    gs.SetMaterial(gs.MatFlashLight, colors, pixels, outlinesize, outlinestr, scanlines, slfreq, slspeed, slnoise, slstr);
    gs.SetMaterial(gs.MatLightOff, colors, pixels, outlinesize, outlinestr, scanlines, slfreq, slspeed, slnoise, slstr);
    gs.SetMaterial(gs.MatOutline, colors, pixels, outlinesize, outlinestr, scanlines, slfreq, slspeed, slnoise, slstr);
    gs.SetMaterial(gs.MatLightOffOutline, colors, pixels, outlinesize, outlinestr, scanlines, slfreq, slspeed, slnoise, slstr);

    if (c != null && c.currentRoom != null) c.currentRoom.UpdateLights();
  }

  public static bool globalLights = true;
  public static bool flashLight = false;
}
