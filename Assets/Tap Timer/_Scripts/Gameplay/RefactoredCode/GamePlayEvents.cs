// Request events — raised by UI (buttons/panels), consumed by GameManager.
public struct PlayRequestedEvent : IEvent { }
public struct StartGameRequestedEvent : IEvent { }
public struct PauseRequestedEvent : IEvent { }
public struct ResumeRequestedEvent : IEvent { }
public struct RestartRequestedEvent : IEvent { }

// Notification events — raised by GameManager, consumed by UIManager/AudioManager/etc.
public struct RoundStartedEvent : IEvent { public int Round; }
public struct HitEvent : IEvent { public int NewScore; }
public struct MissEvent : IEvent { }
public struct GameOverEvent : IEvent { public int FinalScore; public int BestScore; }
public struct PausedEvent : IEvent { }
public struct ResumedEvent : IEvent { }
public struct ButtonClicked : IEvent { }
public struct OnBGMToggle : IEvent { public float Volume; }