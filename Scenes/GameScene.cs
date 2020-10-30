using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameScene {
  public string Name;
  public CutsceneID Id;
  public GameSceneType Type;
  public GameSceneStatus status = GameSceneStatus.NotRunning;

  public List<Condition> globalCondition;
  public List<GameAction> startup;
  public List<GameAction> shutdown;
  public List<GameStep> steps;
//  public bool AmIActive = false;
//  public bool AmIShuttingDown = false;
  public GameAction startupaction = null;
  public int startupactionnum = -1;
  public GameAction shutdownaction = null;
  public int shutdownactionnum = -1;
  public Chars mainChar;
  public List<Chars> participants;

  /*
   * Global condition:
   *    if switched to true, run the setup actions
   *    if switched to false, run the shutdown actions
   * 
   * Run all steps in parallel, all the time their condition is true. Stop their action in case the condition become false
   * 
   */


  public GameScene(string id, string name, string type, string main) {
    Name = name;
    type = type.ToLowerInvariant()+"c";
    if (type[0] == 'c') Type = GameSceneType.Cutscene;
    else if (type[0] == 'a') Type = GameSceneType.ActorBehavior;
    else if (type[0] == 'i') Type = GameSceneType.ItemAction;
    else if (type[0] == 'u') Type = GameSceneType.Unique;

    try {
      Id = (CutsceneID)System.Enum.Parse(typeof(CutsceneID), id, true);
    } catch (System.Exception) { 
      Debug.LogError("Unknown GameScene ID: \"" + id + "\"");
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

  public override string ToString() {
    return Id + " - " + Name + " " + status;
  }

  internal void Reset() {
    AllObjects.SetSceneAsStopped(this);
    if (status != GameSceneStatus.NotRunning) { // Play quickly all actions
      foreach (GameAction a in shutdown) {
        a.RunAction(null, null, null, null);
        a.Complete();
      }
    }
    status = GameSceneStatus.NotRunning;
    startupaction = null;
    startupactionnum = -1;
    shutdownaction = null;
    shutdownactionnum = -1;
    foreach (GameStep s in steps)
      s.Reset();
  }


  /// <summary>
  /// Check if the main conditions are satisfied
  /// </summary>
  public bool IsValid(Actor performer, Actor receiver, Item item1, Item item2, When when) {
    if (AllObjects.UniqueScenesPlaying(this)) {
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

  internal void ForceStop(bool remove = true) {
    foreach (GameAction a in startup) {
      a.Complete();
    }
    foreach (GameAction a in shutdown) {
      a.Complete();
    }
    foreach(GameStep s in steps) {
      foreach (GameAction a in s.actions) {
        a.Complete();
      }
    }
    status = GameSceneStatus.NotRunning;
    startupaction = null;
    startupactionnum = -1;
    if (remove)
      AllObjects.SetSceneAsStopped(this);
  }

  public bool Run(Actor performer, Actor receiver) {
    // Are we valid?
    if (!IsValid(performer, receiver, null, null, When.Always)) return false;

    if (Type == GameSceneType.Cutscene || Type == GameSceneType.Unique) AllObjects.SetSceneAsPlaying(this);

    if (AllObjects.UniqueScenesPlaying(null)) Controller.Dbg("Unique scene " + ToString());


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
        startupaction.RunAction(performer, receiver, null, null);
      }
      else if (startupaction.running == Running.Running) { // Wait it to complete
        startupaction.CheckTime(Time.deltaTime);
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
      foreach (GameStep gs in steps)
        atLeastOne |= gs.Run(this, performer, receiver, null, null);

      if (!atLeastOne) {
        if (AllObjects.SceneRunningWithMe(this, mainChar)) {
          AllObjects.SetSceneAsStopped(this);
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
        AllObjects.SetSceneAsStopped(this);
        return false;
      }
      if (shutdown.Count > 0) {
        if (shutdownaction == null) {
          shutdownaction = shutdown[0];
          shutdownaction.running = Running.NotStarted;
          shutdownactionnum++;
        }

        if (shutdownaction.running == Running.NotStarted) { // Start the action
          shutdownaction.RunAction(performer, receiver, null, null);
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
            AllObjects.SetSceneAsStopped(this);
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

public class GameStep {
  public string name;
  public List<Condition> conditions;
  public List<GameAction> actions;
  public GameAction currentAction;
  public int actionnum = -1;

  public GameStep(string n) {
    name = n;
    conditions = new List<Condition>();
    actions = new List<GameAction>();
    currentAction = null;
    actionnum = -1;
  }

  public override string ToString() {
    return name;
  }

  public bool IsValid(Actor performer, Actor receiver) {
    if (actions.Count == 0) return false;

    foreach (Condition c in conditions)
      if (!c.IsValid(performer, receiver, null, null, When.Always)) return false;

    return true;
  }

  internal void Reset() {
    if (currentAction != null) currentAction.Stop();
    currentAction = null;
    actionnum = -1;
  }

  internal bool Run(GameScene gameScene, Actor performer, Actor receiver, object p1, object p2) {
    if (!IsValid(performer, receiver)) {
      // Stop the actions in case they were running
      if (currentAction != null) currentAction.Stop();
      currentAction = null;
      actionnum = -1;
      return false;
    }

    if (currentAction == null) {
      currentAction = actions[0];
      currentAction.running = Running.NotStarted;
      actionnum = 0;
    }

    if (currentAction.running == Running.NotStarted) { // Start the action
      currentAction.RunAction(performer, receiver, null, null);
      if (currentAction.type == ActionType.Cutscene) {
        // Quickly stop parent scene
        gameScene.ForceStop();
      }
    }
    else if (currentAction.running == Running.Running) { // Wait it to complete
      currentAction.CheckTime(Time.deltaTime);
    }
    else if (currentAction.running == Running.WaitingToCompleteAsync) { // Wait it to complete
    }
    else if (currentAction.running == Running.Completed) { // Get the next
      if (currentAction.type == ActionType.CompleteStep) {
        if (currentAction.id2 == 0) {
          actionnum = -1;
          currentAction = null;
          return false;
        }
        else {
          actionnum = -1;
        }
      }
      actionnum++;
      if (actions.Count > actionnum) {
        currentAction = actions[actionnum];
        currentAction.running = Running.NotStarted;
      }
      else
        return false;
    }
    return true;
  }
}


public enum GameSceneType {
  Cutscene, ActorBehavior, ItemAction, Unique
}

public enum GameSceneStatus {
  NotRunning, Startup, Running, ShutDown
}


