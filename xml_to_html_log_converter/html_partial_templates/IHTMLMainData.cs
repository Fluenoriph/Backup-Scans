// *

interface IHTMLMainData
{
	// Параметр период: месяц или год. Соединить.

    static string CreateGeneralPart(string period, string main_sums)
    {
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
								<caption>Общие суммы</caption>
								<tbody>
									{IHTMLTablePart.PutString(ProtocolTypesAndSums.MAIN_SUMS[0], main_sums)}								
								";
							
		// Это не конец файла. Другие части должны присоединяться.
    }
}
