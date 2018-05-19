Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler
Imports System.Data.SqlClient

Namespace SchedulerBindDynamically
	Partial Public Class Form1
		Inherits Form
		Private dataSet As DataSet
		Private dataAdapter As SqlDataAdapter

		Public Sub New()
			InitializeComponent()

			BindScheduler()
		End Sub

		Private Sub BindScheduler()
			' 1) Retrieve data rows
			Dim connection As New SqlConnection("Data Source=.\SQLExpress;Initial Catalog=SchedulerBindDynamically;Integrated Security=SSPI")
			Dim selectCommand As New SqlCommand("SELECT ID, StartTime, EndTime, Subject FROM CarScheduling", connection)

			dataSet = New DataSet()
			dataAdapter = New SqlDataAdapter(selectCommand)

			dataAdapter.Fill(dataSet, "CarScheduling")

			' 2) Adjust mappings
			Dim schedulerStorage As SchedulerStorage = schedulerControl1.Storage
			Dim appointmentMappings As AppointmentMappingInfo = schedulerStorage.Appointments.Mappings

			appointmentMappings.AppointmentId = "ID"
			appointmentMappings.Start = "StartTime"
			appointmentMappings.End = "EndTime"
			appointmentMappings.Subject = "Subject"

			schedulerStorage.Appointments.CommitIdToDataSource = False

			' 3) Bind scheduler to data
			schedulerStorage.Appointments.DataSource = dataSet.Tables("CarScheduling")
			If schedulerStorage.Appointments.Count > 0 Then
				schedulerControl1.Start = schedulerStorage.Appointments(0).Start
			End If

			' 4) Define Insert, Update, Delete commands
			dataAdapter.InsertCommand = New SqlCommand("INSERT INTO CarScheduling (StartTime, EndTime, Subject, TimeStamp) VALUES (@StartTime, @EndTime, @Subject, GetDate())", connection)

			dataAdapter.InsertCommand.Parameters.Add("@StartTime", SqlDbType.DateTime)
			dataAdapter.InsertCommand.Parameters.Add("@EndTime", SqlDbType.DateTime)
			dataAdapter.InsertCommand.Parameters.Add("@Subject", SqlDbType.NVarChar)

			dataAdapter.InsertCommand.Parameters("@StartTime").SourceColumn = "StartTime"
			dataAdapter.InsertCommand.Parameters("@EndTime").SourceColumn = "EndTime"
			dataAdapter.InsertCommand.Parameters("@Subject").SourceColumn = "Subject"

			dataAdapter.UpdateCommand = New SqlCommand("UPDATE CarScheduling SET StartTime = @StartTime, EndTime = @EndTime, Subject = @Subject, TimeStamp = GetDate() WHERE ID = @ID", connection)

			dataAdapter.UpdateCommand.Parameters.Add("@ID", SqlDbType.Int)
			dataAdapter.UpdateCommand.Parameters.Add("@StartTime", SqlDbType.DateTime)
			dataAdapter.UpdateCommand.Parameters.Add("@EndTime", SqlDbType.DateTime)
			dataAdapter.UpdateCommand.Parameters.Add("@Subject", SqlDbType.NVarChar)

			dataAdapter.UpdateCommand.Parameters("@ID").SourceColumn = "ID"
			dataAdapter.UpdateCommand.Parameters("@StartTime").SourceColumn = "StartTime"
			dataAdapter.UpdateCommand.Parameters("@EndTime").SourceColumn = "EndTime"
			dataAdapter.UpdateCommand.Parameters("@Subject").SourceColumn = "Subject"

			dataAdapter.DeleteCommand = New SqlCommand("DELETE FROM CarScheduling WHERE ID = @ID", connection)
			dataAdapter.DeleteCommand.Parameters.Add("@ID", SqlDbType.Int)
			dataAdapter.DeleteCommand.Parameters("@ID").SourceColumn = "ID"

			' 5) Subscribe to events (data-related operations)
			AddHandler schedulerStorage.AppointmentsInserted, AddressOf Storage_AppointmentsModified
			AddHandler schedulerStorage.AppointmentsChanged, AddressOf Storage_AppointmentsModified
			AddHandler schedulerStorage.AppointmentsDeleted, AddressOf Storage_AppointmentsModified
			AddHandler dataAdapter.RowUpdated, AddressOf Adapter_RowUpdated
		End Sub

		Private Sub Storage_AppointmentsModified(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
			Try
				dataAdapter.Update(dataSet.Tables("CarScheduling"))
			Catch ex As DBConcurrencyException
				MessageBox.Show("Concurrency violation:" & Constants.vbCrLf & ex.Row("Subject").ToString())
			End Try
			'dataSet.AcceptChanges();
		End Sub

		Private Sub Adapter_RowUpdated(ByVal sender As Object, ByVal e As System.Data.SqlClient.SqlRowUpdatedEventArgs)
			If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
				Dim id As Integer = 0
				Using cmd As New SqlCommand("SELECT IDENT_CURRENT('CarScheduling')", dataAdapter.SelectCommand.Connection)
					id = Convert.ToInt32(cmd.ExecuteScalar())
				End Using
				e.Row("ID") = id
			End If
		End Sub
	End Class
End Namespace