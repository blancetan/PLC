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
        //public string GetGUID()
        //{
        //    System.Guid Guid = new Guid();
        //    guid = Guid.NewGuid();
        //    //string str = _guid.ToString();
        //    //return str;
        //    return guid;
        //}
        static void Main(string[] args)
        {
            // read data from  PLC
            //ActUtlTypeLib.ActUtlType ActUtlType1 = new ActUtlTypeLib.ActUtlType();
            ActUtlTypeLib.ActUtlType actUtlType = new ActUtlTypeLib.ActUtlType();

            // logicalStationNumbet 
            //ActUtlType1.ActLogicalStationNumber = 1;
            actUtlType.ActLogicalStationNumber = 1;

            // password
            //ActUtlType1.ActPassword = "";
            actUtlType.ActPassword = "";

            //get connection strings from app.config
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;

            try
            {
                // open PLC
                int iReturnCode = actUtlType.Open();
                if (iReturnCode == 0)
                {
                    // read  D220  datas
                    int IRet = actUtlType.GetDevice2("D220", out short res);
                    //Console.WriteLine(res);

                    // connecting   database
                    using (conn = new SqlConnection(connStr))
                    {
                  
                        conn.Open();
                        Console.WriteLine($"state: {conn.State}");
                        {
                            //  select datas from datbase
                            //string selectSql = "select * from t_ProductHourInfos";

                            // get GUID
                            //Guid Guid = new Guid();
                            //guid = Guid.NewGuid();
                            Console.WriteLine(res);
                            //Console.ReadKey();
                            // insert into datas from database
                            string insertSql = "insert into t_ProductWorkTimeInfos(WorkTime)values(@WorkTime)";


                            // create SqlCommand object
                            //SqlCommand cmd = new SqlCommand(selectSql,conn);
                            SqlCommand cmd = new SqlCommand(insertSql, conn);
                            cmd.Parameters.Add("@WorkTime", SqlDbType.TinyInt);
                            cmd.Parameters["@WorkTime"].Value = res;


                            // create SqlDataAdapter object
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds, "t_ProductWorkTimeInfos");
                            //da.Update(ds, "t_ProductHourInfos");
                            //da.Update(ds);

                        }

                    }

                }
                else
                { 
                
                
                
                
                }



            }
            catch
            {

               

            }
            finally
            { 
            
            
            }
        }
    }
}
