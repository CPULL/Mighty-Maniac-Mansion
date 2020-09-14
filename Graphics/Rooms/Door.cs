

public class Door : Item {
  public Room src;
  public Room dst;
  public UnityEngine.Vector3 camerapos;
  public Door correspondingDoor;

  // FIXME add key or similar
  private void Start() {
    GetComponent<UnityEngine.SpriteRenderer>().color = new UnityEngine.Color32(0, 0, 0, 0);
  }
}
