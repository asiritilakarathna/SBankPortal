using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SBankPortal.Controllers
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
                var list = clientContext.Web.Lists.GetByTitle("TestList");
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                List oList = clientContext.Web.Lists.GetByTitle("TestList");
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem oListItem = oList.AddItem(itemCreateInfo);
                oListItem["Title"] = "My New Item!";

                oListItem.Update();

                clientContext.ExecuteQuery();
            }

            Console.WriteLine("...");
            Console.ReadLine();
            return View();
        }
    }
}
