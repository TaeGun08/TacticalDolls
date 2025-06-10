using System;

public enum GameState //게임 진행 상황을 나타냅니다.
{
    Waiting,
    Playing,
    Ended
}

public enum ActorParent //턴의 조작권을 받습니다.
{
    None,
    Player,
    Enemy
}

public class PlayerTurnEventArgs : EventArgs
{
    public ActorParent Actor { get; }
    public PlayerTurnEventArgs(ActorParent actor) => Actor = actor;
}

public class GameStateEventArgs : EventArgs
{
    public GameState State { get; }
    public GameStateEventArgs(GameState state) => State = state;
}