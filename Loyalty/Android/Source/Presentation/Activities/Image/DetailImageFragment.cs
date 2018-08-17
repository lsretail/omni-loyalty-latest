using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;

namespace Presentation.Activities.Image
{
    public class DetailImageFragment : LoyaltyFragment, View.IOnClickListener
    {
        private View imageContainer;
        private ImageView imageView;

        public LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView ImageView { get; set; }
        public string[] Ids { get; set; }

        public static DetailImageFragment NewInstance()
        {
            var fragment = new DetailImageFragment();
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
            {
                XmlSerializer imageViewSerializer = new XmlSerializer(typeof(LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView));

                using (TextReader textReader = new StringReader(savedInstanceState.GetString("imageView")))
                {

                    ImageView = (LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView)imageViewSerializer.Deserialize(textReader);
                }

                Ids = savedInstanceState.GetStringArray("Ids");
            }

            if (ImageView == null)
            {
                return base.OnCreateView(inflater, container, savedInstanceState);
            }

            var view = Inflate(inflater, Resource.Layout.DetailImageView);

            imageContainer = view.FindViewById(Resource.Id.DetailImageViewImageFrame);
            imageContainer.SetBackgroundColor(Color.ParseColor(ImageView.GetAvgColor()));

            imageView = view.FindViewById<ImageView>(Resource.Id.DetailImageViewImage);
            
            LoadImage();

            imageContainer.SetOnClickListener(this);

            return view;
        }

        private async void LoadImage()
        {
            var image = await new ImageModel(Activity, null).ImageGetById(ImageView.Id, ImageView.ImgSize);

            if (image == null)
            {
                imageContainer.SetBackgroundResource(Resource.Color.transparent);
            }
            else
            {
                Util.Utils.ImageUtils.CrossfadeImage(imageView, Util.Utils.ImageUtils.DecodeImage(image.Image), imageContainer, image.Crossfade);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            
            XmlSerializer imageViewSerializer = new XmlSerializer(typeof(LSRetail.Omni.Domain.DataModel.Base.Retail.ImageView));

            using (var textWriter = new StringWriter())
            {
                imageViewSerializer.Serialize(textWriter, ImageView);

                outState.PutString("imageView", textWriter.ToString());
            }

            outState.PutStringArray("Ids", Ids);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.DetailImageViewImageFrame:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(Activities.Image.FullScreenImageActivity));

                    /*if (Ids.Count() > 1)
                    {
                        intent.PutExtra(BundleConstants.ItemListId, Ids);
                        intent.PutExtra(BundleConstants.CurrentPage, Array.IndexOf(Ids, ImageView.Id));
                    }
                    else*/
                    {
                        intent.PutExtra(BundleConstants.ImageId, ImageView.Id);
                    }
                    Activity.StartActivity(intent);
                    break;
            }
        }
    }
}