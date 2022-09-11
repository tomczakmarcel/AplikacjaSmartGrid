using AplikacjaSmartGrid.Graphs.Model;
using System.Data;

namespace AplikacjaSmartGrid.Shared
{
    public class AppState
    {
        public string productionOfWindEnergy { get; set; }
        public string usageOfEnergyList { get; set; }
        public string productionOfSolarEnergy { get; set; }
        public string carListAtTheGrid { get; set; }
        public double installedPower { get; set; }
    }
}
