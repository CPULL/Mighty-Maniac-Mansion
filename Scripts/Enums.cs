/// <summary>
/// Used to identify what a cliickable object does
/// </summary>
public enum ItemType { None, Readable, Openable, Activable, Usable, Pickable, Walkable, Stairs };

/// <summary>
/// Direction of an actor, to use the correct sprites
/// </summary>
public enum Dir { F=0, B=2, L=1, R=3, None = 99 };


/// <summary>
/// Used to specify which cursor to use
/// </summary>
public enum CursorTypes {  None=0, Examine=3, Wait=4, Open=5, Close=6, PickUp=7, Use=8 };


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
public enum ActionType { Synchro = 0, Teleport = 1, Speak = 2, Move = 3, Expression = 4, Open = 5, Enable = 6, ShowRoom = 7, SetSequence = 8, Sound = 9 };

/// <summary>
/// List of all actors and generic actor references, like Actor1
/// </summary>
public enum Chars {
  None = -1, Current = 0, Actor1 = 1, Actor2 = 2, Actor3= 3, KidnappedActor = 4,
  Fred = 5, Edna = 6, Ted = 7, Ed = 8, Edwige = 9, GreenTentacle = 10, PurpleTentacle = 11,
  Dave = 12, Bernard = 13, Hoagie = 14, Michael = 15, Razor = 16, Sandy = 17, Syd = 18, Wendy = 19, Jeff = 20, Javid = 21,
  otheractorslikepolice=99
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