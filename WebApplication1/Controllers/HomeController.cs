using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using GE = System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NS = Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            string json = "{\"rows\":[";
            Uri siteUri = new Uri("https://nuwantilakarathna.sharepoint.com");
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);

            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal,
                                                                    siteUri.Authority, realm).AccessToken;
            GE.IEnumerable<ImageRotatorViewModel> output;
            GE.List< ImageRotatorViewModel> test = new GE.List<ImageRotatorViewModel>();
            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
            {
                Web currentWeb = clientContext.Web;
                var list = clientContext.Web.Lists.GetByTitle("ImageRotatorContents");
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><RowLimit>100</RowLimit></View>";

                //Use the fully qualified name to disambiguate the ListItemCollection type.
                Microsoft.SharePoint.Client.ListItemCollection listItems = list.GetItems(camlQuery);
                clientContext.Load<List>(list);
                clientContext.Load<Folder>(list.RootFolder);
                clientContext.Load<FileCollection>(list.RootFolder.Files);
                clientContext.Load<Microsoft.SharePoint.Client.ListItemCollection>(listItems);

                clientContext.ExecuteQuery();


                JavaScriptSerializer serializer = new JavaScriptSerializer();

                MemoryStream stream = new MemoryStream();

                foreach (ListItem oListItem in listItems)
                {
                    ClientResult<System.IO.Stream> data = oListItem.File.OpenBinaryStream();
                    clientContext.Load(oListItem.File);
                    clientContext.ExecuteQuery();
                    using (System.IO.MemoryStream mStream = new System.IO.MemoryStream())
                    {
                        if (data != null)
                        {
                            data.Value.CopyTo(mStream);
                            byte[] imageArray = mStream.ToArray();
                            string b64String = Convert.ToBase64String(imageArray);
                            ImageRotatorViewModel temp = new ImageRotatorViewModel
                            {
                                Title = oListItem["Title"] != null ? oListItem["Title"].ToString() : "",
                                Description = oListItem["Description0"] != null ? oListItem["Description0"].ToString() : "",
                                Link = oListItem["Link"] != null ? oListItem["Link"].ToString() : "",
                                FileData = b64String
                            };
                            test.Add(temp);
                        }
                    }
                }
            }
            //ViewData["LibraryDetail"] = output;
            return View( test);
        }  
    }
}
