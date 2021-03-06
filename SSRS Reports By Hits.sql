SELECT
catalog.Name
, ExecutionLog.UserName
--, COUNT(ExecutionLog.UserName)
--, ExecutionLog.TimeStart
, COUNT(catalog.Name) [Hits Per Name]
  FROM [ReportServer].[dbo].ExecutionLog
 Inner join Catalog
  on catalog.ItemID = ExecutionLog.ReportID

  group by Name,UserName

  order by COUNT(catalog.Name) DESC