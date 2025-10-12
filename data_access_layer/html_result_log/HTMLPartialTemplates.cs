// * Файл "HTMLPartialTemplates.cs": класс, деконструкция файла отчета в формате HTML. *

class HTMLPartialTemplates       // Возможно разделение на несколько классов ....   структуры тэгов уровнем ниже ??
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
			<html lang=""ru"">
				<head>
					<meta charset=""UTF-8"">
                    <link href=""style.css"" rel=""stylesheet"">
					<title>{period}</title>   
				</head>
				<body>
					<header>
						<h1>Отчет за {period}</h1>
					</header>
					<main>
                        <section id=""sums"">
						    <h2>Суммы резервного блока</h2>
                            <div>
						        <table>
							        <caption>Общее количество</caption>
							        <tbody>					
			";
    }

	// * Добавить строку таблицы с типом суммы и ее значением. *

    public static string PutTableString(string sum_type, string sum_value)
    {
        return $@"
                <tr>
				    <td>{sum_type}</td><td>{sum_value}</td>
				</tr>";
    }

    // * Добавить закрытие таблицы главных сумм. *

    public static string PutMainSumsTableEnd()
    {
        return @"
                    </tbody>
			    </table>
		    </div>";
    }

    // * Добавить закрытие таблиц и секции сумм протоколов. *

    public static string PutSumsTableSectionEnd()
    {
        return string.Concat(PutMainSumsTableEnd(), "</section>");
    }

	// * Добавить заголовок секции имен протоколов. *

    public static string PutProtocolNamesSectionHeader()
    {
        return $@"
            <section id=""names"">
				<h2>Имена сканов протоколов</h2>";
    }

    // * Добавить заголовок секции таблицы сумм по типам обычных протоколов. *

    public static string PutTableSectionHeader(string section_name)
    {
        return $@"
                <tr>
				    <th colspan=""2"">{section_name}</th>
				</tr>";
    }

	// * Добавить одноуровневый список имен протоколов. *

    public static string PutSingleLevelList(string protocol_names)
    {
        return $@"
            <ul>
				<li>{protocol_names}</li>
			</ul>";
    }

    // * Добавить окончание лог файла. *

    public static string PutEndFile()
    {
        return @"
                        </section>
                    </main>
	            </body>
            </html>";
    }

    // * Добавить заголовок таблицы по суммам обычных протоколов. *

    public static string PutSimpleProtocolsSumsTableHeader()
    {
        return @"
            <div>
                <table>
			        <caption>Суммы обычных протоколов</caption>
				    <tbody>";
    }

    // * Начало раздела. *

    public static string PutStartDivision()
    {
        return "<div>";
    }

    // * Конец раздела. *

    public static string PutEndDivision()
    {
        return "</div>";
    }

    // * Заголовок третьего уровня. *

    public static string PutHeaderThirdLevel(string name)
    {
        return $"<h3>{name}</h3>";
    }

    // * Добавить параграф. *

    public static string PutParagraph(string name)
    {
        return $"<p>{name}</p>";
    }
}
