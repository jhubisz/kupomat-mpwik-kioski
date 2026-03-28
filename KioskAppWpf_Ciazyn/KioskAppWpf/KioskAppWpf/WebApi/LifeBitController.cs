using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows;

namespace KioskAppWpf.WebApi
{
    public class LifeBitController : ApiController 
    {
        [HttpGet]
        [Route("api/lifebit")]
        public async Task<HttpResponseMessage> GetLifeBit()
        {
            var plcLifeBitValue = "no life bit";
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                plcLifeBitValue = mainWindow.PlcLifeBit.ToString();
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(plcLifeBitValue)
            };
        }
    }
}
