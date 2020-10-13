

using UnityEngine;

[System.Serializable]
public class Behavior {

  // Conditions to have it working
  // List of conditions that are in AND, each item of the list is ORed with the other lines
  public static readonly int INITIAL_SIZE = 1;
  public BehaviorConditionLine[] ConditionsInOr;
  public BehaviorAction[] Actions;

  public Behavior() {
    ConditionsInOr = new BehaviorConditionLine[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      ConditionsInOr[i] = new BehaviorConditionLine();
    }
    Actions = new BehaviorAction[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      Actions[i] = new BehaviorAction();
    }
  }

}


[System.Serializable]
public class BehaviorConditionLine {
  public static readonly int INITIAL_SIZE = 1;
  public string name;
  public BehaviorCondition[] ConditionsInAnd;

  public BehaviorConditionLine() {
    ConditionsInAnd = new BehaviorCondition[INITIAL_SIZE];
    for (int i = 0; i < INITIAL_SIZE; i++) {
      ConditionsInAnd[i] = new BehaviorCondition();
    }
  }

  public override string ToString() {
    if (ConditionsInAnd == null || ConditionsInAnd.Length == 0) return "<none>";
    string res = "";
    foreach (BehaviorCondition bc in ConditionsInAnd)
      res += bc.ToString() + "|";
    return res;
  }
}

  [System.Serializable]
public class BehaviorCondition {
  /*
      Item collected        | item |       |      | value |     
      Actor in same room    |      | actor |      | value |     
      Flag                  |      |       | flag | value |     
      Distance of actor     |      | actor |      |       | dist
   */

  public BehaviorConditionType type;
  public ItemEnum item;
  public Chars actor;
  public GameFlag flag;
  public FlagValue value;
  public float dist;

  public override string ToString() {
    switch (type) {
      case BehaviorConditionType.ItemCollected: return item + (value == FlagValue.Yes ? "Collected" : "Not collected");
      case BehaviorConditionType.ActorInSameRoom: return actor + (value == FlagValue.Yes ? "same room" : "other room");
      case BehaviorConditionType.ActorDistanceLess: return actor + " < " + dist;
      case BehaviorConditionType.ActorDistanceMore: return actor + " > " + dist;
      case BehaviorConditionType.Flag: return flag + " is " + (value == FlagValue.Yes ? " yes" : ((value == FlagValue.No ? " no" : " NA")));
    }
    return "undefined";
  }

  public static string CalculateName(BehaviorConditionType typeVal, Chars actorVal, ItemEnum itemVal, GameFlag flagVal, FlagValue valVal, float distVal) {
    switch (typeVal) {
      case BehaviorConditionType.ItemCollected: return itemVal + (valVal == FlagValue.Yes ? "Collected" : "Not collected");
      case BehaviorConditionType.ActorInSameRoom: return actorVal + (valVal == FlagValue.Yes ? "same room" : "other room");
      case BehaviorConditionType.ActorDistanceLess: return actorVal + " < " + distVal;
      case BehaviorConditionType.ActorDistanceMore: return actorVal + " > " + distVal;
      case BehaviorConditionType.Flag: return flagVal + " is " + (valVal == FlagValue.Yes ? " yes" : ((valVal == FlagValue.No ? " no" : " NA")));
    }
    return "undefined";

  }
}

public enum BehaviorConditionType {
  ItemCollected,
  ActorInSameRoom,
  ActorDistanceLess,
  ActorDistanceMore,
  Flag,
}

[System.Serializable]
public class BehaviorAction {
  public string name;
  public BehaviorActionType type;
  public Vector3 pos;
  public string str; // room | text
  public int val1; // actor, item, sound, flag
  public int val2; // expr, val, item
  

  /*
   * 



                            | v3  | str  | int   | str  | int  | int | int  | int   | int
    Teleport                | pos | room |
    Move to specific spot   | pos | room |
    Move to actor           |     | room | actor |
    Speak                   |     |      | actor | text |
    Ask (starts dialogue?)  |     |      | actor | text |
    Expression              |     |      | actor |      | expr |
    Enable/Disable          |     |      |       |      |      | Item | val |
    Open/Close              |     |      |       |      |      | Item | val |
    Lock/Unlock             |     |      |       |      |      | Item | val |
    Sound                   |     |      |       |      |      |      |     | sound |
    AnimActor               |     |      | actor | anim |
    AnimItem                |     |      |       | anim |      | Item |     |
    Set flag                |     |      |       |      |      |      | val |       | flag |
    Give                    |     |      | actor |      |      | Item |     |       |      |
   
   
   
   
   
   */

}

public enum BehaviorActionType {
  Teleport,
  MoveToSpecificSpot,
  MoveToActor,
  Speak,
  Ask,
  Expression,
  EnableDisable,
  OpenClose,
  LockUnlock,
  Sound,
  AnimActor,
  AnimItem,
  SetFlag,
}