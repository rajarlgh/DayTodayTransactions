using Microcharts;

namespace Core.Controls.Controls
{
    public class ChartEntryWrapper
    {
        public ChartEntry Entry { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName => Entry?.Label;

        //public DonutChart CreateChart(IEnumerable<ChartEntryWrapper> entryWrappers)
        //{
        //    return new DonutChart
        //    {
        //        Entries = entryWrappers.Select(wrapper => wrapper.Entry),
        //        LabelMode = LabelMode.LeftAndRight,
        //        IsAnimated = true,
        //        Margin = 10,
        //        LabelTextSize = 40
        //    };
        //}
    }
}
