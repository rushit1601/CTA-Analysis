using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            toolStripStatusLabel1.Text = "Loading...";
            MessageBox.Show("Please note that program take some time to load");
            try
            {
                string filename = this.txtDatabaseFilename.Text;

                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);

                var stationsList = bizTier.GetStations();

                foreach (BusinessTier.CTAStation station in stationsList)
                {
                    this.lstStations.Items.Add(station.Name);
                }

                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", stationsList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check point 103");
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }


        }


        //
        // User has clicked on a station for more info:
        //
        private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
                return;
            toolStripStatusLabel1.Text = "Loading...";
            //
            // clear GUI in case this fails:
            //
            ClearStationUI();
            
            //
            // now display info about selected station:
            //
            string stationName = this.lstStations.Text;
            
            try
            {
                int stationId=-1;//Set the default to negative

                //Object list of stops are set in using bussiness tier classes
                string filename = this.txtDatabaseFilename.Text;
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);
                long totalRidership = -1;
                var stationList = bizTier.GetStations();

                foreach (BusinessTier.CTAStation station in stationList)
                {
                    totalRidership += station.totalRidership;
                }


                    foreach (BusinessTier.CTAStation station in stationList)
                {
                    if(station.Name == stationName)//If station found add Id to local varible and then use it to find the stop
                    {
                        //Adds ridership data to UI
                        //And takes station ID for geting stops
                        this.txtTotalRidership.Text = Convert.ToString(station.totalRidership);
                        this.txtWeekdayRidership.Text = Convert.ToString(station.WeeklyRidership);
                        this.txtSaturdayRidership.Text = Convert.ToString(station.saturdayRidership);
                        this.txtSundayHolidayRidership.Text = Convert.ToString(station.holidayRidership);
                        this.txtStationID.Text = Convert.ToString(station.ID);
                        this.txtAvgDailyRidership.Text = Convert.ToString(station.totalRidership/station.operatoinalDays + "/day");
                        this.txtPercentRidership.Text = Convert.ToString( (Convert.ToDouble(station.totalRidership) / Convert.ToDouble(totalRidership) ) * 100);
                        stationId = station.ID;
                        break;
                    }
                }

                var stopsList = bizTier.GetStops(stationId);

                foreach (BusinessTier.CTAStop stop in stopsList)
                {
                    this.lstStops.Items.Add(stop.Name);
                }

                toolStripStatusLabel1.Text = "Done";

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                
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
      toolStripStatusLabel1.Text = "Loading...";
      string stopName = this.lstStops.Text;
      string filename = this.txtDatabaseFilename.Text;

      try
      {
        //Object list of stops are set in using bussiness tier classes
        BusinessTier.Business bizTier;
        bizTier = new BusinessTier.Business(filename);
        var allStopList = bizTier.GetAllStops();

        //Itetrate through each stop and fill the stop UI
        //Note: We are itrerating through the master list of stops to fill the UI
        foreach(BusinessTier.CTAStop stop in allStopList)
        {
             if(stop.Name == stopName)//If station found add Id to local varible and then use it to find the stop
             {
                  this.txtLocation.Text ="(" + Convert.ToString(stop.Latitude) +" , "+ Convert.ToString(stop.Longitude) + ")";
                  this.txtDirection.Text = stop.Direction;
                  if (stop.ADA)
                    this.txtAccessible.Text = "Yes";
                  else
                    this.txtAccessible.Text = "No";
                  foreach(String line in stop.lines)//Adding lines
                  {
                            this.lstLines.Items.Add(line);
                  }

                  break;
             }
        }
        toolStripStatusLabel1.Text = "Done";

      }
      catch (Exception ex)
      {
        string msg = string.Format("Error: '{0}'.", ex.Message);
        MessageBox.Show(msg);
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
      toolStripStatusLabel1.Text = "Loading...";
      //
      // now load top-10 stations:
      //
      

      try
      {
                //Object list of statoins are set in using bussiness tier classes
                string filename = this.txtDatabaseFilename.Text;
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);
                //input parameter helps in controling the no of top stations
                var topStationsList = bizTier.GetTopStations(10);

                foreach (BusinessTier.CTAStation station in topStationsList)
                {
                    this.lstStations.Items.Add(station.Name);
                }

                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", topStationsList);
      }
      catch (Exception ex)
      {
        string msg = string.Format("Error: '{0}'.", ex.Message);
        MessageBox.Show(msg);
      }

    }

        private void staurdayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            //
            // now load top-10 stations:
            //


            try
            {
                //Object list of statoins are set in using bussiness tier classes
                string filename = this.txtDatabaseFilename.Text;
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);

                /*
                 * Boolean val in funtion GetTopStationsBy if true means we get staurday top 10 and
                 * else we get holiday top 10
                 */
                var topStationsList = bizTier.GetTopStationsBy(true,10);

                foreach (BusinessTier.CTAStation station in topStationsList)
                {
                    this.lstStations.Items.Add(station.Name);
                }


                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", topStationsList);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
        }

        private void sundayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            //
            // now load top-10 stations:
            //


            try
            {
                //Object list of statoins are set in using bussiness tier classes
                string filename = this.txtDatabaseFilename.Text;
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);

                /*
                * Boolean val in funtion GetTopStationsBy if true means we get staurday top 10 and
                * else we get holiday top 10
                */
                var topStationsList = bizTier.GetTopStationsBy(false, 10);

                foreach (BusinessTier.CTAStation station in topStationsList)
                {
                    this.lstStations.Items.Add(station.Name);
                }


                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", topStationsList);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }

        }

        /*
         * Toggle button Changes the status of accesiblility 
         * And then edits the changes to database so it sticks
         */
        private void toggleButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Loading...";
            string accessibility = this.txtAccessible.Text;
            bool newStatus;

            //To toggle status in UI
            if (accessibility == "Yes")
            {
                this.txtAccessible.Text = "No";
                newStatus = false;
            }
            else
            {
                this.txtAccessible.Text = "Yes";
                newStatus = true;
            }

            string stopName = this.lstStops.Text;
            string filename = this.txtDatabaseFilename.Text;

            //Object list of statoins are set in using bussiness tier classes
            BusinessTier.Business bizTier;
            bizTier = new BusinessTier.Business(filename);
            var allStopList = bizTier.GetAllStops();
            var stopId = -1;

            foreach (BusinessTier.CTAStop stop in allStopList)
            {
                if (stop.Name == stopName)//If station found add Id to local varible and then use it to find the stop
                {
                    stopId = stop.ID;
                }
            }

            bizTier.changeAdaStatus(stopId, newStatus);//Adds the changes back to the database
            toolStripStatusLabel1.Text = "Done";
        }


        //Searh funtions reads all the stations from the database and returns a matching statoins
        private void searchButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Loading...";
            ClearStationUI(true);//True is used to clear the default text
            string searchStr = this.searchBox.Text;

            //Object list of statoins are set in using bussiness tier classes
            string filename = this.txtDatabaseFilename.Text;
            BusinessTier.Business bizTier;
            bizTier = new BusinessTier.Business(filename);
            var stationList = bizTier.GetStations();
            List<BusinessTier.CTAStation> searchStations= new List<BusinessTier.CTAStation>(); 

            //Loop throught each statoin object for a match it their is a hit add it to the list
            foreach (BusinessTier.CTAStation station in stationList)
            {
                if(station.Name.Contains(searchStr))
                {
                    searchStations.Add(station);
                }
            }
            //Finally add all the stations to UI
            foreach (BusinessTier.CTAStation  newStations in searchStations)
            {
                this.lstStations.Items.Add(newStations.Name);
            }
            toolStripStatusLabel1.Text = "Done";

        }
    }//class
}//namespace
