namespace CutCraftEngineWebSocketCGLService.DataInput
{
    public interface ICutEngine2DStockSetup
    {
        public int id { get; set; }
        public int aCount { get; set; }
        public double aHeight { get; set; }
        public string aID { get; set; }
        public bool aWaste { get; set; }
        public double aWidth { get; set; }
    }
}