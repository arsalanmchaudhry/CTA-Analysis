using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


// 
// N-tier C# and SQL program to analyze CTA Ridership data. 
// 
// <<Muhammad Arsalan Chaudhry (mchaud25)>> 
// U. of Illinois, Chicago 
// CS341, Fall 2017 
// Project #08 
//



namespace CTA
{

  public partial class Form1 : Form
  {
    private string BuildConnectionString()
    {
      string version = "MSSQLLocalDB";
      string filename = this.txtDatabaseFilename.Text;

      string connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", version, filename);

      return connectionInfo;
    }

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //
      // setup GUI:
      //
      this.lstStations.Items.Add("");
      this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
      this.lstStations.Items.Add("");

      this.lstStations.ClearSelected();

      toolStripStatusLabel1.Text = string.Format("Number of stations:  0");

      // 
      // open-close connect to get SQL Server started:
      //
      SqlConnection db = null;

      try
      {
        string filename = this.txtDatabaseFilename.Text;

        BusinessTier.Business bizTier;
        bizTier = new BusinessTier.Business(filename);

        bizTier.TestConnection();
      }
      catch
      {
        //         
        // ignore any exception that occurs, goal is just to startup         
        //       
      }
      finally
      {
        // close connection:
        if (db != null && db.State == ConnectionState.Open)
          db.Close();
      }
    }


    //
    // File>>Exit:
    //
    private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      this.Close();
    }


    //
    // File>>Load Stations:
    //
    private void toolStripMenuItem2_Click(object sender, EventArgs e)
    {
      //
      // clear the UI of any current results:
      //
      ClearStationUI(true /*clear stations*/);

      //
      // now load the stations from the database:
      //
        //get data from database
        var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
        //go get stations
        var stations = biztier.GetStations();

        //display stations
        foreach (var s in stations) {
          this.lstStations.Items.Add(s.Name);
        }
        toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}",stations.Count);

    }


    //
    // User has clicked on a station for more info:
    //
    private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
    {
      // sometimes this event fires, but nothing is selected...
      if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
        return; 
      
      //
      // clear GUI in case this fails:
      //
      ClearStationUI();

      //
      // now display info about selected station:
      //
      string stationName = this.lstStations.Text;
      stationName = stationName.Replace("'", "''");

      //Bussiness tier
      var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

      //Ridership Info (total,avg,%)
      var ridershipInfo = biztier.GetRiderInfo(stationName);
      this.txtTotalRidership.Text    = ridershipInfo.TotalRid;
      this.txtAvgDailyRidership.Text = ridershipInfo.AvgRid;
      this.txtPercentRidership.Text  = ridershipInfo.PerRid;

      //station id
      var id_day_info = biztier.GetIDAndDay(stationName);
      this.txtStationID.Text = id_day_info.StationId;

      //Days
      this.txtWeekdayRidership.Text = id_day_info.Weekday;
      this.txtSaturdayRidership.Text = id_day_info.Saturday;
      this.txtSundayHolidayRidership.Text = id_day_info.SunHol;

      //
      // finally, what stops do we have at this station?
      //
      var station_stops = biztier.GetStops(Convert.ToInt32(id_day_info.StationId));
      
      // display stops
      foreach (var s in station_stops)
      {
         this.lstStops.Items.Add(s.Name);
      }
    }

    private void ClearStationUI(bool clearStatations = false)
    {
      ClearStopUI();

      this.txtTotalRidership.Clear();
      this.txtTotalRidership.Refresh();

      this.txtAvgDailyRidership.Clear();
      this.txtAvgDailyRidership.Refresh();

      this.txtPercentRidership.Clear();
      this.txtPercentRidership.Refresh();

      this.txtStationID.Clear();
      this.txtStationID.Refresh();

      this.txtWeekdayRidership.Clear();
      this.txtWeekdayRidership.Refresh();
      this.txtSaturdayRidership.Clear();
      this.txtSaturdayRidership.Refresh();
      this.txtSundayHolidayRidership.Clear();
      this.txtSundayHolidayRidership.Refresh();

      this.lstStops.Items.Clear();
      this.lstStops.Refresh();

      if (clearStatations)
      {
        this.lstStations.Items.Clear();
        this.lstStations.Refresh();
      }
    }


    //
    // user has clicked on a stop for more info:
    //
    private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
    {
      // sometimes this event fires, but nothing is selected...
      if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
        return; 

      //
      // clear GUI in case this fails:
      //
      ClearStopUI();

      //
      // now display info about this stop:
      //
      string stopName = this.lstStops.Text;
      stopName = stopName.Replace("'", "''");

      //
      // Let's get some info about the stop:
      //
      // NOTE: we want to use station id, not stop name,
      // because stop name is not unique.  Example: the
      // stop "Damen (Loop-bound)".s
      //

      var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

     //Ridership Info (total,avg,%)
     var stopInfo = biztier.GetStopInfo(stopName, this.txtStationID.Text);

      //Handicap
      this.txtAccessible.Text = "Yes";
      if (!stopInfo.ADA)
      {
        this.txtAccessible.Text = "No";
      }

      //direction
      this.txtDirection.Text = stopInfo.Direction;

      //lat and lon
      this.txtLocation.Text = string.Format("({0:00.0000}, {1:00.0000})",stopInfo.Latitude,stopInfo.Longitude);


      //
      // now we need to know what lines are associated 
      // with this stop:
      //

      //get data from database
      var biztier2 = new BusinessTier.Business(this.txtDatabaseFilename.Text);
      var lines = biztier2.GetLines(stopName);

      //display lines
      foreach (var l in lines)
      {
        this.lstLines.Items.Add(l.Stop_Line);
      }
    }

    private void ClearStopUI()
    {
      this.txtAccessible.Clear();
      this.txtAccessible.Refresh();

      this.txtDirection.Clear();
      this.txtDirection.Refresh();

      this.txtLocation.Clear();
      this.txtLocation.Refresh();

      this.lstLines.Items.Clear();
      this.lstLines.Refresh();
    }


    //
    // Top-10 stations in terms of ridership:
    //
    private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //
      // clear the UI of any current results:
      //
      ClearStationUI(true /*clear stations*/);

      //
      // now load top-10 stations:
      //   
        //get data from database
        var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
        //go get stations
        var top10_stations = biztier.GetTopStations(10); //call function to get top ten stations

        //display Top 10 stations
        foreach (var s in top10_stations)
        {
          this.lstStations.Items.Add(s.Name);
        }

        toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", top10_stations.Count);
      
    }

  }//class
}//namespace
