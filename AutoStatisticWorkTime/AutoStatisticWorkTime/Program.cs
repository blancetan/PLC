using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActProgTypeLib;
using ActUtlTypeLib;
using ACTUWZDLib;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace AutoStatisticHour
{
    class Program
    {
        /// <summary>
        /// main   collecting workTime from PLC  insert into DataBase
        /// </summary>
        /// <param name="args"></param>
        
        static void Main(string[] args)
        {
            // create ActUtlType object
            ActUtlType actUtlType = new ActUtlType();
            // logicalStationNumbet 
            actUtlType.ActLogicalStationNumber = 1;
            // not set password, is null
            actUtlType.ActPassword = "";
            while (true) 
            
            {
                try
                {
                    int iReturnCode = actUtlType.Open();
                    if (iReturnCode == 0)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            Int16 lineNumber = GetLineNumber(actUtlType);
                            ////Int16 seatNumber = GetSeatNumber();
                            Int16 seatNumber = 1;
                            string startDateTime = GetStartDateTime(i, actUtlType);
                            string endDateTime = GetEndDateTime(i, actUtlType);
                            int workTime = GetWorkTime(i, actUtlType);

                            // ..this datas are used to  test
                            //Int16 lineNumber = 1;
                            //Int16 seatNumber = 2;
                            //string startDateTime = "20-3-24 12:34:30";
                            //string endDateTime = "20-3-24 12:34:50";
                            //int workTime = 20;


                            ExcueteInsertToSqlServer(lineNumber, seatNumber, startDateTime, endDateTime, workTime);

                        }

                        actUtlType.Close();

                    }

                    else
                    {
                        Console.WriteLine("connection fail! please reconnecting");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {

                }

            }
          
            
        }


        /// <summary>
        /// ExcueteInsertToSqlServer
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="seatNumber"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="workTime"></param>
        static void ExcueteInsertToSqlServer(Int16 lineNumber, Int16 seatNumber, string startDateTime, string endDateTime, int workTime)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            //SqlConnection conn = null;
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {

                {
                    string insertSql = "insert into t_ProductWorkTimeInfos(LineNumber,SeatNumber,StartDataTime,EndDataTime,WorkTime)" +
                        "values(@LineNumber,@SeatNumber,@StartDataTime,@EndDataTime,@WorkTime)";
                    SqlCommand cmd = new SqlCommand(insertSql, conn);


                    cmd.Parameters.AddWithValue("@LineNumber", lineNumber);
                    cmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
                    cmd.Parameters.AddWithValue("@StartDataTime", startDateTime);
                    cmd.Parameters.AddWithValue("@EndDataTime", endDateTime);
                    cmd.Parameters.AddWithValue("@WorkTime", workTime);

                    //SqlParameter[] paras ={
                    //            new SqlParameter("@LineNumber",SqlDbType.TinyInt);
                    //new SqlParameter("@SeatNumber", SqlDbType.TinyInt);
                    //new SqlParameter("@StartDataTime", SqlDbType.DateTime);
                    //new SqlParameter("@EndDataTime", SqlDbType.DateTime);
                    //new SqlParameter("@WorkTime", SqlDbType.Int);
                    //};

                    //cmd.Parameters.AddRange(lineNumber, seatNumber, startDateTime, endDateTime, workTime);

                    conn.Open();
                    count = cmd.ExecuteNonQuery();
                    conn.Close();

                }

            }
            if (count > 0)
            {
                Console.WriteLine("insert into successfully! ");
                //Console.ReadKey();

            }

        }

        /// <summary>
        /// GetStartDateTime
        /// </summary>
        /// <param name="seatNumber"></param>
        /// <param name="actUtlType"></param>
        /// <returns>startDateTime</returns>
        static string  GetStartDateTime(int seatNumber,ActUtlType actUtlType)
        {
            Dictionary<int, String[]> dict = new Dictionary<int, String[]>();
            string[] str1 = { "D200", "D201", "D202", "D203", "D204", "D205" };
            dict.Add(1, str1);

            //Console.WriteLine($"D200:{dict[seatNumber][0]}");
            //Console.WriteLine($"D201:{dict[seatNumber][1]}");
            //Console.WriteLine($"D202:{dict[seatNumber][2]}");
            //Console.WriteLine($"D203:{dict[seatNumber][3]}");
            //Console.WriteLine($"D204:{dict[seatNumber][4]}");
            //Console.WriteLine($"D205:{dict[seatNumber][5]}");

            int IRetYear = actUtlType.GetDevice2($"{ dict[seatNumber][0]}", out short resYear);
            int IRetMonth = actUtlType.GetDevice2($"{dict[seatNumber][1]}", out short resMonth);
            int IRetDay = actUtlType.GetDevice2($"{dict[seatNumber][2]}", out short resDay);
            int IRetHour = actUtlType.GetDevice2($"{dict[seatNumber][3]}", out short resHour);
            int IRetMin = actUtlType.GetDevice2($"{dict[seatNumber][4]}", out short resMin);
            int IRetSec = actUtlType.GetDevice2($"{dict[seatNumber][5]}", out short resSec);


            //Console.WriteLine($"resMonth:{resYear}");
            //Console.WriteLine($"resMonth:{resMonth}");
            //Console.WriteLine($"resDay:{resDay}");
            //Console.WriteLine($"resHour:{resHour}");
            //Console.WriteLine($"resMin:{resMin}");
            //Console.WriteLine($"resSec:{resSec}");

            string startDateTime = resYear.ToString() + "-" + resMonth.ToString() + "-" + resDay.ToString() + " " +
                    resHour.ToString() + ":" + resMin.ToString() + ":" + resSec.ToString();

            return startDateTime;



        }
       

        /// <summary>
        /// GetEndDateTime
        /// </summary>
        /// <param name="seatNumber"></param>
        /// <returns>endDateTime</returns>
        static string GetEndDateTime(int seatNumber, ActUtlType actUtlType)
        {
            Dictionary<int, String[]> dict = new Dictionary<int, String[]>();
            string[] str1 = { "D207", "D208", "D209", "D210", "D211", "D212" };
            dict.Add(1, str1);

            //Console.WriteLine($"D200:{dict[seatNumber][0]}");
            //Console.WriteLine($"D201:{dict[seatNumber][1]}");
            //Console.WriteLine($"D202:{dict[seatNumber][2]}");
            //Console.WriteLine($"D203:{dict[seatNumber][3]}");
            //Console.WriteLine($"D204:{dict[seatNumber][4]}");
            //Console.WriteLine($"D205:{dict[seatNumber][5]}");

            int IRetYear = actUtlType.GetDevice2($"{ dict[seatNumber][0]}", out short resYear);
            int IRetMonth = actUtlType.GetDevice2($"{dict[seatNumber][1]}", out short resMonth);
            int IRetDay = actUtlType.GetDevice2($"{dict[seatNumber][2]}", out short resDay);
            int IRetHour = actUtlType.GetDevice2($"{dict[seatNumber][3]}", out short resHour);
            int IRetMin = actUtlType.GetDevice2($"{dict[seatNumber][4]}", out short resMin);
            int IRetSec = actUtlType.GetDevice2($"{dict[seatNumber][5]}", out short resSec);


            //Console.WriteLine($"resMonth:{resYear}");
            //Console.WriteLine($"resMonth:{resMonth}");
            //Console.WriteLine($"resDay:{resDay}");
            //Console.WriteLine($"resHour:{resHour}");
            //Console.WriteLine($"resMin:{resMin}");
            //Console.WriteLine($"resSec:{resSec}");

            string endDateTime = resYear.ToString() + "-" + resMonth.ToString() + "-" + resDay.ToString() + " " +
                    resHour.ToString() + ":" + resMin.ToString() + ":" + resSec.ToString();

            return endDateTime;

        }


        /// <summary>
        /// GetWorkTime
        /// </summary>
        /// <param name="SeatNumber"></param>
        /// <param name="actUtlType"></param>
        /// <returns>resWorkTime</returns>
        static Int16 GetWorkTime(int SeatNumber, ActUtlType actUtlType)
        {
            Dictionary<int, String> dict = new Dictionary<int, String>();
            dict.Add(1, "D220");
            int IRetWorkTime = actUtlType.GetDevice2("D220", out short resWorkTime);
            return resWorkTime;

        }

        /// <summary>
        /// GetLineNumber
        /// </summary>
        /// <param name="actUtlType"></param>
        /// <returns>resLineNum</returns>


        static Int16 GetLineNumber(ActUtlType actUtlType)
        {
            int IRetLineNum = actUtlType.GetDevice2("D213", out short resLineNum);
            return resLineNum;

        }

       /*static Int16 GetSeatNumber()
        { 
            
        
        
        }*/
    }


}

