using AplikacjaSmartGrid.Graphs.Model;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class EVEnergyBalanceService
    {
        private List<EVEnergyBalanceModel> EVListOld;
        private List<EVEnergyBalanceModel> EVList;
        private List<EnergyBalanceModel> EnergyBalanceList;
        private double usageOfKWh = 2;

        public EVEnergyBalanceService(List<EVEnergyBalanceModel> EV, List<EnergyBalanceModel> EnergyBalance)
        {
            EVListOld = EV; 
            EnergyBalanceList = EnergyBalance;
            EVList = new List<EVEnergyBalanceModel>();
        }

        public List<EVEnergyBalanceModel> GetUpdatedList(List<EVEnergyBalanceModel> listForFirstIteration, List<EnergyBalanceModel> energyBalanceModelBeforeEV, int iteration)
        {
            var adjustedList = EVList.Where(x => x.TimeStamp == energyBalanceModelBeforeEV[iteration].DateOfProduction.AddMinutes(-1)).ToList();
            for (int y = 0; y < adjustedList.Count; y++)
            {
                listForFirstIteration[y].StoredEnergy = adjustedList[y].StoredEnergy;
            }

            return listForFirstIteration;
        }

        public List<EVEnergyBalanceModel> UpdateListWithCarsThatOutOfGrid(DayOfWeek day, TimeOnly time, int i, double dailyUsageOfkWh, double energyBalance, List<EVEnergyBalanceModel> listOfEVAtAMinute)
        {
            List<int> carsToRemoveFromThisMinute = new List<int>();

            for (int j = 0; j < listOfEVAtAMinute.Count(); j++)
            {
                TimeOnly? comeBackToGridTime = listOfEVAtAMinute[j]?.CarComeBackToGridTime?[day];
                TimeOnly? outOfGridTime = listOfEVAtAMinute[j]?.CarOutOfGridTime?[day];

                if (!(comeBackToGridTime > time && outOfGridTime >= time))
                {
                    if (energyBalance >= 0)
                    {
                        EVList.Add(listOfEVAtAMinute[j]);
                        carsToRemoveFromThisMinute.Add(listOfEVAtAMinute[j].Id);
                    }
                }

                if (carsToRemoveFromThisMinute.Count > 0)
                {
                    if (listOfEVAtAMinute[j]?.CarComeBackToGridTime?[day] == time)
                    {
                        listOfEVAtAMinute[j].StoredEnergy -= dailyUsageOfkWh;
                    }
                }
            }

            for (int x = 0; x < carsToRemoveFromThisMinute.Count; x++)
            {
                int number = carsToRemoveFromThisMinute[x];
                listOfEVAtAMinute.Remove(listOfEVAtAMinute.Single(x => x.Id == number));
            }

            return listOfEVAtAMinute;
        }

        public void WhenEnergyBalanceIsSurplus(List<EVEnergyBalanceModel> listOfEVAtAMinute, double energyBalance, int i)
        {
            List<int> carsToRemoveFromThisMinute = new List<int>();
            int numberOfCars = listOfEVAtAMinute.Count;
            double amountOfEnergy = Math.Abs(energyBalance / numberOfCars);

            do
            {
                for (int p = 0; p < listOfEVAtAMinute.Count; p++)
                {
                    if (listOfEVAtAMinute[p].StoredEnergy < (listOfEVAtAMinute[p].MaxEnergy - amountOfEnergy) && energyBalance > 0)
                    {
                        energyBalance -= amountOfEnergy;
                        listOfEVAtAMinute[p].StoredEnergy += amountOfEnergy;
                    }

                    if (listOfEVAtAMinute[p].StoredEnergy >= listOfEVAtAMinute[p].MaxEnergy)
                    {
                        EVList.Add(listOfEVAtAMinute[p]);
                        carsToRemoveFromThisMinute.Add(listOfEVAtAMinute[p].Id);
                    }
                }

                if (carsToRemoveFromThisMinute.Count > 0)
                {
                    for (int x = 0; x < carsToRemoveFromThisMinute.Count; x++)
                    {
                        int number = carsToRemoveFromThisMinute[x];
                        listOfEVAtAMinute.Remove(listOfEVAtAMinute.Single(x => x.Id == number));
                    }
                }

                carsToRemoveFromThisMinute.Clear();

            } while (energyBalance <= 0 && listOfEVAtAMinute.Count <= 0);

            EnergyBalanceList[i].EnergyBalance = energyBalance;
        }

        public void WhenEnergyBalanceIsNegative(List<EVEnergyBalanceModel> listOfEVAtAMinute, double energyBalance, int i)
        {
            List<int> carsToRemoveFromThisMinute = new List<int>();
            int numberOfCars = listOfEVAtAMinute.Count;
            double amountOfEnergy = Math.Abs(energyBalance / numberOfCars);

            do
            {
                for (int p = 0; p < listOfEVAtAMinute.Count; p++)
                {
                    if (listOfEVAtAMinute[p].StoredEnergy > (listOfEVAtAMinute[p].MaxEnergy / 2) && energyBalance < 0)
                    {
                        energyBalance += amountOfEnergy;
                        listOfEVAtAMinute[p].StoredEnergy -= amountOfEnergy;
                    }

                    if (listOfEVAtAMinute[p].StoredEnergy <= (listOfEVAtAMinute[p].MaxEnergy / 2))
                    {
                        EVList.Add(listOfEVAtAMinute[p]);
                        carsToRemoveFromThisMinute.Add(listOfEVAtAMinute[p].Id);
                    }
                }

                if (carsToRemoveFromThisMinute.Count > 0)
                {
                    for (int x = 0; x < carsToRemoveFromThisMinute.Count; x++)
                    {
                        int number = carsToRemoveFromThisMinute[x];
                        listOfEVAtAMinute.Remove(listOfEVAtAMinute.Single(x => x.Id == number));
                    }
                }

                carsToRemoveFromThisMinute.Clear();

            } while (energyBalance <= 0 && listOfEVAtAMinute.Count <= 0);

            EnergyBalanceList[i].EnergyBalance = energyBalance;
        }

        public List<EVEnergyBalanceModel> GetEVEnergyBalance()
        {
            double energyBalance = 0;
            double usageOfKWh = 2;
            var minutesToGo = EnergyBalanceList.Count;

            for (int i = 0; i < minutesToGo; i++)
            {
                var listOfEVAtAMinute = EVListOld.Where(x => x.TimeStamp == EnergyBalanceList[i].DateOfProduction).ToList();
                var day = EnergyBalanceList[i].DateOfProduction.DayOfWeek;
                var hour = TimeOnly.FromDateTime(EnergyBalanceList[i].DateOfProduction);

                energyBalance = EnergyBalanceList[i].EnergyBalance;

                if (i > 0)
                    listOfEVAtAMinute = GetUpdatedList(listOfEVAtAMinute, EnergyBalanceList, i);

                listOfEVAtAMinute = UpdateListWithCarsThatOutOfGrid(day, hour, i, usageOfKWh, energyBalance, listOfEVAtAMinute);

                if (listOfEVAtAMinute.Count <= 0)
                    continue;

                if (energyBalance > 0)
                {
                    WhenEnergyBalanceIsSurplus(listOfEVAtAMinute, energyBalance, i);
                    continue;
                }

                if (energyBalance < 0)
                {
                    WhenEnergyBalanceIsNegative(listOfEVAtAMinute, energyBalance, i);
                    continue;
                }
            }

            return EVList;
        }

        public List<EVEnergyBalanceModel> GetValuesBeforeUsage(List<EnergyBalanceModel> energyBalanceBeforeBattery, List<EVEnergyBalanceModel> listWithNoValues)
        {
            List<EVEnergyBalanceModel> listWithDefaultValues = new List<EVEnergyBalanceModel>();
            List<DateTime> DateTimeFromEnergyBalanceList = new List<DateTime>();
            foreach (var energyBalance in energyBalanceBeforeBattery)
            {
                DateTimeFromEnergyBalanceList.Add(energyBalance.DateOfProduction);
            }


            var listGroupped = listWithNoValues.GroupBy(x => x.Id).Select(grp => grp.ToList()).ToList();
            foreach (var car in listGroupped)
            {
                for (int i = 0; i < DateTimeFromEnergyBalanceList.Count; i++)
                {
                    listWithDefaultValues.Add(new EVEnergyBalanceModel() { 
                        CarComeBackToGridTime = car.First().CarComeBackToGridTime,
                        CarOutOfGridTime = car.First().CarOutOfGridTime,
                        Id = car.First().Id,
                        MaxEnergy = car.First().MaxEnergy,
                        PercentageEnergy = car.First().PercentageEnergy,
                        StoredEnergy = car.First().StoredEnergy,
                        TimeStamp = DateTimeFromEnergyBalanceList[i]
                    });
                }
            }

            return listWithDefaultValues;
        }
    }
}
