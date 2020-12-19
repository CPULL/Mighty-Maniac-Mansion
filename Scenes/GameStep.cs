using System.Collections.Generic;
using UnityEngine;

public class GameStep {
  public string name;
  public List<Condition> conditions;
  public List<GameAction> actions;
  public GameAction currentAction;
  public int actionnum = -1;
  public Skippable skippable = Skippable.NotSkippable;

  public GameStep(string n, int s) {
    name = n;
    skippable = (Skippable)s;
    conditions = new List<Condition>();
    actions = new List<GameAction>();
    currentAction = null;
    actionnum = -1;
  }

  string lastaction = "";
  public override string ToString() {
    return name + ": " + lastaction;
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

  internal bool Run(GameScene gameScene, Actor performer, Actor receiver, Item item, bool skipped) {
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
//FIXME 
      Debug.Log(ToString() + " => " + actionnum + ") " + currentAction);
    }

    if (currentAction.running == Running.NotStarted) { // Start the action
      currentAction.RunAction(performer, receiver, item, skipped);
      lastaction = currentAction.ToString();
      if (currentAction.type == ActionType.Cutscene) {
        // Quickly stop parent scene
        gameScene.Shutdown(true);
      }
    }
    else if (currentAction.running == Running.Running) { // Wait it to complete
      currentAction.CheckTime(Time.deltaTime);
      lastaction = currentAction.ToString();
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
//FIXME 
        Debug.Log(ToString() + " > " + actionnum + ") " + currentAction);
      }
      else
        return false;
    }
    return true;
  }
}

