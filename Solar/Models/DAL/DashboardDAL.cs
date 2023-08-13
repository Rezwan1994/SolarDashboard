using Microsoft.Extensions.Configuration;
using Solar.Models.BEL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Solar.Models.DAL
{
    public class DashboardDAL
    {
        private string conString;
        public DashboardDAL(IConfiguration configuration)
        {
            conString = configuration.GetConnectionString("DefaultConnection");
        }
        public DataTable GetDataTable(string conString, string qry)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(qry, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // Load data into DataTable only if there are rows
                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                return dt;
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<GenPower> GetAllGenPowerList()
        {
            string sqlQuery = "SELECT * from GenPower";
            DataTable dt = GetDataTable(conString, sqlQuery);
            List<GenPower> item;

            item = (from DataRow row in dt.Rows
                    select new GenPower
                    {
                        AID = Convert.ToInt64(row["AID"].ToString()),
                        DeviceID = row["DeviceID"].ToString(),
                        CurrentPower = Convert.ToInt64(row["CurrentPower"].ToString()),
                        TotalPower = Convert.ToInt64(row["TotalPower"].ToString()),
                        SetDateTime = Convert.ToDateTime(row["SetDateTime"].ToString()),
                        SetTimeStamp = Convert.ToDateTime(row["SetTimeStamp"].ToString()),
                    }).ToList();
            return item;
        }
        public Dictionary<int, string> GetMonthList()
        {
            Dictionary<int, string> monthList = new Dictionary<int, string>();
            monthList.Add(1, "Jan");
            monthList.Add(2, "Feb");
            monthList.Add(3, "Mar");
            monthList.Add(4, "Apr");
            monthList.Add(5, "May");
            monthList.Add(6, "Jun");
            monthList.Add(7, "Jul");
            monthList.Add(8, "Aug");
            monthList.Add(9, "Sep");
            monthList.Add(10, "Oct");
            monthList.Add(11, "Nov");
            monthList.Add(12, "Dec");

            return monthList;
        }

        public List<GenPower> GetAllDevices()
        {
            string sqlQuery = @"SELECT DeviceID from GenPower
                                group by DeviceID";

            DataTable dt = GetDataTable(conString, sqlQuery);
            List<GenPower> item;

            item = (from DataRow row in dt.Rows
                    select new GenPower
                    {
                        DeviceID = row["DeviceID"].ToString(),
                    }).ToList();
            return item;
        }
        public List<GenPowerModel> GetGenPowerListbyDate(string device, string fromDate, string toDate)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(device) && device != "-1")
            {
                filter = " and DeviceID = " + device;
            }
            if(!string.IsNullOrEmpty(fromDate))
            {
                filter += string.Format(" and SetDate between '{0}' and '{1}'",fromDate,toDate);
            }
            string sqlQuery = string.Format(@"Select  DeviceID,CurrentPower,TotalPower,SetDate,isNull((TotalPower- LAG(TotalPower) OVER (PARTITION BY DeviceID ORDER BY SetDate)) ,0)AS DifferenceTotalPower
                                from (
                                Select DeviceID,MAX(CurrentPower/1000) CurrentPower,MAX(TotalPower/1000) TotalPower,CONVERT(varchar(10),SetTimeStamp,121) SetDate
                                from  GenPower
                                GROUP BY DeviceID,CONVERT(varchar(10),SetTimeStamp,121) 
                                ) a where 1=1 {0}", filter);
            DataTable dt = GetDataTable(conString, sqlQuery);
            List<GenPowerModel> item;

            item = (from DataRow row in dt.Rows
                    select new GenPowerModel
                    {
                        DeviceID = row["DeviceID"].ToString(),
                        CurrentPower = Convert.ToInt64(row["CurrentPower"].ToString()),
                        TotalPower = Convert.ToInt64(row["TotalPower"].ToString()),
                        SetDate = Convert.ToDateTime(row["SetDate"].ToString()),
                        DifferenceTotalPower = Convert.ToInt32(row["DifferenceTotalPower"].ToString()),
                    }).ToList();
            return item;
        }

        public List<GenPowerModel> GetAllCurrentAndTotalPowerList(string device,string fromDate,string toDate)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(device) && device != "-1")
            {
                filter = "  AND DeviceID=" + device;
            }
            string sqlQuery = string.Format(@"Select MAX(CurrentPower) CurrentPower,SUM(TotalPower) TotalPower,SetDate from (
                                                Select  DeviceID,CurrentPower,TotalPower,SetDate,isNULL((TotalPower- LAG(TotalPower) OVER (PARTITION BY DeviceID ORDER BY SetDate)),0) AS DifferenceTotalPower
                                                from (
                                                Select DeviceID,MAX(CurrentPower/1000) CurrentPower,MAX(TotalPower/1000) TotalPower,CONVERT(varchar(10),SetTimeStamp,121) SetDate
                                                from  GenPower
                                                GROUP BY DeviceID,CONVERT(varchar(10),SetTimeStamp,121) 
                                                ) a
                                                ) b Where 1=1 and SetDate between '{1}' and '{2}' {0}
                                                Group by SetDate", filter,fromDate,toDate);
            DataTable dt = GetDataTable(conString, sqlQuery);
            List<GenPowerModel> item;

            item = (from DataRow row in dt.Rows
                    select new GenPowerModel
                    {
                        CurrentPower = Convert.ToInt64(row["CurrentPower"].ToString()),
                        TotalPower = Convert.ToInt64(row["TotalPower"].ToString()),
                        SetDate = Convert.ToDateTime(row["SetDate"].ToString()),

                    }).ToList();
            return item;
        }
        public List<TotalPowerModel> GetTotalGeneratedPowerList()
        {
            string sqlQuery = @"select (max(Totalpower) - min(Totalpower))/1000 PowerGenerated , Convert(varchar(10),SetTimeStamp,121) GeneratedDate  from GenPower f where DeviceID = '3009111858'
                                group by  Convert(varchar(10),SetTimeStamp,121)
                                ";
            DataTable dt = GetDataTable(conString, sqlQuery);
            List<TotalPowerModel> item;

            item = (from DataRow row in dt.Rows
                    select new TotalPowerModel
                    {
                        PowerGenerated = Convert.ToDouble(row["PowerGenerated"].ToString()),
                        GeneratedDate = Convert.ToDateTime(row["GeneratedDate"].ToString()),
                    }).ToList();
            return item;
        }
        public Int64 GetRunningPower(string device)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(device) && device != "-1")
            {
                filter = "  AND DeviceID=" + device;
            }

            string sqlQuery = string.Format(@"SELECT top(1) * from GenPower where Convert(varchar(10),SetTimeStamp,121)='{0}' {1} order by AID desc", DateTime.Now.ToString("yyyy-MM-dd"), filter);
            DataTable dt = GetDataTable(conString, sqlQuery);
            GenPower item = new GenPower();
            Int64 runningpower = 0;
            item = (from DataRow row in dt.Rows
                    select new GenPower
                    {
                        AID = Convert.ToInt64(row["AID"].ToString()),
                        DeviceID = row["DeviceID"].ToString(),
                        CurrentPower = Convert.ToInt64(row["CurrentPower"].ToString()),
                        TotalPower = Convert.ToInt64(row["TotalPower"].ToString()),
                        SetDateTime = Convert.ToDateTime(row["SetDateTime"].ToString()),
                        SetTimeStamp = Convert.ToDateTime(row["SetTimeStamp"].ToString()),
                    }).ToList().FirstOrDefault();
            if (item != null)
            {
                runningpower = item.CurrentPower;
            }
            return runningpower / 1000;
        }
    }
}
