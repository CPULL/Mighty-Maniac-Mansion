

public class Door : Item {
  public Room src;
  public Room dst;
  public UnityEngine.Vector3 camerapos;
  public Door correspondingDoor;
  public TransitionType transition;

  // FIXME add key or similar
  private void Start() {
    if (type == ItemType.Walkable)
      GetComponent<UnityEngine.SpriteRenderer>().color = new UnityEngine.Color32(0, 0, 0, 0);
  }
}
