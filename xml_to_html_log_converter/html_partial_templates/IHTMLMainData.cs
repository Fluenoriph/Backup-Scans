// *

interface IHTMLMainData
{
	// *

	// Параметр период: месяц или год. Соединить.

    static string CreateGeneralPart(string period, List<int> main_sums)
    {
		return $@"
				<!DOCTYPE html>
				<html>
					<head>
						<meta charset=""UTF-8"">
						<title>{backuping_month} {CurrentDate.Current_Year_Print_in}</title>
					</head>





					<body>
						<header>
							<h1>Отчет за {backuping_month}</h1>
						</header>
						<main>
							<h2>Суммы резервного блока</h2>
							<table style=""border: 1px #000000 solid; width: 300px;"">
								<caption>Общие суммы</caption>
								<tbody>
									<tr>
										<td style=""border: 1px #000000 solid;"">{ProtocolTypesAndSums.MAIN_SUMS[0]}</td><td style=""border: 1px #000000 solid"">{main_sums[0]}</td>
									</tr>
									<tr>
										<td style=""border: 1px #000000 solid"">{ProtocolTypesAndSums.MAIN_SUMS[1]}</td><td style=""border: 1px #000000 solid"">{main_sums[1]}</td>
									</tr>
									<tr>
										<td style=""border: 1px #000000 solid"">{ProtocolTypesAndSums.MAIN_SUMS[2]}</td><td style=""border: 1px #000000 solid"">{main_sums[2]}</td>
									</tr>
								</tbody>
							</table>";

		// Это не конец файла. Другие части должны присоединяться.
    }
}


/*
 * using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "C:/Users/Carma/Desktop/Мой сайт/html/math.html",
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }
}
*/