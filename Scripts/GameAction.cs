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

  Cutscene = 12, // Starts a Cutscene
  Sound = 13, // Play a sound
  ReceiveCutscene = 14, // Have an actor to receive an item from another actor, accept or decline, say something, and start a cutscene
  ReceiveFlag = 15, // Have an actor to receive an item from another actor, accept or decline, say something, and set a flag

  Fade = 16, // Fade the screen in or out
  Anim = 17, // Make an animation to play on an object or on an actor
  AlterItem = 18, // Changes what you can do with an item
  SetFlag = 19, // Sets a flag
};


[System.Serializable]
public class GameAction {
  public Running running = Running.NotStarted; // FIXME check that we have these values: BehaviorActonStatus
  private float time;


  public ActionType type;
  public bool Repeatable; // Can the action be repeated?
  public float delay; // Delay to use when playing the action

  public int id1;
  public int id2;
  public string str;
  public Vector2 pos;
  public Dir dir;
  public int val;

  public GameAction() {
    type = ActionType.None;
  }

  public GameAction(GameAction gameAction) {
    running = Running.NotStarted;
    time = 0;
    type = gameAction.type;
    Repeatable = gameAction.Repeatable;
    delay = gameAction.delay;
    id1 = gameAction.id1;
    str = gameAction.str;
    pos = gameAction.pos;
    dir = gameAction.dir;
    id2 = gameAction.id2;
    val = gameAction.val;
  }

  public GameAction(string stype, bool rep, float del, string vid1, string vid2, string sv, int iv, string dv, Vector2 vv) {
    type = (ActionType)System.Enum.Parse(typeof(ActionType), stype, true);
    if (!System.Enum.IsDefined(typeof(ActionType), type)) {
      Debug.LogError("Unknown ActionType: \"" + stype + "\"");
    }
    Repeatable = rep;
    delay = del;

    // id1 and id2 should be parsed based on the type
    switch(type) {
      case ActionType.Teleport:
      case ActionType.Speak: {
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

      case ActionType.Open: {
        ItemEnum id = (ItemEnum)System.Enum.Parse(typeof(ItemEnum), vid1, true);
        if (!System.Enum.IsDefined(typeof(ItemEnum), id)) {
          Debug.LogError("Unknown Item: \"" + vid1 + "\"");
        }
        id1 = (int)id;
      }
      break;

      case ActionType.ShowRoom:
        int.TryParse(vid2, out id2); // 0 for immediate, not zero for panning
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
      }
      break;

      case ActionType.Expression: {
        Chars id = (Chars)System.Enum.Parse(typeof(Chars), vid1, true);
        if (!System.Enum.IsDefined(typeof(Chars), id)) {
          Debug.LogError("Unknown Chars: \"" + vid1 + "\"");
        }
        id1 = (int)id;
        id2 = (int)Expression.Normal;
        char ex = (sv.ToLowerInvariant() + "n")[0];
        switch(ex) {
          case 'h': id2 = (int)Expression.Happy; break;
          case 's': id2 = (int)Expression.Sad; break;
          case 'n': id2 = (int)Expression.Normal; break;
          case 'o': id2 = (int)Expression.Open; break;
          case 'b': id2 = (int)Expression.BigOpen; break;
        }
      }
      break;


    }

    str = sv;
    val = iv;
    pos = vv;


  }

  public override string ToString() {
    string res = "Action name not yet calculated";
    switch (type) {
      case ActionType.None: return "No action";
      case ActionType.ShowRoom: return "Show room " + str + (id2 == 0 ? "" : " [panning]");
      case ActionType.Teleport: return "Teleport " + (Chars)id1 + " [" + pos.x + "," + pos.y + "]";
      case ActionType.Speak: return (Chars)id1 + " say: \"" + str.Substring(0, str.Length > 10 ? 10 : str.Length).Replace("\n", "");
      case ActionType.Expression: return (Chars)id1 + " " + (Expression)id2;
      case ActionType.WalkToPos: return (Chars)id1 + " walk [" + pos.x + "," + pos.y + "]";
      case ActionType.WalkToActor: return (Chars)id1 + " walk [" + (Chars)id2 + "]";
      case ActionType.BlockActorX: return (Chars)id1 + " block [" + pos.x + "," + pos.y + "]";
      case ActionType.UnBlockActor: return (Chars)id1 + " unblock";
      case ActionType.Open: return (ItemEnum)id1 + " = " + str;
      case ActionType.EnableDisable: return (ItemEnum)id1 + " " + (((FlagValue)val == FlagValue.Yes) ? "Enable" : "Disable");
      case ActionType.Cutscene: return "Cutscene: " + (CutsceneID)id1;
      case ActionType.Sound: return "Sound: " + (Audios)id1;
      case ActionType.ReceiveCutscene: return "";
      case ActionType.ReceiveFlag: return "";
      case ActionType.Fade: return "";
      case ActionType.Anim: return "";
      case ActionType.AlterItem: return "Alter " + (ItemEnum)id1 + " " + str;
      case ActionType.SetFlag: return "Set " + (GameFlag)id1 + " " + (FlagValue)val;
    }

    return res;
  }


  internal void SetActor(string a) {
    if (a == null) {
      id1 = 0;
      return;
    }
    string n = a.ToLowerInvariant();

    if (n == "none") id1 = (int)Chars.None;
    if (n == "current") id1 = (int)Chars.Current;
    if (n == "actor1") id1 = (int)Chars.Actor1;
    if (n == "actor2") id1 = (int)Chars.Actor2;
    if (n == "actor3") id1 = (int)Chars.Actor3;
    if (n == "receiver") id1 = (int)Chars.Receiver;
    if (n == "player") id1 = (int)Chars.Player;
    if (n == "enemy") id1 = (int)Chars.Enemy;
    if (n == "kidnapped") id1 = (int)Chars.KidnappedActor;
    if (n == "player") id1 = (int)Chars.Player;
    if (n == "enemy") id1 = (int)Chars.Enemy;

    if (n == "fred") id1 = (int)Chars.Fred;
    if (n == "edna") id1 = (int)Chars.Edna;
    if (n == "ted") id1 = (int)Chars.Ted;
    if (n == "ed") id1 = (int)Chars.Ed;
    if (n == "edwige") id1 = (int)Chars.Edwige;
    if (n == "greententacle") id1 = (int)Chars.GreenTentacle;
    if (n == "purpletentacle") id1 = (int)Chars.PurpleTentacle;
    if (n == "bluetentacle") id1 = (int)Chars.BlueTentacle;
    if (n == "purplemeteor") id1 = (int)Chars.PurpleMeteor;

    if (n == "dave") id1 = (int)Chars.Dave;
    if (n == "bernard") id1 = (int)Chars.Bernard;
    if (n == "wendy") id1 = (int)Chars.Wendy;
    if (n == "syd") id1 = (int)Chars.Syd;
    if (n == "hoagie") id1 = (int)Chars.Hoagie;
    if (n == "razor") id1 = (int)Chars.Razor;
    if (n == "michael") id1 = (int)Chars.Michael;
    if (n == "jeff") id1 = (int)Chars.Jeff;
    if (n == "javid") id1 = (int)Chars.Javid;
    if (n == "laverne") id1 = (int)Chars.Laverne;
    if (n == "ollie") id1 = (int)Chars.Ollie;
    if (n == "sandy") id1 = (int)Chars.Sandy;
  }

  internal void SetDir(string value) {
    string d = value.ToLowerInvariant();
    if (d == "b") dir = Dir.B;
    if (d == "f") dir = Dir.F;
    if (d == "l") dir = Dir.L;
    if (d == "r") dir = Dir.R;
  }

  internal void SetSound(string value) {
    string d = value.ToLowerInvariant();
    if (d == "doorbell") id2 = (int)Audios.Doorbell;
  }

  internal void SetPos(float x, float y) {
    pos = new Vector2(x, y);
  }

  internal void SetText(string txt) {
    str = txt;
  }
  internal void SetWait(float w) {
    delay = w;
  }

  internal void SetVal(bool m) {
    val = (int)(m ? FlagValue.Yes : FlagValue.No);
  }
  internal void SetID(int i) {
    id2 = i;
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

  public static string CalculateName(ActionType type, int actor, string str, Vector2 pos, Dir dir, int id, int val) {
    switch (type) {
      case ActionType.None: return "No action";
      case ActionType.ShowRoom: return "Show room " + str;
      case ActionType.Teleport: return "Teleport " + (Chars)actor + " [" + pos.x + "," + pos.y + "]";
      case ActionType.Speak: return (Chars)actor + " say: \"" + str.Substring(0, str.Length > 10 ? 10 : str.Length);
      case ActionType.Expression: return (Chars)actor + " " + (Expression)id;
      case ActionType.WalkToPos: return (Chars)actor + " walk [" + pos.x + "," + pos.y + "]";
      case ActionType.WalkToActor: return (Chars)actor + " walk [" + (Chars)id + "]";
      case ActionType.BlockActorX: return (Chars)actor + " block [" + pos.x + "," + pos.y + "]";
      case ActionType.UnBlockActor: return (Chars)actor + " unblock";
      case ActionType.Open: return (ItemEnum)id + " " + (((FlagValue)val == FlagValue.Yes) ? "Open" : "Close");
      case ActionType.EnableDisable: return (ItemEnum)id + " " + (((FlagValue)val == FlagValue.Yes) ? "Enable" : "Disable");
      //case ActionType.Lockunlock: return (ItemEnum)id + " " + (((FlagValue)val == FlagValue.Yes) ? "Lock" : "Unlock");
      case ActionType.Cutscene: return "Cutscene: " + (CutsceneID)id;
      case ActionType.Sound: return "Sound: " + (Audios)id;
      case ActionType.ReceiveCutscene: return "";
      case ActionType.ReceiveFlag: return "";
      case ActionType.Fade: return "";
      case ActionType.Anim: return "";
      case ActionType.AlterItem: return "Alter " + (ItemEnum)id + " " + str;
      case ActionType.SetFlag: return "Set " + (GameFlag)id + " " + (FlagValue)val;
    }
    return type.ToString() + " " + actor + " " + str;
  }



  public void RunAction(Chars perf, Chars recv, ItemEnum item1, ItemEnum item2) {
    Actor performer = Controller.GetActor(perf);
    Actor secondary = Controller.GetActor(recv);
    RunAction(performer, secondary, item1, item2);
  }
  public void RunAction(Actor performer, Actor secondary, ItemEnum item1, ItemEnum item2) {
    Debug.Log("Playing: " + ToString());
    switch (type) {
      case ActionType.ShowRoom: {
        GD.c.currentRoom = AllObjects.GetRoom(str);
        Vector3 rpos = pos;
        rpos.z = -10;
        GD.c.cam.transform.position = rpos;
        foreach (Room r in GD.a.roomsList)
          r.gameObject.SetActive(false);
        GD.c.currentRoom.gameObject.SetActive(true);
        foreach (Actor a in GD.c.allActors) {
          if (a == null) continue;
          a.gameObject.SetActive(a.currentRoom == GD.c.currentRoom);
        }
        foreach (Actor a in GD.c.allEnemies) {
          if (a == null) continue;
          a.gameObject.SetActive(a.currentRoom == GD.c.currentRoom);
        }
        Complete();
      }
      break;

      case ActionType.Teleport: {
        Actor a = Controller.GetActor((Chars)id1);
        Room aroom = AllObjects.GetRoom(str);
        if (aroom != null) {
          a.currentRoom = aroom;
          a.gameObject.SetActive(aroom == GD.c.currentRoom);
        }
        a.transform.position = pos;
        a.SetDirection(dir);
        RaycastHit2D hit = Physics2D.Raycast(pos, GD.c.cam.transform.forward, 10000, GD.c.pathLayer);
        if (hit.collider != null) {
          PathNode p = hit.collider.GetComponent<PathNode>();
          a.SetScaleAndPosition(pos, p);
        }
        else {
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
        if (hit.collider != null) {
          PathNode p = hit.collider.GetComponent<PathNode>();
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
        if (walker.WalkTo(destAct.transform, (FlagValue)val, this))
          Complete(); // Not possible to reach
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
        Item item = AllObjects.FindItemByID((ItemEnum)id2);
        if (item == null) {
          Debug.LogError("Item not defined for Open " + ToString());
          Complete();
          return;
        }
        item.ForceOpen((FlagValue)val);
        Complete();
      }
      break;

      case ActionType.EnableDisable: {
        Item item = AllObjects.FindItemByID((ItemEnum)id2);
        if (item == null) {
          Debug.LogError("Item not defined for Enable");
          Complete();
          return;
        }
        item.gameObject.SetActive((FlagValue)val != FlagValue.No);
        Complete();
      }
      break;

      /*
      case ActionType.Lockunlock: {
        Item item = AllObjects.FindItemByID((ItemEnum)id2);
        if (item == null) {
          Debug.LogError("Item not defined for Lock");
          Complete();
          return;
        }
        item.ForceLock((FlagValue)val);
        Complete();
      }
      break;
      */

      case ActionType.Cutscene: {
        Controller.StartCutScene(AllObjects.GetCutscene((CutsceneID)id2));
        Complete();
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
        if ((FlagValue)val == FlagValue.Yes) { // Yes
          Item item = AllObjects.FindItemByID((ItemEnum)id1);
          performer.inventory.Remove(item);
          secondary.inventory.Add(item);
          item.owner = Controller.GetCharFromActor(secondary);
          Controller.UpdateInventory();
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            Play();

            Controller.StartCutScene(AllObjects.GetCutscene((CutsceneID)id2));
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
        if ((FlagValue)val == FlagValue.Yes) { // Yes
          Item item = AllObjects.FindItemByID((ItemEnum)id1);
          performer.inventory.Remove(item);
          secondary.inventory.Add(item);
          item.owner = Controller.GetCharFromActor(secondary);
          Controller.UpdateInventory();
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            AllObjects.SetFlag((GameFlag)id2, FlagValue.Yes);
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
        if ((FlagValue)val == FlagValue.Yes)
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
        Item item = AllObjects.FindItemByID((ItemEnum)id2);
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
        Item item = AllObjects.FindItemByID((ItemEnum)id2);
        switch (str[0]) {
          case 'R': item.whatItDoesL = WhatItDoes.Read; break;
          case 'W': item.whatItDoesL = WhatItDoes.Walk; break;
          case 'P': item.whatItDoesL = WhatItDoes.Pick; break;
          case 'U': item.whatItDoesL = WhatItDoes.Use; break;
        }
        switch (str[1]) {
          case 'R': item.whatItDoesR = WhatItDoes.Read; break;
          case 'W': item.whatItDoesR = WhatItDoes.Walk; break;
          case 'P': item.whatItDoesR = WhatItDoes.Pick; break;
          case 'U': item.whatItDoesR = WhatItDoes.Use; break;
        }
        item.dir = dir;
        Complete();
      }
      break;

      case ActionType.SetFlag: {
        GameFlag flag = (GameFlag)id2;
        AllObjects.SetFlag(flag, (FlagValue)val);
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


