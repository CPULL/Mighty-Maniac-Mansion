using UnityEngine;

[System.Serializable]
public class Behavior {

  public string name;
  public GameAction currentAction = null;

  // Conditions to have it working
  // List of conditions that are in AND, each item of the list is ORed with the other lines
  public static readonly int INITIAL_SIZE = 1;
  public BehaviorConditionLine[] ConditionsInOr;
  public GameAction[] Actions;

  public Behavior() {
    ConditionsInOr = new BehaviorConditionLine[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      ConditionsInOr[i] = new BehaviorConditionLine();
    }
    Actions = new GameAction[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      Actions[i] = new GameAction();
    }
  }

  internal bool IsValid(Actor caller) {
    foreach(BehaviorConditionLine bcl in ConditionsInOr) {
      if (bcl.IsValid(caller)) return true;
    }

    return false;
  }

  internal GameAction GetNextAction(GameAction currentAction) {
    if (currentAction == null) {
      if (Actions == null || Actions.Length == 0) return null;
      Actions[0].running = Running.NotStarted;
      return Actions[0];
    }
    for (int i = 0; i < Actions.Length - 1; i++) {
      if (Actions[i] == currentAction) return Actions[i + 1];
    }
    return null;
  }

  internal void CheckActions() {
    if (currentAction == null || currentAction.type == ActionType.None) {
      currentAction = Actions[0];
    }

    // Check the status of the action. In case is completed grab the next one
    if (currentAction.running == Running.Completed) {
      for (int i = 0; i < Actions.Length; i++) {
        if (Actions[i] == currentAction) {
          currentAction = Actions[(i + 1) % Actions.Length];
          currentAction.running = Running.NotStarted;
          return;
        }
      }

    }

  }
}

public enum BehaviorID {
  GreenTentacleBlockingPath,
  EdnaBrowsingFridge,
  EdGettingCheese
}


