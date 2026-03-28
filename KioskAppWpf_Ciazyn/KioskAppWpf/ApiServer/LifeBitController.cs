using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Windows;

namespace ApiServer
{
    public class LifeBitController : ApiController 
    {
        [HttpGet]
        [Route("api/lifebit")]
        public HttpResponseMessage GetLifeBit()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            var plcLifeBitValue = mainWindow.PlcLifeBit;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("bit bit")
            };
        }
    }
}
