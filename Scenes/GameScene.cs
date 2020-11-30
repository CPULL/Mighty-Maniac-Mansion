using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameScene {
  public string Name;
  public CutsceneID Id;
  public GameSceneType Type;
  public GameSceneStatus status = GameSceneStatus.NotRunning;
  public bool skipped = false;

  public List<Condition> globalCondition;
  public List<GameAction> startup;
  public List<GameAction> shutdown;
  public List<GameStep> steps;
  public GameAction startupaction = null;
  public int startupactionnum = -1;
  public GameAction shutdownaction = null;
  public int shutdownactionnum = -1;
  public Chars mainChar;
  public List<Chars> participants;

  public Skippable skippable = Skippable.NotSkippable;


  public GameScene(string name, GameSceneType type, Chars main) {
    Id = CutsceneID.NONE;
    Type = type;
    Name = name;
    mainChar = main;
    globalCondition = new List<Condition>();
    steps = new List<GameStep>();
    startup = new List<GameAction>();
    shutdown = new List<GameAction>();
  }

  public GameScene(string id, string name, string type, string main) {
    Name = name;
    type = type.ToLowerInvariant()+"c";
    if (type[0] == 'c') Type = GameSceneType.Cutscene;
    else if (type[0] == 'a') Type = GameSceneType.ActorBehavior;
    else if (type[0] == 'i') Type = GameSceneType.ItemAction;
    else if (type[0] == 'u') Type = GameSceneType.Unique;
    else if (type[0] == 's') Type = GameSceneType.SetOfActions;

    try {
      Id = (CutsceneID)System.Enum.Parse(typeof(CutsceneID), id, true);
    } catch (System.Exception) { 
      //Debug.LogError("Unknown GameScene ID: \"" + id + "\"");
    }

    if (!string.IsNullOrEmpty(main)) {
      try {
        mainChar = (Chars)System.Enum.Parse(typeof(Chars), main, true);
        participants = new List<Chars>() { mainChar };
      } catch (System.Exception) {
        Debug.LogError("Unknown Char: \"" + main + "\"");
      }
    }
    else {
      mainChar = Chars.None;
      participants = new List<Chars>();
    }

    globalCondition = new List<Condition>();
    steps = new List<GameStep>();
    startup = new List<GameAction>();
    shutdown = new List<GameAction>();
  }


  string lastaction = "";

  public override string ToString() {
    if (Id == CutsceneID.NONE)
      return Name + " - " + " " + status + " " + skippable.ToString().Substring(0, 4) + " " + (skipped ? "[sk]" : "") + " " + lastaction;
    else
      return Id + " - " + " " + status + " " + skippable.ToString().Substring(0, 4) + " " + (skipped ? "[sk]" : "") + " " + lastaction;
  }

  /// <summary>
  /// Check if the main conditions are satisfied
  /// </summary>
  public bool IsValid(Actor performer, Actor receiver, Item item1, Item item2, When when) {
    if (Id == CutsceneID.NONE && Type != GameSceneType.SetOfActions) return false;
    if (GameScenesManager.UniqueScenesPlaying(this)) {
      status = GameSceneStatus.NotRunning;
      return false;
    }

    bool valid = true;
    foreach (Condition c in globalCondition)
      if (!c.IsValid(performer, receiver, item1, item2, when)) {
        valid = false;
        break;
      }

    if (!valid && (status == GameSceneStatus.Running || status == GameSceneStatus.Startup) && shutdown.Count > 0) {
      status = GameSceneStatus.ShutDown;
      return true;
    }
    return valid;
  }

  internal void Shutdown(bool brutal) {
    foreach (GameAction a in startup) {
      a.Complete();
    }
    foreach(GameStep s in steps) {
      foreach (GameAction a in s.actions) {
        a.Complete();
      }
    }
    if (brutal) {
      foreach (GameAction a in shutdown) {
        a.Complete();
      }
    }
    else {
      foreach (GameAction a in shutdown) {
        a.RunAction(null, null, skipped);
        a.Complete();
      }
    }
    status = GameSceneStatus.NotRunning;
    startupaction = null;
    startupactionnum = -1;
  }

  public void Start() {
    if (!IsValid(null, null, null, null, When.Always)) return;

    skipped = Type == GameSceneType.SetOfActions;
    if (startup.Count > 0) {
      status = GameSceneStatus.Startup;
      startupaction = null;
      startupactionnum = -1;
    }
    else {
      status = GameSceneStatus.Running;
    }
  }

  public bool Skip() {
    if (skippable != Skippable.NotSkippable && !skipped) {
      skipped = true;
      return true;
    }
    return false;
  }


  public bool Run(Actor performer, Actor receiver) {
    // Are we valid?
    if (!IsValid(performer, receiver, null, null, When.Always)) {
      if (status == GameSceneStatus.NotRunning) return false;
    }

    if (status == GameSceneStatus.NotRunning) { // Startup is present, else Running *********************************************************************************
      if (startup.Count > 0)
        status = GameSceneStatus.Startup;
      else
        status = GameSceneStatus.Running;
      return true;

    }
    else if (status == GameSceneStatus.Startup) { // Run Startup and then go in Running *********************************************************************************
      if (startupaction == null) {
        startupaction = startup[0];
        startupaction.running = Running.NotStarted;
        startupactionnum++;
      }

      if (startupaction.running == Running.NotStarted) { // Start the action
        startupaction.RunAction(performer, receiver, skipped);
        lastaction = startupaction.ToString();
      }
      else if (startupaction.running == Running.Running) { // Wait it to complete
        startupaction.CheckTime(Time.deltaTime);
        lastaction = startupaction.ToString();
      }
      else if (startupaction.running == Running.WaitingToCompleteAsync) { // Wait it to complete
      }
      else if (startupaction.running == Running.Completed) { // Get the next
        startupactionnum++;
        if (startupactionnum < startup.Count) {
          startupaction = startup[startupactionnum];
          startupaction.running = Running.NotStarted;
        }
        else
          status = GameSceneStatus.Running;
      }
      return true;

    }
    else if (status == GameSceneStatus.Running) { // Run until we have actions and we are valid *********************************************************************************
      bool atLeastOne = false;
      skippable = Skippable.NotSkippable;
      foreach (GameStep gs in steps) {
        bool run = gs.Run(this, performer, receiver, skipped);
        atLeastOne |= run;
        if (run) {
          lastaction = gs.ToString();

          skippable = gs.skippable;
          if (skippable == Skippable.NotSkippable) skipped = false;
        }
      }

      if (!atLeastOne) {
        if (GameScenesManager.SceneRunningWithMe(this, mainChar)) {
          GameScenesManager.RemoveScene(this);
          status = GameSceneStatus.NotRunning;
          return false;
        }
        if (shutdown.Count > 0) {
          shutdownaction = null;
          shutdownactionnum = -1;
          status = GameSceneStatus.ShutDown;
          return true;
        }
        return false;
      }
      return true;

    }
    else if (status == GameSceneStatus.ShutDown) { // Just shutdown until completed *********************************************************************************

      if (shutdown.Count == 0) {
        status = GameSceneStatus.NotRunning;
        GameScenesManager.RemoveScene(this);
        return false;
      }
      if (shutdown.Count > 0) {
        if (shutdownaction == null) {
          shutdownaction = shutdown[0];
          shutdownaction.running = Running.NotStarted;
          shutdownactionnum++;
        }

        if (shutdownaction.running == Running.NotStarted) { // Start the action
          shutdownaction.RunAction(performer, receiver, skipped);
        }
        else if (shutdownaction.running == Running.Running) { // Wait it to complete
          shutdownaction.CheckTime(Time.deltaTime);
        }
        else if (shutdownaction.running == Running.WaitingToCompleteAsync) { // Wait it to complete
        }
        else if (shutdownaction.running == Running.Completed) { // Get the next
          shutdownactionnum++;
          if (shutdownactionnum < shutdown.Count) {
            shutdownaction = shutdown[shutdownactionnum];
            shutdownaction.running = Running.NotStarted;
            return true;
          }
          else {
            status = GameSceneStatus.NotRunning;
            GameScenesManager.RemoveScene(this);
            return false;
          }
        }
      }
      return true;
    }

    Debug.LogError("We should never be here");
    return false;
  }


}


public enum Skippable {
  NotSkippable = 0, Skippable = 1, Silent = 2
}

public enum GameSceneType {
  Cutscene, ActorBehavior, ItemAction, Unique, SetOfActions
}

public enum GameSceneStatus {
  NotRunning, Startup, Running, ShutDown
}


