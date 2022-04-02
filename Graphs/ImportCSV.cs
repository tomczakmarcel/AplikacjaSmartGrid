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

            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                searchParameters.Add(new UserUsageModel { PPE = csvTable.Rows[i][0].ToString(), DATACZAS = Convert.ToDateTime(csvTable.Rows[i][11]), ZUZYCIE = Convert.ToDouble(csvTable.Rows[i][4]) });
            }

            return searchParameters;

        }
    }
}