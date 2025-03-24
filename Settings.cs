using LibUsbDotNet.Main;
using Microsoft.Extensions.Configuration;

class Settings
{
    private readonly IConfigurationSection _section;

    public int VendorId => Convert.ToInt32(_section["VendorId"]!, 16);
    public int ProductId => Convert.ToInt32(_section["ProductId"]!, 16);
    public int Interface => int.Parse(_section["Interface"]!);
    public ReadEndpointID Endpoint => (ReadEndpointID)Convert.ToByte(_section["Endpoint"]!, 16);
    public EndpointType TransferType => Enum.Parse<EndpointType>(_section["TransferType"]!);
    public int PacketSize => int.Parse(_section["PacketSize"]!);
    public int ReadTimeout => int.Parse(_section["ReadTimeout"]!);
    public int ReadInterval => int.Parse(_section["ReadInterval"]!);

    public Settings()
    {
        var path = Directory
            .GetParent(AppContext.BaseDirectory)
            ?.Parent?.Parent?.Parent?.FullName!;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _section = configuration.GetSection("UsbSettings");
    }
}
