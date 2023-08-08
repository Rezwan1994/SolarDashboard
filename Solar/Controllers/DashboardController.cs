using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Solar.Models.BEL;
using Solar.Models.DAL;

namespace Solar.Controllers
{
    public class DashboardController : Controller
    {
        private string connectionString;
        private DashboardDAL _dahboardDAL;
        public DashboardController(IConfiguration configuration, DashboardDAL dashboardDal)
        {
            // Get the connection string from the configuration
            connectionString = configuration.GetConnectionString("DefaultConnection");
            _dahboardDAL = dashboardDal;
        }
        public IActionResult Index()
        {
            List<SelectListItem> devices = new List<SelectListItem>();
      

            List<GenPower> model = _dahboardDAL.GetAllDevices();
           
            devices.Add(new SelectListItem
            {
                Text = "ALL",
                Value = "-1",
            });
            foreach (var item in model)
            {
                devices.Add(new SelectListItem
                {
                    Text = item.DeviceID,
                    Value = item.DeviceID,

                });
            }
            ViewBag.DeviceList = devices;
            return View();
        }

        public IActionResult SolarDashboard(string device)
        {
            List<GenPowerModel> modelList = _dahboardDAL.GetGenPowerListbyDate(device);
            ViewBag.maxCurrentPowerToday = modelList
                                            ?.Where(x => x.SetDate == DateTime.Now.Date)
                                            .FirstOrDefault()
                                            ?.CurrentPower;
            if (ViewBag.maxCurrentPowerToday == null)
            {
                ViewBag.maxCurrentPowerToday = 0;
            }
            ViewBag.maxTotalPowerToday = modelList
                                            ?.Where(x => x.SetDate == DateTime.Now.Date)
                                            .FirstOrDefault()
                                            ?.DifferenceTotalPower;
            if (ViewBag.maxTotalPowerToday == null)
            {
                ViewBag.maxTotalPowerToday = 0;
            }

            ViewBag.maxCurrentPowerYesterday = modelList
                                            ?.Where(x => x.SetDate == DateTime.Now.AddDays(-1).Date)
                                            .FirstOrDefault()
                                            ?.CurrentPower;
            if (ViewBag.maxCurrentPowerYesterday == null)
            {
                ViewBag.maxCurrentPowerYesterday = 0;
            }

            ViewBag.maxTotalPowerYesterday = modelList
                                            ?.Where(x => x.SetDate == DateTime.Now.AddDays(-1).Date)
                                            .FirstOrDefault()
                                            ?.DifferenceTotalPower;
            if (ViewBag.maxTotalPowerYesterday == null)
            {
                ViewBag.maxTotalPowerYesterday = 0;
            }

            List<string> xAxisCategoryList = new List<string>();
            int month = Convert.ToInt32(DateTime.Now.AddDays(-1).ToString("MM"));
            string monthname = DateTime.Now.AddDays(-1).ToString("MMM");
            int year = Convert.ToInt32(DateTime.Now.AddDays(-1).ToString("yyyy"));

            int num = DateTime.DaysInMonth(year, month);
            for (int index = 1; index <= num; ++index)
            {
                string str2 = index.ToString() + " " + monthname;
                xAxisCategoryList.Add(str2);
            }
            ViewBag.xAxisCategoryList = xAxisCategoryList;
            List<GenPowerModel> daywisePowerList = _dahboardDAL.GetAllCurrentAndTotalPowerList(device);
            List<double> yAxisCurrentPowerList = new List<double>();
            List<double> yAxisTotalPowerList = new List<double>();
            for (int i = 0; i < 31; i++)
            {
                yAxisCurrentPowerList.Add(0);
                yAxisTotalPowerList.Add(0);
            }
            if (daywisePowerList != null)
            {
                foreach(var item in daywisePowerList)
                {
                    int day = Convert.ToInt32(item.SetDate.Day);
                    yAxisCurrentPowerList[day - 1] = item.CurrentPower;
                    yAxisTotalPowerList[day - 1] = item.TotalPower/1000;
                }
            }

            ViewBag.CurrentPowerList = yAxisCurrentPowerList;
            ViewBag.TotalPowerList = yAxisTotalPowerList;


            return View();
        }

        public List<TotalPowerModel> GetTotalGeneratedPowerList()
        {
            List<TotalPowerModel> model = new List<TotalPowerModel>();
            model = _dahboardDAL.GetTotalGeneratedPowerList();
            return model;
        }
    }
}