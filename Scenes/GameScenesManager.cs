using UnityEngine;

public class GameScenesManager : MonoBehaviour {

  static SList<GameScene> scenes;
  public static SceneStatus sceneStatus;



  void Awake() {
    scenes = new SList<GameScene>(16);
  }






  void Update() {
    // Here we will play the scenes that are active
    foreach (GameScene s in scenes) {
      if (s == null) continue;
      // Is the scene valid?
      if (!s.IsValid(null, null, null, null, When.Always)) {
        RemoveScene(s);
        continue;
      }
      if (s.Run()) {
        if (s.status == GameSceneStatus.ShutDown) {
          s.skipped = true;
        }
      }
      else { // Completed
        Debug.Log("Completed cutscene " + s.ToString());
        RemoveScene(s);
      }
    }

    CalculateSkippedStatus();

    // FIXME remove
    string dbg = "sceneStatus = " + sceneStatus + "\n";
    foreach (GameScene s in scenes) {
      dbg += s + "\n";
    }
//FIXME    Controller.Dbg(dbg);
  }

  public static void StartScene(GameScene scene, Actor performer = null, Actor receiver = null, Item item = null) {
    Debug.Log("--> Starting scene: " + scene);
    // Stop other scenes with the same main character
    Chars c = scene.mainChar;
    foreach (GameScene s in scenes) {
      if (s.mainChar == c) {
        scenes.Remove(s);
        scene.Shutdown(true);
      }
    }

    foreach (GameScene s in scenes) {
      if (s == scene) {
        s.performer = performer;
        s.receiver = receiver;
        s.sceneItem = item;
        s.Start();
        CalculateSkippedStatus();
        return;
      }
    }
    scene.performer = performer;
    scene.receiver = receiver;
    scene.sceneItem = item;
    scenes.Add(scene);
    scene.Start();
    CalculateSkippedStatus();
  }

  public static void StopScene(GameScene scene, bool brutal) {
    Debug.Log("--> Stopping scene: " + scene);
    foreach (GameScene s in scenes) {
      if (s == scene) {
        scenes.Remove(s);
      }
    }
    scene.Shutdown(brutal);
    CalculateSkippedStatus();
  }

  public static void RemoveScene(GameScene scene) {
    foreach (GameScene s in scenes) {
      if (s == scene) {
//FIXME        Debug.Log("--> Removing scene: " + scene);
        scenes.Remove(s);
        return;
      }
    }
    CalculateSkippedStatus();
  }
  

  public static bool IsSceneRunning(GameScene scene) {
    foreach (GameScene s in scenes) {
      if (s == scene) {
        return scene.status != GameSceneStatus.NotRunning;
      }
    }
    return false;
  }

  public static bool SkipScenes() { // Set all skippable scenes as skipped
    bool res = false;
    foreach (GameScene s in scenes)
      res = res || s.Skip();
    return res;
  }

  public static bool BlockingScene() { // Checks if a blocking scene is running
    foreach (GameScene s in scenes)
      if (s.skippable == Skippable.NotSkippable) 
        return true;
    return false;
  }

  public static bool NoUserControl() {
    return sceneStatus == SceneStatus.NonSkippableCutscene || sceneStatus == SceneStatus.SkippableCutscene;
  }

  static bool CalculateSkippedStatus() {
    if (scenes.Count == 0) {
      sceneStatus = SceneStatus.NoScenes;
      return true;
    }

    sceneStatus = SceneStatus.BackgroundScenes;
    bool allSkipped = true;
    foreach (GameScene s in scenes) {
      if (s.Type == GameSceneType.SetOfActions || s.Type == GameSceneType.ActorBehavior || s.Type == GameSceneType.ItemAction) continue;

      if (s.skippable == Skippable.NotSkippable) {
        sceneStatus = SceneStatus.NonSkippableCutscene;
        return false;
      }
      if (s.skippable == Skippable.Skippable) {
        if (!s.skipped) { 
          sceneStatus = SceneStatus.SkippableCutscene;
          allSkipped = false; 
        }
      }
      if (s.skippable == Skippable.Silent) {
        allSkipped = false;
      }
    }
    if (allSkipped) sceneStatus = SceneStatus.SkippedCutscene;

    return allSkipped;
  }



  internal static bool SceneRunningWithMe(GameScene toExclude, Chars mainChar) {
    foreach (GameScene s in scenes) {
      if (s != toExclude && s.mainChar == mainChar) return true;
    }
    return false;
  }

  internal static bool UniqueScenesPlaying(GameScene toExclude) {
    foreach (GameScene s in scenes) {
      if (s != toExclude && s.Type == GameSceneType.Unique) return true;
    }
    return false;
  }

  internal static void StopScenesForChar(Chars id) {
    foreach(GameScene s in scenes) {
      if (s.mainChar == id) {
        s.Shutdown(false);
        RemoveScene(s);
      }
    }
  }
}
