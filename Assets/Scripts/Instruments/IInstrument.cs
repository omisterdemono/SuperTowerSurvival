public interface IInstrument
{
    //todo maybe move properties to scriptable object
    void Obtain();
    float Strength { get; set; }
    float Durability { get; set; }
    InstrumentType InstrumentType { get; set; }
}
