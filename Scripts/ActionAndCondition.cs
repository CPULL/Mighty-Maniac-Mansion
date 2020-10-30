using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public Condition Condition;
  [SerializeField] public GameAction Action;
  public string Name {
    get { return "[" + Condition.ToString() + "] " + Action.ToString(); }
  }
}


