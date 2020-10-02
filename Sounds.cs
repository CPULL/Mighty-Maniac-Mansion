using UnityEngine;

public class Sounds : MonoBehaviour {
  public AudioClip[] sounds;
  AudioSource source;

  private void Awake() {
    GD.s = this;
    source = GetComponent<AudioSource>();
  }

  public static void Play(Audios sound) {
    if (GD.s == null) return;

    int num = (int)sound;
    if (num < 0 || num >= GD.s.sounds.Length) return;

    GD.s.source.clip = GD.s.sounds[num];
    GD.s.source.Play();
  }

  public static void Play(Audios sound, Vector3 pos) {
    if (GD.s == null) return;

    GD.s.transform.position = pos;
    int num = (int)sound;
    if (num < 0 || num >= GD.s.sounds.Length) return;

    GD.s.source.clip = GD.s.sounds[num];
    GD.s.source.Play();
  }

  public static void Stop() {
    if (GD.s == null) return;
    GD.s.source.Stop();
  }

}
