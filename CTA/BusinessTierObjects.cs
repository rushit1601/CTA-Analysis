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
    //Additional instance variables by student
    public int totalRidership { get; set; }
    public int WeeklyRidership { get; set; }
    public int saturdayRidership { get; set; }
    public int holidayRidership { get; set; }
    public int operatoinalDays { get; set; }



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
        public List<String> lines { get; set; }



        public int getIdFromName(String name)
        {
            if (name == this.Name)
            {
                return this.ID;
            }

            return -1;
        }



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

   public class CTARidership
  {
    //Additional methods by student
    public int stationID { get; private set; }
    public int totalRidership { get; set; }
    public int WeeklyRidership { get; set; }
    public int saturdayRidership { get; set; }
    public int holidayRidership { get; set; }


    public CTARidership(int stationId,int total, int weekly, int saturday,int holiday)
    {
      stationID=stationId;
      totalRidership = total;
      WeeklyRidership = weekly;
      saturdayRidership = saturday;
      holidayRidership =holiday;
    }


   }

}//namespace
