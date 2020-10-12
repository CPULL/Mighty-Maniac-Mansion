using System;
using System.Collections.Generic;

[System.Serializable]
public class Behavior {

  // Conditions to have it working
  // List of conditions that are in AND, each item of the list is ORed with the other lines
  public List<BehaviorConditionLine> BehaviorConditionLines;

  // Sequence of actions
  public List<BehaviorAction> Actions;
  public int currentAction;
}


[System.Serializable]
public class BehaviorConditionLine {
  public List<BehaviorCondition> BehaviorConditions;
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
      case BehaviorConditionType.ItemCollected: return item + " is " + (value == FlagValue.Yes ? "Collected" : "Not collected");
      case BehaviorConditionType.ActorInSameRoom: return actor + " is " + (value == FlagValue.Yes ? "same room" : "other room");
      case BehaviorConditionType.ActorDistanceLess: return actor + " < " + dist;
      case BehaviorConditionType.ActorDistanceMore: return actor + " > " + dist;
      case BehaviorConditionType.Flag: return flag + " is " + (value == FlagValue.Yes ? " yes" : ((value == FlagValue.No ? " no" : " NA")));
    }
    return "undefined";
  }

  public static string CalculateName(BehaviorConditionType typeVal, Chars actorVal, ItemEnum itemVal, GameFlag flagVal, FlagValue valVal, float distVal) {
    switch (typeVal) {
      case BehaviorConditionType.ItemCollected: return itemVal + " is " + (valVal == FlagValue.Yes ? "Collected" : "Not collected");
      case BehaviorConditionType.ActorInSameRoom: return actorVal + " is " + (valVal == FlagValue.Yes ? "same room" : "other room");
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

public class BehaviorAction {
  public bool debug;


  /*
   Teleport
  Move to specific spot
  Move to actor
  Speak
  Ask (starts dialogue?)
  Expression
  Enable/Disable
  Open/Close
  Lock/Unlock
  Sound
  AnimActor
  AnimItem
  Set flag
   
   
   
   
   
   */

}