using AplikacjaSmartGrid.Graphs.Model;
using System.Text;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class EVEnergyBalanceService
    {
        private List<EVEnergyBalanceModel> EVList;
        private List<EnergyBalanceModel> EnergyBalanceList;
        private double usageOfKWh;
        private double howMuchEnergyCanBeLoadedAt1Minute;
        private double howMuchEnergyCanBeDeloadedAt1Minute; 
        private double quotientForEnergyBalance;

        public EVEnergyBalanceService(List<EVEnergyBalanceModel> EV, List<EnergyBalanceModel> EnergyBalance, double NewkWh = 3, double ValueOfEnergyCanBeLoadedAt1Minute = 0.116, double ValueOfEnergyCanBeDeloadedAt1Minute = 0.4, double quotientForEnergyBalanceCalculation = 1000)
        {
            EVList = EV;
            EnergyBalanceList = EnergyBalance;
            usageOfKWh = NewkWh;
            howMuchEnergyCanBeDeloadedAt1Minute = ValueOfEnergyCanBeDeloadedAt1Minute;
            howMuchEnergyCanBeLoadedAt1Minute =  ValueOfEnergyCanBeLoadedAt1Minute;
            quotientForEnergyBalance = quotientForEnergyBalanceCalculation;
        }

        public List<EnergyBalanceModel> GetEVEnergyBalance()
        {
            double energyBalance = 0;
            var minutesToGo = EnergyBalanceList.Count;

            for (int i = 0; i < minutesToGo; i++)
            {
                var day = EnergyBalanceList[i].DateOfProduction.DayOfWeek;
                var hour = TimeOnly.FromDateTime(EnergyBalanceList[i].DateOfProduction);
                energyBalance = EnergyBalanceList[i].EnergyBalance;

                if (i > 0)
                    GetUpdatedList(i);

                var carsOutsideOfGrid = UpdateListWithCarsThatOutOfGrid(day, hour, i, usageOfKWh);

                if (carsOutsideOfGrid.Count == EVList.Count)
                    continue;

                foreach (var EV in EVList)
                {
                    if (!carsOutsideOfGrid.Contains(EV.Id) && EV.StoredEnergy[i] <= EV.MaxEnergy / 2)
                    {
                        EV.StoredEnergy[i] += howMuchEnergyCanBeLoadedAt1Minute;
                        carsOutsideOfGrid.Add(EV.Id);
                        EnergyBalanceList[i].EnergyBalance -= howMuchEnergyCanBeLoadedAt1Minute;
                    }
                }

                if (energyBalance > 0)
                {
                    LoadUpCarsFromPositiveEnergyBalance(energyBalance, i, carsOutsideOfGrid);
                    continue;
                }

                if (energyBalance < 0)
                {
                    SupportEnergyBalanceFromEV(energyBalance, i, carsOutsideOfGrid);
                    continue;
                }
            }

            return EnergyBalanceList;
        }

        public void GetUpdatedList(int iteration)
        {
            for (int y = 0; y < EVList.Count; y++)
            {
                double valueMinuteBefore = EVList[y].StoredEnergy[iteration - 1];
                EVList[y].StoredEnergy.Add(valueMinuteBefore);
                EVList[y].LoadedEnergyForAMinute = 0;
            }
        }

        public List<int> UpdateListWithCarsThatOutOfGrid(DayOfWeek day, TimeOnly time, int i, double dailyUsageOfkWh)
        {
            List<int> carsToRemoveFromThisMinute = new List<int>();

            for (int j = 0; j < EVList.Count(); j++)
            {
                TimeOnly? comeBackToGridTime = EVList[j]?.CarComeBackToGridTime?[day];
                TimeOnly? outOfGridTime = EVList[j]?.CarOutOfGridTime?[day];

                bool OutOfGrid = comeBackToGridTime > time && outOfGridTime <= time;

                if (OutOfGrid)
                {
                    carsToRemoveFromThisMinute.Add(EVList[j].Id);
                }

                if (EVList[j]?.CarComeBackToGridTime?[day] == time)
                {
                    EVList[j].StoredEnergy[i] -= dailyUsageOfkWh;
                }

                EVList[j].LoadedEnergyForAMinute = 0;
            }

            return carsToRemoveFromThisMinute;
        }

        public void LoadUpCarsFromPositiveEnergyBalance(double energyBalance, int i, List<int> carsToRemove)
        {
            List<int> carsToRemoveFromThisMinute = new List<int>();
            int numberOfCars = EVList.Count;
            double amountOfEnergy = Math.Abs(energyBalance / numberOfCars);
            bool continueCalculation = true;

            List<EVEnergyBalanceModel> listOfEVAtAMinute = new List<EVEnergyBalanceModel>();
            for (int j = 0; j < EVList.Count; j++)
                listOfEVAtAMinute.Add(EVList[j]);

            if (amountOfEnergy > howMuchEnergyCanBeLoadedAt1Minute)
            {
                do
                {
                    amountOfEnergy = amountOfEnergy / quotientForEnergyBalance;
                } while (amountOfEnergy > howMuchEnergyCanBeLoadedAt1Minute);
            }

            do
            {
                for (int p = 0; p < numberOfCars; p++)
                {
                    bool iDOfCarIsToRemove = carsToRemove.Contains(EVList[p].Id);

                    if (EVList[p].StoredEnergy[i] < (EVList[p].MaxEnergy - amountOfEnergy) && energyBalance > 0 
                        && EVList[p].LoadedEnergyForAMinute <= howMuchEnergyCanBeLoadedAt1Minute && !iDOfCarIsToRemove)
                    {
                        energyBalance -= amountOfEnergy;
                        EVList[p].StoredEnergy[i] += amountOfEnergy;
                        EVList[p].LoadedEnergyForAMinute += amountOfEnergy;
                    }

                    if (EVList[p].StoredEnergy[i] >= (EVList[p].MaxEnergy - amountOfEnergy) || Math.Round(EVList[p].LoadedEnergyForAMinute, 3) >= howMuchEnergyCanBeLoadedAt1Minute 
                        || energyBalance <= 0 || iDOfCarIsToRemove)
                    {
                        carsToRemoveFromThisMinute.Add(EVList[p].Id);
                    }
                }

                if (carsToRemoveFromThisMinute.Count > 0)
                {
                    for (int x = 0; x < carsToRemoveFromThisMinute.Count; x++)
                    {
                        int number = carsToRemoveFromThisMinute[x];
                        var idToRemove = listOfEVAtAMinute.FirstOrDefault(x => x.Id == number);
                        if (idToRemove != null)
                            listOfEVAtAMinute.Remove(idToRemove);
                    }
                }

                carsToRemoveFromThisMinute.Clear();
                if (listOfEVAtAMinute.Count <= 0)
                    continueCalculation = false;

            } while (continueCalculation);

            EnergyBalanceList[i].EnergyBalance = energyBalance;
        }

        public void SupportEnergyBalanceFromEV(double energyBalance, int i, List<int> carsToRemove)
        {
            List<int> carsToRemoveFromThisMinute = new List<int>();
            int numberOfCars = EVList.Count;
            double amountOfEnergy = Math.Abs(energyBalance / numberOfCars);
            List<EVEnergyBalanceModel> listOfEVAtAMinute = new List<EVEnergyBalanceModel>();
            for (int j = 0; j < EVList.Count; j++)
                listOfEVAtAMinute.Add(EVList[j]);

            if (amountOfEnergy > howMuchEnergyCanBeDeloadedAt1Minute)
            {
                do
                {
                    amountOfEnergy = amountOfEnergy / quotientForEnergyBalance;
                } while (amountOfEnergy > howMuchEnergyCanBeDeloadedAt1Minute);
            }
            bool continueCalculation = true;

            do
            {
                for (int p = 0; p < numberOfCars; p++)
                {
                    bool idOfCarIsToRemove = carsToRemove.Contains(EVList[p].Id);

                    if (EVList[p].StoredEnergy[i] > (EVList[p].MaxEnergy / 2) && energyBalance < 0 
                        && Math.Abs(EVList[p].LoadedEnergyForAMinute) <= howMuchEnergyCanBeDeloadedAt1Minute && !idOfCarIsToRemove)
                    {
                        energyBalance += amountOfEnergy;
                        EVList[p].StoredEnergy[i] -= amountOfEnergy;
                        EVList[p].LoadedEnergyForAMinute -= amountOfEnergy;
                    }

                    if (EVList[p].StoredEnergy[i] <= (EVList[p].MaxEnergy / 2) 
                        || Math.Abs(Math.Round(EVList[p].LoadedEnergyForAMinute, 2)) >= howMuchEnergyCanBeDeloadedAt1Minute 
                        || energyBalance >= 0 || idOfCarIsToRemove)
                    {
                        carsToRemoveFromThisMinute.Add(EVList[p].Id);
                    }
                }

                if (carsToRemoveFromThisMinute.Count > 0)
                {
                    for (int x = 0; x < carsToRemoveFromThisMinute.Count; x++)
                    {
                        int number = carsToRemoveFromThisMinute[x];
                        var idToRemove = listOfEVAtAMinute.FirstOrDefault(x => x.Id == number);
                        if (idToRemove != null)
                            listOfEVAtAMinute.Remove(idToRemove);
                    }
                }

                carsToRemoveFromThisMinute.Clear();
                if (listOfEVAtAMinute.Count <= 0)
                    continueCalculation = false;

            } while (continueCalculation);

            EnergyBalanceList[i].EnergyBalance = energyBalance;
        }
    }
}