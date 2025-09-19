interface IXMLNullError<T>
{
    static void CheckItem(T? item)
    {
        if (item is null)
        {
            _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
        }
    }
}
