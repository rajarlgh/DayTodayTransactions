using Microcharts;

namespace Core.Controls.Controls
{
    public class ChartEntryWrapper
    {
        public ChartEntry Entry { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName => Entry?.Label;
    }
}
