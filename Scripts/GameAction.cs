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

  OpenClose = 9, // Open or close a door or a container
  EnableDisable = 10, // Enable or disable an item
  Lockunlock = 11, // Lock or unlock a door or a container

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

  public int actor;
  public string str;
  public Vector2 pos;
  public Dir dir;
  public int id;
  public int val;

  public GameAction() {
    type = ActionType.None;
  }
  public GameAction(string stype) {
    string t = stype.ToLowerInvariant();

    if (t == "none") type = ActionType.None;
    else if (t == "showroom") type = ActionType.ShowRoom;
    else if (t == "teleport") type = ActionType.Teleport;
    else if (t == "speak") type = ActionType.Speak;
    else if (t == "say") type = ActionType.Speak;
    else if (t == "expression") type = ActionType.Expression;
    else if (t == "expr") type = ActionType.Expression;
    else if (t == "walk") type = ActionType.WalkToPos;
    else if (t == "walktopos") type = ActionType.WalkToPos;
    else if (t == "walktoactor") type = ActionType.WalkToActor;
    else if (t == "blockactorx") type = ActionType.BlockActorX;
    else if (t == "unblockactor") type = ActionType.UnBlockActor;
    else if (t == "open") type = ActionType.OpenClose;
    else if (t == "openclose") type = ActionType.OpenClose;
    else if (t == "close") type = ActionType.OpenClose;
    else if (t == "enable") type = ActionType.EnableDisable;
    else if (t == "enabledisable") type = ActionType.EnableDisable;
    else if (t == "disable") type = ActionType.EnableDisable;
    else if (t == "lock") type = ActionType.Lockunlock;
    else if (t == "lockunlock") type = ActionType.Lockunlock;
    else if (t == "unlock") type = ActionType.Lockunlock;
    else if (t == "cutscene") type = ActionType.Cutscene;
    else if (t == "sound") type = ActionType.Sound;
    else if (t == "receivecutscene") type = ActionType.ReceiveCutscene;
    else if (t == "receiveflag") type = ActionType.ReceiveFlag;
    else if (t == "fade") type = ActionType.Fade;
    else if (t == "anim") type = ActionType.Anim;
    else if (t == "alteritem") type = ActionType.AlterItem;
    else if (t == "setflag") type = ActionType.SetFlag;
    else Debug.LogError("Unknown type for GameAction: *" + t + "*");
  }

  public GameAction(GameAction gameAction) {
    running = Running.NotStarted;
    time = 0;
    type = gameAction.type;
    Repeatable = gameAction.Repeatable;
    delay = gameAction.delay;
    actor = gameAction.actor;
    str = gameAction.str;
    pos = gameAction.pos;
    dir = gameAction.dir;
    id = gameAction.id;
    val = gameAction.val;
  }

  internal void SetActor(string a) {
    if (a == null) {
      actor = 0;
      return;
    }
    string n = a.ToLowerInvariant();

    if (n == "none") actor = (int)Chars.None;
    if (n == "current") actor = (int)Chars.Current;
    if (n == "actor1") actor = (int)Chars.Actor1;
    if (n == "actor2") actor = (int)Chars.Actor2;
    if (n == "actor3") actor = (int)Chars.Actor3;
    if (n == "receiver") actor = (int)Chars.Receiver;
    if (n == "player") actor = (int)Chars.Player;
    if (n == "enemy") actor = (int)Chars.Enemy;
    if (n == "kidnapped") actor = (int)Chars.KidnappedActor;
    if (n == "player") actor = (int)Chars.Player;
    if (n == "enemy") actor = (int)Chars.Enemy;

    if (n == "fred") actor = (int)Chars.Fred;
    if (n == "edna") actor = (int)Chars.Edna;
    if (n == "ted") actor = (int)Chars.Ted;
    if (n == "ed") actor = (int)Chars.Ed;
    if (n == "edwige") actor = (int)Chars.Edwige;
    if (n == "greententacle") actor = (int)Chars.GreenTentacle;
    if (n == "purpletentacle") actor = (int)Chars.PurpleTentacle;
    if (n == "bluetentacle") actor = (int)Chars.BlueTentacle;
    if (n == "purplemeteor") actor = (int)Chars.PurpleMeteor;

    if (n == "dave") actor = (int)Chars.Dave;
    if (n == "bernard") actor = (int)Chars.Bernard;
    if (n == "wendy") actor = (int)Chars.Wendy;
    if (n == "syd") actor = (int)Chars.Syd;
    if (n == "hoagie") actor = (int)Chars.Hoagie;
    if (n == "razor") actor = (int)Chars.Razor;
    if (n == "michael") actor = (int)Chars.Michael;
    if (n == "jeff") actor = (int)Chars.Jeff;
    if (n == "javid") actor = (int)Chars.Javid;
    if (n == "laverne") actor = (int)Chars.Laverne;
    if (n == "ollie") actor = (int)Chars.Ollie;
    if (n == "sandy") actor = (int)Chars.Sandy;
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
    if (d == "doorbell") id = (int)Audios.Doorbell;
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
    id = i;
  }

  
  internal void Play() {
    running = Running.Running;
    time = delay;
  }

  internal void Complete() {
    Debug.Log("Completed: " + ToString());
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

  public override string ToString() {
    return CalculateName(type, actor, str, pos, dir, id, val);
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
      case ActionType.OpenClose: return (ItemEnum)id + " " + (((FlagValue)val == FlagValue.Yes) ? "Open" : "Close");
      case ActionType.EnableDisable: return (ItemEnum)id + " " + (((FlagValue)val == FlagValue.Yes) ? "Enable" : "Disable");
      case ActionType.Lockunlock: return (ItemEnum)id + " " + (((FlagValue)val == FlagValue.Yes) ? "Lock" : "Unlock");
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



  public void RunAction(Actor performer, Actor secondary) {
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
        Actor a = Controller.GetActor((Chars)actor);
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
        Actor a = Controller.GetActor((Chars)actor);
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
        Actor a = Controller.GetActor((Chars)actor);
        if (a == null) a = performer;
        if (a == null) {
          Complete();
          return;
        }
        a.SetDirection(dir);
        a.SetExpression((Expression)id);
        Play();
      }
      break;

      case ActionType.WalkToPos: {
        Actor a = Controller.GetActor((Chars)actor);
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
        Actor walker = Controller.GetActor((Chars)actor);
        if (walker == null) walker = performer;
        if (walker == null) {
          Debug.LogError("Missing walker in action: " + ToString());
          Complete();
          return;
        }
        Actor destAct = Controller.GetActor((Chars)id);
        if (walker.WalkTo(destAct.transform, (FlagValue)val, this))
          Complete(); // Not possible to reach
      }
      break;

      case ActionType.BlockActorX: {
        Actor act = Controller.GetActor((Chars)actor);
        if ((Chars)actor == Chars.Player) act = GD.c.currentActor;
        if (act == null) {
          Debug.Log("No actor: " + (Chars)actor);
          Complete();
          return;
        }
        act.SetMinMaxX(pos.x, pos.y);
        Complete();
      }
      break;

      case ActionType.UnBlockActor: {
        Actor act = Controller.GetActor((Chars)actor);
        if ((Chars)actor == Chars.Player) act = GD.c.currentActor;
        if (act == null) {
          Debug.Log("No actor: " + (Chars)actor);
          Complete();
          return;
        }
        act.SetMinMaxX(-float.MaxValue, float.MaxValue);
        Complete();
      }
      break;

      case ActionType.OpenClose: {
        Item item = AllObjects.FindItemByID((ItemEnum)id);
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
        Item item = AllObjects.FindItemByID((ItemEnum)id);
        if (item == null) {
          Debug.LogError("Item not defined for Enable");
          Complete();
          return;
        }
        item.gameObject.SetActive((FlagValue)val != FlagValue.No);
        Complete();
      }
      break;

      case ActionType.Lockunlock: {
        Item item = AllObjects.FindItemByID((ItemEnum)id);
        if (item == null) {
          Debug.LogError("Item not defined for Lock");
          Complete();
          return;
        }
        item.ForceLock((FlagValue)val);
        Complete();
      }
      break;

      case ActionType.Cutscene: {
        Controller.StartCutScene(AllObjects.GetCutscene((CutsceneID)id));
        Complete();
      }
      break;

      case ActionType.Sound: {
        Actor a = Controller.GetActor((Chars)actor);
        if (a == null) a = performer;
        if (a != null) {
          if (dir != Dir.None) a.SetDirection(dir);
          Sounds.Play((Audios)id, a.transform.position);
        }
        else
          Sounds.Play((Audios)id, GD.c.currentActor.transform.position);
        Play();
      }
      break;

      case ActionType.ReceiveCutscene: {
        // Are we accpeting?
        if ((FlagValue)val == FlagValue.Yes) { // Yes
          Item item = AllObjects.FindItemByID((ItemEnum)actor);
          performer.inventory.Remove(item);
          secondary.inventory.Add(item);
          item.owner = Controller.GetCharFromActor(secondary);
          Controller.UpdateInventory();
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            Play();

            Controller.StartCutScene(AllObjects.GetCutscene((CutsceneID)id));
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
          Item item = AllObjects.FindItemByID((ItemEnum)actor);
          performer.inventory.Remove(item);
          secondary.inventory.Add(item);
          item.owner = Controller.GetCharFromActor(secondary);
          Controller.UpdateInventory();
          if (secondary != null) {
            secondary.Say(str, this);
            secondary.SetDirection(dir);
            AllObjects.SetFlag((GameFlag)id, FlagValue.Yes);
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
        if (actor != 0) {
          Actor a = Controller.GetActor((Chars)actor);
          Debug.LogError("FIXME anim actor not implemented!"); // FIXME
          Complete();
          return;
        }
        Item item = AllObjects.FindItemByID((ItemEnum)id);
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
        Item item = AllObjects.FindItemByID((ItemEnum)id);
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
        GameFlag flag = (GameFlag)id;
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


