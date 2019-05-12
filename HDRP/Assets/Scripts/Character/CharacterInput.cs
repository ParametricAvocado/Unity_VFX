

public abstract class CharacterInput : CharacterComponent
{
    public float MoveX { get; protected set; }
    public float MoveY { get; protected set; }

    public bool Jump { get; protected set; }
    public bool Crouch { get; protected set; }
    public bool Action0 { get; protected set; }
    public bool Action1 { get; protected set; }
    public bool Action2 { get; protected set; }
    public bool Action3 { get; protected set; }
}
