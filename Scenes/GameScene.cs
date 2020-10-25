using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameScene {
  public string Name;
  public CutsceneID Id;
  public GameSceneType Type;

  public Running status = Running.NotStarted;

  public int stepnum = -1;
  public GameStep currentStep = null;
  public int actionnum = -1;
  public GameAction currentAction = null;

  public List<Condition> conditions;
  public List<GameStep> steps;


  /*

A behavior should run only if the condition is valid.
!The behavior should have currentstep, currentaction, stepnum, actionnum
!If a behavior was valid and it is no more, the actions that are running should be blocked   
   
  if it is valid, and nothing is running, the steps will be checked in sequence
  the first one that is valid will be run

  step run meand get the action and run it until is completed, then get the next action.
  Once completed the actions check for the step to run

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

    conditions = new List<Condition>();
    steps = new List<GameStep>();
  }

  public override string ToString() {
    return Id + " - " + Name + " (" + stepnum + "/" + actionnum + ")";
  }

  internal void Reset() {
    status = Running.NotStarted;
    currentStep = null;
    stepnum = -1;
    currentAction = null;
    actionnum = -1;
  }


  /// <summary>
  /// Check if the main conditions are satisfied
  /// </summary>
  public bool IsValid(Actor performer, Actor receiver, Item item1, Item item2, When when) {
    foreach (Condition c in conditions)
      if (!c.IsValid(performer, receiver, item1, item2, when, stepnum)) {
        currentStep = null;
        stepnum = -1;
        if (currentAction != null) {
          currentAction.running = Running.NotStarted;
        }
        actionnum = -1;
        return false;
      }

    return true;
  }

  private bool RunAction(Actor performer, Actor receiver, Item item1, Item item2) {
    if (currentAction.running == Running.NotStarted) { // Start the action
      currentAction.RunAction(performer, receiver, item1, item2);
    }
    else if (currentAction.running == Running.Running) { // Wait it to complete
      currentAction.CheckTime(Time.deltaTime);
    }
    else if (currentAction.running == Running.WaitingToCompleteAsync) { // Wait it to complete
    }
    else if (currentAction.running == Running.Completed) { // Get the next
      if (currentAction.type == ActionType.CompleteStep) {
        actionnum = -1;
        currentAction = null;
        return !GetNextStep(performer, receiver, stepnum);
      }
      actionnum++;
      if (steps[stepnum].actions.Count > actionnum) {
        currentAction = steps[stepnum].actions[actionnum];
        currentAction.running = Running.NotStarted;
      }
      else
        return true;
    }
    return false;
  }

  private bool GetNextStep(Actor performer, Actor receiver, int step) {
    for (int i = stepnum + 1; i < steps.Count; i++) {
      if (steps[i].IsValid(performer, receiver, step)) {
        currentStep = steps[i];
        stepnum = i;
        status = Running.Running;
        return true;
      }
    }
    for (int i = 0; i < stepnum + 1; i++) {
      if (steps[i].IsValid(performer, receiver, step)) {
        currentStep = steps[i];
        stepnum = i;
        status = Running.Running;
        return true;
      }
    }
    stepnum = -1;
    currentStep = null;
    status = Running.Completed;
    return false;
  }

  public bool Run(Actor performer, Actor receiver) {
    if (currentStep != null) {
      // Check if step is still valid
      if (currentStep.IsValid(performer, receiver, stepnum)) {
        // Do we have an action to run?
        if (currentAction != null) { // Yes
          return !RunAction(performer, receiver, null, null);
        }
        else { // No, get the first one
          actionnum = 0;
          currentAction = currentStep.actions[0];
          currentAction.running = Running.NotStarted;
        }
        return true;
      }
      else { // Not valid, stop the action if it was running, and check for thee next valid step
        if (currentAction != null) {
          currentAction.running = Running.NotStarted;
          actionnum = -1;
        }
        return GetNextStep(performer, receiver, stepnum);
      }
    }
    else { // No current step. Find one
      stepnum = -1;
      return GetNextStep(performer, receiver, stepnum);
    }
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

  public bool IsValid(Chars performer, Chars receiver, int step) {
    foreach (Condition c in conditions)
      if (!c.IsValid(performer, receiver, null, null, When.Always, step)) return false;

    return true;
  }

  public bool IsValid(Actor performer, Actor receiver, int step) {
    foreach (Condition c in conditions)
      if (!c.IsValid(performer, receiver, null, null, When.Always, step)) return false;

    return true;
  }
}


public enum GameSceneType {
  Cutscene, ActorBehavior, ItemAction
}
