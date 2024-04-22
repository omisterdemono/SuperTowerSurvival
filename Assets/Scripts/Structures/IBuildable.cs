public interface IBuildable
{
    bool IsBeingBuilt { get; set; }
    void Init();
    void Build();
}
