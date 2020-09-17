[System.Serializable]
public class GameCondition {

  // Specific actor
  // Item owned by current actor
  // Interactable active
  public Chars actor;
  public Item item;
  public int num;
  public Condition condition;
  public string actionID;


  internal string Verify() {
    return null; // FIXME a string telling why it cannot be executed (but only if mandatory)
  }
}



public enum Condition {
  None,
  CurrentActorEqual,
  CurrentActorNotEqual,
  ActorIsAvailable,
  HasItem,
  DoesNotHaveItem,
  ItemIsOpen,
  ItemIsLocked,
  ItemIsClosed,
  ActionCompleted,
  ActionNotStarted,
  ActionRunning
}
