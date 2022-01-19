/// <summary>
/// Used to identify what a cliickable object does
/// </summary>
public enum ItemType { None, Readable, Usable, Pickable, Walkable, Stairs };

/// <summary>
/// Direction of an actor, to use the correct sprites
/// </summary>
public enum Dir { F=0, B=2, L=1, R=3, None = 4 };


/// <summary>
/// Used to specify which cursor to use
/// </summary>
//public enum CursorTypes {  Normal=0, Examine=9, Wait=10, Open=5, Close=6, PickUp=7, Use=8, Item=11 };


/// <summary>
/// Used to show and hide the text
/// </summary>
public enum TextMsgMode {  None, Appearing, Disappearing, Visible };

/// <summary>
/// Used to understand what axis we are talking about
/// </summary>
public enum Axis { X, Y, Z };

/// <summary>
/// Global status of the game
/// </summary>
public enum GameStatus { NotYetLoaded, IntroVideo, CharSelection, NormalGamePlay, StartGame, Confirming, GameOver };

/// <summary>
/// List of all actors and generic actor references, like Actor1
/// </summary>
public enum Chars {
  None = 0, Current = 1, Actor1 = 2, Actor2 = 3, Actor3 = 4, Kidnapped = 5, Receiver = 6, Self = 7, Player = 8, Enemy = 9,
  Fred = 10, Edna = 11, Ted = 12, Ed = 13, Edwige = 14, GreenTentacle = 15, PurpleTentacle = 16, BlueTentacle = 17, PurpleMeteor = 18, Sam = 19,
  Dave = 20, Bernard = 21, Wendy = 22, Syd = 23, Hoagie = 24, Razor = 25, Michael = 26, Jeff = 27, Javid = 28, Laverne = 29, Ollie = 30, Sandy = 31,
  Unused32 = 32, Unused33 = 33, Unused34 = 34, Unused35 = 35, Unused36 = 36, Unused37 = 37, Unused38 = 38, Unused39 = 39, 
  MarkEteer=40, otheractorslikepolice = 41, // FIXME this name will change



  Male = 100, Female = 101
};

/// <summary>
/// Used to control facial exprfessions
/// </summary>
public enum Expression { Normal = 2, Happy = 0, Sad = 1, Open = 3, BigOpen = 4 };


// Define an extension method in a non-nested static class.
public static class Enums {
  public static bool GoodByDefault(this ActionType type) {
    switch (type) {
      case ActionType.None: return true;
      case ActionType.ShowRoom: return true;
      case ActionType.Teleport: return true;
      case ActionType.Speak: return true;
      case ActionType.Expression: return true;
      case ActionType.WalkToPos: return true;
      case ActionType.WalkToActor: return true;
      case ActionType.BlockActorX: return true;
      case ActionType.UnBlockActor: return true;
      case ActionType.Open: return false;
      case ActionType.EnableDisable: return false;
      case ActionType.Cutscene: return true;
      case ActionType.Sound: return true;
      case ActionType.ReceiveCutscene: return false;
      case ActionType.ReceiveFlag: return false;
      case ActionType.Fade: return true;
      case ActionType.Anim: return true;
      case ActionType.AlterItem: return true;
      case ActionType.SetFlag: return true;
      case ActionType.CompleteStep: return true;
      case ActionType.Wait: return true;
      case ActionType.PressAndFlag: return true;
      case ActionType.PressAndItem: return true;
      case ActionType.SwitchRoomLight: return true;
      case ActionType.StopScenes: return true;
      case ActionType.SetCurrentRoomActor: return true;
      case ActionType.Cursor: return true;
      case ActionType.ChangeSprites: return true;
      case ActionType.SwitchFlashlight: return true;
    }
    return false;
  }
}


/// <summary>
/// Used for Sequences and Actions
/// </summary>
public enum Running { NotStarted, Running, WaitingToCompleteAsync, Completed };

/// <summary>
/// Used to transition between rooms 
/// </summary>
public enum TransitionDirection { Left, Right, Up, Down, In, Out };






/// <summary>
/// Skills the actor have, they can be used inside conditions
/// </summary>
public enum Skill {
  None,
  Strenght,
  Courage,
  Culinary,
  Handyman,
  Geek,
  Nerd,
  Music,
  Writing
}

/// <summary>
/// Used to specify if an item has a property and if the property is active or not (like "IsOpen", "IsLocked", etc.)
/// </summary>
public enum Tstatus {
  NotUsable,
  Pickable,
  Usable,
  Openable,
  Swithchable
}

public enum OpenStatus { 
  Open,
  Closed,
  Irrelevant
}

public enum LockStatus { 
  Unlocked,
  UnlockedAutolock,
  Locked,
  Autolock
}

/// <summary>
/// Generic user actions possible in game
/// </summary>
public enum WhatItDoes {
  Use,
  Read,
  Pick,
  Walk
}

/// <summary>
/// Used to check when a coondition of an action should be considered
/// </summary>
public enum When {
  Pick,
  Use,
  Give,
  Cutscene,
  Always,
  UseTogether
}


public enum FloorType {
  None,
  Grass1=1, Grass2=2,
  Concrete=3, Wood=4, Marble=5, Carpet=6
}

public enum BatteriesUsed {
  NoBatteries, OldBatteries, Batteries, PlutoniumBatteries
}
