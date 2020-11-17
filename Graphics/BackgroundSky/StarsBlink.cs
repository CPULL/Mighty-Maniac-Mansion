using UnityEngine;
using UnityEngine.UI;

public class StarsBlink : MonoBehaviour {
  public SpriteRenderer[] bsrs;
  public Image[] bis;
  float time = 0;
  bool byImages = false;

  private void Awake() {
    byImages = (bis != null && bis.Length == 4 && bis[0] != null);
  }

  void Update() {
    time += 10 * Time.deltaTime;

    if (byImages) {
      float val = (Mathf.Sin(time * Random.Range(.95f, 1.05f)) + 1.5f) / 2.5f;
      byte c = (byte)(val * 255);
      bis[0].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(time * Random.Range(.95f, 1.05f) * 1.5f) + 2) / 3;
      c = (byte)(val * 255);
      bis[1].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(2.3f + time * Random.Range(.95f, 1.05f) * 2.5f) + 4) / 5;
      c = (byte)(val * 255);
      bis[2].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(3.3f - time * Random.Range(.975f, 1.025f) * 1.35f) + 3) / 4;
      c = (byte)(val * 255);
      bis[3].color = new Color32(c, c, c, 255);
    }
    else {
      float val = (Mathf.Sin(time * Random.Range(.95f, 1.05f)) + 1.5f) / 2.5f;
      byte c = (byte)(val * 255);
      bsrs[0].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(time * Random.Range(.95f, 1.05f) * 1.5f) + 2) / 3;
      c = (byte)(val * 255);
      bsrs[1].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(2.3f + time * Random.Range(.95f, 1.05f) * 2.5f) + 4) / 5;
      c = (byte)(val * 255);
      bsrs[2].color = new Color32(c, c, c, 255);

      val = (Mathf.Sin(3.3f - time * Random.Range(.975f, 1.025f) * 1.35f) + 3) / 4;
      c = (byte)(val * 255);
      bsrs[3].color = new Color32(c, c, c, 255);
    }
  }
}
