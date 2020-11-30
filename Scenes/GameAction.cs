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

  SwitchRoomLight = 23, // Turns on and off the lights. A room can be specified or the global light can be specified. The mode can be On or Off.
  StopScenes = 24, // Stops all not unique cutscenes with the given actor
  SetCurrentRoomActor = 25, // Changes the current actor with the one in the same room of ID1 (ID2 == 0 for any actor, == 1 for player)

  Cursor = 26, // Used to set the cursor in a specific mode, side effect of skipping the cutscene
  ChangeSprites = 27, // Changes sprites of items

  SwitchFlashlight = 28, // Turns on and off the flashlight
  Wear = 29, // Wear/unwear coat and hazman suite

  ShowMap = 30, // Show and hide the Map of the Woods
  Pick = 31, // Acts like a player picked an item
};


[System.Serializable]
public class GameAction {
  public Running running = Running.NotStarted; // FIXME check that we have these values: BehaviorActonStatus
  private float time;
  public string msg;

  public ActionType type;
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
      case ActionType.Speak: return (Chars)id1 + " say: \"" + str.Substring(0, str.Length > 15 ? 15 : str.Length).Replace("\n", "");
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
      case ActionType.Sound: return "Sound: " + (Audios)id2;
      case ActionType.ReceiveCutscene: {
        if (val == 0) { // Yes
          return "Accept " + (ItemEnum)id1 + "->" + (CutsceneID)id2 + ": " + str.Substring(0, str.Length > 15 ? 15 : str.Length).Replace("\n", "");
        }
        else { // No
          return "Refuse " + (ItemEnum)id1 + ": " + str.Substring(0, str.Length > 15 ? 15 : str.Length).Replace("\n", "");
        }
      }
      case ActionType.ReceiveFlag: {
        if (val == 0) { // Yes
          return "Accept " + (ItemEnum)id1 + "->" + (GameFlag)id2 + ": " + str.Substring(0, str.Length > 15 ? 15 : str.Length).Replace("\n", "");
        }
        else { // No
          return "Refuse " + (ItemEnum)id1 + ": " + str.Substring(0, str.Length > 15 ? 15 : str.Length).Replace("\n", "");
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
      case ActionType.SwitchFlashlight: return "Switch flashlights on and off";

      case ActionType.Cursor: return "Set currsor as " + (CursorTypes)id1;

      case ActionType.ChangeSprites: return "Change " + (ItemEnum)id1 + " op=" + id2 + " cl=" + val;

      case ActionType.Wear: return "Wear " + (ItemEnum)id1;

      case ActionType.ShowMap: return "Show/Hide Woods map";
      case ActionType.Pick: return "Pick item " + (ItemEnum)id2 + " by " + (Chars)id1;
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
      case ActionType.SwitchFlashlight:
      case ActionType.Cursor:
      case ActionType.ChangeSprites:
      case ActionType.Wear:
      case ActionType.ShowMap:
      case ActionType.Pick:
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

  public GameAction(string stype, float del, string vid1, string vid2, string sv, int iv, string dv, Vector2 vv) {
    type = (ActionType)System.Enum.Parse(typeof(ActionType), stype, true);
    if (!System.Enum.IsDefined(typeof(ActionType), type)) {
      Debug.LogError("Unknown ActionType: \"" + stype + "\"");
    }
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
        id1 = SafeParse(typeof(Chars), vid1);
        dir = (Dir)SafeParse(typeof(Dir), dv);
      }
      break;

      case ActionType.Expression: {
        id1 = SafeParse(typeof(Chars), vid1);
        dir = (Dir)SafeParse(typeof(Dir), dv);

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
        id1 = SafeParse(typeof(ItemEnum), vid1);
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

      case ActionType.EnableDisable: {
        id1 = SafeParse(typeof(ItemEnum), vid1);
        val = string.IsNullOrEmpty(sv) ? 1 : 0;
      }
      break;

      case ActionType.ShowRoom: {
        int.TryParse(vid2, out id2); // 0 for immediate, not zero for panning
      }
      break;

      case ActionType.WalkToActor: {
        id1 = SafeParse(typeof(Chars), vid1);
        id2 = SafeParse(typeof(Chars), vid2);
        dir = (Dir)SafeParse(typeof(Dir), dv);
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
        id1 = SafeParse(typeof(Chars), vid1);
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
        id1 = SafeParse(typeof(ItemEnum), vid1);
        val = iv;
      }
      break;

      case ActionType.StopScenes: {
        if (!string.IsNullOrEmpty(vid1)) {
          id1 = SafeParse(typeof(Chars), vid1);
        }
        else if (!string.IsNullOrEmpty(vid2)) {
          id2 = SafeParse(typeof(CutsceneID), vid2);
        }
        else {
          throw new System.Exception("Unspecified scene to stop");
        }
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

      case ActionType.Sound: {
        if (!string.IsNullOrEmpty(vid1)) {
          id1 = SafeParse(typeof(Chars), vid1);
        }
        id2 = SafeParse(typeof(Audios), vid2);
        if (!string.IsNullOrEmpty(dv))
          dir = (Dir)SafeParse(typeof(Dir), dv);
        else
          dir = Dir.None;
      }
      break;

      case ActionType.ChangeSprites: {
        id1 = SafeParse(typeof(ItemEnum), vid1);
        int.TryParse(vid2, out id2); // 0-3 to get the other sprite
        if (id2 < 0 || id2 > 3) {
          Debug.LogError("Invalid value for id2 (only 0, 1, 2, and 3 are valid values)");
          id2 = 0;
        }
        if (val < 0 || val > 3) {
          Debug.LogError("Invalid value for val (only 0, 1, 2, and 3 are valid values)");
          val = 0;
        }
      }
      break;


      case ActionType.Anim: {
        if (!string.IsNullOrEmpty(vid1)) {
          id1 = SafeParse(typeof(Chars), vid1);
        }
        if (!string.IsNullOrEmpty(vid2)) {
          id2 = SafeParse(typeof(ItemEnum), vid2);
        }
      }
      break;

      case ActionType.AlterItem: {
        id1 = SafeParse(typeof(ItemEnum), vid1);
        dir = (Dir)SafeParse(typeof(Dir), dv);

        char l = (sv + "r").ToLowerInvariant()[0];
        char r = (sv + "rr").ToLowerInvariant()[1];

        switch(l) {
          case 'w': id2 = (int)WhatItDoes.Walk; break;
          case 'r': id2 = (int)WhatItDoes.Read; break;
          case 'p': id2 = (int)WhatItDoes.Pick; break;
          case 'u': id2 = (int)WhatItDoes.Use; break;
        }

        switch(r) {
          case 'w': val = (int)WhatItDoes.Walk; break;
          case 'r': val = (int)WhatItDoes.Read; break;
          case 'p': val = (int)WhatItDoes.Pick; break;
          case 'u': val = (int)WhatItDoes.Use; break;
        }
      }
      break;

      case ActionType.Pick: {
        id1 = SafeParse(typeof(Chars), vid1);
        id2 = SafeParse(typeof(ItemEnum), vid1);
      }
      break;

    }
  }

  private int SafeParse(System.Type type, string val) {
    try {
      object en = System.Enum.Parse(type, val, true);
      if (!System.Enum.IsDefined(type, en)) {
        Debug.LogError("Unknown " + type + ": \"" + val + "\"");
        return 0;
      }
      return (int)en;
    } catch (System.Exception) {
      throw new System.Exception("Failed enum parsing: " + type + " \"" + val + "\"");
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

  internal void CheckTime(float deltaTime) {
    if (time > 0) {
      time -= deltaTime;
      if (time <= 0) {
        running = Running.Completed;
        time = delay;
      }
    }
    else if (!(type == ActionType.Speak || type == ActionType.Expression || type == ActionType.WalkToPos || type == ActionType.WalkToActor || 
        type == ActionType.Sound || type == ActionType.Fade || type == ActionType.Anim || type == ActionType.CompleteStep || type == ActionType.Wait))
      running = Running.Completed;
  }


  public void RunAction(Actor performer, Actor secondary, bool skipped) {
//    Debug.Log("Playing: " + ToString());
    switch (type) {
      case ActionType.ShowRoom: {
        if (skipped) {
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
        a.Stop();
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
          a.WalkToPos(pos, p,
          new System.Action<Actor, Item>((actor, item) => {
            actor.SetDirection(copy.dir);
            copy.Complete();
          }));
        }
        else {
          Debug.LogError("No collider for walk");
          Complete();
        }
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
        walker.Stop();
        GameAction copy = this;
        Play();
        walker.WalkToActor(destAct.transform, dir, this);
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
        // Are we disabling inside a container?
        Container c = item.transform.parent.GetComponent<Container>();
        if (c != null) {
          c.EnableItem(item, val == 0);
        }
        else {
          item.gameObject.SetActive(val == 0);
          item.isEnabled = val == 0;
        }
        Complete();
      }
      break;

      case ActionType.Cutscene: {
        CutsceneID id = (CutsceneID)id1;
        if (id != CutsceneID.NONE) GameScenesManager.StartScene(AllObjects.GetCutscene(id));
        Complete();
      }
      break;

      case ActionType.StopScenes: {
        if (id1 != 0) {
          Chars id = (Chars)id1;
          GameScenesManager.StopScenesForChar(id);
        }
        else {
          CutsceneID id = (CutsceneID)id2;
          GameScenesManager.StopScene(AllObjects.GetCutscene(id), false);
        }
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
        if (a != null) { // Play on an actor
          if (dir != Dir.None) a.SetDirection(dir);
          Sounds.Play((Audios)id2, a.transform.position);
          Play();
        }
        else { // Play continously somewhere or stop it (depending on the Dir value)
          if (dir == Dir.None) { // Stop it
            Sounds.StopContinously((Audios)id2);
          }
          else { // Play it
            Sounds.PlayContinously((Audios)id2, pos);
          }
          Complete();
        }
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
            if (id != CutsceneID.NONE) GameScenesManager.StartScene(AllObjects.GetCutscene(id));
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
        if (skipped) {
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
        if (item.PlayAnim(str, delay))
          Complete();
        else
          Play();
      }
      break;

      case ActionType.AlterItem: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        item.whatItDoesL = (WhatItDoes)id2;
        item.whatItDoesR = (WhatItDoes)val;
        item.dir = dir;
        if (item.Usable == Tstatus.Pickable && item.whatItDoesR == WhatItDoes.Use) item.Usable = Tstatus.Usable;
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

      case ActionType.SwitchFlashlight: {
        Controller.SwitchFlashLight((BatteriesUsed)id1);
        Complete();
      }
      break;

      case ActionType.SwitchRoomLight: {
        if (string.IsNullOrEmpty(str)) {
          if (val == 0) GD.globalLights = true;
          else if (val == 1) GD.globalLights = false;
          else if (val == 2) GD.globalLights = !GD.globalLights;
          foreach (Room r in AllObjects.RoomList) {
            r.UpdateLights();
          }
        }
        else {
          Room r = AllObjects.GetRoom(str);
          if (r == null) {
            Debug.LogError("Unknown room: " + str);
            return;
          }
          if (val == 0) r.SetLights(LightMode.On);
          if (val == 1) r.SetLights(LightMode.Off);
          else if (val == 2) { 
            if (r.lights == LightMode.On)
              r.SetLights(LightMode.Off);
            else
              r.SetLights(LightMode.On);
          }
        }
        Complete();
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
        CursorHandler.Set((CursorTypes)id1, (CursorTypes)id1);
        if (val == 1 || (val == 2 && !skipped))
          GD.c.StartCoroutine(GD.c.FadeToRoomActor());
        Complete();
      }
      break;


      case ActionType.ChangeSprites: {
        Item item = AllObjects.GetItem((ItemEnum)id1);
        if (id2 == 1) item.openImage = item.closeImage;
        if (id2 == 2) item.openImage = item.lockImage;
        if (id2 == 3) item.openImage = item.iconImage;

        if (val == 0) item.closeImage = item.openImage;
        if (val == 2) item.closeImage = item.lockImage;
        if (val == 3) item.closeImage = item.iconImage;

        if (item.IsOpen())
          item.sr.sprite = item.openImage;
        else
          item.sr.sprite = item.closeImage;
        Complete();
      }
      break;

      case ActionType.Wear: {
        performer.Wear((ItemEnum)id1);
        Complete();
      }
      break;

      case ActionType.ShowMap: {
        GD.c.ShowMap();
        Complete();
      }
      break;

      case ActionType.Pick: {
        Item item = AllObjects.GetItem((ItemEnum)id2);
        if (item.owner == GD.actor1 || item.owner == GD.actor2 || item.owner == GD.actor3) {
          Complete();
          return;
        }
        Actor a = Controller.GetActor((Chars)id1);
        a.inventory.Add(item);
        item.owner = a.id;
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
