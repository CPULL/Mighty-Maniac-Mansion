using System;
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
public enum CursorTypes {  None=0, Examine=3, Wait=4, Open=5, Close=6, PickUp=7, Use=8, Item=9 };


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
public enum GameStatus { NotYetLoaded, IntroVideo, CharSelection, Cutscene, NormalGamePlay, StartGame, Confirming };

/// <summary>
/// Used to specify the type of actions
/// </summary>
public enum ActionType { None = 0, 
  Teleport = 1, // Teleport an actor somewhere 
  Speak = 2, // Have an actor to say something
  Move = 3, // Have an actor to walk to a destination
  Expression = 4, // Set an expression to an actor
  Open = 5, // Open or close a door
  Enable = 6, // Enable or disable an item
  Lock = 7, // Lock or unlock a door
  ShowRoom = 8, // Jumps to a specific room enabling it
  Cutscene = 9, // Starts a Cutscene (FIXME not done)
  Sound = 10, // Play a sound
  ReceiveY = 11, // Have an actor to receive an item from another actor and say something (item is accepted)
  ReceiveN = 12 // Have an actor to deny the receival of an item from another actor and say something (item is not accepted)
};

/// <summary>
/// List of all actors and generic actor references, like Actor1
/// </summary>
public enum Chars {
  None = 0, Current = 1, Actor1 = 2, Actor2 = 3, Actor3 = 4, KidnappedActor = 5, Receiver = 6, Unused07 = 7, Unused08 = 8, Unused09 = 9,
  Fred = 10, Edna = 11, Ted = 12, Ed = 13, Edwige = 14, GreenTentacle = 15, PurpleTentacle = 16, Unused1 = 18, Unused18 = 18, Unused19 = 19,
  Dave = 20, Bernard = 21, Wendy = 22, Syd = 23, Hoagie = 24, Razor = 25, Michael = 26, Jeff = 27, Javid = 28, Laverne = 29, Ollie = 30, Sandy = 31,
  Unused32 = 32, Unused33 = 33, Unused34 = 34, Unused35 = 35, Unused36 = 36, Unused37 = 37, Unused38 = 38, Unused39 = 39, 
  otheractorslikepolice = 40 // FIXME this name will change
};

/// <summary>
/// Used to control facial exprfessions
/// </summary>
public enum Expression { Normal = 2, Happy = 0, Sad = 1, Open = 3, BigOpen = 4 };


// Define an extension method in a non-nested static class.
public static class Enums {
  public static Expression GetExp(string val) {
    char v = char.ToLowerInvariant((val+" ")[0]);
    if (v == 'h') return Expression.Happy;
    if (v == 's') return Expression.Sad;
    if (v == 'o') return Expression.Open;
    if (v == 'b') return Expression.BigOpen;
    return Expression.Normal;
  }

  public static int GetSnd(string val) {
    string v = val.ToLowerInvariant();
    if (v == "doorbell") return 0;
    return -1;
  }

  internal static GameStatus GetStatus(string val, GameStatus status) {
    string v = val.ToLowerInvariant();
    if (v == "video") return GameStatus.IntroVideo;
    if (v == "charsel") return GameStatus.CharSelection; 
    if (v == "cutscene") return GameStatus.Cutscene;
    if (v == "play") return GameStatus.NormalGamePlay;
    return status;
  }

}


/// <summary>
/// Used for Sequences and Actions
/// </summary>
public enum Running { NotStarted, Running, Completed };


/// <summary>
/// Used to transition between rooms 
/// </summary>
public enum TransitionType { ScrollL, ScrollR, ScrollU, ScrollD, ZoomIn, ZoomOut };


/// <summary>
/// Used to list all possible sounds and musics
/// </summary>
public enum Audios { Doorbell = 0 };






/// <summary>
/// Skills the actor have, they can be used inside conditions
/// </summary>
public enum Skill {
  None,
  Strenght,
  Courage,
  Chef,
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
  OpenableOpen,
  OpenableClosed,
  OpenableLocked,
  OpenableLockedAutolock,
  OpenableOpenAutolock,
  OpenableClosedAutolock
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
  Always
}

public enum ChangeWay {
  Ignore = 0,
  EnOpenLock = 1,
  DisCloseUnlock = 2,
  SwapSwitch = 3
}
