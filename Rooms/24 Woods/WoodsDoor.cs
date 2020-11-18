using UnityEngine;

public class WoodsDoor : MonoBehaviour {
  public Tree[] Trees;
  public Door Door;
  public GameObject HomeSign;
  public uint mode; // 0=no door, 1=next random, 2=home, 3=cemetery

  public void Generate(uint key, float miny, float maxy, float scale) {
    mode = key;

    if (mode == 0) { // No door, activate trees
      Door.gameObject.SetActive(false);
      HomeSign.gameObject.SetActive(false);
      foreach(Tree t in Trees) {
        t.gameObject.SetActive(true);
        t.Randomize(miny, maxy, scale);
      }
    }
    else if (mode == 1) { // Door, with random woods
      Door.gameObject.SetActive(true);
      HomeSign.gameObject.SetActive(false);
      foreach(Tree t in Trees) {
        t.gameObject.SetActive(false);
      }
    }
    else if (mode == 2) { // Door home
      Door.gameObject.SetActive(true);
      HomeSign.gameObject.SetActive(true);
      foreach(Tree t in Trees) {
        t.gameObject.SetActive(false);
      }
      HomeSign.GetComponent<SpriteRenderer>().sortingOrder = ScaleByPosition(HomeSign.transform.position.y, miny, maxy, scale);
    }
    else if (mode == 3) { // Cemetery
      Door.gameObject.SetActive(true);
      HomeSign.gameObject.SetActive(false);
      foreach (Tree t in Trees) {
        t.gameObject.SetActive(false);
      }
    }
  }

  int ScaleByPosition(float y, float minY, float maxY, float scalePerc) {
    float ty = y;
    if (ty < minY) ty = minY;
    if (ty > maxY) ty = maxY;
    float scaley = -.05f * (ty - minY - 1.9f) + .39f;
    scaley *= scalePerc;
    return (int)(scaley * 10000);
  }

}
