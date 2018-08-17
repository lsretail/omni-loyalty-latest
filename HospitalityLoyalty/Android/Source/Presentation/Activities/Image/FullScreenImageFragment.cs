using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Utils;
using Xamarin.It.Sephiroth.Android.Library.Imagezoom;

namespace Presentation.Activities.Image
{
    public class FullScreenImageFragment : BaseFragment, View.IOnTouchListener
    {
        //private ImageModel model;
        private ImageModel imageModel;
        private ImageViewTouch image;

        private float oldX = 0;
        private string imageId;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            imageModel = new ImageModel(Activity);

            var view = Inflate(inflater, Resource.Layout.FullScreenImageScreen, null);

            image = view.FindViewById<ImageViewTouch>(Resource.Id.image);

            imageId = Arguments.GetString(BundleUtils.ImageId);

            LoadImage();

            image.SetOnTouchListener(this);

            return view;
        }

        private async void LoadImage()
        {
            var loadedImage = await imageModel.ImageGetById(imageId, null);

            if (image != null && loadedImage != null)
            {
                ImageUtils.DisplayImage(image, ImageUtils.DecodeImage(loadedImage.Image));
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    oldX = e.GetX();
                    break;

                case MotionEventActions.Move:
                    var deltaX = e.GetX() - oldX;

                    if (deltaX < 0)
                    {
                        if (image.CanScroll(-1))
                        {
                            image.Parent.RequestDisallowInterceptTouchEvent(true);
                        }
                        else
                        {
                            image.Parent.RequestDisallowInterceptTouchEvent(false);
                             
                            //e.Action = MotionEventActions.Down;
                            //image.DispatchTouchEvent(e);
                        }
                    }
                    else if (deltaX > 0)
                    {
                        if (image.CanScroll(1))
                        {
                            image.Parent.RequestDisallowInterceptTouchEvent(true);
                        }
                        else
                        {
                            image.Parent.RequestDisallowInterceptTouchEvent(false);

                            //e.Action = MotionEventActions.Down;
                            //image.DispatchTouchEvent(e);
                        }
                    }

                    oldX = e.GetX();
                    break;
            }

            return false;
        }
    }
}