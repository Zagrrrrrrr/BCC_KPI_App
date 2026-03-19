using System;

namespace BCC_KPI_App.Models
{
    public class ChartData
    {
        public string UnitName { get; set; }
        public decimal TargetValue { get; set; }
        public decimal ActualValue { get; set; }

        public decimal CompletionPercentage
        {
            get
            {
                if (TargetValue > 0)
                    return Math.Round((ActualValue / TargetValue) * 100, 2);
                return 0;
            }
        }
    }
}