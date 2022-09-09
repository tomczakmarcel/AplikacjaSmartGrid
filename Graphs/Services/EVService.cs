using AplikacjaSmartGrid.Graphs.Model;
using LumenWorks.Framework.IO.Csv;
using System.Data;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class EVService
    {
        public List<EVEnergyBalanceModel> getEVListFromCSV(string csvLink)
        {
            List<EVEnergyBalanceModel> listOfEvs = new List<EVEnergyBalanceModel>();
            var csvTable = new DataTable();
            using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(csvLink)), true, ';'))
            {
                csvTable.Load(csvReader);
            }

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                TimeOnly mondayFridayTimeOut = TimeOnly.FromDateTime(Convert.ToDateTime(csvTable.Rows[i][3]));
                TimeOnly saturdaySundayTimeOut = TimeOnly.FromDateTime(Convert.ToDateTime(csvTable.Rows[i][5]));
                var carOutTimesOut = GetDictionaryForACar(mondayFridayTimeOut, saturdaySundayTimeOut);

                TimeOnly mondayFridayTimeIn = TimeOnly.FromDateTime(Convert.ToDateTime(csvTable.Rows[i][4]));
                TimeOnly saturdaySundayTimeIn = TimeOnly.FromDateTime(Convert.ToDateTime(csvTable.Rows[i][6]));
                var carOutTimesIn = GetDictionaryForACar(mondayFridayTimeIn, saturdaySundayTimeIn);
                
                EVEnergyBalanceModel car = new EVEnergyBalanceModel();
                listOfEvs.Add(new EVEnergyBalanceModel { Id = Convert.ToInt32(csvTable.Rows[i][0]), StoredEnergy = new List<double>() { Convert.ToDouble(csvTable.Rows[i][1]) }, MaxEnergy = Convert.ToDouble(csvTable.Rows[i][2]), LoadedEnergyForAMinute = 0, CarComeBackToGridTime = carOutTimesIn, CarOutOfGridTime = carOutTimesOut});
            }

            return listOfEvs;
        }

        private Dictionary<DayOfWeek, TimeOnly> GetDictionaryForACar(TimeOnly mondayFridayTime, TimeOnly saturdaySundayTime)
        {
            Dictionary<DayOfWeek, TimeOnly> carOutInList = new Dictionary<DayOfWeek, TimeOnly>();
            carOutInList.Add(DayOfWeek.Monday, mondayFridayTime);
            carOutInList.Add(DayOfWeek.Tuesday, mondayFridayTime);
            carOutInList.Add(DayOfWeek.Wednesday, mondayFridayTime);
            carOutInList.Add(DayOfWeek.Thursday, mondayFridayTime);
            carOutInList.Add(DayOfWeek.Friday, mondayFridayTime);
            carOutInList.Add(DayOfWeek.Saturday, saturdaySundayTime);
            carOutInList.Add(DayOfWeek.Sunday, saturdaySundayTime);

            return carOutInList;
        }
    }
}
