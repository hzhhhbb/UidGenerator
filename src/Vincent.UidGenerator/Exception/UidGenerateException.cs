namespace Vincent.UidGenerator.Exception;

public class UidGenerateException:System.Exception
{
    public UidGenerateException():base()
    {
    }

    public UidGenerateException(string message) : base(message)
    {
        
    }

    public UidGenerateException( string message,System.Exception e) : base(message, e)
    {
        
    }
}