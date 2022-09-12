namespace AplikacjaSmartGrid.Shared
{
    public class AppState
    {
        public AppState()
        {
        }

        public string? ProductionOfWindEnergy { get; set; }
        public string? UsageOfEnergyList { get; set; }
        public string? ProductionOfSolarEnergy { get; set; }
        public string? CarListAtTheGrid { get; set; }
        public double InstalledPower { get; set; }
        public double UsageOfEVCarsDaily { get; set; }
        public double HowMuchEnergyCanEVDonateToGridForAMinute { get; set; }
        public double HowMuchEnergyCanEVLoadFromGridForAMinute { get; set; }
    }
}
