using Psybook.Objects.DbModels;
using Psybook.Objects.Reporting;

namespace Psybook.Services.Reporting.Visualization
{
    /// <summary>
    /// Service for generating advanced data visualizations and chart configurations
    /// </summary>
    public interface IDataVisualizationService
    {
        /// <summary>
        /// Generates chart configuration for booking trends
        /// </summary>
        Task<ChartConfiguration> GenerateBookingTrendChartAsync(List<CalendarSlot> bookings, TrendPeriod period);
        
        /// <summary>
        /// Generates chart configuration for experience performance
        /// </summary>
        Task<ChartConfiguration> GenerateExperiencePerformanceChartAsync(List<ExperienceStatistic> experiences);
        
        /// <summary>
        /// Generates chart configuration for geographic distribution
        /// </summary>
        Task<ChartConfiguration> GenerateGeographicChartAsync(List<GeographicStatistic> geographic);
        
        /// <summary>
        /// Generates chart configuration for customer segmentation
        /// </summary>
        Task<ChartConfiguration> GenerateCustomerSegmentationChartAsync(CustomerAnalytics customerStats);
        
        /// <summary>
        /// Generates heatmap data for booking patterns
        /// </summary>
        Task<HeatmapData> GenerateBookingHeatmapAsync(List<CalendarSlot> bookings);
        
        /// <summary>
        /// Generates funnel chart for booking conversion
        /// </summary>
        Task<FunnelChartData> GenerateBookingFunnelAsync(List<CalendarSlot> bookings);
    }

    public class DataVisualizationService : IDataVisualizationService
    {
        public async Task<ChartConfiguration> GenerateBookingTrendChartAsync(List<CalendarSlot> bookings, TrendPeriod period)
        {
            var (startDate, endDate) = GetPeriodDates(period);
            var groupedData = GroupBookingsByPeriod(bookings, period, startDate, endDate);

            return new ChartConfiguration
            {
                Type = ChartType.Line,
                Title = "Booking Trends",
                XAxisTitle = GetPeriodLabel(period),
                YAxisTitle = "Number of Bookings",
                Series = new List<ChartSeries>
                {
                    new ChartSeries
                    {
                        Name = "Total Bookings",
                        Data = groupedData.Select(d => (double)d.Value).ToArray(),
                        Color = "#2196F3"
                    },
                    new ChartSeries
                    {
                        Name = "Confirmed Bookings",
                        Data = groupedData.Select(d => (double)d.ConfirmedCount).ToArray(),
                        Color = "#4CAF50"
                    }
                },
                Labels = groupedData.Select(d => d.Label).ToArray(),
                Options = new ChartOptions
                {
                    Responsive = true,
                    ShowLegend = true,
                    ShowGrid = true
                }
            };
        }

        public async Task<ChartConfiguration> GenerateExperiencePerformanceChartAsync(List<ExperienceStatistic> experiences)
        {
            var topExperiences = experiences.OrderByDescending(e => e.BookingCount).Take(8).ToList();

            return new ChartConfiguration
            {
                Type = ChartType.Bar,
                Title = "Experience Performance",
                XAxisTitle = "Experience Type",
                YAxisTitle = "Number of Bookings",
                Series = new List<ChartSeries>
                {
                    new ChartSeries
                    {
                        Name = "Bookings",
                        Data = topExperiences.Select(e => (double)e.BookingCount).ToArray(),
                        Color = "#FF9800"
                    }
                },
                Labels = topExperiences.Select(e => TruncateText(e.ExperienceName, 15)).ToArray(),
                Options = new ChartOptions
                {
                    Responsive = true,
                    ShowLegend = true,
                    ShowGrid = true,
                    DualAxis = true
                }
            };
        }

        public async Task<ChartConfiguration> GenerateGeographicChartAsync(List<GeographicStatistic> geographic)
        {
            var topRegions = geographic.OrderByDescending(g => g.BookingCount).Take(10).ToList();

            return new ChartConfiguration
            {
                Type = ChartType.Pie,
                Title = "Geographic Distribution",
                Series = new List<ChartSeries>
                {
                    new ChartSeries
                    {
                        Name = "Bookings by Region",
                        Data = topRegions.Select(g => (double)g.BookingCount).ToArray()
                    }
                },
                Labels = topRegions.Select(g => g.PostcodeArea).ToArray(),
                Options = new ChartOptions
                {
                    Responsive = true,
                    ShowLegend = true,
                    ShowPercentages = true
                }
            };
        }

        public async Task<ChartConfiguration> GenerateCustomerSegmentationChartAsync(CustomerAnalytics customerStats)
        {
            return new ChartConfiguration
            {
                Type = ChartType.Donut,
                Title = "Customer Segmentation",
                Series = new List<ChartSeries>
                {
                    new ChartSeries
                    {
                        Name = "Customer Types",
                        Data = new double[] 
                        { 
                            customerStats.NewCustomers,
                            customerStats.ReturningCustomers 
                        }
                    }
                },
                Labels = new[] { "New Customers", "Returning Customers" },
                Options = new ChartOptions
                {
                    Responsive = true,
                    ShowLegend = true,
                    ShowPercentages = true,
                    DonutSize = 0.4
                }
            };
        }

        public async Task<HeatmapData> GenerateBookingHeatmapAsync(List<CalendarSlot> bookings)
        {
            var heatmapData = new HeatmapData
            {
                Title = "Booking Activity Heatmap",
                XAxisLabels = Enumerable.Range(0, 24).Select(h => $"{h:00}:00").ToArray(),
                YAxisLabels = Enum.GetNames<DayOfWeek>()
            };

            // Initialize data matrix
            heatmapData.Data = new double[7, 24]; // 7 days, 24 hours

            // Populate data
            foreach (var booking in bookings)
            {
                var dayIndex = (int)booking.Start.DayOfWeek;
                var hourIndex = booking.Start.Hour;
                heatmapData.Data[dayIndex, hourIndex]++;
            }

            return heatmapData;
        }

        public async Task<FunnelChartData> GenerateBookingFunnelAsync(List<CalendarSlot> bookings)
        {
            var totalInquiries = bookings.Count; // In real scenario, this would include all inquiries
            var pendingBookings = bookings.Count(b => b.Status == Objects.Enums.BookingStatus.Pending);
            var confirmedBookings = bookings.Count(b => b.Status == Objects.Enums.BookingStatus.Confirmed);
            var completedBookings = bookings.Count(b => b.Status == Objects.Enums.BookingStatus.Completed);

            return new FunnelChartData
            {
                Title = "Booking Conversion Funnel",
                Stages = new List<FunnelStage>
                {
                    new FunnelStage { Name = "Initial Inquiries", Value = totalInquiries, Color = "#E3F2FD" },
                    new FunnelStage { Name = "Booking Requests", Value = totalInquiries, Color = "#BBDEFB" },
                    new FunnelStage { Name = "Pending Confirmation", Value = pendingBookings, Color = "#90CAF9" },
                    new FunnelStage { Name = "Confirmed Bookings", Value = confirmedBookings, Color = "#64B5F6" },
                    new FunnelStage { Name = "Completed Experiences", Value = completedBookings, Color = "#2196F3" }
                }
            };
        }

        #region Helper Methods

        private (DateTime startDate, DateTime endDate) GetPeriodDates(TrendPeriod period)
        {
            var now = DateTime.UtcNow;
            return period switch
            {
                TrendPeriod.Last7Days => (now.AddDays(-7), now),
                TrendPeriod.Last30Days => (now.AddDays(-30), now),
                TrendPeriod.Last90Days => (now.AddDays(-90), now),
                TrendPeriod.Last12Months => (now.AddMonths(-12), now),
                TrendPeriod.YearToDate => (new DateTime(now.Year, 1, 1), now),
                _ => (now.AddDays(-30), now)
            };
        }

        private List<PeriodData> GroupBookingsByPeriod(List<CalendarSlot> bookings, TrendPeriod period, DateTime startDate, DateTime endDate)
        {
            var data = new List<PeriodData>();

            switch (period)
            {
                case TrendPeriod.Last7Days:
                case TrendPeriod.Last30Days:
                case TrendPeriod.Last90Days:
                    // Group by day
                    var days = (int)(endDate - startDate).TotalDays;
                    for (int i = 0; i <= days; i++)
                    {
                        var date = startDate.AddDays(i);
                        var dayBookings = bookings.Where(b => b.Start.Date == date.Date).ToList();
                        
                        data.Add(new PeriodData
                        {
                            Label = date.ToString("MMM dd"),
                            Value = dayBookings.Count,
                            ConfirmedCount = dayBookings.Count(b => b.Status == Objects.Enums.BookingStatus.Confirmed),
                            Date = date
                        });
                    }
                    break;

                case TrendPeriod.Last12Months:
                case TrendPeriod.YearToDate:
                    // Group by month
                    var months = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1;
                    for (int i = 0; i < months; i++)
                    {
                        var monthStart = startDate.AddMonths(i);
                        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                        var monthBookings = bookings.Where(b => b.Start >= monthStart && b.Start <= monthEnd).ToList();
                        
                        data.Add(new PeriodData
                        {
                            Label = monthStart.ToString("MMM yyyy"),
                            Value = monthBookings.Count,
                            ConfirmedCount = monthBookings.Count(b => b.Status == Objects.Enums.BookingStatus.Confirmed),
                            Date = monthStart
                        });
                    }
                    break;
            }

            return data.OrderBy(d => d.Date).ToList();
        }

        private string GetPeriodLabel(TrendPeriod period)
        {
            return period switch
            {
                TrendPeriod.Last7Days => "Days",
                TrendPeriod.Last30Days => "Days",
                TrendPeriod.Last90Days => "Days",
                TrendPeriod.Last12Months => "Months",
                TrendPeriod.YearToDate => "Months",
                _ => "Time Period"
            };
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }

        #endregion
    }

    #region Data Models

    public class ChartConfiguration
    {
        public ChartType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string XAxisTitle { get; set; } = string.Empty;
        public string YAxisTitle { get; set; } = string.Empty;
        public List<ChartSeries> Series { get; set; } = new();
        public string[] Labels { get; set; } = Array.Empty<string>();
        public ChartOptions Options { get; set; } = new();
    }

    public class ChartSeries
    {
        public string Name { get; set; } = string.Empty;
        public double[] Data { get; set; } = Array.Empty<double>();
        public string Color { get; set; } = string.Empty;
        public int YAxisIndex { get; set; } = 0;
    }

    public class ChartOptions
    {
        public bool Responsive { get; set; } = true;
        public bool ShowLegend { get; set; } = true;
        public bool ShowGrid { get; set; } = true;
        public bool ShowPercentages { get; set; } = false;
        public bool DualAxis { get; set; } = false;
        public double DonutSize { get; set; } = 0.5;
    }

    public enum ChartType
    {
        Line,
        Bar,
        Pie,
        Donut,
        Area,
        Scatter,
        Heatmap,
        Funnel
    }

    public class HeatmapData
    {
        public string Title { get; set; } = string.Empty;
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();
        public string[] YAxisLabels { get; set; } = Array.Empty<string>();
        public double[,] Data { get; set; } = new double[0, 0];
    }

    public class FunnelChartData
    {
        public string Title { get; set; } = string.Empty;
        public List<FunnelStage> Stages { get; set; } = new();
    }

    public class FunnelStage
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Color { get; set; } = string.Empty;
        public double ConversionRate => Value > 0 ? (double)Value / Value * 100 : 0;
    }

    public class PeriodData
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public int ConfirmedCount { get; set; }
        public DateTime Date { get; set; }
    }

    #endregion
}