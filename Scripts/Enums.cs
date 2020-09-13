

// Used to identify what a cliickable object does
public enum ItemType { None, Readable };

// Direction of an actor, to use the correct sprites
public enum Dir { F=0, B=2, L=1, R=3 };


// Used to specify which cursor to use
public enum CursorTypes {  None, Examine };


// Used to show and hide the text
public enum TextMsgMode {  None, Appearing, Disappearing, Visible };

// Used to understand what axis we are talking about
public enum Axis { X, Y, Z };

// Global status of the game
public enum GameStatus { IntroVideo, CharSelection, IntroDialogue, NormalGamePlay };

// Used to specify the type of actions
public enum ActionType { Synchro=0, MoveRelative=1, MoveAbsolute=2, Teleport=3, Disappear=4, Speak=5, Expression=6 };

public enum Chars {
  None = 0, Actor1 = 1, Actor2 = 2, Actor3= 3, KidnappedActor = 4,
  Fred = 5, Edna = 6, Ted = 7, Ed = 8, Edwige = 9, GreenTentacle = 10, PurpleTentacle = 11,
  Dave = 12, Bernard = 13, Hoagie = 14, Michael = 15, Razor = 16, Sandy = 17, Syd = 18, Wendy = 19, Jeff = 20, Javid = 21,
  otheractorslikepolice=99
};

// Used to control facial exprfessions
public enum Expression { Normal = 2, Happy = 0, Sad = 1, Open = 3, BigOpen = 4 };

// Used for Sequences and Actions
public enum Running { NotStarted, Running, Completed }