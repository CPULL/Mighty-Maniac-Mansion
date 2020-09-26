using System.Collections.Generic;

[System.Serializable]
public class GameSequence {
  private Running running = Running.NotStarted;
  private int step = 0;

  public string id;
  public string name;
  public GameCondition condition;
  public List<GameAction> actions;

  public GameSequence(string i, string n) {
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
    return new ContextualizedAction { action = a, performer = null, secondary = null, item = null };
  }
}
