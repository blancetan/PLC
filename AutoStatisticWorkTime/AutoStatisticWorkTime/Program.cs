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
        /// main   collecting workTime from PLC and  insert into DataBase
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

                    {  //  Set value of i  based on the quantity of seats
                        for (int i = 1; i <= 1; i++)
                        {
                            Int16 lineNumber = GetLineNumber(actUtlType);
                            Int16 seatNumber = GetSeatNumber(i, actUtlType);
                            string startDateTime = GetStartDateTime(i, actUtlType);
                            string endDateTime = GetEndDateTime(i, actUtlType);
                            int workTime = GetWorkTime(i, actUtlType);
                            bool isSensorExist = CheckSensorIsExist(i, actUtlType);

                            ExcueteInsertToSqlServer(lineNumber, seatNumber, startDateTime, endDateTime, workTime,isSensorExist);

                        }

                        actUtlType.Close();
                        System.Threading.Thread.Sleep(1000);  // sleep  1s

                    }

                    else

                    {
                        Console.WriteLine("PLC connecting  fail! please check wire and reconnecting");
                      
                    }

                }
                catch (Exception ex)

                {
                    Console.WriteLine(ex.ToString());
                    
                }
            }
          
            
        }


        /// <summary>
        /// ExcueteInsertToSqlServer
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="seatNumber"></param>
        /// <param name="startDateTime"></param>S
        /// <param name="endDateTime"></param>
        /// <param name="workTime"></param>
        /// <param name="isSensorExist"></param>
        static void ExcueteInsertToSqlServer(Int16 lineNumber, Int16 seatNumber, string startDateTime, string endDateTime, int workTime, bool isSensorExist)
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            //SqlConnection conn = null;
            int count = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {

                    {
                        string insertSql = "insert into t_ProductWorkTimeInfos(LineNumber,SeatNumber,StartDataTime,EndDataTime,WorkTime,IsSensorExist)" +
                            "values(@LineNumber,@SeatNumber,@StartDataTime,@EndDataTime,@WorkTime,@IsSensorExist)";
                        SqlCommand cmd = new SqlCommand(insertSql, conn);


                        cmd.Parameters.AddWithValue("@LineNumber", lineNumber);
                        cmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
                        cmd.Parameters.AddWithValue("@StartDataTime", startDateTime);
                        cmd.Parameters.AddWithValue("@EndDataTime", endDateTime);
                        cmd.Parameters.AddWithValue("@WorkTime", workTime);
                        cmd.Parameters.AddWithValue("@IsSensorExist", isSensorExist);

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


            }

            catch
            {
                Console.WriteLine("Database connecting  fail, please check wire and reconnecting! ");

            }

            finally 
            {
                if (count > 0)
                {
                    Console.WriteLine("insert into successfully! ");
                    //Console.ReadKey();

                }

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
            string[] seat1 = { "D200", "D201", "D202", "D203", "D204", "D205" };
            dict.Add(1, seat1);

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


            //Console.WriteLine($"resYear:{resYear}");
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


            //Console.WriteLine($"resYear:{resYear}");
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
            int IRetWorkTime = actUtlType.GetDevice2(dict[SeatNumber], out short resWorkTime);
            return resWorkTime;

        }


        /// <summary>
        /// GetLineNumber
        /// </summary>
        /// <param name="actUtlType"></param>
        /// <returns>resLineNum</returns>
        static Int16 GetLineNumber(ActUtlType actUtlType)
        {
            int IRetLineNum_Write = actUtlType.SetDevice2("D512", 1);
            int IRetLineNum_Read = actUtlType.GetDevice2("D512", out short resLineNum);
            return resLineNum;

        }


        /// <summary>
        /// GetSeatNumber
        /// </summary>
        /// <param name="seatNumber"></param>
        /// <param name="actUtlType"></param>
        /// <returns>resSeatNum</returns>
        static Int16 GetSeatNumber(int seatNumber, ActUtlType actUtlType)
        {

            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "D513");
            int IRetLineNum_Write = actUtlType.SetDevice(dict[seatNumber], seatNumber);
            int IRetLineNum_Read = actUtlType.GetDevice2(dict[seatNumber], out short resSeatNum);
            return resSeatNum;


        }


        /// <summary>
        /// check  sensors are or not online
        /// </summary>
        /// <param name="seatNumber"></param>
        /// <param name="actUtlType"></param>
        /// <returns>isSensorExist</returns>
        static bool CheckSensorIsExist(int seatNumber, ActUtlType actUtlType)
        {
            // isSensorExist default  is true     
            bool isSensorExist = true;
            Dictionary<int, String[]> dict = new Dictionary<int, String[]>();
            string[] str1 = { "D206", "D213"};
            dict.Add(1, str1);

            int IRetFrontSensor = actUtlType.GetDevice2(dict[seatNumber][0], out short resFrontSensor);
            int IRetBehindSensor = actUtlType.GetDevice2(dict[seatNumber][1], out short resBehindSensor);

            if ((resFrontSensor == 1) || (resBehindSensor == 1))
            {
                isSensorExist = false;
            }
            
            return isSensorExist;

        }
    }


}

