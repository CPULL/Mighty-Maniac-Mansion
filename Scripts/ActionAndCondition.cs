using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public GameCondition Condition;
  [SerializeField] public GameAction Action;
  public string Name {
    get { return "[" + Condition.ToString() + "] " + Action.ToString(); }
  }
}

[System.Serializable]
public class JustCondition {
  [SerializeField] public GameCondition Condition;
  [SerializeField] public string Result;
}

