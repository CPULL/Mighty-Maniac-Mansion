using UnityEngine;
using UnityEngine.UI;

public class StarsBlink : MonoBehaviour {
  public Image b1;
  public Image b2;
  public Image b3;
  public Image b4;
  float time = 0;

  void Update() {
    time += 10 * Time.deltaTime;

    float val = (Mathf.Sin(time * Random.Range(.95f, 1.05f)) + 1.5f) / 2.5f;
    byte c = (byte)(val * 255);
    b1.color = new Color32(c, c, c, 255);

    val = (Mathf.Sin(time * Random.Range(.95f, 1.05f) * 1.5f) + 2) / 3;
    c = (byte)(val * 255);
    b2.color = new Color32(c, c, c, 255);

    val = (Mathf.Sin(2.3f + time * Random.Range(.95f, 1.05f) * 2.5f) + 4) / 5;
    c = (byte)(val * 255);
    b3.color = new Color32(c, c, c, 255);

    val = (Mathf.Sin(3.3f - time * Random.Range(.975f, 1.025f) * 1.35f) + 3) / 4;
    c = (byte)(val * 255);
    b4.color = new Color32(c, c, c, 255);
  }
}
