using LibUsbDotNet.Main;
using LibUsbDotNet;

var settings = new Settings();
var deviceFinder = new UsbDeviceFinder(settings.VendorId, settings.ProductId);
var device = UsbDevice.OpenUsbDevice(deviceFinder);
if (device == null)
{
    Console.WriteLine("Device not found.");
    return;
}

var wholeDevice = device as IUsbDevice;
if (wholeDevice != null)
{
    wholeDevice.SetConfiguration(1);
    wholeDevice.ClaimInterface(settings.Interface);
}

var reader = device.OpenEndpointReader(
    settings.Endpoint, 
    UsbEndpointReader.DefReadBufferSize, 
    settings.TransferType);

var continueReading = true;
var readThread = new Thread(() => 
{
    var readBuffer = new byte[settings.PacketSize];

    while (continueReading)
    {
        var error = reader.Read(readBuffer, settings.ReadTimeout, out int bytesRead);

        if (error == ErrorCode.None && bytesRead > 0)
            Console.WriteLine($"{bytesRead} bytes: {BitConverter.ToString(readBuffer, 0, bytesRead)}");
        else if (error != ErrorCode.None && error != ErrorCode.IoTimedOut)
            Console.WriteLine("Error: " + error);
        
        Thread.Sleep(settings.ReadInterval);
    }
});

readThread.Start();

Console.WriteLine("Reading started. Press 'q' to stop.");
while (Console.ReadKey(true).KeyChar != 'q')
{ }

continueReading = false;
readThread.Join();
wholeDevice?.ReleaseInterface(0);
device.Close();

UsbDevice.Exit();
