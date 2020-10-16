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

  public GameAction(string stype) {
    string t = stype.ToLowerInvariant();

    if (t == "none") type = ActionType.None;
    if (t == "showroom") type = ActionType.ShowRoom;
    if (t == "teleport") type = ActionType.Teleport;
    if (t == "speak") type = ActionType.Speak;
    if (t == "say") type = ActionType.Speak;
    if (t == "expression") type = ActionType.Expression;
    if (t == "expr") type = ActionType.Expression;
    if (t == "walk") type = ActionType.WalkToPos;
    if (t == "walktopos") type = ActionType.WalkToPos;
    if (t == "walktoactor") type = ActionType.WalkToActor;
    if (t == "blockactorx") type = ActionType.BlockActorX;
    if (t == "unblockactor") type = ActionType.UnBlockActor;
    if (t == "open") type = ActionType.OpenClose;
    if (t == "openclose") type = ActionType.OpenClose;
    if (t == "close") type = ActionType.OpenClose;
    if (t == "enable") type = ActionType.EnableDisable;
    if (t == "enabledisable") type = ActionType.EnableDisable;
    if (t == "disable") type = ActionType.EnableDisable;
    if (t == "lock") type = ActionType.Lockunlock;
    if (t == "lockunlock") type = ActionType.Lockunlock;
    if (t == "unlock") type = ActionType.Lockunlock;
    if (t == "cutscene") type = ActionType.Cutscene;
    if (t == "sound") type = ActionType.Sound;
    if (t == "receivecutscene") type = ActionType.ReceiveCutscene;
    if (t == "receiveflag") type = ActionType.ReceiveFlag;
    if (t == "fade") type = ActionType.Fade;
    if (t == "anim") type = ActionType.Anim;
    if (t == "alteritem") type = ActionType.AlterItem;
    if (t == "setflag") type = ActionType.SetFlag;

    Debug.LogError("Unknown type for GameAction: *" + t + "*");
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
      case ActionType.Teleport: return "";
      case ActionType.Speak: return "";
      case ActionType.Expression: return "";
      case ActionType.WalkToPos: return "";
      case ActionType.WalkToActor: return "";
      case ActionType.BlockActorX: return "";
      case ActionType.UnBlockActor: return "";
      case ActionType.OpenClose: return "";
      case ActionType.EnableDisable: return "";
      case ActionType.Lockunlock: return "";
      case ActionType.Cutscene: return "";
      case ActionType.Sound: return "";
      case ActionType.ReceiveCutscene: return "";
      case ActionType.ReceiveFlag: return "";
      case ActionType.Fade: return "";
      case ActionType.Anim: return "";
      case ActionType.AlterItem: return "";
      case ActionType.SetFlag: return "";
    }
    return type.ToString() + " " + actor + " " + str;
  }
}


