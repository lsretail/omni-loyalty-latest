using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Stores;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup;
using Presentation.Utils;

namespace Presentation.Models
{
    public class StoreModel : BaseModel
    {
        private StoreService service;

        public StoreModel(Context context, IRefreshableActivity refreshableActivity = null)
            : base(context, refreshableActivity)
        {
        }

        protected override void CreateService()
        {
            service = new StoreService(new StoreRepository());
        }

        public void GetStores(Action onSuccess)
        {
            Show(true);

            BeginWsCall();

            var worker = new BackgroundWorker();

            worker.DoWork += (sender, args) =>
                {
                    var stores = service.GetStores();

                    args.Result = stores;
                };

            worker.RunWorkerCompleted += (sender, args) =>
                {
                    try
                    {
                        if (args.Error != null)
                            throw args.Error;

                        var stores = (List<Store>) args.Result;

                        AppData.Stores = stores;

                        onSuccess();
                    }
                    catch (Exception ex)
                    {
                        HandleUIException(ex);
                    }
                    finally
                    {
                        Show(false);
                    }
                };

            worker.RunWorkerAsync();
        }
    }
}