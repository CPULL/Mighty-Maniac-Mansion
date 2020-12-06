using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public Condition Condition;
  [SerializeField] public Condition Condition2;
  [SerializeField] public int NumActions;
  [SerializeField] public bool Blocking;
  [SerializeField] public GameAction[] Actions;

  internal string RunAsSequence(Actor performer, Actor secondary, Item item, string name) {
    string res = null;
    foreach (GameAction a in Actions) {
      if (!a.type.GoodByDefault() && !string.IsNullOrEmpty(a.msg)) res = a.msg;
    }

    if (Actions.Length < 2) {
      Actions[0].RunAction(performer, secondary, false);
      return res;
    }

    GameScene scene = new GameScene() {
      Id = CutsceneID.BackgroundSetOfActions,
      Type = Blocking ? GameSceneType.Cutscene : GameSceneType.SetOfActions,
      Name = "A&C from " + name,
      mainChar = (performer == null ? Chars.None : performer.id)
    };
    foreach (GameAction a in Actions) {
      scene.startup.Add(a);
    }
    GameScenesManager.StartScene(scene, performer, secondary, item);

    return res;
  }

  internal bool IsValid(Actor actor, Actor secondary, Item item1, Item item2, When when) {
    if (Condition.type == ConditionType.None) return true;
    bool valid = Condition.IsValid(actor, secondary, item1, item2, when);
    if (!valid) return false;
    return Condition2.IsValid(actor, secondary, item1, item2, when);
  }

  internal string GetConditionMsg(Actor actor, Actor secondary, When when, Item item1, Item item2) {
    if (Condition.type == ConditionType.None) return null;
    if (!Condition.IsValid(actor, secondary, item1, item2, when) && !string.IsNullOrEmpty(Condition.msg)) return Condition.msg;
    if (!Condition2.IsValid(actor, secondary, item1, item2, when) && !string.IsNullOrEmpty(Condition2.msg)) return Condition2.msg;
    return null;
  }
}


