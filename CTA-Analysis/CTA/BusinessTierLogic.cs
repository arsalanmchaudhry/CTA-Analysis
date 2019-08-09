//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;



// 
// N-tier C# and SQL program to analyze CTA Ridership data. 
// 
// <<Muhammad Arsalan Chaudhry (mchaud25)>> 
// U. of Illinois, Chicago 
// CS341, Fall 2017 
// Project #08 
//

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

      try
      {

        //
        // TODO!
        //
        //db = new SqlConnection(BuildConnectionString());
        //db.Open();

        string sql = string.Format(@"SELECT Name, StationID FROM Stations ORDER BY Name ASC;");

        //MessageBox.Show(sql);

        //SqlCommand cmd = new SqlCommand();
        //cmd.Connection = db;
        //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

        //cmd.CommandText = sql;
        //dapter.Fill(ds);
       
        foreach (DataRow row in ds.Tables["TABLE"].Rows)
        {
          //this.lstStations.Items.Add(row["Name"].ToString());
          var ss = new CTAStation(Convert.ToInt32(row["StationID"]), row["Name"].ToString() );

          //add to stations
          stations.Add(ss);
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

      try
      {

        //
        // TODO!
        //
        // String sql = string.Format(@"SELECT Stops.Name as StopName FROM Stops INNER JOIN Stations ON Stops.StationID = Stations.StationID WHERE Stations.Name = '{0}' ORDER BY Stops.Name ASC;", nameStation);
        string sql = string.Format(@"SELECT Latitude, Longitude, ADA, Direction, StopID,Name,StationID FROM Stops WHERE StationID = {0} ORDER BY Name ASC;", stationID);

        //adapter = new SqlDataAdapter(cmd);
        DataSet ds = dataTier.ExecuteNonScalarQuery(sql);/*new DataSet();*/


        //stops
        foreach (DataRow row in ds.Tables["TABLE"].Rows)
        {
          //this.listBox2.Items.Add(row7["StopName"].ToString());
          //int stopID, string stopName, int stationID, string direction, bool ada, double latitude, double longitude
          var ss = new CTAStop( Convert.ToInt32(row["StopID"]),
                                row["Name"].ToString(),
                                Convert.ToInt32(row["StationID"]),
                                row["Direction"].ToString(),
                                Convert.ToBoolean(row["ADA"]),
                                Convert.ToDouble(row["Latitude"]),
                                Convert.ToDouble(row["Longitude"])
                                );

          //Add stops
          stops.Add(ss);

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

      try
      {

        //
        // TODO!
        //
        string sql = string.Format(@"
SELECT Top {0} Name, Stations.StationID, Sum(DailyTotal) As TotalRiders 
FROM Riderships
INNER JOIN Stations ON Riderships.StationID = Stations.StationID 
GROUP BY Stations.StationID, Name
ORDER BY TotalRiders DESC;
",N);

        //MessageBox.Show(sql);

        //SqlCommand cmd = new SqlCommand();
        //cmd.Connection = db;
        //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        DataSet ds = dataTier.ExecuteNonScalarQuery(sql);/*new DataSet();*/

        //cmd.CommandText = sql;
        //adapter.Fill(ds);

        // display stations:
        foreach (DataRow row in ds.Tables["TABLE"].Rows)
        {
          var ss = new CTAStation(Convert.ToInt32(row["StationID"]), row["Name"].ToString());

          //add to stations
          stations.Add(ss);

          //this.lstStations.Items.Add(row["Name"].ToString());
        }

      }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    }


    //Function to get Ridership Info
    public CTARidership GetRiderInfo(string nameStation) {

  
      String sql = string.Format(@" SELECT Sum(DailyTotal) As Total, Avg(DailyTotal) As Average FROM Riderships INNER JOIN Stations ON Riderships.StationID = Stations.StationID WHERE Name = '{0}';", nameStation);
      
      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

      DataRow row = ds.Tables["TABLE"].Rows[0];
      DataRow row2 = ds.Tables["TABLE"].Rows[0];

      //% Ridership
      string sql2 = string.Format(@"SELECT Sum(Convert(bigint,DailyTotal)) As TotalAll FROM Riderships;");
      object result = dataTier.ExecuteScalarQuery(sql2);

      string per = string.Format("{0:0.00}%", ((double)Convert.ToInt64(row["Total"])) / Convert.ToInt64(result) * 100.0);
      
      var ss = new CTARidership(Convert.ToInt32(row["Total"]).ToString("#,0"), Convert.ToDouble(row2["Average"]).ToString("0#,##0/day"),per);

      return ss;
    }

    //Function to get station id and day info
    public CTADay GetIDAndDay(string nameStation)
    {

      //station id
      string sql = string.Format(@" SELECT Riderships.StationID as sID FROM Riderships INNER JOIN Stations ON Riderships.StationID = Stations.StationID WHERE Name = '{0}';", nameStation);
      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
      DataRow row = ds.Tables["TABLE"].Rows[0];
      string statID = Convert.ToInt32(row["sID"]).ToString("#0");


      //TypeOfDay: a single character where ‘W’ denotes a weekday, ‘A’ denotes Saturday, and ‘U’ denotes Sunday or Holiday.   
      //weekday
      string sql1 = string.Format(@" SELECT Sum(DailyTotal) As Weekday FROM Riderships INNER JOIN Stations ON Riderships.StationID = Stations.StationID WHERE Name = '{0}' AND TypeOfDay = 'W';", nameStation);
      DataSet ds1 = dataTier.ExecuteNonScalarQuery(sql1);
      DataRow row1 = ds1.Tables["TABLE"].Rows[0];
      string weekday = Convert.ToInt32(row1["Weekday"]).ToString("#,0");

      //Saturday
      string sql2 = string.Format(@" SELECT Sum(DailyTotal) As Saturday FROM Riderships INNER JOIN Stations ON Riderships.StationID = Stations.StationID WHERE Name = '{0}' AND TypeOfDay = 'A';", nameStation);
      DataSet ds2 = dataTier.ExecuteNonScalarQuery(sql2);
      DataRow row2 = ds2.Tables["TABLE"].Rows[0];
      string sat = Convert.ToInt32(row2["Saturday"]).ToString("#,0");

      //Sunday and Holiday
      string sql3 = string.Format(@" SELECT Sum(DailyTotal) As SundayHol FROM Riderships INNER JOIN Stations ON Riderships.StationID = Stations.StationID WHERE Name = '{0}' AND TypeOfDay = 'U';", nameStation);
      DataSet ds3 = dataTier.ExecuteNonScalarQuery(sql3);
      DataRow row3 = ds3.Tables["TABLE"].Rows[0];
      string sunHol = Convert.ToInt32(row3["SundayHol"]).ToString("#,0");

      var ss = new CTADay(statID,weekday,sat,sunHol);
      return ss;
    }

    //Function to get info on each stop (handicap,direction of travel, lat/lon)
    public CTAStop GetStopInfo(string name_stop,string station_id)
    {

      string sql = string.Format(@"SELECT * FROM Stops WHERE Name = '{0}' AND StationID = {1};", name_stop, station_id);
      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
      DataRow row = ds.Tables["TABLE"].Rows[0];

      //Handicap
      /*
      string ada = "Yes";
      if (!Convert.ToBoolean(row["ADA"]))
      {
        ada = "No";
      }*/

      //Direction
      string direction = row["Direction"].ToString();

      //Location
      double lat = Convert.ToDouble(row["Latitude"]);
      double lon = Convert.ToDouble(row["Longitude"]);

      var ss = new CTAStop(Convert.ToInt32(row["StopID"]),
                      row["Name"].ToString(),
                      Convert.ToInt32(row["StationID"]),
                      direction,
                      Convert.ToBoolean(row["ADA"]),
                      lat,
                      lon
                      );
      return ss;
    }



    public IReadOnlyList<CTALine> GetLines(string name_stop)
    {
      List<CTALine> stop_lines = new List<CTALine>();

      try
      {
        string sql = string.Format(@"SELECT Color as Line FROM Lines INNER JOIN StopDetails ON Lines.LineID = StopDetails.LineID INNER JOIN Stops ON StopDetails.StopID = Stops.StopID WHERE Stops.Name = '{0}'", name_stop);

        DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

        foreach (DataRow row in ds.Tables["TABLE"].Rows)
        {
          var line = new CTALine(Convert.ToString(row["Line"]));

          stop_lines.Add(line);
        }

      }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stop_lines;
    }


  }//class
}//namespace
