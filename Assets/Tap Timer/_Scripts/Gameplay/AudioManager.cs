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
        EventBus<RoundStartedEvent>.Subscribe(HandleRoundStart);
        EventBus<HitEvent>.Subscribe(HandleHit);
        EventBus<MissEvent>.Subscribe(HandleMiss);
        EventBus<ButtonClicked>.Subscribe(PlayButtonClickSfx);
        EventBus<OnBGMToggle>.Subscribe(SetBGMVolume);
    }

    private void OnDisable()
    {
        EventBus<RoundStartedEvent>.Unsubscribe(HandleRoundStart);
        EventBus<HitEvent>.Unsubscribe(HandleHit);
        EventBus<MissEvent>.Unsubscribe(HandleMiss);
        EventBus<ButtonClicked>.Unsubscribe(PlayButtonClickSfx);
        EventBus<OnBGMToggle>.Unsubscribe(SetBGMVolume);
    }

    private void HandleRoundStart(RoundStartedEvent e)
    {
        if (e.Round == 1) PlayBgm(); // fires once per StartGame(), restarts BGM from the top each run
    }

    private void HandleHit(HitEvent e) => PlaySfx(correctTapClip);

    private void HandleMiss(MissEvent e) => PlaySfx(wrongTapClip);

    // Wire this to every button's OnClick() in the Inspector (Play, Pause, Resume, Restart).
    public void PlayButtonClickSfx(ButtonClicked e) => PlaySfx(buttonClickClip);

    private void PlayBgm()
    {
        if (bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    private void SetBGMVolume(OnBGMToggle e)
    {
        bgmSource.volume = e.Volume;
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}