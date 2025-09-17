interface IHTMLTablePart
{
    static string PutString(string sum_type, string sum_value)
    {
        return $@"
                <tr>
				    <td style=""border: 1px #000000 solid"">{sum_type}</td><td style=""border: 1px #000000 solid"">{sum_value}</td>
				</tr>";
    }
}
