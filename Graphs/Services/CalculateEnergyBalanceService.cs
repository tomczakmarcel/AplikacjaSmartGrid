using AplikacjaSmartGrid.Graphs.Model;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class CalculateEnergyBalanceService
    {
        public List<EnergyBalanceModel> CalculatedEnergyBalanceModelBeforeBattery(List<SolarProductionDataModel> solarProduction, List<WindProductionDataModel> windProduction, List<UserUsageModel> userUsage)
        {
            List<EnergyBalanceModel> energyBalanceModel = new List<EnergyBalanceModel>();

            if (solarProduction.Count == windProduction.Count && userUsage.Count == solarProduction.Count)
            {
                for (int i = 0; i < solarProduction.Count; i++)
                {
                    DateTime dateOfProduction = new DateTime();
                    double energyBalance = new double();

                    dateOfProduction = solarProduction[i].DateOfProduction;
                    energyBalance = solarProduction[i].SolarProduction + windProduction[i].WindProduction - userUsage[i].ZUZYCIE;

                    energyBalanceModel.Add(new EnergyBalanceModel() { DateOfProduction = dateOfProduction, EnergyBalance = energyBalance });
                }
                return energyBalanceModel;
            }
            else
                return null;
        }
    }
}