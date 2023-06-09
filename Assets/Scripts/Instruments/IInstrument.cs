public interface IInstrument
{
    void Collect();
    float Strength { get; set; }
    float Durability { get; set; }
    InstrumentType InstrumentType { get; set; }
}
