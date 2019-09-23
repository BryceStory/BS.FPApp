namespace FiiiPay.Foundation.Business.Agent
{
    public interface IImage
    {
        byte[] Download(string id);

        string Upload(string fileName, byte[] bytes);
    }
}
