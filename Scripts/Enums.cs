

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
  None,
  Fred, Edna, Ted, Ed, Edwige,
  GreenTentacle, PurpleTentacle,
  Actor1, Actor2, Actor3, KidnappedActor,
  Dave, Bernard, Hoagie, Michael, Razor, Sandy, Syd, Wendy, Jeff, Javid
};

// Used to control facial exprfessions
public enum Expression { Normal = 2, Happy = 0, Sad = 1, Open = 3, BigOpen = 4 };

// Used for Sequences and Actions
public enum Running { NotStarted, Running, Completed }