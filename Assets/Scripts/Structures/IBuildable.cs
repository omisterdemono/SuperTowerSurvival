public interface IBuildable
{
    bool IsBuilt { get; set; }
    void Init();
    void Build();
}
