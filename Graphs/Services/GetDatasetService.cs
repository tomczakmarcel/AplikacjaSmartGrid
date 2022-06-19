using ChartJs.Blazor.LineChart;
using AplikacjaSmartGrid.Graphs.Model;

namespace AplikacjaSmartGrid.Graphs.Services
{
    public class GetDatasetService
    {
        DateTime fromDate = new DateTime(2019, 3, 1);
        DateTime toDate = new DateTime(2019, 11, 1);

        public LineDataset<double> GetDataset(List<UserUsageModel> userUsageModel, string client, DateTime? fromDateMethod = null, DateTime? toDateMethod = null)
        {
            if (fromDateMethod == null & toDateMethod == null)
            {
                fromDateMethod = fromDate;
                toDateMethod = toDate;
            }

            string lastPPE = string.Empty;
            double zuzycie;
            LineDataset<double> lineDataset = new LineDataset<double>();

            foreach (UserUsageModel userUsageObject in userUsageModel)
            {
                if (userUsageObject.PPE == client && userUsageObject.DATACZAS > fromDateMethod && userUsageObject.DATACZAS < toDateMethod)
                {
                    zuzycie = userUsageObject.ZUZYCIE;
                    lineDataset.Add(zuzycie);
                }
            }

            return lineDataset;
        }

        public LineDataset<double> GetAllDataset(List<UserUsageModel> userUsageModel, string[] clientsToRemove, DateTime? fromDateMethod = null, DateTime? toDateMethod = null)
        {
            if (fromDateMethod == null & toDateMethod == null)
            {
                fromDateMethod = fromDate;
                toDateMethod = toDate;
            }

            string lastPPE = string.Empty;
            double zuzycie;
            LineDataset<double> lineDataset = new LineDataset<double>();
            var userUsageModelWithoutCorruptData = userUsageModel.Where(x => !clientsToRemove.Contains(x.PPE)).ToList();
            var sumAllUsage = userUsageModelWithoutCorruptData
                .GroupBy(x => x.DATACZAS)
                .Select(group => new
                {
                    DATACZAS = group.Key,
                    ZUZYCIE = group.Select(UserUsageModel => UserUsageModel.ZUZYCIE).Sum()
                });

            foreach (var usage in sumAllUsage)
            {
                if (usage.DATACZAS > fromDateMethod && usage.DATACZAS < toDateMethod)
                {
                    zuzycie = usage.ZUZYCIE;
                    lineDataset.Add(zuzycie);
                }
            }

            return lineDataset;
        }

        public LineDataset<double> GetOnlySolarProductionDataset(List<SolarProductionDataModel> solarWindProductionDataModel)
        {
            LineDataset<double> lineDataset = new LineDataset<double>();
            var solarWindProductionDataSet = solarWindProductionDataModel.ToList();

            foreach (var productionDay in solarWindProductionDataSet)
            {
                lineDataset.Add(productionDay.SolarProduction);
            }

            return lineDataset;
        }

        public LineDataset<double> GetOnlyWindProductionDataset(List<WindProductionDataModel> solarWindProductionDataModel)
        {
            LineDataset<double> lineDataset = new LineDataset<double>();
            var solarWindProductionDataSet = solarWindProductionDataModel.ToList();

            foreach (var productionDay in solarWindProductionDataSet)
            {
                lineDataset.Add(productionDay.WindProduction);
            }

            return lineDataset;
        }

        public LineDataset<double> GetOnlySolarProductionDayDataset(List<SolarProductionDataModel> solarWindProductionDataModel, DateTime? fromDateTime, DateTime? toDateTime)
        {
            if (fromDateTime == null)
                fromDateTime = new DateTime(1, 1, 19);
            if (toDateTime == null)
                toDateTime = new DateTime(1, 2, 19);            

            LineDataset<double> lineDataset = new LineDataset<double>();
            var solarWindProductionDataSet = solarWindProductionDataModel.ToList();

            foreach (var productionDay in solarWindProductionDataSet)
            {
                if (productionDay.DateOfProduction > fromDateTime && productionDay.DateOfProduction < toDateTime)
                {
                    lineDataset.Add(productionDay.SolarProduction);
                }
            }

            return lineDataset;
        }

        public LineDataset<double> GetOnlyWindProductionDayDataset(List<WindProductionDataModel> solarWindProductionDataModel, DateTime? fromDateTime, DateTime? toDateTime)
        {
            if (fromDateTime == null)
                fromDateTime = new DateTime(1, 1, 19);
            if (toDateTime == null)
                toDateTime = new DateTime(1, 2, 19);

            LineDataset<double> lineDataset = new LineDataset<double>();
            var solarWindProductionDataSet = solarWindProductionDataModel.ToList();

            foreach (var productionDay in solarWindProductionDataSet)
            {
                if (productionDay.DateOfProduction > fromDateTime && productionDay.DateOfProduction < toDateTime)
                {
                    lineDataset.Add(productionDay.WindProduction);
                }
            }

            return lineDataset;
        }

        public List<string> GetAxisX(List<UserUsageModel> userUsageModel, string client, DateTime? fromDateMethod = null, DateTime? toDateMethod = null)
        {
            if (fromDateMethod == null & toDateMethod == null)
            {
                fromDateMethod = fromDate;
                toDateMethod = toDate;
            }

            List<string> time = new List<string>();
            string lastPPE = string.Empty;
            foreach (UserUsageModel userUsageObject in userUsageModel)
            {
                if(userUsageObject.PPE == client)
                { 
                    if (userUsageObject.DATACZAS > fromDateMethod && userUsageObject.DATACZAS < toDateMethod)
                    {
                        time.Add((userUsageObject.DATACZAS).ToString());
                    }
                }
            }

            return time;
        }

        public List<string> GetAxisXForSolarHourly(List<SolarProductionDataModel> solarProductionDataModel, DateTime? fromDateTime, DateTime? toDateTime)
        {
            if (fromDateTime == null)
                fromDateTime = new DateTime(1, 1, 19);
            if (toDateTime == null)
                toDateTime = new DateTime(1, 2, 19);

            List<string> time = new List<string>();
            string lastPPE = string.Empty;
            foreach (SolarProductionDataModel solarProductionDataModelObject in solarProductionDataModel)
            {
                if (solarProductionDataModelObject.DateOfProduction > fromDateTime && solarProductionDataModelObject.DateOfProduction < toDateTime)
                {
                    time.Add(solarProductionDataModelObject.DateOfProduction.Hour.ToString());
                }
            }

            return time;
        }

        public List<string> GetAxisXForWindHourly(List<WindProductionDataModel> solarProductionDataModel, DateTime? fromDateTime, DateTime? toDateTime)
        {
            if (fromDateTime == null)
                fromDateTime = new DateTime(1, 1, 19);
            if (toDateTime == null)
                toDateTime = new DateTime(1, 2, 19);

            List<string> time = new List<string>();
            string lastPPE = string.Empty;
            foreach (WindProductionDataModel solarProductionDataModelObject in solarProductionDataModel)
            {
                if (solarProductionDataModelObject.DateOfProduction > fromDateTime && solarProductionDataModelObject.DateOfProduction < toDateTime)
                {
                    time.Add(solarProductionDataModelObject.DateOfProduction.Hour.ToString());
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
