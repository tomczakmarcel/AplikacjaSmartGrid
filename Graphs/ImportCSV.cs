using LumenWorks.Framework.IO.Csv;
using System.Data;

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
            using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"D:\PULPIT\MTomczak_TEST.csv")), true, ';'))
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
    }
}