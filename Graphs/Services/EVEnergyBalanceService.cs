using AplikacjaSmartGrid.Graphs.Model;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class EVEnergyBalanceService
    {
        public List<EVEnergyBalanceModel> GetEVEnergyBalance(List<EVEnergyBalanceModel> listBeforeCalculation, List<EnergyBalanceModel> energyBalanceModelBeforeEV)
        {
            List<EVEnergyBalanceModel> listEV = new List<EVEnergyBalanceModel>();
            List<EnergyBalanceModel> listEnergyBalance = new List<EnergyBalanceModel>();

            double energyBalance = 0;
            double usageOfKWh = 10;


            for (int i = 0; i < energyBalanceModelBeforeEV.Count; i++)
            {
                var test = listBeforeCalculation.Where(x => x.TimeStamp == energyBalanceModelBeforeEV[i].DateOfProduction).ToList();
                if (i > 0)
                {
                    var test2 = listBeforeCalculation.Where(x => x.TimeStamp == energyBalanceModelBeforeEV[i].DateOfProduction.AddMinutes(-1)).ToList();

                    for (int y = 0; y < test.Count; y++)
                    {
                        test[y].StoredEnergy = test2[y].StoredEnergy;
                    }
                }

                energyBalance = energyBalanceModelBeforeEV[i].EnergyBalance;
                bool noCars = false;
                var day = energyBalanceModelBeforeEV[i].DateOfProduction.DayOfWeek;
                var hour = TimeOnly.FromDateTime(energyBalanceModelBeforeEV[i].DateOfProduction);
                List<int> carsToRemove = new List<int>();

                for (int j = 0; j < test.Count(); j++)
                {
                    if (!(test[j]?.CarComeBackToGridTime?[day] > hour && test[j]?.CarOutOfGridTime?[day] <= hour))
                    {
                        if (test[j].PercentageEnergy >= 1.0 && energyBalanceModelBeforeEV[i].EnergyBalance > 0)
                        {
                            listEV.Add(test[j]);
                            carsToRemove.Add(test[j].Id);
                        }
                    }

                    if (test[j]?.CarComeBackToGridTime?[day] == hour)
                    {
                        test[j].StoredEnergy -= usageOfKWh;
                    }
                }

                for (int x = 0; x < carsToRemove.Count; x++)
                {
                    int number = carsToRemove[x];
                    test.Remove(test.Single(x => x.Id == number));
                }

                if (test.Count <= 0)
                {
                    continue;
                }

                if (energyBalanceModelBeforeEV[i].EnergyBalance > 0)
                {
                    int numberOfCars = test.Count;
                    double amountOfEnergy = Math.Abs(energyBalanceModelBeforeEV[i].EnergyBalance / numberOfCars);

                    do
                    {
                        foreach (var carToBeLoaded in test)
                        {
                            if (!(carToBeLoaded.CarComeBackToGridTime?[day] > hour && carToBeLoaded.CarOutOfGridTime?[day] <= hour))
                            {
                                if (carToBeLoaded.StoredEnergy < (carToBeLoaded.MaxEnergy - amountOfEnergy))
                                {
                                    energyBalance -= amountOfEnergy;
                                    carToBeLoaded.StoredEnergy += amountOfEnergy; //amount of energy musi brac z poprzedniego + jak powyższy warunek się nie zgadza to trzeba jakoś z tego wyjść
                                }
                            }
                        }

                        foreach (var carToBeLoaded in test)
                        {
                            if (carToBeLoaded.CarComeBackToGridTime?[day] > hour && carToBeLoaded.CarOutOfGridTime?[day] <= hour || energyBalance <= 0)
                            {
                                listEV.Add(carToBeLoaded);
                                numberOfCars -= 1;
                            }

                            if (numberOfCars <= 0)
                            {
                                noCars = true;
                            }
                        }

                        if (energyBalance <= 0 && noCars == false)
                        {
                            foreach (var carToBeLoaded in test)
                            {
                                listEV.Add(carToBeLoaded);
                                numberOfCars -= 1;

                                if (numberOfCars <= 0)
                                {
                                    noCars = true;
                                    energyBalanceModelBeforeEV[i].EnergyBalance = energyBalance;
                                }
                            }
                        }
                    } while (!noCars);
                }

                if (energyBalanceModelBeforeEV[i].EnergyBalance < 0)
                {
                    int numberOfCars = test.Count;
                    double amountOfEnergy = Math.Abs(energyBalanceModelBeforeEV[i].EnergyBalance / numberOfCars);

                    do
                    {
                        foreach (var carToBeDeLoaded in test)
                        {
                            if (!(carToBeDeLoaded.CarComeBackToGridTime?[day] > hour && carToBeDeLoaded.CarOutOfGridTime?[day] <= hour))
                            {
                                if (carToBeDeLoaded.StoredEnergy > (carToBeDeLoaded.MaxEnergy / 2) && energyBalance <= 0)
                                {
                                    energyBalance += amountOfEnergy;
                                    carToBeDeLoaded.StoredEnergy -= amountOfEnergy; //dodać elsa w sytuacji w której nie da sie spelnic warunku powyzej
                                }
                            }
                        }

                        foreach (var carToBeLoaded in test)
                        {
                            if ((carToBeLoaded.CarComeBackToGridTime?[day] > hour && carToBeLoaded.CarOutOfGridTime?[day] <= hour && energyBalance <= 0) || carToBeLoaded.StoredEnergy < (carToBeLoaded.MaxEnergy / 2))
                            {
                                listEV.Add(carToBeLoaded);
                                numberOfCars -= 1;
                            }

                            if (numberOfCars <= 0)
                            {
                                noCars = true;
                            }
                        }

                        if (energyBalance >= 0 && noCars == false)
                        {
                            foreach (var carToBeDeLoaded in test)
                            {
                                listEV.Add(carToBeDeLoaded);
                                numberOfCars -= 1;

                                if (numberOfCars <= 0)
                                {
                                    noCars = true;
                                    energyBalanceModelBeforeEV[i].EnergyBalance = energyBalance;
                                }
                            }
                        }
                    }
                    while (!noCars);
                }
            }

            return listEV;
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

            GetEVEnergyBalance(listWithDefaultValues, energyBalanceBeforeBattery);

            return listWithDefaultValues;
        }
    }
}
