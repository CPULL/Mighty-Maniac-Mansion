using System;
/// <summary>
/// Used to identify what a cliickable object does
/// </summary>
public enum ItemType { None, Readable, Usable, Pickable, Walkable, Stairs };

/// <summary>
/// Direction of an actor, to use the correct sprites
/// </summary>
public enum Dir { F=0, B=2, L=1, R=3, None = 99 };


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
public enum GameStatus { IntroVideo, CharSelection, IntroDialogue, NormalGamePlay, RoomTransition };

/// <summary>
/// Used to specify the type of actions
/// </summary>
public enum ActionType { None = 0, Teleport = 1, Speak = 2, Move = 3, Expression = 4, Open = 5, Enable = 6, Lock = 7, ShowRoom = 8, SetSequence = 9, Sound = 10 };

/// <summary>
/// List of all actors and generic actor references, like Actor1
/// </summary>
public enum Chars {
  None = 0, Current = 1, Actor1 = 2, Actor2 = 3, Actor3 = 4, KidnappedActor = 5,
  Fred = 6, Edna = 7, Ted = 8, Ed = 9, Edwige = 10, GreenTentacle = 11, PurpleTentacle = 12,
  Dave = 13, Bernard = 14, Hoagie = 15, Michael = 16, Razor = 17, Sandy = 18, Syd = 19, Wendy = 20, Jeff = 21, Javid = 22,
  otheractorslikepolice = 23 // FIXME this name will change
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
    if (v == "intro") return GameStatus.IntroDialogue;
    if (v == "normal") return GameStatus.NormalGamePlay;
    if (v == "play") return GameStatus.NormalGamePlay;
    if (v == "transition") return GameStatus.RoomTransition;
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
  OpenableLocked
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







