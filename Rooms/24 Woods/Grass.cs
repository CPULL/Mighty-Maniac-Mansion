using UnityEngine;

public class Grass : MonoBehaviour {
  public SpriteRenderer sr;
  public Room Woods;

  Vector3 pos = Vector3.zero;

  private void Start() {
    pos = transform.localPosition;
    Randomize();
  }

  public void Randomize() {
    transform.localPosition = pos + Vector3.up * Random.Range(-.1f, .1f) + Vector3.right * Random.Range(-.05f, .05f);
    transform.localScale = new Vector3(Random.Range(.275f, .4f), Random.Range(.19f, .25f), 1);

    int baseY = ScaleByPosition(transform.position.y);
    sr.sortingOrder = baseY;
    sr.flipX = Random.Range(0, 2) == 0;
  }

  int ScaleByPosition(float y) {
    float ty = y;
    if (ty < Woods.minY) ty = Woods.minY;
    if (ty > Woods.maxY) ty = Woods.maxY;
    float scaley = -.05f * (ty - Woods.minY - 1.9f) + .39f;

    scaley *= Woods.scalePerc;
    return (int)(scaley * 10000);
  }



  // FIXME
  void Update() {
    if (Input.GetKeyDown(KeyCode.Z)) Randomize();
  }
}