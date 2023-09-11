<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128633773/14.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E4436)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# WinForms Scheduler - Bind to an SQL Server database and customize Update-Delete-Insert queries without the use of SqlCommandBuilder

This example demonstrates how to bind the WinForms Scheduler control at runtime. The example shows hot to configure appointment mappings and SQL queries in code. This technique allows you to easily modify data-binding options and SQL queries.

In this example, we insert a [GETDATE](https://learn.microsoft.com/en-us/sql/t-sql/functions/getdate-transact-sql?view=sql-server-ver16&redirectedfrom=MSDN) function result into a **TimeStamp** column to save the modification time along with the modified row:</p>

```sql
INSERT INTO CarScheduling (StartTime, EndTime, Subject, TimeStamp) VALUES (@StartTime, @EndTime, @Subject, GetDate())

UPDATE CarScheduling SET StartTime = @StartTime, EndTime = @EndTime, Subject = @Subject, TimeStamp = GetDate() WHERE ID = @ID

DELETE FROM CarScheduling WHERE ID = @ID
```

Do the following to test this example locally:

* Setup the "SchedulerBindDynamically" sample database in your SQL Server instance.
* Use the *SchedulerBindDynamically.sql* file attached to this example to generate a sample database and table schema.

> **Note**
>
> We do not use the [SqlCommandBuilder](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommandbuilder?view=dotnet-plat-ext-7.0&redirectedfrom=MSDN) class to generate SQL queries as it is shown in [How to bind SchedulerControl to MS SQL Server database at runtime](https://supportcenter.devexpress.com/ticket/details/e551/winforms-scheduler-bind-to-ms-sql-server-runtime). The `DbCommandBuilder.ConflictOption` property enables optimistic concurrency to prevent a concurrency violation error. Since we do not use this functionality, you might encounter this error when using this example. Read the following MSDN article for additional information: [Introduction to Data Concurrency in ADO.NET](http://msdn.microsoft.com/en-us/library/cs6hb8k4.aspx).


## Files to Review

* [Form1.cs](./CS/Form1.cs) (VB: [Form1.vb](./VB/Form1.vb))


## See Also

* [Updating Data Sources with DataAdapters](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/updating-data-sources-with-dataadapters?redirectedfrom=MSDN)
* [WinForms Scheduler - Getting Started](https://docs.devexpress.com/WindowsForms/2949/controls-and-libraries/scheduler/getting-started)
