using UnityEngine;

public class Tree : MonoBehaviour {
  public Sprite[] Trunks;
  public SpriteRenderer Trunk;
  public SpriteRenderer[] Leafs;
  public Room Woods;

  Vector3 pos = Vector3.zero;

  private void Start() {
    pos = transform.localPosition;

    Randomize();
  }

  public void Randomize() {
    transform.localPosition = pos + Vector3.up * Random.Range(-.2f, .2f) + Vector3.right * Random.Range(-.2f, .2f);
    transform.localScale = new Vector3((Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(.45f, .55f), Random.Range(.95f, 1.05f), 1);

    int baseY = ScaleByPosition(transform.position.y);
    Trunk.sprite = Trunks[Random.Range(0, Trunks.Length)];
    Trunk.sortingOrder = baseY + 5;
    foreach (SpriteRenderer sr in Leafs) {
      if (Random.Range(0, 10) == 0) {
        sr.enabled = false;
      }
      else {
        sr.enabled = true;
        sr.sortingOrder = baseY + Random.Range(0, 10);
        sr.transform.localPosition = new Vector3(Random.Range(-.01f, .01f), 2 + Random.Range(-.01f, .01f), 0);
        sr.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
      }
    }
    Trunk.flipX = Random.Range(0, 2) == 0;
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
