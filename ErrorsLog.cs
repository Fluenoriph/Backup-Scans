enum ErrorsCodes
{

}


class ErrorsLog
{
    public ErrorsLog()   // enum errors
    {
        using StreamWriter errors_file = new("C:\\Users\\Mahabhara\\Desktop\\error.txt", true);    // current program dir !!
            errors_file.WriteLine("103");
    }
}
