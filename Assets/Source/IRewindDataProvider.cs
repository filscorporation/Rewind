namespace Source
{
    public enum ProviderState
    {
        Writing,
        Reading,
    }
    
    public interface IRewindDataProvider
    {
        RewindData GetData();

        void ApplyData(RewindData data);

        void SetState(ProviderState state);
    }
}