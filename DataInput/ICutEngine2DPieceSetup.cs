namespace CutCraftEngineWebSocketCGLService.DataInput
{
    public interface ICutEngine2DPieceSetup
    {
        int aCount { get; }
        double aHeight { get; }
        string aID { get; }
        bool aRotatable { get; }
        double aWidth { get; }

        bool Rotatable();
        dynamic SizeReal();
        int Surplus(int dimension);
    }
}