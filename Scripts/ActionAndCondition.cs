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
  [SerializeField] public string ConditionResult;
  [SerializeField] public GameCondition Condition;
}


[System.Serializable]
public class ContextualizedAction {
  [SerializeField] public GameAction action;
  [SerializeField] public Actor performer;
  [SerializeField] public Actor secondary;
  [SerializeField] public Item item;

  internal void Complete() {
    action.Complete();
  }

  internal void Play() {
    action.Play();
  }

  internal bool NotStarted() {
    return action.NotStarted();
  }

  internal bool IsPlaying() {
    return action.IsPlaying();
  }

  internal bool IsCompleted() {
    return action.IsCompleted();
  }
}

