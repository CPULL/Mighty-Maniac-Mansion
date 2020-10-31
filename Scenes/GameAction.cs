using UnityEngine;

/// <summary>
/// Used to specify the type of actions
/// </summary>
public enum ActionType {
  None = 0,
  ShowRoom = 1, // Jumps to a specific room enabling it
  Teleport = 2, // Teleport an actor somewhere 
  Speak = 3, // Have an actor to say something
  Expression = 4, // Set an expression to an actor

  WalkToPos = 5, // Have an actor to walk to a destination
  WalkToActor = 6, // Move to the lef tor right of another actor
  BlockActorX = 7, // Have the movement of an actor (player) limited between a min and max X coordinate
  UnBlockActor = 8, // Remove movement limits from the actor (player)

  Open = 9, // Open or close or lock or unlock a door or a container
  EnableDisable = 10, // Enable or disable an item

  Cutscene = 11, // Starts a Cutscene
  Sound = 12, // Play a sound
  ReceiveCutscene = 13, // Have an actor to receive an item from another actor, accept or decline, say something, and start a cutscene
  ReceiveFlag = 14, // Have an actor to receive an item from another actor, accept or decline, say something, and set a flag

  Fade = 15, // Fade the screen in or out
  Anim = 16, // Make an animation to play on an object or on an actor
  AlterItem = 17, // Changes what you can do with an item
  SetFlag = 18, // Sets a flag
  CompleteStep = 19, // Sets the step of a gamescene as completed and moves to the next one
  Wait = 20, // Wait the specified time

  PressAndFlag = 21, // Press an item and set a flag (until the actor moves away)
  PressAndItem = 22, // Press an item and set a door (until the actor moves away)

  SwitchRoomLight = 23, // Turns on and off a light in a room (all rooms in case no room is specified)
  StopScenes = 24, // Stops all not unique cutscenes with the given actor
  SetCurrentRoomActor = 25, // Changes the current actor with the one in the same room of ID1 (ID2 == 0 for any actor, == 1 for player)

  Cursor = 26, // Used to set the cursor in a specific mode, side effect of skipping the cutscene
};


[System.Serializable]
public class GameAction {
  public Running running = Running.NotStarted; // FIXME check that we have these values: BehaviorActonStatus
  private float time;
  public string msg;


  public ActionType type;
  public bool Repeatable; // Can the action be repeated?
  public float delay; // Delay to use when playing the action

  public int id1;
  public int id2;
  public string str;
  public Vector2 pos;
  public Dir dir;
  public int val;


  public static string StringName(ActionType type, int id1, int id2, string str, Vector2 pos, Dir dir, int val)   {
    string res = "Action name not yet calculated " + type;
    switch (type) {
      case ActionType.None: return "No action";
      case ActionType.ShowRoom: return "Show room " + str + (id2 == 0 ? "" : " [panning]");
      case ActionType.Teleport: return "Teleport " + (Chars)id1 + " [" + pos.x + "," + pos.y + "]";
      case ActionType.Speak: return (Chars)id1 + " say: \"" + str.Substring(0, str.Length > 10 ? 10 : str.Length).Replace("\n", "");
      case ActionType.Expression: return (Chars)id1 + " " + (Expression)id2;
      case ActionType.WalkToPos: return (Chars)id1 + " walk [" + pos.x + "," + pos.y + "]";
      case ActionType.WalkToActor: return (Chars)id1 + " walk [" + (Chars)id2 + ", " + dir + "]";
      case ActionType.BlockActorX: return (Chars)id1 + " block [" + pos.x + "," + pos.y + "]";
      case ActionType.UnBlockActor: return (Chars)id1 + " unblock";
      case ActionType.Open: {
        if (val == 0) return "Open " + (ItemEnum)id1;
        if (val == 1) return "Close " + (ItemEnum)id1;
        if (val == 2) return "Lock " + (ItemEnum)id1;
        if (val == 3) return "Unlock " + (ItemEnum)id1;
      }
      break;
      case ActionType.EnableDisable: return (ItemEnum)id1 + " " + (val == 0 ? "Enable" : "Disable");
      case ActionType.Cutscene: return "Cutscene: " + (CutsceneID)id1;
      case ActionType.StopScenes: return "Stop cutscenes for " + (Chars)id1;
      case ActionType.SetCurrentRoomActor: return "Set current as " + (id2 == 0 ? "any" : "player") + " in same room " + (Chars)id1;
      case ActionType.Sound: return "Sound: " + (Audios)id1;
      case ActionType.ReceiveCutscene: {
        if (val == 0) { // Yes
          return "Accept " + (ItemEnum)id1 + "->" + (CutsceneID)id2 + ": " + str.Substring(0, str.Length > 10 ? 10 : str.Length).Replace("\n", "");
        }
        else { // No
          return "Refuse " + (ItemEnum)id1 + ": " + str.Substring(0, str.Length > 10 ? 10 : str.Length).Replace("\n", "");
        }
      }
      case ActionType.ReceiveFlag: {
        if (val == 0) { // Yes
          return "Accept " + (ItemEnum)id1 + "->" + (GameFlag)id2 + ": " + str.Substring(0, str.Length > 10 ? 10 : str.Length).Replace("\n", "");
        }
        else { // No
          return "Refuse " + (ItemEnum)id1 + ": " + str.Substring(0, str.Length > 10 ? 10 : str.Length).Replace("\n", "");
        }
      }
      case ActionType.Fade: return (val == 0) ? "Fade In" : "Fade Out";
      case ActionType.Anim: {
        if ((Chars)id1 == Chars.None) return (ItemEnum)id2 + " anim: " + str;
        return (Chars)id1 + " anim: " + str;
      }
      case ActionType.AlterItem: return "Alter " + (ItemEnum)id1 + " L[" + (WhatItDoes)id2 + "] R[" + (WhatItDoes)val + "]";
      case ActionType.SetFlag: return "Set " + (GameFlag)id1 + " = " + val;
      case ActionType.CompleteStep: return "Complete step";
      case ActionType.Wait: return "Wait";

      case ActionType.PressAndFlag: return "Press and set " + (GameFlag)id1 + " = " + val;
      case ActionType.PressAndItem: {
        if (val == 0) return "Press and Open " + (ItemEnum)id1;
        if (val == 1) return "Press and Close " + (ItemEnum)id1;
        if (val == 2) return "Press and Lock " + (ItemEnum)id1;
        if (val == 3) return "Press and Unlock " + (ItemEnum)id1;
      }
      break;

      case ActionType.SwitchRoomLight: return "Switch lights on " + (string.IsNullOrEmpty(str) ? "Everywhere" : str);

      case ActionType.Cursor: return "Set currsor as " + (CursorTypes)id1;
    }
    return res;
  }

  internal void Stop() {
    running = Running.NotStarted;
    switch (type) {
      case ActionType.None:
      case ActionType.ShowRoom:
      case ActionType.Teleport:
      case ActionType.Expression:
      case ActionType.BlockActorX:
      case ActionType.UnBlockActor:
      case ActionType.Open:
      case ActionType.EnableDisable:
      case ActionType.Cutscene:
      case ActionType.StopScenes:
      case ActionType.SetCurrentRoomActor:
      case ActionType.ReceiveCutscene:
      case ActionType.ReceiveFlag:
      case ActionType.Fade:
      case ActionType.Anim:
      case ActionType.AlterItem:
      case ActionType.SetFlag:
      case ActionType.CompleteStep:
      case ActionType.Wait:
      case ActionType.SwitchRoomLight:
      case ActionType.Cursor:
        return;

      case ActionType.Speak: {
        Balloon.Stop();
        return;
      }

      case ActionType.WalkToActor:
      case ActionType.WalkToPos: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) return;
        a.Stop();
      }
      break;

      case ActionType.Sound: {
        Sounds.Stop();
      }
      break;

      case ActionType.PressAndFlag: {
        GD.c.pressActions[0].Reset((GameFlag)id1);
        GD.c.pressActions[1].Reset((GameFlag)id1);
        GD.c.pressActions[2].Reset((GameFlag)id1);
      }
      break;

      case ActionType.PressAndItem: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        GD.c.pressActions[0].Reset(item);
        GD.c.pressActions[1].Reset(item);
        GD.c.pressActions[2].Reset(item);
      }
      break;

    }
  }

  public GameAction() {
    type = ActionType.None;
  }

  public GameAction(string stype, bool rep, float del, string vid1, string vid2, string sv, int iv, string dv, Vector2 vv) {
    type = (ActionType)System.Enum.Parse(typeof(ActionType), stype, true);
    if (!System.Enum.IsDefined(typeof(ActionType), type)) {
      Debug.LogError("Unknown ActionType: \"" + stype + "\"");
    }
    Repeatable = rep;
    delay = del;
    str = sv;
    val = iv;
    pos = vv;

    // id1 and id2 should be parsed based on the type
    switch (type) {
      case ActionType.Teleport:
      case ActionType.Speak:
      case ActionType.BlockActorX:
      case ActionType.UnBlockActor:
      case ActionType.WalkToPos: {
        Chars id = (Chars)System.Enum.Parse(typeof(Chars), vid1, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        dir = (Dir)System.Enum.Parse(typeof(Dir), dv, true);
        if (!System.Enum.IsDefined(typeof(Dir), dir)) {
          Debug.LogError("Unknown Dir: \"" + dv + "\"");
          dir = Dir.None;
        }
      }
      break;

      case ActionType.Expression: {
        Chars id = (Chars)System.Enum.Parse(typeof(Chars), vid1, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        dir = (Dir)System.Enum.Parse(typeof(Dir), dv, true);
        if (!System.Enum.IsDefined(typeof(Dir), dir)) {
          Debug.LogError("Unknown Dir: \"" + dv + "\"");
          dir = Dir.None;
        }

        id2 = (int)Expression.Normal;
        char ex = (sv.ToLowerInvariant() + "!")[0];
        switch (ex) {
          case 'h': id2 = (int)Expression.Happy; break;
          case 's': id2 = (int)Expression.Sad; break;
          case 'n': id2 = (int)Expression.Normal; break;
          case 'o': id2 = (int)Expression.Open; break;
          case 'b': id2 = (int)Expression.BigOpen; break;
          case '!': { // Try by ID
            Expression ide = (Expression)System.Enum.Parse(typeof(Expression), vid2, true);
            if (System.Enum.IsDefined(typeof(Expression), ide)) {
              id2 = (int)ide;
            }
          }
          break;
        }
      }
      break;

      case ActionType.Open: {
        ItemEnum id = (ItemEnum)System.Enum.Parse(typeof(ItemEnum), vid1, true);
        if (!System.Enum.IsDefined(typeof(ItemEnum), id)) {
          Debug.LogError("Unknown Item: \"" + vid1 + "\"");
        }
        id1 = (int)id;

        char ov = (sv.ToLowerInvariant()+"X")[0];
        switch(ov) {
          case 'o': val = 0; break; // Open
          case 'c': val = 1; break; // Close
          case 'l': val = 2; break; // Lock
          case 'u': val = 3; break; // Unlock
          default: Debug.LogError("Unknown Open/Close/Lock/Unlock for " + (ItemEnum)id1); break;
        }
      }
      break;

      case ActionType.ShowRoom: {
        int.TryParse(vid2, out id2); // 0 for immediate, not zero for panning
      }
      break;

      case ActionType.WalkToActor: {
        Chars id = (Chars)System.Enum.Parse(typeof(Chars), vid1, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        id = (Chars)System.Enum.Parse(typeof(Chars), vid2, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid2 + "\"");
        }
        id2 = (int)id;
        dir = (Dir)System.Enum.Parse(typeof(Dir), dv, true);
        if (!System.Enum.IsDefined(typeof(Dir), dir)) {
          Debug.LogError("Unknown Dir: \"" + dv + "\"");
          dir = Dir.None;
        }
      }
      break;

      case ActionType.SetFlag: {
        GameFlag id = (GameFlag)System.Enum.Parse(typeof(GameFlag), vid1, true);
        if (!System.Enum.IsDefined(typeof(GameFlag), id)) {
          Debug.LogError("Unknown GameFlag: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        if (!string.IsNullOrEmpty(sv)) {
          if (sv.ToLowerInvariant()[0] == 'y') val = 0;
          else val = 1;
        }
        else {
          val = iv;
        }
      }
      break;

      case ActionType.Cutscene: {
        CutsceneID id = (CutsceneID)System.Enum.Parse(typeof(CutsceneID), vid1, true);
        if (!System.Enum.IsDefined(typeof(CutsceneID), id)) {
          Debug.LogError("Unknown Cutscene ID: \"" + vid1 + "\"");
        }
        id1 = (int)id;
      }
      break;

      case ActionType.CompleteStep: {
        int.TryParse(vid2, out id2); // 0 for immediate, 1 for restart step
      }
      break;

      case ActionType.SetCurrentRoomActor: {
        Chars id = (Chars)System.Enum.Parse(typeof(Chars), vid1, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        int.TryParse(vid2, out id2); // 0 for any, 1 for a player
      }
      break;

      case ActionType.PressAndFlag: {
        GameFlag id = (GameFlag)System.Enum.Parse(typeof(GameFlag), vid1, true);
        if (!System.Enum.IsDefined(typeof(GameFlag), id)) {
          Debug.LogError("Unknown GameFlag: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        val = iv;
      }
      break;

      case ActionType.PressAndItem: {
        ItemEnum id = (ItemEnum)System.Enum.Parse(typeof(ItemEnum), vid1, true);
        if (!System.Enum.IsDefined(typeof(ItemEnum), id)) {
          Debug.LogError("Unknown ItemEnum: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        val = iv;
      }
      break;

      case ActionType.StopScenes: {
        Chars id = (Chars)System.Enum.Parse(typeof(Chars), vid1, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid1 + "\"");
        }
        id1 = (int)id;
      }
      break;

      case ActionType.Cursor: {
        CursorTypes id = (CursorTypes)System.Enum.Parse(typeof(CursorTypes), vid1, true);
        if (!System.Enum.IsDefined(typeof(CursorTypes), id)) {
          Debug.LogError("Unknown CursorTypes: \"" + vid1 + "\"");
        }
        id1 = (int)id;
      }
      break;

    }
  }

  public override string ToString() {
    return StringName(type, id1, id2, str, pos, dir, val);
  }

  
  internal void Play() {
    running = Running.Running;
    time = delay;
  }

  internal void Complete() {
    running = Running.Completed;
  }

  internal bool IsCompleted() {
    return running == Running.Completed;
  }

  internal bool IsPlaying() {
    return running == Running.Running;
  }

  internal bool NotStarted() {
    return running == Running.NotStarted;
  }

  internal void CheckTime(float deltaTime) {
    if (time > 0) {
      time -= deltaTime;
      if (time <= 0) {
        running = Running.Completed;
        time = delay;
      }
    }
  }

  internal void Reset() {
    running = Running.NotStarted;
    time = delay;
  }


  public void RunAction(Actor performer, Actor secondary, Item item1, Item item2) {
    Debug.Log("Playing: " + ToString());
    switch (type) {
      case ActionType.ShowRoom: {
        if (Controller.SceneSkipped) {
          Complete();
          return;
        }
        GD.c.currentRoom = AllObjects.GetRoom(str);
        Vector3 rpos = pos;
        rpos.z = -10;
        foreach (Room r in GD.a.roomsList)
          r.gameObject.SetActive(false);
        GD.c.currentRoom.gameObject.SetActive(true);
        foreach (Actor a in GD.c.allActors) {
          if (a == null) continue;
          a.SetVisible(a.currentRoom == GD.c.currentRoom);
        }
        foreach (Actor a in GD.c.allEnemies) {
          if (a == null) continue;
          a.SetVisible(a.currentRoom == GD.c.currentRoom);
        }
        GD.c.currentRoom.UpdateLights();
        if (id2 != 0) {
          Controller.PanCamera(rpos, delay); // PAN
        }
        else {
          GD.c.cam.transform.position = rpos;
          Controller.StopPanningCamera();
        }
        Complete();
      }
      break;

      case ActionType.Teleport: {
        Actor a = Controller.GetActor((Chars)id1);
        Room aroom = AllObjects.GetRoom(str);
        if (aroom != null) {
          a.currentRoom = aroom;
          a.SetVisible(aroom == GD.c.currentRoom);
        }
        a.transform.position = pos;
        a.SetDirection(dir);
        if (aroom != null) {
          a.SetScaleAndPosition(pos);
        }
        Complete();
      }
      break;

      case ActionType.Speak: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) a = performer;
        if (a == null) {
          Complete();
          return;
        }
        a.Say(str, this);
        a.SetDirection(dir);
        Play();
      }
      break;

      case ActionType.Expression: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) a = performer;
        if (a == null) {
          Complete();
          return;
        }
        a.SetDirection(dir);
        a.SetExpression((Expression)id2);
        Play();
      }
      break;

      case ActionType.WalkToPos: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) a = performer;
        if (a == null) {
          Complete();
          return;
        }
        RaycastHit2D hit = Physics2D.Raycast(pos, GD.c.cam.transform.forward, 10000, GD.c.pathLayer);
        PathNode p = null;
        if (hit.collider != null) {
          p = hit.collider.GetComponent<PathNode>();
        }
        else {
          Debug.Log("No path collider");
          p = a.currentRoom.GetPathNode(pos);
          if (p != null) {
            // Fix the pos, it should be closest to the path borders
            Vector2 p1s = pos;
            Vector2 p1e = p.Center();
            Vector2 p2s, p2e;
            p2s.x = p.tl.x;
            p2e.x = p.tr.x;
            p2s.y = p.tl.y;
            p2e.y = p.tr.y;
            Vector2 intersectT = NavPath.FindIntersection(p1s, p1e, p2s, p2e);

            p2s.x = p.tl.x;
            p2e.x = p.bl.x;
            p2s.y = p.tl.y;
            p2e.y = p.bl.y;
            Vector2 intersectL = NavPath.FindIntersection(p1s, p1e, p2s, p2e);

            p2s.x = p.tr.x;
            p2e.x = p.br.x;
            p2s.y = p.tr.y;
            p2e.y = p.br.y;
            Vector2 intersectR = NavPath.FindIntersection(p1s, p1e, p2s, p2e);

            p2s.x = p.bl.x;
            p2e.x = p.br.x;
            p2s.y = p.bl.y;
            p2e.y = p.br.y;
            Vector2 intersectB = NavPath.FindIntersection(p1s, p1e, p2s, p2e);

            float mindist = float.PositiveInfinity;
            Vector2 best = pos;
            if (intersectT.x != float.NaN && Vector2.Distance(pos, intersectT) < mindist) {
              mindist = Vector2.Distance(pos, intersectT);
              best = intersectT;
            }
            if (intersectB.x != float.NaN && Vector2.Distance(pos, intersectB) < mindist) {
              mindist = Vector2.Distance(pos, intersectB);
              best = intersectB;
            }
            if (intersectL.x != float.NaN && Vector2.Distance(pos, intersectL) < mindist) {
              mindist = Vector2.Distance(pos, intersectL);
              best = intersectL;
            }
            if (intersectR.x != float.NaN && Vector2.Distance(pos, intersectR) < mindist) {
              mindist = Vector2.Distance(pos, intersectR);
              best = intersectR;
            }

            Debug.Log("Updated pos from " + pos.x + "," + pos.y + " to " + best.x + "," + best.y);
            pos = best;
          }
        }

        if (p != null) {
          GameAction copy = this;
          Play();
          a.WalkTo(pos, p,
          new System.Action<Actor, Item>((actor, item) => {
            actor.SetDirection(copy.dir);
            copy.Complete();
          }));
        }
        else
          Debug.LogError("No collider for walk");
      }
      break;

      case ActionType.WalkToActor: { // This action is set in the actor and run in the actor itself, it is completed when the actor is reached (or the actor goes in another room), but it will not end until a different teleport or walk is done
        Actor walker = Controller.GetActor((Chars)id1);
        if (walker == null) walker = performer;
        if (walker == null) {
          Debug.LogError("Missing walker in action: " + ToString());
          Complete();
          return;
        }
        Actor destAct = Controller.GetActor((Chars)id2);
        if (walker.WalkTo(destAct.transform, dir, val == 0 ? this : null))
          Complete(); // Not possible to reach
        Play();
      }
      break;

      case ActionType.BlockActorX: {
        Actor act = Controller.GetActor((Chars)id1);
        if ((Chars)id1 == Chars.Player) act = GD.c.currentActor;
        if (act == null) {
          Debug.Log("No actor: " + (Chars)id1);
          Complete();
          return;
        }
        act.SetMinMaxX(pos.x, pos.y);
        Complete();
      }
      break;

      case ActionType.UnBlockActor: {
        Actor act = Controller.GetActor((Chars)id1);
        if ((Chars)id1 == Chars.Player) act = GD.c.currentActor;
        if (act == null) {
          Debug.Log("No actor: " + (Chars)id1);
          Complete();
          return;
        }
        act.SetMinMaxX(-float.MaxValue, float.MaxValue);
        Complete();
      }
      break;

      case ActionType.Open: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        if (item == null) {
          Debug.LogError("Item not defined for Open " + ToString());
          Complete();
          return;
        }
        item.ForceStatus(val);
        Complete();
      }
      break;

      case ActionType.EnableDisable: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        if (item == null) {
          Debug.LogError("Item not defined for Enable");
          Complete();
          return;
        }
        item.gameObject.SetActive(val == 0);
        Complete();
      }
      break;

      case ActionType.Cutscene: {
        CutsceneID id = (CutsceneID)id1;
        if (id != CutsceneID.NONE) Controller.StartCutScene(AllObjects.GetCutscene(id));
        Complete();
      }
      break;

      case ActionType.StopScenes: {
        Chars id = (Chars)id1;
        AllObjects.StopScenes(id);
        Complete();
      }
      break;

      case ActionType.SetCurrentRoomActor: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) {
          Complete();
          return;
        }
        if (id2 == 1) {
          if (GD.c.actor1.currentRoom.Equals(a.currentRoom)) Controller.SelectActor(GD.c.actor1, true);
          if (GD.c.actor2.currentRoom.Equals(a.currentRoom)) Controller.SelectActor(GD.c.actor2, true);
          if (GD.c.actor3.currentRoom.Equals(a.currentRoom)) Controller.SelectActor(GD.c.actor3, true);
          Complete();
        }
        else { // Go over all actors
          foreach (Actor act in GD.c.allActors)
            if (act.gameObject.activeSelf && act.currentRoom.Equals(a.currentRoom)) {
              GD.c.receiverActor = act;
              Complete();
              return;
            }
          foreach (Actor act in GD.c.allEnemies)
            if (act.gameObject.activeSelf && act.currentRoom.Equals(a.currentRoom)) {
              GD.c.receiverActor = act;
              Complete();
              return;
            }
          Complete();
        }
      }
      break;

      case ActionType.Sound: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) a = performer;
        if (a != null) {
          if (dir != Dir.None) a.SetDirection(dir);
          Sounds.Play((Audios)id2, a.transform.position);
        }
        else
          Sounds.Play((Audios)id2, GD.c.currentActor.transform.position);
        Play();
      }
      break;

      case ActionType.ReceiveCutscene: {
        // Are we accpeting?
        if (val == 0) { // Yes
          Item item = AllObjects.GetItem((ItemEnum)id1);
          performer.inventory.Remove(item);
          secondary.inventory.Add(item);
          item.owner = Controller.GetCharFromActor(secondary);
          Controller.UpdateInventory();
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            Play();

            CutsceneID id = (CutsceneID)id2;
            if (id != CutsceneID.NONE) Controller.StartCutScene(AllObjects.GetCutscene(id));
            Complete();
          }
          else
            Complete();
        }
        else { // No
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            Play();
          }
          else
            Complete();
        }
      }
      break;

      case ActionType.ReceiveFlag: {
        // Are we accpeting?
        if (val == 0) { // Yes
          Item item = AllObjects.GetItem((ItemEnum)id1);
          performer.inventory.Remove(item);
          secondary.inventory.Add(item);
          item.owner = Controller.GetCharFromActor(secondary);
          Controller.UpdateInventory();
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            AllObjects.SetFlag((GameFlag)id2, 1);
            Play();
          }
          else
            Complete();
        }
        else { // No
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            Play();
          }
          else
            Complete();
        }
      }
      break;

      case ActionType.Fade: {
        if (Controller.SceneSkipped) {
          Complete();
          return;
        }
        if (val == 0)
          Fader.FadeIn();
        else
          Fader.FadeOut();
        Play();
      }
      break;

      case ActionType.Anim: {
        if (id1 != 0) {
          Actor a = Controller.GetActor((Chars)id1);
          Debug.LogError("FIXME anim actor not implemented!"); // FIXME
          Complete();
          return;
        }
        Item item = AllObjects.GetItem((ItemEnum)id2);
        Animator anim = item.GetComponent<Animator>();
        if (anim == null) {
          Debug.LogError("Missing animator for animated item: " + item.gameObject.name);
          return;
        }
        anim.enabled = true;
        anim.Play(str);
        Play();
      }
      break;

      case ActionType.AlterItem: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        item.whatItDoesL = (WhatItDoes)id2;
        item.whatItDoesR = (WhatItDoes)val;
        item.dir = dir;
        Complete();
      }
      break;

      case ActionType.SetFlag: {
        GameFlag flag = (GameFlag)id1;
        AllObjects.SetFlag(flag, val);
        Complete();
      }
      break;

      case ActionType.CompleteStep: {
        if (delay > 0)
          Play();
        else
          Complete();
      }
      break;

      case ActionType.Wait: {
        if (delay > 0)
          Play();
        else
          Complete();
      }
      break;

      case ActionType.SwitchRoomLight: {
        if (string.IsNullOrEmpty(str)) {
          bool lightsOn = !AllObjects.GetRoom("MainHall").lights;
          foreach (Room r in AllObjects.RoomList) {
            r.SetLights(lightsOn);
          }
        }
        else {
          Room r = AllObjects.GetRoom(str);
          if (r != null) r.SetLights(!r.lights);
        }
      }
      break;

      case ActionType.PressAndFlag: {
        int pos = 0;
        if (performer == GD.c.actor2) pos = 1;
        if (performer == GD.c.actor3) pos = 2;
        GD.c.pressActions[pos].Set(performer, (GameFlag)id1);
        AllObjects.SetFlag((GameFlag)id1, val);
      }
      break;

      case ActionType.PressAndItem: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        int pos = 0;
        if (performer == GD.c.actor2) pos = 1;
        if (performer == GD.c.actor3) pos = 2;
        GD.c.pressActions[pos].Set(performer, item);
        item.ForceStatus(val);
      }
      break;

      case ActionType.Cursor: {
        Controller.SceneSkipped = true;
        Controller.SetCursor((CursorTypes)id1);
        GD.c.StartCoroutine(GD.c.FadeToRoomActor());
        Complete();
      }
      break;

      default: {
        // FIXME do the other actions
        Debug.Log("Not implemented action: " + ToString());
        Complete(); // Just to avoid to block the game for action not yet done
      }
      break;
    }
  }

}

public class ActionRes {
  public bool actionDone = false;
  public string res = null;
}
