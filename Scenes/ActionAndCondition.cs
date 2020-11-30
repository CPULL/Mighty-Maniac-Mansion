using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public Condition Condition;
  [SerializeField] public int NumActions;
  [SerializeField] public GameAction[] Actions;

}


