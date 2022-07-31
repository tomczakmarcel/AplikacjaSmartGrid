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

        public static List<UserUsageModel> ReturnList(bool sortByMinute = false, bool allUsers = false)
        {
            DateTime fromDate = new DateTime(2019, 4, 1);
            DateTime toDate = new DateTime(2019, 11, 1);

            var csvTable = LoadCSV();
            List<UserUsageModel> searchParameters = new List<UserUsageModel>();
            List<UserUsageModel> searchParameters4 = new List<UserUsageModel>();
            List<UserUsageModel> searchParametersHourAllUsers = new List<UserUsageModel>();
            List<UserUsageModel> searchParametersHours = new List<UserUsageModel>();
            List<UserUsageModel> searchParametersMinutes = new List<UserUsageModel>();
            List<UserUsageModel> searchParametersMinutesGrouped = new List<UserUsageModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                string dateTime2 = csvTable.Rows[i][1].ToString() + " " + csvTable.Rows[i][2].ToString();
                string newDateTime2 = dateTime2.Replace('.', '-');
                string format2 = "M-d-yyyy HH:mm";
                DateTime czas2 = DateTime.ParseExact(newDateTime2, format2, CultureInfo.InvariantCulture);

                if (czas2 < toDate && czas2 >= fromDate)
                {
                    DateTime test = Convert.ToDateTime("31.03.2019 03:00:00");
                    string dateTime = csvTable.Rows[i][1].ToString() + " " + csvTable.Rows[i][2].ToString();
                    string newDateTime = dateTime.Replace('.', '-');
                    string format = "M-d-yyyy HH:mm";
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime czas = DateTime.ParseExact(newDateTime, format, CultureInfo.InvariantCulture);
                    if (czas.Minute == 59)
                    {
                        czas = czas.AddMinutes(1);
                    }

                    if (czas <= toDate && czas >= fromDate)
                    {
                        searchParameters.Add(new UserUsageModel { PPE = csvTable.Rows[i][0].ToString(), DATACZAS = czas, ZUZYCIE = Convert.ToDouble(csvTable.Rows[i][4]) });
                    }
                }            
            }

            if (sortByMinute == true)
            {
                var searchParametersGrouped= searchParameters
                    .GroupBy(x => (x.DATACZAS))
                    .Select(group => new
                    {
                        DATACZAS = group.Key,
                        ZUZYCIE = group.Select(UserUsageModel => UserUsageModel.ZUZYCIE).Sum()
                    });

                foreach (var fifteenMinutes in searchParametersGrouped)
                {
                    DateTime hourProduction = fifteenMinutes.DATACZAS;
                    double kiloWattsProduction = fifteenMinutes.ZUZYCIE;
                    hourProduction = hourProduction.AddMinutes(-15);

                    for (DateTime date = hourProduction; date < hourProduction.AddMinutes(15); date = date.AddMinutes(1))
                    {
                        double randomDouble = GetRandomDouble(0.9, 1.1);
                        searchParametersMinutesGrouped.Add(new UserUsageModel { DATACZAS = date, ZUZYCIE = kiloWattsProduction / 15 * randomDouble });
                    }
                }

                return searchParametersMinutesGrouped;
            }

            if (allUsers && !sortByMinute)
            {
                var test3 = searchParameters
                .GroupBy(x => (x.DATACZAS.Date))
                .Select(group => new
                {
                    DATACZAS = group.Key.Date,
                    ZUZYCIE = group.Select(UserUsageModel => UserUsageModel.ZUZYCIE).Sum()
                });

                foreach (var element in test3)
                {
                    if (element.DATACZAS < toDate && element.DATACZAS >= fromDate)
                    {
                        searchParameters4.Add(new UserUsageModel { DATACZAS = element.DATACZAS, ZUZYCIE = element.ZUZYCIE });
                    }
                }

                return searchParameters4;
            }

            double productionForAHour = 0;
            int iterations = 0;

            var searchParametersX = searchParameters
                .GroupBy(x => (x.DATACZAS))
                .Select(group => new
                {
                    DATACZAS = group.Key,
                    ZUZYCIE = group.Select(UserUsageModel => UserUsageModel.ZUZYCIE).Sum()
                });

            foreach (var hours in searchParametersX)
            {
                searchParametersHours.Add(new UserUsageModel { DATACZAS = hours.DATACZAS, ZUZYCIE = hours.ZUZYCIE });
            }

            for (int j = 0; j < searchParametersHours.Count; j++)
            {
                productionForAHour += searchParametersHours[j].ZUZYCIE;
                iterations++;

                if (iterations == 4)
                {
                    searchParametersHourAllUsers.Add(new UserUsageModel { DATACZAS = searchParametersHours[j].DATACZAS.AddHours(-1), ZUZYCIE = productionForAHour });
                    productionForAHour = 0;
                    iterations = 0;
                }
            }

            return searchParametersHourAllUsers;
        }

        public static List<UserUsageModel> ReturnListDetailed()
        {
            var csvTable = LoadCSV();
            List<UserUsageModel> searchParameters = new List<UserUsageModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                searchParameters.Add(new UserUsageModel { PPE = csvTable.Rows[i][0].ToString(), DATACZAS = DateTime.Parse(csvTable.Rows[i][11].ToString()), ZUZYCIE = Convert.ToDouble(csvTable.Rows[i][4]) });
            }

            return searchParameters;
        }

        public static List<SolarProductionDataModel> ReturnListSolar(bool forADay = false, bool forAHour = false)
        {
            DateTime fromDate = new DateTime(2019, 4, 1);
            DateTime toDate = new DateTime(2019, 11, 1);

            var csvTable = LoadCSV(@"D:\PULPIT\daneslonecznezprodukcja.csv");
            List<SolarProductionDataModel> solarHourlyProduction = new List<SolarProductionDataModel>();
            List<SolarProductionDataModel> solarMinutesProduction = new List<SolarProductionDataModel>();
            List<SolarProductionDataModel> solarDailyProductionValues = new List<SolarProductionDataModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                if (DateTime.Parse(Convert.ToString(csvTable.Rows[i][0])) < toDate && DateTime.Parse(Convert.ToString(csvTable.Rows[i][0])) >= fromDate)
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

                for (DateTime date = hourProduction; date < hourProduction.AddHours(1); date = date.AddMinutes(1))
                {
                     solarMinutesProduction.Add(new SolarProductionDataModel { DateOfProduction = date, SolarProduction = kiloWattsProduction / 60 * randomDouble });
                }
            }

                return solarMinutesProduction;
        }

        public static List<WindProductionDataModel> ReturnListWind(bool forADay = false, bool forAHour = false)
        {
            double maxProduction = 5917.243;
            double installedPower = 20;
            DateTime fromDate = new DateTime(2019, 4, 1);
            DateTime toDate = new DateTime(2019, 11, 1);

            var csvTable = LoadCSV(@"D:\PULPIT\OZEProdukcjaPL\elektrowniewiatrowezuzycie2019.csv");
            List<WindProductionDataModel> windProduction = new List<WindProductionDataModel>();
            List<WindProductionDataModel> windHourlyProduction = new List<WindProductionDataModel>();
            List<WindProductionDataModel> windMinutesProduction = new List<WindProductionDataModel>();
            List<WindProductionDataModel> windDailyProductionValues = new List<WindProductionDataModel>();

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                double hour = Convert.ToDouble(csvTable.Rows[i][1]) - 1;
                if (DateTime.Parse(Convert.ToString(csvTable.Rows[i][0] + " " + Convert.ToString(hour + ":00"))) < toDate && DateTime.Parse(Convert.ToString(csvTable.Rows[i][0] + " " + Convert.ToString(hour + ":00"))) >= fromDate)
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

                for (DateTime date = hourProduction; date < hourProduction.AddHours(1); date = date.AddMinutes(1))
                {
                    windMinutesProduction.Add(new WindProductionDataModel { DateOfProduction = date, WindProduction = kiloWattsProduction / 60 * randomDouble });
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