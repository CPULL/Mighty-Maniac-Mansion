using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour {
  public Sprite[] Trunks;
  public SpriteRenderer Trunk;
  public SpriteRenderer[] Leafs;
  float minY, maxY, scalePerc;

  Vector3 pos = Vector3.zero;

  private void Start() {
    pos = transform.localPosition;
  }

  public void Randomize(float miny, float maxy, float scale, float w=1, float h = 1) {
    minY = miny;
    maxY = maxy;
    scalePerc = scale;
    transform.localPosition = pos + h * Vector3.up * Random.Range(-.15f, .15f) + w * Vector3.right * Random.Range(-.3f, .3f);
    transform.localScale = new Vector3((Random.Range(0, 2) == 0 ? 1 : -1) * Random.Range(.45f, .55f), Random.Range(.95f, 1.05f), 1);

    int baseY = ScaleByPosition(transform.position.y);
    Trunk.sprite = Trunks[Random.Range(0, Trunks.Length)];
    Trunk.sortingOrder = baseY;
    foreach (SpriteRenderer sr in Leafs) {
      if (Random.Range(0, 20) == 0) {
        sr.enabled = false;
      }
      else {
        sr.enabled = true;
        sr.sortingOrder = baseY + Random.Range(-5, 5);
        sr.transform.localPosition = new Vector3(Random.Range(-.01f, .01f), 2 + Random.Range(-.01f, .01f), 0);
        sr.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
      }
    }
    Trunk.flipX = Random.Range(0, 2) == 0;

    StartCoroutine(FitPersp());
  }

  int ScaleByPosition(float y) {
    float ty = y;
    if (ty < minY) ty = minY;
    if (ty > maxY) ty = maxY;
    float scaley = -.05f * (ty - minY - 1.9f) + .39f;
    scaley *= scalePerc;
    return (int)(scaley * 10000);
  }

  IEnumerator FitPersp() {
    yield return null;
    yield return null;
    int baseY = ScaleByPosition(transform.position.y);
    Trunk.sortingOrder = baseY;
    foreach (SpriteRenderer sr in Leafs) {
      sr.sortingOrder = baseY + Random.Range(-5, 5);
    }
  }


  // FIXME
  void Update() {
    if (Input.GetKeyDown(KeyCode.Z)) Randomize(minY, maxY, scalePerc);
  }
}
