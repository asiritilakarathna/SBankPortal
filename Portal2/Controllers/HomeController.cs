using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using System.Web.Script.Serialization;

namespace Portal2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Uri siteUri = new Uri("https://nuwantilakarathna.sharepoint.com");
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);

            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal,
                                                                    siteUri.Authority, realm).AccessToken;

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
            {
                Web currentWeb = clientContext.Web;
                var list = clientContext.Web.Lists.GetByTitle("ImageRotatorContents");
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><RowLimit>100</RowLimit></View>";

                //Use the fully qualified name to disambiguate the ListItemCollection type.
                Microsoft.SharePoint.Client.ListItemCollection listItems = list.GetItems(camlQuery);
                clientContext.Load<List>(list);
                clientContext.Load<Microsoft.SharePoint.Client.ListItemCollection>(listItems);

                clientContext.ExecuteQuery();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewData["JsonRegionList"] = serializer.Serialize(listItems);
            }

            Console.WriteLine("...");
            Console.ReadLine();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}