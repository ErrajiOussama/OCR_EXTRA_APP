select c.column_name
from information_schema.tables t
inner join information_schema.columns c on c.table_name = t.table_name
where c.table_name = 'acte_ocr'