using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public Condition Condition;
  [SerializeField] public int NumActions;
  [SerializeField] public bool Blocking;
  [SerializeField] public GameAction[] Actions;

  internal string RunAsSequence(string name, Chars performer) {
    string res = null;
    foreach (GameAction a in Actions) {
      if (!a.type.GoodByDefault() && !string.IsNullOrEmpty(a.msg)) res = a.msg;
    }

    if (Actions.Length < 2) {
      Actions[0].RunAction(Controller.GetActor(performer), null, false);
      return res;
    }

    GameScene scene = new GameScene() {
      Id = CutsceneID.BackgroundSetOfActions,
      Type = Blocking ? GameSceneType.Cutscene : GameSceneType.SetOfActions,
      Name = "A&C from " + name,
      mainChar = performer
    };
    foreach (GameAction a in Actions) {
      scene.startup.Add(a);
    }
    GameScenesManager.StartScene(scene);

    return res;
  }
}


