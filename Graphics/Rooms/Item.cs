using UnityEngine;

public class Item : MonoBehaviour {
  SpriteRenderer img;
  public ItemType type;
  public string ItemName;
  [TextArea(3, 10)]
  public string Description;
  public Vector3 InteractionPosition;
  public Dir preferredDirection;
  public Color32 OverColor = Color.yellow;
  public Color32 NotOverColor = Color.white;
  public bool isLocked = false;
  public bool isOpen = false;
  public Sprite[] openSprites;

  private void Awake() {
    img = GetComponent<SpriteRenderer>();
  }


  private void OnMouseEnter() {
    img.color = OverColor;
    Controller.SetCurrentItem(this);
  }

  private void OnMouseExit() {
    img.color = NotOverColor;
    Controller.SetCurrentItem(null);
  }

  internal bool Open() {
    if (type != ItemType.Openable && type != ItemType.Activable) return false;
    if (isLocked) return true;

    isOpen = !isOpen;
    img.sprite = openSprites[isOpen ? 1 : 0];
    return false;
  }

  void OnDrawGizmosSelected() {
    Gizmos.color = new Color(1, 0, 1, 0.75F);
    Vector3 pos = InteractionPosition;

    float roomy = transform.position.y;
    Transform p = transform.parent;
    while (p != null) {
      Room r = p.GetComponent<Room>();
      if (r == null) p = p.parent;
      else {
        roomy = r.ground;
        p = null;
      }
    }

    pos.y = roomy;
    Gizmos.DrawSphere(pos, .1f);
  }
}
