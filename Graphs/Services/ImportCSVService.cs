using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.Globalization;
using AplikacjaSmartGrid.Graphs.Model;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class ImportCSVService
    {

        public ImportCSVService()
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
                searchParameters.Add(new UserUsageModel { PPE = csvTable.Rows[i][0].ToString(), DATACZAS = DateTime.Parse(csvTable.Rows[i][11].ToString().Substring(0, csvTable.Rows[i][11].ToString().IndexOf(' '))), ZUZYCIE = Convert.ToDouble(csvTable.Rows[i][4]) });
            }

            var test3 = searchParameters
                .GroupBy(x => (x.PPE, x.DATACZAS))
                .Select(group => new
                {
                    PPE = group.Key.PPE,
                    DATACZAS = group.Key.DATACZAS.Date,
                    ZUZYCIE = group.Select(UserUsageModel => UserUsageModel.ZUZYCIE).Sum()
                });

            foreach (var element in test3)
            {
                searchParameters2.Add(new UserUsageModel { PPE = element.PPE, DATACZAS = element.DATACZAS, ZUZYCIE = element.ZUZYCIE });
            }

            return searchParameters2;
        }

        public static List<UserUsageModel> ReturnListDetailed()
        {
            var csvTable = LoadCSV();
            List<UserUsageModel> searchParameters = new List<UserUsageModel>();
            List<UserUsageModel> searchParameters2 = new List<UserUsageModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                searchParameters.Add(new UserUsageModel { PPE = csvTable.Rows[i][0].ToString(), DATACZAS = DateTime.Parse(csvTable.Rows[i][11].ToString()), ZUZYCIE = Convert.ToDouble(csvTable.Rows[i][4]) });
            }

            return searchParameters;
        }

        public static List<SolarProductionDataModel> ReturnListSolar(bool forADay = false, bool forAHour = false)
        {
            DateTime fromDate = new DateTime(2019, 1, 1);
            DateTime toDate = new DateTime(2019, 12, 31);

            var csvTable = LoadCSV(@"D:\PULPIT\daneslonecznezprodukcja.csv");
            List<SolarProductionDataModel> solarHourlyProduction = new List<SolarProductionDataModel>();
            List<SolarProductionDataModel> solarMinutesProduction = new List<SolarProductionDataModel>();
            List<SolarProductionDataModel> solarDailyProductionValues = new List<SolarProductionDataModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                solarHourlyProduction.Add(new SolarProductionDataModel { DateOfProduction = DateTime.Parse(Convert.ToString(csvTable.Rows[i][0])), SolarProduction = Convert.ToDouble(csvTable.Rows[i][1]) * 0.001 });
            }

            if (forAHour)
                return solarHourlyProduction;

            var groupedValuesForDayNew = solarHourlyProduction
                .GroupBy(x => x.DateOfProduction.Date)
                .Select(group => new
                {
                    DateOfProduction = group.Key,
                    SolarProduction = group.Select(solarHourlyProduction => solarHourlyProduction.SolarProduction).Sum()
                });

            foreach (var element in groupedValuesForDayNew)
            {
                solarDailyProductionValues.Add(new SolarProductionDataModel { DateOfProduction = element.DateOfProduction, SolarProduction = element.SolarProduction });
            }

            if (forADay)
                return solarDailyProductionValues;

            foreach (var hour in solarHourlyProduction)
            {
                DateTime hourProduction = hour.DateOfProduction;
                double kiloWattsProduction = hour.SolarProduction;
                double randomDouble = GetRandomDouble(0.9, 1.1);

                for (DateTime date = hourProduction; date <= hourProduction.AddHours(1); date = date.AddMinutes(1))
                {
                    solarMinutesProduction.Add(new SolarProductionDataModel { DateOfProduction = date, SolarProduction = kiloWattsProduction / 60 * randomDouble });
                }
            }

                return solarMinutesProduction;
        }

        public static List<WindProductionDataModel> ReturnListWind(bool forADay = false, bool forAHour = false)
        {
            double maxProduction = 5917.243;
            double installedPower = 100;
            DateTime fromDate = new DateTime(2019, 3, 1);
            DateTime toDate = new DateTime(2019, 10, 31);

            var csvTable = LoadCSV(@"D:\PULPIT\OZEProdukcjaPL\elektrowniewiatrowezuzycie2019.csv");
            List<WindProductionDataModel> windProduction = new List<WindProductionDataModel>();
            List<WindProductionDataModel> windHourlyProduction = new List<WindProductionDataModel>();
            List<WindProductionDataModel> windMinutesProduction = new List<WindProductionDataModel>();
            List<WindProductionDataModel> windDailyProductionValues = new List<WindProductionDataModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                double hour = Convert.ToDouble(csvTable.Rows[i][1]) - 1;
                windProduction.Add(new WindProductionDataModel { DateOfProduction = DateTime.Parse(Convert.ToString(csvTable.Rows[i][0] + " " + Convert.ToString(hour + ":00"))), WindProduction = Convert.ToDouble(csvTable.Rows[i][2]) / maxProduction * installedPower});
            }

            if (forAHour)
                return windProduction;


            var windProductionDay = windProduction
                .GroupBy(x => x.DateOfProduction.Date)
                .Select(group => new
                {
                    DateOfProduction = group.Key,
                    WindProduction = group.Select(windProduction => windProduction.WindProduction).Sum()
                });

            foreach (var obj in windProductionDay)
            {
                windDailyProductionValues.Add(new WindProductionDataModel { DateOfProduction = obj.DateOfProduction, WindProduction = obj.WindProduction });
            }

            if (forADay)
                return windDailyProductionValues;

            foreach (var hour in windProduction)
            {
                DateTime hourProduction = hour.DateOfProduction;
                double kiloWattsProduction = hour.WindProduction;
                double randomDouble = GetRandomDouble(0.9, 1.1);

                for (DateTime date = hourProduction; date <= hourProduction.AddHours(1); date = date.AddMinutes(1))
                {
                    windHourlyProduction.Add(new WindProductionDataModel { DateOfProduction = date, WindProduction = kiloWattsProduction / 60 * randomDouble });
                }
            }

            return windMinutesProduction;

        }
            static private double GetRandomDouble(double min, double max)
        {
            Random random = new Random();
            return min + (random.NextDouble() * (max - min));
        }
    }
}