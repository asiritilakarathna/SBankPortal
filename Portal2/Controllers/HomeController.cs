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
                clientContext.Load<Folder>(list.RootFolder);
                clientContext.Load<FileCollection>(list.RootFolder.Files);
                clientContext.Load<Microsoft.SharePoint.Client.ListItemCollection>(listItems);

                clientContext.ExecuteQuery();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                //ViewData["JsonRegionList"] = serializer.Serialize(listItems);
                int rowcnt = listItems.Count;
                //variables for the field names, value & to build the json
                string fld_name = "";
                string fval = "";
                string json = "{\"rows\":[";
                //Loop through the list item collection
                int i = 0;
                foreach (ListItem oListItem in listItems)
                {
                    int fcount = oListItem.FieldValues.Keys.Count;
                    //Loop through the fields in this item
                    for (int j = 0; j < fcount; j++)
                    {
                        // get field name & try to get a handle on its contents
                        fld_name = oListItem.FieldValues.Keys.ElementAt(j);
                        try
                        {
                            fval = HttpUtility.HtmlEncode(oListItem.FieldValues[fld_name].ToString());
                        }
                        catch
                        {
                            fval = "Missing or invalid Value";
                        }
                        //try catch
                        json += '"' + fld_name + '"' + ":" + '"' + fval + '"' + ",";
                    }
                    //for j field loop
                    // counter ensures we have commas after each row except last
                    i++;
                    if (i < listItems.Count - 1)
                    {
                        json += "},";
                    }
                    else
                    {
                        json += "}";
                    }
                    //if test for comma
                }
                //foreach row
                json += "]}";
                ViewData["JsonRegionList"] = json;
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