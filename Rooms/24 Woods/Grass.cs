using System.Collections;
using UnityEngine;

public class Grass : MonoBehaviour {
  public SpriteRenderer sr;
  float minY, maxY, scalePerc;

  Vector3 pos = Vector3.zero;

  private void Start() {
    pos = transform.localPosition;
  }

  public void Randomize(float miny, float maxy, float scale) {
    minY = miny;
    maxY = maxy;
    scalePerc = scale;
    transform.localPosition = pos + Vector3.up * Random.Range(-.2f, .2f) + Vector3.right * Random.Range(-.1f, .1f);
    transform.localScale = new Vector3(Random.Range(.275f, .5f), Random.Range(.19f, .3f), 1);

    int baseY = ScaleByPosition(transform.position.y);
    sr.sortingOrder = baseY;
    sr.flipX = Random.Range(0, 2) == 0;

    StartCoroutine(FitPersp());
  }

  int ScaleByPosition(float y) {
    float ty = y;
    float scaley = -.05f * (ty - minY - 1.9f) + .39f;
    scaley *= scalePerc;
    return (int)(scaley * 10000);
  }

  IEnumerator FitPersp() {
    yield return null;
    sr.sortingOrder = ScaleByPosition(transform.position.y);
    yield return null;
    sr.sortingOrder = ScaleByPosition(transform.position.y);
    yield return null;
    sr.sortingOrder = ScaleByPosition(transform.position.y);
  }

  // FIXME
  void Update() {
    if (Input.GetKeyDown(KeyCode.Z)) Randomize(minY, maxY, scalePerc);
  }
}