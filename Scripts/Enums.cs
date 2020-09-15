

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
public enum ActionType { Synchro=0, MoveRelative=1, MoveAbsolute=2, Teleport=3, Disappear=4, Speak=5, Expression=6 };

/// <summary>
/// List of all actors and generic actor references, like Actor1
/// </summary>
public enum Chars {
  None = 0, Actor1 = 1, Actor2 = 2, Actor3= 3, KidnappedActor = 4,
  Fred = 5, Edna = 6, Ted = 7, Ed = 8, Edwige = 9, GreenTentacle = 10, PurpleTentacle = 11,
  Dave = 12, Bernard = 13, Hoagie = 14, Michael = 15, Razor = 16, Sandy = 17, Syd = 18, Wendy = 19, Jeff = 20, Javid = 21,
  otheractorslikepolice=99
};

/// <summary>
/// Used to control facial exprfessions
/// </summary>
public enum Expression { Normal = 2, Happy = 0, Sad = 1, Open = 3, BigOpen = 4 };

/// <summary>
/// Used for Sequences and Actions
/// </summary>
public enum Running { NotStarted, Running, Completed };


/// <summary>
/// Used to transition between rooms 
/// </summary>
public enum TransitionType { ScrollL, ScrollR, ScrollU, ScrollD, ZoomIn, ZoomOut };
