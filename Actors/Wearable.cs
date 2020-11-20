using UnityEngine;

public class Wearable : MonoBehaviour {
  public Sprite F;
  public Sprite B;
  public Sprite L;
  public Sprite R;
  public Sprite Back;

  public SpriteRenderer srf, srb;
  Actor parent;
  Dir dir = Dir.None;

  // Start is called before the first frame update
  void Start() {
    parent = transform.parent.GetComponent<Actor>();
    srb.sprite = Back;
    srf.enabled = false;
    srb.enabled = false;
  }

  // Update is called once per frame
  void Update() {
    srf.enabled = parent.IsVisible;
    transform.position = parent.transform.position;
    if (parent.dir != dir) {
      dir = parent.dir;
    }

    switch (dir) {
      case Dir.F:
        srf.sprite = F;
        srb.enabled = true;
        break;
      case Dir.B:
        srf.sprite = B;
        srb.enabled = false;
        break;
      case Dir.L:
        srf.sprite = L;
        srb.enabled = false;
        break;
      case Dir.R:
        srf.sprite = R;
        srb.enabled = false;
        break;
      case Dir.None:
        srf.enabled = false;
        srb.enabled = false;
        break;
    }
  }

  public void SetSortingOrder(int zpos) {
    srf.sortingOrder = zpos + 3;
    srb.sortingOrder = zpos - 1;
  }
}
