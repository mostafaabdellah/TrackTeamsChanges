using Microsoft.SharePoint.Client.EventReceivers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Helpers;
using TrackTeamsChanges;

namespace Contoso.Core.EventReceiversWeb.Services
{

    public class AppEventReceiver : IRemoteEventService
    {

        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {

            SPRemoteEventResult result = new SPRemoteEventResult();

            //switch (properties.EventType)
            //{
            //    case SPRemoteEventType.AppInstalled:
            //        HandleAppInstalled(properties);
            //        break;
            //    case SPRemoteEventType.AppUninstalling:
            //        HandleAppUninstalling(properties);
            //        break;
            //    case SPRemoteEventType.ItemAdded:
            //        HandleItemAdded(properties);
            //        break;
            //}


            return result;
        }


        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            Task.Factory.StartNew(() =>
            {
                RemoteEvent remoteEvent = new RemoteEvent()
                {
                    EventType = (int)properties.EventType,
                    BeforeProperties = JsonConvert.SerializeObject(properties.ItemEventProperties.BeforeProperties, Formatting.Indented),
                    AfterProperties = JsonConvert.SerializeObject(properties.ItemEventProperties.AfterProperties, Formatting.Indented),
                    BeforeUrl = properties.ItemEventProperties.BeforeUrl,
                    AfterUrl = properties.ItemEventProperties.AfterUrl,
                    CurrentUserId = properties.ItemEventProperties.CurrentUserId,
                    IsBackgroundSave = properties.ItemEventProperties.IsBackgroundSave,
                    ListId = properties.ItemEventProperties.ListId.ToString(),
                    ListItemId = properties.ItemEventProperties.ListItemId,
                    ListTitle = properties.ItemEventProperties.ListTitle,
                    UserDisplayName = properties.ItemEventProperties.UserDisplayName,
                    UserLoginName = properties.ItemEventProperties.UserLoginName,
                    Versionless = properties.ItemEventProperties.Versionless,
                    WebUrl = properties.ItemEventProperties.WebUrl
                };
                DbOperations.AddRemoteEvent(remoteEvent);
            });
        }
    }
}


