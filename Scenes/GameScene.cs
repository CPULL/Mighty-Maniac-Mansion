using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameScene {
  public string Name;
  public CutsceneID Id;
  public GameSceneType Type;

  public List<Condition> globalCondition;
  public List<GameAction> startup;
  public List<GameAction> shutdown;
  public List<GameStep> steps;
  public bool AmIActive = false;
  public GameAction startupaction = null;
  public int startupactionnum = -1;
  public GameAction shutdownaction = null;
  public int shutdownactionnum = -1;


  /*
   * Global condition:
   *    if switched to true, run the setup actions
   *    if switched to false, run the shutdown actions
   * 
   * Run all steps in parallel, all the time their condition is true. Stop their action in case the condition become false
   * 
   */


  public GameScene(string id, string name, string type) {
    Name = name;
    type = type.ToLowerInvariant()+"c";
    if (type[0] == 'c') Type = GameSceneType.Cutscene;
    else if (type[0] == 'a') Type = GameSceneType.ActorBehavior;
    else if (type[0] == 'i') Type = GameSceneType.ItemAction;

    try {
      Id = (CutsceneID)System.Enum.Parse(typeof(CutsceneID), id, true);
    } catch (System.Exception) { 
      Debug.LogError("Unknown GameScene ID: \"" + id + "\"");
    }

    globalCondition = new List<Condition>();
    steps = new List<GameStep>();
    startup = new List<GameAction>();
    shutdown = new List<GameAction>();
  }

  public override string ToString() {
    return Id + " - " + Name + (AmIActive ? " (running)": "");
  }

  internal void Reset() {
    if (AmIActive)
      PlayActions(shutdown);
    AmIActive = false;
    startupaction = null;
    startupactionnum = -1;
  }


  /// <summary>
  /// Check if the main conditions are satisfied
  /// </summary>
  public bool IsValid(Actor performer, Actor receiver, Item item1, Item item2, When when) {
    bool valid = true;
    foreach (Condition c in globalCondition)
      if (!c.IsValid(performer, receiver, item1, item2, when)) {
        valid = false;
        break;
      }

    if (!valid && AmIActive) {
      PlayActions(shutdown);
      AmIActive = false;
    }

    return valid;
  }

  private void PlayActions(List<GameAction> actions) {
    foreach (GameAction a in actions) {
      a.RunAction(null, null, null, null);
      a.Complete();
    }
  }


  public bool Run(Actor performer, Actor receiver) {
    // Are we valid?
    if (!IsValid(performer, receiver, null, null, When.Always)) return false;


    if (!AmIActive) {
      shutdownaction = null;
      shutdownactionnum = -1;
      if (startup.Count == 0) {
        AmIActive = true;
      }
      if (startup.Count > 0) {
        if (startupaction == null) {
          startupaction = startup[0];
          startupaction.running = Running.NotStarted;
          startupactionnum++;
        }

        if (startupaction.running == Running.NotStarted) { // Start the action
          startupaction.RunAction(performer, receiver, null, null);
        }
        else if (startupaction.running == Running.Running) { // Wait it to complete
          if (startupaction.type == ActionType.WalkToPos || startupaction.type == ActionType.WalkToActor) {
            if (performer != null && !performer.IsWalking()) performer.RestoreWalking();
          }
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
            AmIActive = true;
          return true;
        }
      }
      return true;
    }


    // Check all the behaviros that are valid and run all of them
    bool atLeastOne = false;
    foreach (GameStep gs in steps)
      atLeastOne |= gs.Run(performer, receiver, null, null);

    if (!atLeastOne && AmIActive) {
      if (shutdown.Count == 0) {
        AmIActive = false;
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
          if (shutdownaction.type == ActionType.WalkToPos || startupaction.type == ActionType.WalkToActor) {
            if (performer != null && !performer.IsWalking()) performer.RestoreWalking();
          }
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
            AmIActive = false;
            return false;
          }
        }
      }
      return true;
    }

    return atLeastOne;
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

  internal bool Run(Actor performer, Actor receiver, object p1, object p2) {
    if (!IsValid(performer, receiver)) {
      // Stop the actions in case they were running
      if (currentAction != null) currentAction.Stop();
      currentAction = null;
      actionnum = -1;
      return false;
    }

    if (currentAction == null) {
      currentAction = actions[0];
      actionnum = 0;
    }

    if (currentAction.running == Running.NotStarted) { // Start the action
      currentAction.RunAction(performer, receiver, null, null);
    }
    else if (currentAction.running == Running.Running) { // Wait it to complete
      if (currentAction.type == ActionType.WalkToPos || currentAction.type == ActionType.WalkToActor) {
        if (!performer.IsWalking()) performer.RestoreWalking();
      }
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
  Cutscene, ActorBehavior, ItemAction
}
