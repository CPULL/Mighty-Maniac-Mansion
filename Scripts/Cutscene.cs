using System.Collections.Generic;

[System.Serializable]
public class Cutscene {
  private Running running = Running.NotStarted;
  private int step = 0;

  public string idstr;
  public CutsceneID id;
  public string name;
  public GameCondition condition;
  public List<GameAction> actions;

  public Cutscene(string i, string n) {
    idstr = i.ToLowerInvariant();
    id = (CutsceneID)System.Enum.Parse(typeof(CutsceneID), i, true);
    if (!System.Enum.IsDefined(typeof(CutsceneID), id)) {
      UnityEngine.Debug.LogError("Invalid ID for cutscene: \"" + i + "\"");
    }
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
    return new ContextualizedAction { action = a, performer = Controller.GetActor((Chars)a.actor), secondary = null };
  }

  internal void Reset() {
    step = 0;
  }
}


/// <summary>
/// List of all the actions that have an ID (mostly sequences)
/// </summary>

public enum CutsceneID {
  NONE,
  Intro,
  Doorbell,
  EdHungryCheese,
  EdnaBrowsingFridge,
  FredTalkingToKidnapped,
  Javidx9 // FIXME
}