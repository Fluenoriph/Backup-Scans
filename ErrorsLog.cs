namespace ErrorsLog
{
    class LogError
    {
        public LogError()   // enum errors
        {
            using StreamWriter errors_file = new("C:\\Users\\Mahabhara\\Desktop\\error.txt", true);
                errors_file.WriteLine("103");
        }
    }



}
