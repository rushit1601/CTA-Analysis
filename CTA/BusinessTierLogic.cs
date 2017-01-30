//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

    //
    // Business:
    //
    public class Business
    {
        //
        // Fields:
        //
        private string _DBFile;
        private DataAccessTier.Data dataTier;


        ///
        /// <summary>
        /// Constructs a new instance of the business tier.  The format
        /// of the filename should be either |DataDirectory|\filename.mdf,
        /// or a complete Windows pathname.
        /// </summary>
        /// <param name="DatabaseFilename">Name of database file</param>
        /// 
        public Business(string DatabaseFilename)
        {
            _DBFile = DatabaseFilename;

            dataTier = new DataAccessTier.Data(DatabaseFilename);
        }


        ///
        /// <summary>
        ///  Opens and closes a connection to the database, e.g. to
        ///  startup the server and make sure all is well.
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        /// 
        public bool TestConnection()
        {
            return dataTier.OpenCloseConnection();
        }


        ///
        /// <summary>
        /// Returns all the CTA Stations, ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 
        public IReadOnlyList<CTAStation> GetStations()
        {
            List<CTAStation> stations = new List<CTAStation>();
            string stationSql = "Select Name,StationID from Stations;";

            try
            {
                DataSet dataSet = dataTier.ExecuteNonScalarQuery(stationSql);

                foreach (DataRow row in dataSet.Tables["TABLE"].Rows)
                {
                    string totalRidershipSql = string.Format(@"Select SUM(DailyTotal)from Riderships where StationID = {0};", Convert.ToInt32(row["StationID"]));
                    object totalRidership = dataTier.ExecuteScalarQuery(totalRidershipSql);
                    string weeklyRidershipSql = string.Format(@"Select SUM(DailyTotal)from Riderships where StationID = {0} AND TypeOfDay = 'W';", Convert.ToInt32(row["StationID"]));
                    object weeklyRidership = dataTier.ExecuteScalarQuery(weeklyRidershipSql);
                    string saturdayRiderhsipSql = string.Format(@"Select SUM(DailyTotal)from Riderships where StationID = {0} AND TypeOfDay = 'A';", Convert.ToInt32(row["StationID"]));
                    object saturdayRidership = dataTier.ExecuteScalarQuery(saturdayRiderhsipSql);
                    string holidayRiderhsipSql = string.Format(@"Select SUM(DailyTotal)from Riderships where StationID = {0} AND TypeOfDay = 'U';", Convert.ToInt32(row["StationID"]));
                    object holidayRidership = dataTier.ExecuteScalarQuery(holidayRiderhsipSql);
                    string operationalDaysSql = string.Format(@"Select COUNT(DailyTotal) from Riderships where StationID = {0};", Convert.ToInt32(row["StationID"]));
                    object operationalDays = dataTier.ExecuteScalarQuery(operationalDaysSql);
                    var tmpStation = new CTAStation(Convert.ToInt32(row["StationID"]), Convert.ToString(row["Name"]));
                    tmpStation.totalRidership = Convert.ToInt32(totalRidership);
                    tmpStation.WeeklyRidership = Convert.ToInt32(weeklyRidership);
                    tmpStation.saturdayRidership = Convert.ToInt32(saturdayRidership);
                    tmpStation.holidayRidership = Convert.ToInt32(holidayRidership);
                    tmpStation.operatoinalDays = Convert.ToInt32(operationalDays);
                    stations.Add(tmpStation);

                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }


        ///
        /// <summary>
        /// Returns the CTA Stops associated with a given station,
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStop objects</returns>
        ///
        public IReadOnlyList<CTAStop> GetStops(int stationID)
        {
            List<CTAStop> stops = new List<CTAStop>();
            string stationSql = String.Format(@"Select * from Stops where StationID = {0};", stationID);

            try
            {
                DataSet dataSet = dataTier.ExecuteNonScalarQuery(stationSql);


                foreach (DataRow row in dataSet.Tables["TABLE"].Rows)
                {
                    stops.Add(new CTAStop(Convert.ToInt32(row["StopID"]), Convert.ToString(row["Name"]), Convert.ToInt32(row["StationID"]), Convert.ToString(row["Direction"]), Convert.ToBoolean(row["ADA"]), Convert.ToDouble(row["Latitude"]), Convert.ToDouble(row["Longitude"])));
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stops;
        }


        ///
        /// <summary>
        /// Returns the top N CTA Stations by ridership, 
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 
        public IReadOnlyList<CTAStation> GetTopStations(int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();
            string topStationSql = "select Stations.Name, Riderships.StationID , SUM(CAST(DailyTotal AS BIGINT)) as Total from Riderships inner join Stations on Riderships.StationID =Stations.StationID  where Riderships.StationID IN (select StationID from Stations) Group By Riderships.StationID ,Stations.Name  order by Total DESC; ";

            try
            {
                DataSet dataSet = dataTier.ExecuteNonScalarQuery(topStationSql);
                foreach (DataRow row in dataSet.Tables["TABLE"].Rows)
                {
                    var tmpStation = new CTAStation(Convert.ToInt32(row["StationID"]), Convert.ToString(row["Name"]));
                    stations.Add(tmpStation);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }

        ///
        /// <summary>
        /// Returns all the CTA Stops, ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStops objects</returns>
        /// 
        public IReadOnlyList<CTAStop> GetAllStops()
        {
            List<CTAStop> stops = new List<CTAStop>();
            string stopSql = String.Format(@"Select * from Stops;");
            

            try
            {
                DataSet stopsDataSet = dataTier.ExecuteNonScalarQuery(stopSql);
                

                foreach (DataRow row in stopsDataSet.Tables["TABLE"].Rows)
                {
                    CTAStop tmpStop = new CTAStop(Convert.ToInt32(row["StopID"]), Convert.ToString(row["Name"]), Convert.ToInt32(row["StationID"]), Convert.ToString(row["Direction"]), Convert.ToBoolean(row["ADA"]), Convert.ToDouble(row["Latitude"]), Convert.ToDouble(row["Longitude"]));
                    string linesSql = String.Format(@"Select distinct  Color, StopID from StopDetails, Lines where StopDetails.LineID = Lines.LineID AND StopID = {0};", Convert.ToInt32(row["StopID"]));
                    DataSet linesDataSet = dataTier.ExecuteNonScalarQuery(linesSql);
                    List<String> tmpList = new List<String>();
                    foreach (DataRow line in linesDataSet.Tables["TABLE"].Rows)
                    {
                        tmpList.Add(Convert.ToString(line["Color"]));
                        Console.WriteLine("The value of line at bussiness:" + Convert.ToString(line["Color"]));
                    }
                    tmpStop.lines = tmpList;
                    stops.Add(tmpStop);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stops;
        }

        public IReadOnlyList<CTAStation> GetTopStationsBy(Boolean sortBy,int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();
            string topStationSql;
            if (sortBy)
            {
                 topStationSql= "select Stations.Name, Riderships.StationID , SUM(CAST(DailyTotal AS BIGINT)) as Total from Riderships inner join Stations on Riderships.StationID =Stations.StationID  where Riderships.StationID IN (select StationID from Stations) and Riderships.TypeOfDay ='A' Group By Riderships.StationID ,Stations.Name  order by Total DESC; ";
            }
            else
            {
                topStationSql = "select Stations.Name, Riderships.StationID , SUM(CAST(DailyTotal AS BIGINT)) as Total from Riderships inner join Stations on Riderships.StationID =Stations.StationID  where Riderships.StationID IN (select StationID from Stations) and Riderships.TypeOfDay ='U' Group By Riderships.StationID ,Stations.Name  order by Total DESC; ";
            }
            

            try
            {
                DataSet dataSet = dataTier.ExecuteNonScalarQuery(topStationSql);
                foreach (DataRow row in dataSet.Tables["TABLE"].Rows)
                {
                    var tmpStation = new CTAStation(Convert.ToInt32(row["StationID"]), Convert.ToString(row["Name"]));
                    stations.Add(tmpStation);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }


        public Boolean changeAdaStatus(int stopId,bool newStatus)
        {
            if(stopId <0)
            {
                throw new ApplicationException( "Couldn't find station" );
            }
            string changeAdaSql;
            if (newStatus)
            {
                changeAdaSql = String.Format(@"UPDATE Stops SET ADA = 1 WHERE StopID = {0};", stopId);
            }
            else
            {
                changeAdaSql = String.Format(@"UPDATE Stops SET ADA = 0 WHERE StopID = {0};", stopId);
            }
            
            dataTier.ExecuteActionQuery(changeAdaSql);
            return false;
        }       

    }//class
}//namespace
