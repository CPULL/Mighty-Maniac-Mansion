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

  public List<Condition> condition;
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

    condition = new List<Condition>();
    steps = new List<GameStep>();
  }

  public override string ToString() {
    return Id + " " + Name;
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

}


public enum GameSceneType {
  Cutscene, ActorBehavior, ItemAction
}
