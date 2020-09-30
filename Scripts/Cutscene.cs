﻿using System.Collections.Generic;

[System.Serializable]
public class Cutscene {
  private Running running = Running.NotStarted;
  private int step = 0;

  public string id;
  public string name;
  public GameCondition condition;
  public List<GameAction> actions;

  public Cutscene(string i, string n) {
    id = i.ToLowerInvariant();
    name = n;
    condition = null;
    actions = new List<GameAction>();
  }

  public bool IsCompleted() {
    return running == Running.Running;
  }

  internal void Start() {
    running = Running.Running;
  }

  internal ContextualizedAction GetNextAction() {
    if (step >= actions.Count) {
      running = Running.Running;
      return null;
    }
    GameAction a = actions[step];
    step++;
    return new ContextualizedAction { action = a, performer = Controller.GetActor(a.actor), secondary = null, item = Controller.GetItem(a.strValue) };
  }

  internal void Reset() {
    step = 0;
  }
}
