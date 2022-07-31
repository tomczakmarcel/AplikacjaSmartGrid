namespace AplikacjaSmartGrid.Graphs.Model
{
    public class EVEnergyBalanceModel
    {
        public int Id { get; set; }
        public double StoredEnergy { get; set; }
        public double MaxEnergy { get; set; }
        public double PercentageEnergy { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Dictionary<DayOfWeek, TimeOnly>? CarOutOfGridTime { get; set; }
        public Dictionary<DayOfWeek, TimeOnly>? CarComeBackToGridTime { get; set; }
    }
}
