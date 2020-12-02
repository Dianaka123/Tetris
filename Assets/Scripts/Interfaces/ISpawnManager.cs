namespace UnityAcademy.TreeOfControllersExample
{
    public interface ISpawnManager
    {
        IGridManager GridManager { get; set; }
        void Spawn();
    }
}