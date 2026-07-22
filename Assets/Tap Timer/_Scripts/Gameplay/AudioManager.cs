using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip correctTapClip;
    [SerializeField] private AudioClip wrongTapClip;
    [SerializeField] private AudioClip buttonClickClip;

    private void OnEnable()
    {
        GameEvents.OnRoundStart += HandleRoundStart;
        GameEvents.OnHit += HandleHit;
        GameEvents.OnMiss += HandleMiss;
    }

    private void OnDisable()
    {
        GameEvents.OnRoundStart -= HandleRoundStart;
        GameEvents.OnHit -= HandleHit;
        GameEvents.OnMiss -= HandleMiss;
    }

    private void HandleRoundStart(int round)
    {
        if (round == 1) PlayBgm(); // fires once per StartGame(), restarts BGM from the top each run
    }

    private void HandleHit(int newScore) => PlaySfx(correctTapClip);

    private void HandleMiss() => PlaySfx(wrongTapClip);

    // Wire this to every button's OnClick() in the Inspector (Play, Pause, Resume, Restart).
    public void PlayButtonClickSfx() => PlaySfx(buttonClickClip);

    private void PlayBgm()
    {
        if (bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}