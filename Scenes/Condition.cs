using UnityEngine;

[System.Serializable]
public class Condition {
  public ConditionType type;
  public int id;
  public int iv;
  public float fv;
  public string sv;
  public bool bv;
  public string msg;
  public When when = When.Always;

  public Condition(string stype, int idi, string ids, int siv, bool sbv, string svs, float svf) {
    type = (ConditionType)System.Enum.Parse(typeof(ConditionType), stype, true);
    if (!System.Enum.IsDefined(typeof(ConditionType), type)) {
      Debug.LogError("Unknown ConditionType: \"" + stype + "\"");
    }

    iv = siv;
    sv = svs;
    bv = sbv;
    fv = svf;

    if (!string.IsNullOrEmpty(ids)) {
      Chars ch;
      GameFlag gf;
      ItemEnum ie;

      switch (type) {
        case ConditionType.ActorIs:
        case ConditionType.ActorHasSkill:      // ID of actor and ID of skill                            (ID1, IV1, BV)
        case ConditionType.ActorInRoom:    // ID of item                                             (ID1, SV, BV)
        case ConditionType.ActorDistanceLess:  // ID and dist value                                      (ID1, FV, BV)
        case ConditionType.ActorXLess:         // ID and dist value                                      (ID1, FV, BV)
        case ConditionType.RecipientIs:        // ID of actor                                            (ID1, BV)
          if (System.Enum.TryParse<Chars>(ids, out ch)) {
            id = (int)ch;
          }
          break;

        case ConditionType.CurrentActorIs:
          if (System.Enum.TryParse<Chars>(ids, out ch)) {
            id = (int)ch;
          }
          break;

        case ConditionType.FlagValueIs:
          if (System.Enum.TryParse<GameFlag>(ids, out gf)) {
            id = (int)gf;
          }
          break;

        case ConditionType.ItemCollected:
        case ConditionType.ItemOpen:
        case ConditionType.UsedWith:
          if (System.Enum.TryParse<ItemEnum>(ids, out ie)) {
            id = (int)ie;
          }
          break;

        case ConditionType.SameRoom:
          if (System.Enum.TryParse<Chars>(ids, out ch)) {
            id = (int)ch;
          }
          if (System.Enum.TryParse<Chars>(sv, out ch)) {
            iv = (int)ch;
          }
          break;

        case ConditionType.RoomIsInExt:
          if (System.Enum.TryParse<Chars>(ids, out ch)) {
            id = (int)ch;
          }
          break;
      }
    }
    else
      id = idi;

  }

  public override string ToString() {
    return StringName(type, when, id, iv, fv, sv, bv);
  }

  public static string StringName(ConditionType type, When when, int id1, int iv1, float fv1, string sv, bool bv) {
    switch (type) {
      case ConditionType.None:
        switch (when) {
          case When.Pick: return "<Pick>";
          case When.Use: return "<Use>";
          case When.Give: return "<Give>";
          case When.Cutscene: return "<Cutscene>";
          case When.Always: return "<none>";
        }
        break;
      case ConditionType.ActorIs: return "Actor is " + (!bv ? "not " : "") + (Chars)id1;
      case ConditionType.CurrentActorIs: return "Current Actor is " + (!bv ? "not " : "") + (Chars)id1;
      case ConditionType.ActorHasSkill: return "Actor " + (Chars)id1 + " has " + (!bv ? "not skill " : "skill ") + (Skill)iv1;
      case ConditionType.CurrentRoomIs: return "Room is " + (!bv ? "not " : "") + sv;
      case ConditionType.SameRoom: return (!bv ? "Not same" : "Same") + " Room " + (Chars)id1 + " & " + (Chars)iv1;
      case ConditionType.RoomIsInExt: return " Room " + (Chars)id1 + (bv ? " is internal" : " is external");
      case ConditionType.FlagValueIs: return "Flag " + (GameFlag)id1 + (bv ? " == " : " != ") + iv1;
      case ConditionType.ItemCollected: return "Item " + (ItemEnum)id1 + (bv ? " is collected by " : " is not collected by ") + (Chars)iv1;
      case ConditionType.ActorInRoom: return "Actor " + (Chars)id1 + " is " + (!bv ? "not in " : "in ") + sv;
      case ConditionType.ActorDistanceLess: return "Actor " + (Chars)id1 + " dist " + (bv ? "< " : "> ") + fv1;
      case ConditionType.ActorXLess: return "Actor " + (Chars)id1 + " X " + (bv ? "< " : "> ") + fv1;
      case ConditionType.ItemOpen: return "Item " + (ItemEnum)id1 + (bv ? " is " : " is not ") + (iv1 == 0 ? "Open" : "Locked");
      case ConditionType.RecipientIs: return "Recipient " + (bv ? "is " : "is not ") + (Chars)id1;
      case ConditionType.WhenIs: return "When " + (bv ? "is " : "is not ") + (When)id1;
      case ConditionType.UsedWith: return "Used with " + (bv ? "" : "not ") + (ItemEnum)id1;
    }

    return "Undefined";
  }

  public bool IsValid(Chars performer, Chars receiver, ItemEnum item1, ItemEnum item2, When when) {
    Actor p = Controller.GetActor(performer);
    Actor r = Controller.GetActor(receiver);
    Item i1 = AllObjects.GetItem(item1);
    Item i2 = AllObjects.GetItem(item2);
    return IsValid(p, r, i1, i2, when);
  }

  public bool IsValid(Chars performer, Chars receiver, Item item1, Item item2, When when) {
    Actor p = Controller.GetActor(performer);
    Actor r = Controller.GetActor(receiver);
    return IsValid(p, r, item1, item2, when);
  }

  public bool IsValid(Actor performer, Actor receiver, ItemEnum item1, ItemEnum item2, When when) {
    Item i1 = AllObjects.GetItem(item1);
    Item i2 = AllObjects.GetItem(item2);
    return IsValid(performer, receiver, i1, i2, when);
  }


  public bool IsValid(Actor performer, Actor receiver, Item item1, Item item2, When when) {
    if (this.when != When.Always && this.when != when) return false;
    if (item1 != null && item2 != null && type != ConditionType.UsedWith) return false;

    switch (type) {
      case ConditionType.None: return true;

      case ConditionType.ActorIs: {
        bool res;
        if ((Chars)id == Chars.Current) res = (GD.c.currentActor == performer || GD.c.currentActor == receiver);
        else if ((Chars)id == Chars.Actor1) res = (GD.c.actor1 == performer || GD.c.actor1 == receiver);
        else if ((Chars)id == Chars.Actor2) res = (GD.c.actor2 == performer || GD.c.actor2 == receiver);
        else if ((Chars)id == Chars.Actor3) res = (GD.c.actor3 == performer || GD.c.actor3 == receiver);
        else if ((Chars)id == Chars.KidnappedActor) res = (GD.c.kidnappedActor == performer || GD.c.kidnappedActor == receiver);
        else if ((Chars)id == Chars.Player) res = (GD.c.actor1 == performer || GD.c.actor2 == performer || GD.c.actor3 == performer);
        else res = performer.id == (Chars)id;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.CurrentActorIs: {
        bool res = Controller.ValidActor((Chars)id, GD.c.currentActor);
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorHasSkill: {
        bool res = Controller.GetActor((Chars)id).HasSkill((Skill)iv);
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.CurrentRoomIs: {
        bool res = sv.ToLowerInvariant().Equals(GD.c.currentRoom.ID.ToLowerInvariant());
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.RoomIsInExt: {
        Actor a1;
        if ((Chars)id == Chars.Current) a1 = GD.c.currentActor;
        else if ((Chars)id == Chars.Actor1) a1 = GD.c.actor1;
        else if ((Chars)id == Chars.Actor2) a1 = GD.c.actor2;
        else if ((Chars)id == Chars.Actor3) a1 = GD.c.actor3;
        else if ((Chars)id == Chars.KidnappedActor) a1 = GD.c.kidnappedActor;
        else if ((Chars)id == Chars.Player) a1 = GD.c.currentActor;
        else if ((Chars)id == Chars.Self) a1 = performer;
        else if ((Chars)id == Chars.Receiver) a1 = receiver;
        else a1 = Controller.GetActor((Chars)id);

        if (a1 == null) return false;
        if (bv)
          return !a1.currentRoom.external;
        else
          return a1.currentRoom.external;
      }

      case ConditionType.SameRoom: {
        Actor a1, a2;
        if ((Chars)id == Chars.Current) a1 = GD.c.currentActor;
        else if ((Chars)id == Chars.Actor1) a1 = GD.c.actor1;
        else if ((Chars)id == Chars.Actor2) a1 = GD.c.actor2;
        else if ((Chars)id == Chars.Actor3) a1 = GD.c.actor3;
        else if ((Chars)id == Chars.KidnappedActor) a1 = GD.c.kidnappedActor;
        else if ((Chars)id == Chars.Player) a1 = GD.c.currentActor;
        else if ((Chars)id == Chars.Self) a1 = performer;
        else if ((Chars)id == Chars.Receiver) a1 = receiver;
        else a1 = Controller.GetActor((Chars)id);

        bool res = false;
        switch ((Chars)iv) {
          case Chars.None: return false;
          case Chars.Current: res = GD.c.currentActor.currentRoom.Equals(a1.currentRoom); break;
          case Chars.Actor1: res = GD.c.actor1.currentRoom.Equals(a1.currentRoom); break;
          case Chars.Actor2: res = GD.c.actor2.currentRoom.Equals(a1.currentRoom); break;
          case Chars.Actor3: res = GD.c.actor3.currentRoom.Equals(a1.currentRoom); break;
          case Chars.KidnappedActor: res = GD.c.kidnappedActor.currentRoom.Equals(a1.currentRoom); break;
          case Chars.Receiver: res = (receiver != null && receiver.currentRoom.Equals(a1.currentRoom)); break;
          case Chars.Self: return true;
          case Chars.Player: res = (GD.c.actor1.currentRoom.Equals(a1.currentRoom)) || (GD.c.actor2.currentRoom.Equals(a1.currentRoom)) || (GD.c.actor3.currentRoom.Equals(a1.currentRoom)); break;
          case Chars.Enemy: return false;
          case Chars.Fred:
          case Chars.Edna:
          case Chars.Ted:
          case Chars.Ed:
          case Chars.Edwige:
          case Chars.GreenTentacle:
          case Chars.PurpleTentacle:
          case Chars.BlueTentacle:
          case Chars.PurpleMeteor:
          case Chars.Dave:
          case Chars.Bernard:
          case Chars.Wendy:
          case Chars.Syd:
          case Chars.Hoagie:
          case Chars.Razor:
          case Chars.Michael:
          case Chars.Jeff:
          case Chars.Javid:
          case Chars.Laverne:
          case Chars.Ollie:
          case Chars.Sandy:
            a2 = Controller.GetActor((Chars)id);
            if (a2 == null) return false;
            res = a2.currentRoom.Equals(a1.currentRoom);
            break;
          case Chars.Male: return false;
          case Chars.Female: return false;
        }

        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.FlagValueIs: {
        if (bv)
          return AllObjects.CheckFlag((GameFlag)id, iv);
        else
          return !AllObjects.CheckFlag((GameFlag)id, iv);
      }

      case ConditionType.ItemCollected: {
        Item item = AllObjects.GetItem((ItemEnum)id);
        bool res = item != null && item.owner != Chars.None;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorInRoom: {
        Actor a = Controller.GetActor((Chars)id);
        if (a == null) return false;
        bool res = a.currentRoom.ID.ToLowerInvariant().Equals(sv.ToLowerInvariant());
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorDistanceLess: {
        Actor a = Controller.GetActor((Chars)id);
        if (a == null) return false;
        bool res = Vector2.Distance(a.transform.position, performer.transform.position) < fv;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorXLess: {
        Actor a = Controller.GetActor((Chars)id);
        if (a == null) return false;
        bool res = a.transform.position.x < fv;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ItemOpen:
        if (iv == 0) {
          if (bv)
            return item1.IsOpen();
          else
            return !item1.IsOpen();
        }
        else {
          if (bv)
            return item1.IsLocked();
          else
            return !item1.IsLocked();
        }

      case ConditionType.RecipientIs: {
        if (receiver == null) return false;
        Chars idc = (Chars)id;
        if (idc == Chars.Player) {
          if (bv)
            return (GD.c.actor1 == receiver) || (GD.c.actor2 == receiver) || (GD.c.actor3 == receiver);
          else
            return (GD.c.actor1 != receiver) && (GD.c.actor2 != receiver) && (GD.c.actor3 != receiver);
        }
        if (idc == Chars.Enemy) {
          if (bv)
            return (receiver.id == Chars.Fred) || (receiver.id == Chars.Edna) || (receiver.id == Chars.Ed) || (receiver.id == Chars.Ted) || (receiver.id == Chars.Edwige) || (receiver.id == Chars.GreenTentacle) || (receiver.id == Chars.PurpleTentacle) || (receiver.id == Chars.BlueTentacle) || (receiver.id == Chars.PurpleMeteor);
          else
            return (receiver.id != Chars.Fred) && (receiver.id != Chars.Edna) && (receiver.id != Chars.Ed) && (receiver.id != Chars.Ted) && (receiver.id != Chars.Edwige) && (receiver.id != Chars.GreenTentacle) && (receiver.id != Chars.PurpleTentacle) && (receiver.id != Chars.BlueTentacle) && (receiver.id != Chars.PurpleMeteor);
        }
        if (bv)
          return (idc == receiver.id);
        else
          return (idc != receiver.id);
      }

      case ConditionType.WhenIs: {
        bool res = (When)id == when;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.UsedWith: {
        if (item2 != null) {
          if (bv)
            return ((ItemEnum)id == item2.Item);
          else
            return ((ItemEnum)id != item2.Item);
        }
        else if (item2 != null) {
          if (bv)
            return ((ItemEnum)id == item1.Item);
          else
            return ((ItemEnum)id != item1.Item);
        }
        else return false;
      }
    }
    return false;
  }
}



public enum ConditionType {
  None,
  ActorIs,            // ID of the actor like "player", "actor1", etc. And ID of the value that should be  (ID1, IV1, BV) -> BV true is actor is, false if actor should not be
  ActorHasSkill,      // ID of actor and ID of skill                                                       (ID1, IV1, BV)
  CurrentRoomIs,      // String name of the room                                                           (SV, BV)
  FlagValueIs,        // ID of flag, and value                                                             (ID1, IV1, BV)
  ItemCollected,      // ID of item                                                                        (ID1, BV)
  ActorInRoom,    // ID of item                                                                        (ID1, SV, BV)
  ActorDistanceLess,  // ID and dist value                                                                 (ID1, FV, BV)
  ActorXLess,         // ID and dist value                                                                 (ID1, FV, BV)
  ItemOpen,           // ID of item, value for open, closed, locked                                        (ID1, IV1)
  RecipientIs,        // ID of actor                                                                       (ID1, BV)
  WhenIs,             // ID of action (give, pick, use, etc.)                                              (ID1, BV)
  UsedWith,           // ID of items                                                                       (ID1, BV)
  CurrentActorIs,     // ID of actor                                                                       (ID1, BV)
  SameRoom,           // ID f actor, ID of other actor                                                     (ID1, IV1, BV)
  RoomIsInExt,        // ID f actor, bool to check if internal or external                                 (ID1, BV)
}



