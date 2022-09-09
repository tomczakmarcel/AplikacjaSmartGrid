namespace AplikacjaSmartGrid.Shared
{
    public class AppState
    {
        public string productionOfWindEnergy { get; set; }
        public string usageOfEnergyList { get; set; }
        public string productionOfSolarEnergy { get; set; }
        public string carListAtTheGrid { get; set; }
        public DateTime dateStartOfSimulation { get; set; }
        public DateTime dateEndOfSimulation { get; set; }
    }
}
