namespace Game.Core.Interfaces
{
    public enum SoundType
    {
        ShapeMove,
        ShapeRotate,
        Background,
        Destroy
    }
    
    public interface ISoundManager
    {
        void PlaybackSound(SoundType soundType);
        void Stop();
    }
}