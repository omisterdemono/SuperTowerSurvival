public interface IInstrument
{
    void Obtain();
    float Strength { get; set; }
    float Durability { get; set; }
    InstrumentType InstrumentType { get; set; }
    bool NeedFlip { get; set; }
}
