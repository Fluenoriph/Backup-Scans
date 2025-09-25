class HTMLPartialTemplates
{
	// * Создание главной части лог файла. *
	
    public static string PutMainPart(string? period = null)
    {
        // Входной параметр не null, это отчет за месяц, иначе за год.

        if (period is not null)
		{
			period = $"{period} {CurrentDate.Current_Year_Print_in}";
		}
		else
		{
            period = CurrentDate.Current_Year_Print_in;
        }

        return $@"
			<!DOCTYPE html>
			<html>
				<head>
					<meta charset=""UTF-8"">
					<title>{period}</title>   
				</head>
				<body>
					<header>
						<h1>Отчет за {period}</h1>
					</header>
					<main>
						<h2>Суммы резервного блока</h2>
						<table style=""border: 1px #000000 solid; width: 300px;"">
							<caption>Общее количество</caption>
							<tbody>					
			";
    }

	// * Добавить строку таблицы с типом суммы и ее значением. *

    public static string PutString(string sum_type, string sum_value)
    {
        return $@"
                <tr>
				    <td style=""border: 1px #000000 solid"">{sum_type}</td><td style=""border: 1px #000000 solid"">{sum_value}</td>
				</tr>";
    }

	// * Добавить заголовок секции имен протоколов. *

    public static string PutProtocolNamesSectionHeader()
    {
        return @"
            <hr>
				<h2>Имена сканов протоколов</h2>";
    }

    // * Добавить заголовок секции таблицы сумм по типам обычных протоколов. *

    public static string PutTableSectionHeader(string section_name)
    {
        return $@"
                <tr>
				    <th colspan=""2"" style=""border: 1px #000000 solid"">{section_name}</th>
				</tr>";
    }

	// * Добавить одноуровневый список имен протоколов по названию типа. *

    public static string PutSingleLevelList(string protocol_type, string protocol_names)
    {
        return $@"
            <p>{protocol_type}</p>
				<ul>
					<li>{protocol_names}</li>
				</ul>";
    }

	// * Добавить двухуровневый список имен протоколов по названию локации (города). *

    public static string PutTwoLevelList(string location_name, string protocol_names)
    {
        return $@"
            <li>{location_name}:
			    <ul>
				    <li>{protocol_names}</li>
				</ul>
			</li>";
    }

    // * Добавить окончание лог файла. *

    public static string PutEndFile()
    {
        return @"
                    </main>
	            </body>
            </html>";
    }

    // * Добавить заголовок таблицы по суммам обычных протоколов. *

    public static string PutSimpleProtocolsSumsTableHeader()
    {
        return @"
            <table style=""border: 1px #000000 solid"">
			    <caption>Суммы обычных протоколов</caption>
				<tbody>";
    }

    // * Добавить конец таблицы сумм обычных протоколов. *

    public static string PutSimpleProtocolsSumsTableEnd()
    {
        return @"
                </tbody>
		    </table>";
    }

    // * Добавить заголовок типа обычных протоколов. *

    public static string PutProtocolNamesSimpleTypeHeader(string protocol_type)
    {
        return $@"
            <p>{protocol_type}</p>
				<ul>";
    }

    // Конец секции имен типов обычных протоколов.

    public const string SIMPLE_PROTOCOL_TYPE_NAMES_SECTION_END = "</ul>";
}
