namespace AplikacjaSmartGrid.Graphs
{
    public class SolarWindProductionDataModel
    {
        public DateOnly DateOfProduction { get; set; }
        public double SolarProduction { get; set; }
        public double MaxValueOfSolarProdDay { get; set; }
        public double WindProduction { get; set; }
    }
}
