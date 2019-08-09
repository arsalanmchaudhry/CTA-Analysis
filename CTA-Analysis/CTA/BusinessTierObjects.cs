//
// BusinessTier objects:  these classes define the objects serving as data 
// transfer between UI and business tier.  These objects carry the data that
// is normally displayed in the presentation tier.  The classes defined here:
//
//    CTAStation
//    CTAStop
//
// NOTE: the presentation tier should not be creating instances of these objects,
// but instead calling the BusinessTier logic to obtain these objects.  You can 
// create instances of these objects if you want, but doing so has no impact on
// the underlying data store --- to change the data store, you have to call the
// BusinessTier logic.
//

// 
// N-tier C# and SQL program to analyze CTA Ridership data. 
// 
// <<Muhammad Arsalan Chaudhry (mchaud25)>> 
// U. of Illinois, Chicago 
// CS341, Fall 2017 
// Project #08 
//

using System;
using System.Collections.Generic;


namespace BusinessTier
{

  ///
  /// <summary>
  /// Info about one CTA station.
  /// </summary>
  /// 
  public class CTAStation
  {
    public int ID { get; private set; }
    public string Name { get; private set; }

    public CTAStation(int stationID, string stationName)
    {
      ID = stationID;
      Name = stationName;
    }
  }
  
  ///
  /// <summary>
  /// Info about one CTA stop.
  /// </summary>
  /// 
  public class CTAStop
  {
    public int ID { get; private set; }

    public string Name { get; private set; }

    public int StationID { get; private set; }

    public string Direction { get; private set; }

    public bool ADA { get; private set; }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public CTAStop(int stopID, string stopName, int stationID, string direction, bool ada, double latitude, double longitude)
    {
      ID = stopID;
      Name = stopName;
      StationID = stationID;
      Direction = direction;
      ADA = ada;
      Latitude = latitude;
      Longitude = longitude;
    }
  }

  //Line info
  public class CTALine {
    public string Stop_Line { get; private set; }

    public CTALine(string stop_line)
    {
      Stop_Line = stop_line;
    }

  }
  //Ridership info about each station
  //Total Ridiership, average ridership, and %
  public class CTARidership {

    public string TotalRid { get; private set; }

    public string AvgRid { get; private set; }

    public string PerRid { get; private set; }



    public CTARidership(string total, string avg, string per)
    {
      TotalRid = total;
      AvgRid = avg;
      PerRid = per;
    }
  }


  //Station Id and Days
  public class CTADay
  {

    public string StationId { get; private set; }

    public string Weekday{ get; private set; }

    public string Saturday { get; private set; }

    public string SunHol { get; private set; }

    public CTADay(string stationId, string weekday, string sat, string sunHol)
    {
      StationId = stationId;
      Weekday = weekday;
      Saturday = sat;
      SunHol = sunHol;
    }
  }
}//namespace
