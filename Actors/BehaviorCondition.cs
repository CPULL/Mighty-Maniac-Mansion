using UnityEngine;



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

        return adist < num;
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

        return adist > num;
      }

      case BehaviorConditionType.Flag: {
        return GD.a.CheckFlag(flag, value);
      }

      case BehaviorConditionType.Timed: {
        if (toRun < System.DateTime.Now) {
          toRun = System.DateTime.Now.AddMilliseconds(num * 1000);
          return true;
        }
        return false;
      }
    }
    return false;
  }

  private System.DateTime toRun = System.DateTime.Now;
  public BehaviorConditionType type;
  public ItemEnum item;
  public Chars actor;
  public GameFlag flag;
  public FlagValue value;
  public float num;

  public override string ToString() {
    switch (type) {
      case BehaviorConditionType.ItemCollected: return item + (value == FlagValue.Yes ? "Collected" : "Not collected");
      case BehaviorConditionType.ActorInSameRoom: return actor + (value == FlagValue.Yes ? "same room" : "other room");
      case BehaviorConditionType.ActorDistanceLess: return actor + " < " + num;
      case BehaviorConditionType.ActorDistanceMore: return actor + " > " + num;
      case BehaviorConditionType.Flag: return flag + " is " + (value == FlagValue.Yes ? " yes" : ((value == FlagValue.No ? " no" : " NA")));
      case BehaviorConditionType.Timed: return "Every " + num + " secs";
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
      case BehaviorConditionType.Timed: return "Every " + distVal + " secs";
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
  Timed,
}

