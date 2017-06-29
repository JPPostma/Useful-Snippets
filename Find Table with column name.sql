select top (100) * from INFORMATION_SCHEMA.COLUMNS 
where COLUMN_NAME like '%City%' 
order by TABLE_NAME