namespace AplikacjaSmartGrid.Graphs.Model
{
    public class EVEnergyBalanceModel
    {
        public int Id { get; set; }
        public List<double>? StoredEnergy { get; set; }
        public double MaxEnergy { get; set; }
        public double LoadedEnergyForAMinute { get; set; }
        public Dictionary<DayOfWeek, TimeOnly>? CarOutOfGridTime { get; set; }
        public Dictionary<DayOfWeek, TimeOnly>? CarComeBackToGridTime { get; set; }
    }
}

