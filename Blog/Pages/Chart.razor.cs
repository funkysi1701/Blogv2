using Blog.Core;
using Blog.Data.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Blog.Pages
{
    public class ChartBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }

        [Parameter]
        public int OffSet { get; set; }

        [Parameter]
        public MetricType Type { get; set; } = 0;

        protected string Title;
        protected bool LoadCompleteH = false;
        protected bool LoadCompleteD = false;
        protected bool LoadCompleteM = false;

        protected List<DateTime> hourlyLabel = new();
        protected List<DateTime> dailyLabel = new();
        protected List<DateTime> monthlyLabel = new();

        protected List<decimal> hourlyData = new();
        protected List<decimal> dailyData = new();
        protected List<decimal> monthlyData = new();

        protected List<decimal> hourlyPrevData = new();
        protected List<decimal> dailyPrevData = new();
        protected List<decimal> monthlyPrevData = new();

        protected override void OnInitialized()
        {
            Load();
        }

        public void RefreshMe()
        {
            StateHasChanged();
        }

        protected void LoadHourly()
        {
            IList<IList<ChartView>> hourlyChart = BlogService.GetChart((int)Type, (int)MyChartType.Hourly, OffSet).GetAwaiter().GetResult();
            foreach (var subitem in hourlyChart[0].OrderBy(x => x.Date))
            {
                if (subitem.Total.HasValue)
                {
                    hourlyLabel.Add(subitem.Date);
                    hourlyData.Add(subitem.Total.Value);
                }
            }

            foreach (var subitem in hourlyChart[1].OrderBy(x => x.Date))
            {
                if (subitem.Total.HasValue)
                {
                    hourlyLabel.Add(subitem.Date);
                    hourlyPrevData.Add(subitem.Total.Value);
                }
            }
            LoadCompleteH = true;
        }

        protected void LoadDaily()
        {
            IList<IList<ChartView>> dailyChart = BlogService.GetChart((int)Type, (int)MyChartType.Daily, OffSet).GetAwaiter().GetResult();
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyPrevData.Add(item.Value.Value);
                }
            }
            else
            {
                var result =
                    from s in dailyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyData.Add(item.Value.Value);
                }

                result =
                    from s in dailyChart[1].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, s.Date.Day)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    dailyLabel.Add(item.Date);
                    dailyPrevData.Add(item.Value.Value);
                }
            }
            LoadCompleteD = true;
        }

        protected void LoadMonthly()
        {
            IList<IList<ChartView>> monthlyChart = BlogService.GetChart((int)Type, (int)MyChartType.Monthly, OffSet).GetAwaiter().GetResult();
            if (Type == MetricType.Gas || Type == MetricType.Electricity)
            {
                var result =
                    from s in monthlyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Sum(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyData.Add(item.Value.Value);
                }
            }
            else
            {
                var result =
                    from s in monthlyChart[0].OrderBy(x => x.Date)
                    group s by new
                    {
                        Date = new DateTime(s.Date.Year, s.Date.Month, 1)
                    } into g
                    select new
                    {
                        g.Key.Date,
                        Value = g.Max(x => x.Total),
                    };

                foreach (var item in result)
                {
                    monthlyLabel.Add(item.Date);
                    monthlyData.Add(item.Value.Value);
                }
            }
            LoadCompleteM = true;
        }

        protected void Load()
        {
            Title = GetEnumDescription(Type);

            LoadHourly();

            LoadDaily();

            LoadMonthly();
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}
