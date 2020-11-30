using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public Condition Condition;
  [SerializeField] public int NumActions;
  [SerializeField] public GameAction[] Actions;

  internal string RunAsSequence(string name, Chars performer) {
    string res = null;
    foreach (GameAction a in Actions) {
      if (!a.type.GoodByDefault() && !string.IsNullOrEmpty(a.msg)) res = a.msg;
    }

    GameScene scene = new GameScene("A&C from " + name, GameSceneType.SetOfActions, performer);
    foreach (GameAction a in Actions) {
      scene.startup.Add(a);
    }
    GameScenesManager.StartScene(scene);

    return res;
  }
}


