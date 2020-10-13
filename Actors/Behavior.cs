

using System;
using UnityEngine;

[System.Serializable]
public class Behavior {

  public BehaviorID name;

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

  internal bool IsValid(Actor caller) {
    foreach(BehaviorConditionLine bcl in ConditionsInOr) {
      if (bcl.IsValid(caller)) return true;
    }

    return false;
  }

  internal BehaviorAction GetNextAction(BehaviorAction currentAction) {
    if (currentAction == null) {
      if (Actions == null || Actions.Length == 0) return null;
      Actions[0].status = BehaviorActonStatus.NotStarted;
      return Actions[0];
    }
    for (int i = 0; i < Actions.Length - 1; i++) {
      if (Actions[i] == currentAction) return Actions[i + 1];
    }
    return null;
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

  public bool IsValid(Actor caller) {
    foreach (BehaviorCondition bc in ConditionsInAnd)
      if (!bc.IsValid(caller)) return false;
    return true;
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


  public bool IsValid(Actor caller) {
    switch (type) {
      case BehaviorConditionType.ItemCollected: return Controller.IsItemCollected(item);

      case BehaviorConditionType.ActorInSameRoom: {
        bool same;
        if (actor == Chars.Self) same = true;
        else if (actor == Chars.Player) {
          same = (Controller.GetActor(Chars.Actor1).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.Actor2).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.Actor3).currentRoom == caller.currentRoom);
        }
        else if (actor == Chars.Enemy) {
          same = (Controller.GetActor(Chars.Fred).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.Edna).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.Ed).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.Edwige).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.Ted).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.GreenTentacle).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.PurpleTentacle).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.BlueTentacle).currentRoom == caller.currentRoom) ||
            (Controller.GetActor(Chars.PurpleMeteor).currentRoom == caller.currentRoom);
        }
        else
          same = Controller.GetActor(actor).currentRoom == caller.currentRoom;

        if (value == FlagValue.No) return !same;
        return same;
      }

      case BehaviorConditionType.ActorDistanceLess: {
        float adist;
        if (actor == Chars.Self) adist = -1;
        else if (actor == Chars.Player) {
          float d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Actor1).transform.position);
          adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Actor2).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Actor3).transform.position);
          if (adist > d) adist = d;
        }
        else if (actor == Chars.Enemy) {
          float d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Fred).transform.position);
          adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Edna).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Ed).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Edwige).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Ted).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.GreenTentacle).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.PurpleTentacle).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.BlueTentacle).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.PurpleMeteor).transform.position);
          if (adist > d) adist = d;
        }
        else
          adist = Vector3.Distance(caller.transform.position, Controller.GetActor(actor).transform.position);

        return adist < dist;
      }

      case BehaviorConditionType.ActorDistanceMore: {
        float adist;
        if (actor == Chars.Self) adist = -1;
        else if (actor == Chars.Player) {
          float d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Actor1).transform.position);
          adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Actor2).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Actor3).transform.position);
          if (adist > d) adist = d;
        }
        else if (actor == Chars.Enemy) {
          float d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Fred).transform.position);
          adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Edna).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Ed).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Edwige).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.Ted).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.GreenTentacle).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.PurpleTentacle).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.BlueTentacle).transform.position);
          if (adist > d) adist = d;
          d = Vector3.Distance(caller.transform.position, Controller.GetActor(Chars.PurpleMeteor).transform.position);
          if (adist > d) adist = d;
        }
        else
          adist = Vector3.Distance(caller.transform.position, Controller.GetActor(actor).transform.position);

        return adist > dist;
      }

      case BehaviorConditionType.Flag: {
        return GD.a.CheckFlag(flag, value);
      }
    }


    return false;
  }




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
  public BehaviorActonStatus status = BehaviorActonStatus.NotStarted;
  public BehaviorActionType type;
  public Vector3 pos;
  public string str; // room | text
  public int val1; // actor, item, sound, flag
  public int val2; // expr, val, item

  internal void Start() { // FIXME check if we need it
    status = BehaviorActonStatus.Running;
    switch (type) {
      case BehaviorActionType.Teleport: 
        break;
      case BehaviorActionType.MoveToSpecificSpot:
        break;
      case BehaviorActionType.MoveToActor:
        break;
      case BehaviorActionType.Speak:
        break;
      case BehaviorActionType.Ask:
        break;
      case BehaviorActionType.Expression:
        break;
      case BehaviorActionType.EnableDisable:
        break;
      case BehaviorActionType.OpenClose:
        break;
      case BehaviorActionType.LockUnlock:
        break;
      case BehaviorActionType.Sound:
        break;
      case BehaviorActionType.AnimActor:
        break;
      case BehaviorActionType.AnimItem:
        break;
      case BehaviorActionType.SetFlag:
        break;
    }
  }

  public override string ToString() {
    string name = "FIXME";

    switch (type) {
      case BehaviorActionType.Teleport: return "Teleport " + pos;
      case BehaviorActionType.MoveToSpecificSpot: return "Move to " + pos;
      case BehaviorActionType.MoveToActor: return "Move to " + (Chars)val1;
      case BehaviorActionType.Speak: return "Say " + str + ": " + (Chars)val1;
      case BehaviorActionType.Ask: return "Ask " + str + ": " + (Chars)val1;
      case BehaviorActionType.Expression:
        break;
      case BehaviorActionType.EnableDisable:
        break;
      case BehaviorActionType.OpenClose:
        break;
      case BehaviorActionType.LockUnlock:
        break;
      case BehaviorActionType.Sound:
        break;
      case BehaviorActionType.AnimActor:
        break;
      case BehaviorActionType.AnimItem:
        break;
      case BehaviorActionType.SetFlag:
        break;
    }

    return name;
  }

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

public enum BehaviorActonStatus {
  NotStarted,
  Running,
  WaitingToCompleteAsync,
  Completed
}

public enum BehaviorID {
  GreenTentacleBlockingPath,
  EdnaBrowsingFridge,
  EdGettingCheese
}