using UnityEngine;

public class WoodsDoor : MonoBehaviour {
  public Tree[] Trees;
  public Door DoorHome;
  public Door DoorCemetery;
  public Item DoorFake;
  public GameObject HomeSign;

  public void Generate(byte mode, float miny, float maxy, float scale) {
    if (mode == 0) { // No door, activate trees
      if (HomeSign != null) HomeSign.gameObject.SetActive(false);
      if (DoorHome != null) DoorHome.gameObject.SetActive(false);
      if (DoorCemetery != null) DoorCemetery.gameObject.SetActive(false);
      DoorFake.gameObject.SetActive(false);
      foreach(Tree t in Trees) {
        t.gameObject.SetActive(true);
        t.Randomize(miny, maxy, scale);
      }
    }
    else if (mode == 1) { // Door, with random woods
      if (HomeSign != null) HomeSign.gameObject.SetActive(false);
      if (DoorHome != null) DoorHome.gameObject.SetActive(false);
      if (DoorCemetery != null) DoorCemetery.gameObject.SetActive(false);
      DoorFake.gameObject.SetActive(true);
      foreach(Tree t in Trees) {
        t.gameObject.SetActive(false);
      }
    }
    else if (mode == 2) { // Door home
      if (HomeSign != null) HomeSign.gameObject.SetActive(true);
      if (DoorHome != null) DoorHome.gameObject.SetActive(true);
      if (DoorCemetery != null) DoorCemetery.gameObject.SetActive(false);
      DoorFake.gameObject.SetActive(false);
      foreach(Tree t in Trees) {
        t.gameObject.SetActive(false);
      }
      HomeSign.GetComponent<SpriteRenderer>().sortingOrder = ScaleByPosition(HomeSign.transform.position.y, miny, maxy, scale);
    }
    else if (mode == 3) { // Cemetery
      if (HomeSign != null) HomeSign.gameObject.SetActive(false);
      if (DoorHome != null) DoorHome.gameObject.SetActive(false);
      if (DoorCemetery != null) DoorCemetery.gameObject.SetActive(true);
      DoorFake.gameObject.SetActive(false);
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
