namespace SQLReplicator.Application
{
    public class AppState
    {
        public AppState()
        {
            ShouldRun = true;
        }
        public bool ShouldRun { get; set; }
    }
}
