using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameScene {
  public string Name;
  public CutsceneID Id;
  public GameSceneType Type;

  public Running status = Running.NotStarted;
  public int step = 0;
  public GameAction currentAction = null;

  public List<Condition> conditions;
  public List<GameStep> steps;

  public GameScene(string id, string name, string type) {
    Name = name;
    type = type.ToLowerInvariant()+"c";
    if (type[0] == 'c') Type = GameSceneType.Cutscene;
    else if (type[0] == 'a') Type = GameSceneType.ActorBehavior;
    else if (type[0] == 'i') Type = GameSceneType.ItemAction;


    Id = (CutsceneID)System.Enum.Parse(typeof(CutsceneID), id, true);
    if (!System.Enum.IsDefined(typeof(CutsceneID), Id)) {
      Debug.LogError("Unknown GameScene ID: \"" + id + "\"");
    }

    conditions = new List<Condition>();
    steps = new List<GameStep>();
  }

  public override string ToString() {
    return Id + " " + Name;
  }

  internal void Reset() {
    status = Running.NotStarted;
    currentAction = null;
    step = 0;
  }


  /// <summary>
  /// Check if the main conditions are satisfied
  /// </summary>
  public bool IsValid() {
    foreach (Condition c in conditions)
      if (!c.IsValid(Chars.None)) return false;

    return true;
  }

  public GameAction GetNext() {
    // If we have an action for a step get the next action. If none get the next valid step
    if (currentAction != null) {
      int pos = -1;
      for(int i = 0; i < steps[step].actions.Count; i++)
        if (steps[step].actions[i] == currentAction) {
          pos = i;
          break;
        }
      if (pos == -1) { // Not found, that is a problem. Run the first action of the current step, if valid. Or the first valid one after
        if (steps[step].IsValid()) {
          currentAction = steps[step].actions[0];
          return currentAction;
        }
        else {
          for (int i = step + 1; i < steps.Count; i++) {
            if (steps[i].IsValid()) {
              step = i;
              currentAction = steps[step].actions[0];
              return currentAction;
            }
          }
          // No steps are valid
          currentAction = null;
          return null;
        }
      }

      // Try next action
      if (pos + 1 < steps[step].actions.Count) {
        currentAction = steps[step].actions[pos + 1];
        return currentAction;
      }

      // Find the next valid step
      for (int i = step + 1; i < steps.Count; i++) {
        if (steps[i].IsValid()) {
          step = i;
          currentAction = steps[step].actions[0];
          return currentAction;
        }
      }
      // No steps are valid
      currentAction = null;
      return null;
    }

    // Find the first step that is valid
    for (int i = 0; i < steps.Count; i++) {
      if (steps[i].IsValid()) {
        step = i;
        currentAction = steps[step].actions[0];
        return currentAction;
      }
    }
    // No steps are valid
    currentAction = null; // Redundant, was already null
    return null;
  }



}

public class GameStep {
  public string name;
  public List<Condition> conditions;
  public List<GameAction> actions;

  public GameStep(string n) {
    name = n;
    conditions = new List<Condition>();
    actions = new List<GameAction>();
  }

  public override string ToString() {
    return name;
  }

  public bool IsValid() {
    foreach (Condition c in conditions)
      if (!c.IsValid(Chars.None)) return false;

    return true;
  }
}


public enum GameSceneType {
  Cutscene, ActorBehavior, ItemAction
}
