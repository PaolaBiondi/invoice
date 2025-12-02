namespace eInvoice.domain;

public class Class1
{

}

public interface IXmlMessagePort
{
    Task SendAsync(string message, string destination);
}
