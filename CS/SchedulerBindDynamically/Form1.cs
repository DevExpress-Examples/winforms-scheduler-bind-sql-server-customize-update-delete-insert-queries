using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using System.Data.SqlClient;

namespace SchedulerBindDynamically {
    public partial class Form1 : Form {
        private DataSet dataSet;
        private SqlDataAdapter dataAdapter;

        public Form1() {
            InitializeComponent();

            BindScheduler();
        }

        private void BindScheduler() {
            // 1) Retrieve data rows
            SqlConnection connection = new SqlConnection(@"Data Source=.\SQLExpress;Initial Catalog=SchedulerBindDynamically;Integrated Security=SSPI");
            SqlCommand selectCommand = new SqlCommand("SELECT ID, StartTime, EndTime, Subject FROM CarScheduling", connection);

            dataSet = new DataSet();
            dataAdapter = new SqlDataAdapter(selectCommand);

            dataAdapter.Fill(dataSet, "CarScheduling");

            // 2) Adjust mappings
            SchedulerStorage schedulerStorage = schedulerControl1.Storage;
            AppointmentMappingInfo appointmentMappings = schedulerStorage.Appointments.Mappings;

            appointmentMappings.AppointmentId = "ID";
            appointmentMappings.Start = "StartTime";
            appointmentMappings.End = "EndTime";
            appointmentMappings.Subject = "Subject";

            schedulerStorage.Appointments.CommitIdToDataSource = false;

            // 3) Bind scheduler to data
            schedulerStorage.Appointments.DataSource = dataSet.Tables["CarScheduling"];
            if (schedulerStorage.Appointments.Count > 0)
                schedulerControl1.Start = schedulerStorage.Appointments[0].Start;

            // 4) Define Insert, Update, Delete commands
            dataAdapter.InsertCommand = new SqlCommand("INSERT INTO CarScheduling (StartTime, EndTime, Subject, TimeStamp) VALUES (@StartTime, @EndTime, @Subject, GetDate())", connection);

            dataAdapter.InsertCommand.Parameters.Add("@StartTime", SqlDbType.DateTime);
            dataAdapter.InsertCommand.Parameters.Add("@EndTime", SqlDbType.DateTime);
            dataAdapter.InsertCommand.Parameters.Add("@Subject", SqlDbType.NVarChar);

            dataAdapter.InsertCommand.Parameters["@StartTime"].SourceColumn = "StartTime";
            dataAdapter.InsertCommand.Parameters["@EndTime"].SourceColumn = "EndTime";
            dataAdapter.InsertCommand.Parameters["@Subject"].SourceColumn = "Subject";

            dataAdapter.UpdateCommand = new SqlCommand("UPDATE CarScheduling SET StartTime = @StartTime, EndTime = @EndTime, Subject = @Subject, TimeStamp = GetDate() WHERE ID = @ID", connection);

            dataAdapter.UpdateCommand.Parameters.Add("@ID", SqlDbType.Int);
            dataAdapter.UpdateCommand.Parameters.Add("@StartTime", SqlDbType.DateTime);
            dataAdapter.UpdateCommand.Parameters.Add("@EndTime", SqlDbType.DateTime);
            dataAdapter.UpdateCommand.Parameters.Add("@Subject", SqlDbType.NVarChar);

            dataAdapter.UpdateCommand.Parameters["@ID"].SourceColumn = "ID";
            dataAdapter.UpdateCommand.Parameters["@StartTime"].SourceColumn = "StartTime";
            dataAdapter.UpdateCommand.Parameters["@EndTime"].SourceColumn = "EndTime";
            dataAdapter.UpdateCommand.Parameters["@Subject"].SourceColumn = "Subject";

            dataAdapter.DeleteCommand = new SqlCommand("DELETE FROM CarScheduling WHERE ID = @ID", connection);
            dataAdapter.DeleteCommand.Parameters.Add("@ID", SqlDbType.Int);
            dataAdapter.DeleteCommand.Parameters["@ID"].SourceColumn = "ID";

            // 5) Subscribe to events (data-related operations)
            schedulerStorage.AppointmentsInserted += Storage_AppointmentsModified;
            schedulerStorage.AppointmentsChanged += Storage_AppointmentsModified;
            schedulerStorage.AppointmentsDeleted += Storage_AppointmentsModified;
            dataAdapter.RowUpdated += Adapter_RowUpdated;
        }

        void Storage_AppointmentsModified(object sender, PersistentObjectsEventArgs e) {
            try {
                dataAdapter.Update(dataSet.Tables["CarScheduling"]);
            }
            catch (DBConcurrencyException ex) {
                MessageBox.Show("Concurrency violation:\r\n" + ex.Row["Subject"].ToString());
            }
            //dataSet.AcceptChanges();
        }

        void Adapter_RowUpdated(object sender, System.Data.SqlClient.SqlRowUpdatedEventArgs e) {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert) {
                int id = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT IDENT_CURRENT('CarScheduling')", dataAdapter.SelectCommand.Connection)) {
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
                e.Row["ID"] = id;
            }
        }
    }
}