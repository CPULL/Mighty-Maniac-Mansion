using System;
using UnityEngine;

[System.Serializable]
public class GameAction {
  public ActionType type;
  private Running running = Running.NotStarted;

  public Chars actor;
  public Chars other;
  public Vector3 pos;
  public Dir dir;
  public Expression expr;
  public string msg;

  public float time;

  public GameAction(string stype) {
    string t = stype.ToLowerInvariant();
    if (t == "synchro") type = ActionType.Synchro;
    if (t == "teleport") type = ActionType.Teleport;
    if (t == "speak") type = ActionType.Speak;
    if (t == "expression") type = ActionType.Expression;
    if (t == "disappear") type = ActionType.Disappear;
    if (t == "moveabsolute") type = ActionType.MoveAbsolute;
    if (t == "moverelative") type = ActionType.MoveRelative;
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

  internal void SetOther(string a) {
    if (a == null) {
      other = Chars.None;
      return;
    }
    string n = a.ToLowerInvariant();
    if (n == "none") other = Chars.None;
    if (n == "fred") other = Chars.Fred;
    if (n == "edna") other = Chars.Edna;
    if (n == "ted") other = Chars.Ted;
    if (n == "ed") other = Chars.Ed;
    if (n == "edwige") other = Chars.Edwige;
    if (n == "greententacle") other = Chars.GreenTentacle;
    if (n == "purpletentacle") other = Chars.PurpleTentacle;
    if (n == "actor1") other = Chars.Actor1;
    if (n == "actor2") other = Chars.Actor2;
    if (n == "actor3") other = Chars.Actor3;
    if (n == "kidnappedactor") other = Chars.KidnappedActor;
    if (n == "dave") other = Chars.Dave;
    if (n == "bernard") other = Chars.Bernard;
    if (n == "hoagie") other = Chars.Hoagie;
    if (n == "michael") other = Chars.Michael;
    if (n == "razor") other = Chars.Razor;
    if (n == "sandy") other = Chars.Sandy;
    if (n == "syd") other = Chars.Syd;
    if (n == "wendy") other = Chars.Wendy;
    if (n == "jeff") other = Chars.Jeff;
    if (n == "javid") other = Chars.Javid;
  }

  internal void SetDir(string value) {
    string d = value.ToLowerInvariant();
    if (d == "b") dir = Dir.B;
    if (d == "f") dir = Dir.F;
    if (d == "l") dir = Dir.L;
    if (d == "r") dir = Dir.R;
  }

  internal void SetPos(float x, float y, float z) {
    pos = new Vector3(x, y, z);
  }

  internal void SetText(string txt) {
    msg = txt;
  }

  internal void SetExpr(string exp) {
    string e = exp.ToLowerInvariant();
    expr = Expression.Normal;
    if (e == "happy") expr = Expression.Happy;
    if (e == "sad") expr = Expression.Sad;
    if (e == "open") expr = Expression.Open;
    if (e == "bigopen") expr = Expression.BigOpen;
  }

  internal void SetWait(float w) {
    time = w;
  }

  internal void Play() {
    running = Running.Running;
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
    string res = type.ToString();
    if (type == ActionType.Teleport) res += " " + actor.ToString();
    if (type == ActionType.Speak) res += " " + actor.ToString() + " : " + msg;
    // FIXME do the others

    return res;
  }

  internal void AddTime(float deltaTime) {
    if (time > 0) {
      time -= deltaTime;
      if (time <= 0) {
        running = Running.Completed;
      }
    }
  }
}

public class Condition {
  public string description;
}


/* Param:
 *  Teleport -> Direction of actor
 *  Expression -> 0 happy, 1 sad, 2 none, 3 open, 4 bigopen
 * 
 */



/*
 Conditions?

Time?
Some action completed?
Some object acquired?
Particular char available?
Going in a location?
 
 */