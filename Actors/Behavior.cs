using UnityEngine;

[System.Serializable]
public class Behavior {

  public BehaviorID name;

  // Conditions to have it working
  // List of conditions that are in AND, each item of the list is ORed with the other lines
  public static readonly int INITIAL_SIZE = 1;
  public BehaviorConditionLine[] ConditionsInOr;
  public BehaviorAction[] Actions;

  public Behavior() {
    ConditionsInOr = new BehaviorConditionLine[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      ConditionsInOr[i] = new BehaviorConditionLine();
    }
    Actions = new BehaviorAction[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      Actions[i] = new BehaviorAction();
    }
  }

  internal bool IsValid(Actor caller) {
    foreach(BehaviorConditionLine bcl in ConditionsInOr) {
      if (bcl.IsValid(caller)) return true;
    }

    return false;
  }

  internal BehaviorAction GetNextAction(BehaviorAction currentAction) {
    if (currentAction == null) {
      if (Actions == null || Actions.Length == 0) return null;
      Actions[0].status = BehaviorActonStatus.NotStarted;
      return Actions[0];
    }
    for (int i = 0; i < Actions.Length - 1; i++) {
      if (Actions[i] == currentAction) return Actions[i + 1];
    }
    return null;
  }
}

public enum BehaviorID {
  GreenTentacleBlockingPath,
  EdnaBrowsingFridge,
  EdGettingCheese
}


