using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.Globalization;

namespace AplikacjaSmartGrid.Graphs
{
    public class ImportCSV
    {
        public ImportCSV()
        {
        }

        private static DataTable LoadCSV(string csvPath = @"D:\PULPIT\MTomczak_TEST.csv")
        {
            var csvTable = new DataTable();
            using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(csvPath)), true, ';'))
            {
                csvTable.Load(csvReader);
            }

            return csvTable;
        }

        public static List<UserUsageModel> ReturnList()
        {
            var csvTable = LoadCSV();
            List<UserUsageModel> searchParameters = new List<UserUsageModel>();
            List<UserUsageModel> searchParameters2 = new List<UserUsageModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                searchParameters.Add(new UserUsageModel { PPE = csvTable.Rows[i][0].ToString(), DATACZAS = DateOnly.Parse(csvTable.Rows[i][11].ToString().Substring(0, csvTable.Rows[i][11].ToString().IndexOf(' '))), ZUZYCIE = Convert.ToDouble(csvTable.Rows[i][4]) });
            }

            var test3 = searchParameters
                .GroupBy(x => (x.PPE, x.DATACZAS))
                .Select(group => new
                {
                    PPE = group.Key.PPE,
                    DATACZAS = group.Key.DATACZAS,
                    ZUZYCIE = group.Select(UserUsageModel => UserUsageModel.ZUZYCIE).Sum()
                });

            foreach (var element in test3)
            {
                searchParameters2.Add(new UserUsageModel { PPE = element.PPE, DATACZAS = element.DATACZAS, ZUZYCIE = element.ZUZYCIE});
            }

            return searchParameters2;
        }

        public static List<SolarWindProductionDataModel> ReturnListWindSolar()
        {
            double efficiency = 0.18;
            double maxPower = 15;
            var csvTable = LoadCSV(@"D:\PULPIT\EnergyWindProduction2021\AllDataEnergyWindProd.csv");
            List<SolarWindProductionDataModel> searchParameters = new List<SolarWindProductionDataModel>();
            List<SolarWindProductionDataModel> searchParameters2 = new List<SolarWindProductionDataModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                searchParameters.Add(new SolarWindProductionDataModel { DateOfProduction = DateOnly.Parse(Convert.ToString(csvTable.Rows[i][0])), SolarProduction = Convert.ToDouble(csvTable.Rows[i][3], CultureInfo.InvariantCulture) * efficiency * maxPower, WindProduction = Convert.ToDouble(csvTable.Rows[i][2], CultureInfo.InvariantCulture) });
            }

            var groupedValuesForDay = searchParameters
                .GroupBy(x => x.DateOfProduction)
                .Select(group => new
                {
                    DateOfProduction = group.Key,
                    SolarProduction = group.Select(SolarWindProductionDataModel => SolarWindProductionDataModel.SolarProduction).Sum(),
                    WindProduction = group.Select(SolarWindProductionDataModel => SolarWindProductionDataModel.WindProduction).Sum()
                });

            foreach (var element in groupedValuesForDay)
            {
                searchParameters2.Add(new SolarWindProductionDataModel { DateOfProduction = DateOnly.Parse(Convert.ToString(element.DateOfProduction)), WindProduction = element.WindProduction, SolarProduction = element.SolarProduction });
            }

            return searchParameters2;
        }
    }
}