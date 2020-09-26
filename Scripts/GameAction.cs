using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GameAction {
  public Running running = Running.NotStarted;
  private float time;


  public ActionType type;
  public bool Repeatable;
  public Chars actor;
  public Vector2 pos;
  public string strValue;
  public Expression expression;
  public Audios sound;
  public Dir dir;
  public float delay;
  public ItemEnum item;
  public ActionEnum action;
  public ChangeWay change = ChangeWay.Ignore;

  public GameAction(string stype) {
    string t = stype.ToLowerInvariant();
    if (t == "synchro") type = ActionType.None;
    if (t == "none") type = ActionType.None;
    if (t == "teleport") type = ActionType.Teleport;
    if (t == "speak") type = ActionType.Speak;
    if (t == "move") type = ActionType.Move;
    if (t == "expression") type = ActionType.Expression;
    if (t == "open") type = ActionType.Open;
    if (t == "enable") type = ActionType.Enable;
    if (t == "showroom") type = ActionType.ShowRoom;
    if (t == "setsequence") type = ActionType.Cutscene;
    if (t == "sound") type = ActionType.Sound;
  }

  internal void SetActor(string a) {
    if (a == null) {
      actor = Chars.None;
      return;
    }
    string n = a.ToLowerInvariant();
    if (n == "none") actor = Chars.None;
    if (n == "fred") actor = Chars.Fred;
    if (n == "edna") actor = Chars.Edna;
    if (n == "ted") actor = Chars.Ted;
    if (n == "ed") actor = Chars.Ed;
    if (n == "edwige") actor = Chars.Edwige;
    if (n == "greententacle") actor = Chars.GreenTentacle;
    if (n == "purpletentacle") actor = Chars.PurpleTentacle;
    if (n == "actor1") actor = Chars.Actor1;
    if (n == "actor2") actor = Chars.Actor2;
    if (n == "actor3") actor = Chars.Actor3;
    if (n == "kidnappedactor") actor = Chars.KidnappedActor;
    if (n == "dave") actor = Chars.Dave;
    if (n == "bernard") actor = Chars.Bernard;
    if (n == "hoagie") actor = Chars.Hoagie;
    if (n == "michael") actor = Chars.Michael;
    if (n == "razor") actor = Chars.Razor;
    if (n == "sandy") actor = Chars.Sandy;
    if (n == "syd") actor = Chars.Syd;
    if (n == "wendy") actor = Chars.Wendy;
    if (n == "jeff") actor = Chars.Jeff;
    if (n == "javid") actor = Chars.Javid;
  }

  internal void SetDir(string value) {
    string d = value.ToLowerInvariant();
    if (d == "b") dir = Dir.B;
    if (d == "f") dir = Dir.F;
    if (d == "l") dir = Dir.L;
    if (d == "r") dir = Dir.R;
  }

  internal void SetPos(float x, float y) {
    pos = new Vector2(x, y);
  }

  internal void SetValue(string txt) {
    strValue = txt;
  }
  internal void SetWait(float w) {
    delay = w;
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
    return CalculateName(type, actor.ToString(), item.ToString(), strValue, expression, sound, pos, change);
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

  public static string CalculateName(ActionType type, string actor, string item, string strValue, Expression exp, Audios sound, Vector2 pos, ChangeWay change) {
    string changename = "IGNORED";
    if (change == ChangeWay.SwapSwitch) changename = "Swapped";
    if (change == ChangeWay.EnOpenLock && type == ActionType.Enable) changename = "enabled";
    if (change == ChangeWay.DisCloseUnlock && type == ActionType.Enable) changename = "disabled";
    if (change == ChangeWay.EnOpenLock && type == ActionType.Open) changename = "open";
    if (change == ChangeWay.DisCloseUnlock && type == ActionType.Open) changename = "close";
    if (change == ChangeWay.EnOpenLock && type == ActionType.Lock) changename = "locked";
    if (change == ChangeWay.DisCloseUnlock && type == ActionType.Lock) changename = "unlocked";

    string msg = strValue.Substring(0, System.Math.Min(20, strValue.Length)).Replace("\n", " ").Trim();
    if (strValue.Length > 20) msg += "...";
    switch (type) {
      case ActionType.None: return "No action";
      case ActionType.ShowRoom: return "Show Room " + strValue;
      case ActionType.Teleport: return "Teleport " + actor + " to " + pos.ToString();
      case ActionType.Speak:
        return actor + " say \"" + msg + "\"";
      case ActionType.Expression: return actor + " expression " + exp;
      case ActionType.Sound: return "Play " + sound;
      case ActionType.Enable: return "item " + item + " will be " + changename;
      case ActionType.Open: return "item " + item + " will be " + changename;
      case ActionType.Lock: return "item " + item + " will be " + changename;
      case ActionType.Move: return actor + " moves";
      case ActionType.Cutscene: return "Cutscene: " + strValue;
      case ActionType.ReceiveY: return "Accept item: " + msg;
      case ActionType.ReceiveN: return "Refuse item: " + msg;
    }
    return type.ToString() + " " + actor + " " + item.ToString() + " " + strValue;
  }
}


