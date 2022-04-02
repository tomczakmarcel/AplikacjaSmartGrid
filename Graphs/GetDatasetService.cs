using ChartJs.Blazor.LineChart;

namespace AplikacjaSmartGrid.Graphs
{
    public class GetDatasetService
    {
        DateTime fromDate = new DateTime(2019, 3, 1, 0, 0, 0);
        DateTime toDate = new DateTime(2019, 11, 1, 0, 0, 0);

        public LineDataset<double> GetDataset(List<UserUsageModel> userUsageModel, string client)
        {
            string lastPPE = string.Empty;
            double zuzycie;
            LineDataset<double> lineDataset = new LineDataset<double>();

            foreach (UserUsageModel userUsageObject in userUsageModel)
            {
                if (userUsageObject.PPE == client && userUsageObject.DATACZAS > fromDate && userUsageObject.DATACZAS < toDate)
                {
                    zuzycie = userUsageObject.ZUZYCIE;
                    lineDataset.Add(zuzycie);
                }
            }

            return lineDataset;
        }

        public List<string> GetAxisX(List<UserUsageModel> userUsageModel, string client)
        {
            List<string> time = new List<string>();
            string lastPPE = string.Empty;
            foreach (UserUsageModel userUsageObject in userUsageModel)
            {
                if(userUsageObject.PPE == client)
                { 
                    if (userUsageObject.DATACZAS > fromDate && userUsageObject.DATACZAS < toDate)
                    {
                        time.Add((userUsageObject.DATACZAS).ToString());
                    }
                }
            }

            return time;
        }

        public List<string> GetAllClients(List<UserUsageModel> userUsageModel)
        {
            List<string> ppes = new List<string>();

            foreach (UserUsageModel userUsageObject in userUsageModel)
            {
                string PPE = userUsageObject.PPE;
                if (!ppes.Contains(PPE))
                {
                    ppes.Add(PPE);
                }
            }

            return ppes; 
        }
    }
}
