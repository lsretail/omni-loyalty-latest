using System;

using Android.OS;
using Android.Views;

using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.GUIExtensions.Android.Widget.ImageView;

namespace Presentation.Activities.Image
{
    public class FullScreenImageFragment : LoyaltyFragment//, View.IOnTouchListener
    {
        private ImageModel imageModel;
        private ScaleImageView imageView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.FullScreenImageScreen, null);

            imageView = view.FindViewById<ScaleImageView>(Resource.Id.image);

            var imageId = Arguments.GetString(BundleConstants.ImageId);

            imageModel = new ImageModel(Activity, null);
            
            LoadImage(imageId);

            //imageView.SetOnTouchListener(this);

            return view;
        }

        private async void LoadImage(string imageId)
        {
            var image = await imageModel.ImageGetById(imageId, new ImageSize(Int32.MaxValue, Int32.MaxValue));

            if (image != null)
            {
                Utils.ImageUtils.CrossfadeImage(imageView, Utils.ImageUtils.DecodeImage(image.Image), null, image.Crossfade);
            }
        }

        /*public bool OnTouch(View v, MotionEvent e)
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
                        if (imageView.CanScroll(-1))
                        {
                            imageView.Parent.RequestDisallowInterceptTouchEvent(true);
                        }
                        else
                        {
                            imageView.Parent.RequestDisallowInterceptTouchEvent(false);

                            //e.Action = MotionEventActions.Down;
                            //imageView.DispatchTouchEvent(e);
                        }
                    }
                    else if (deltaX > 0)
                    {
                        if (imageView.CanScroll(1))
                        {
                            imageView.Parent.RequestDisallowInterceptTouchEvent(true);
                        }
                        else
                        {
                            imageView.Parent.RequestDisallowInterceptTouchEvent(false);

                            //e.Action = MotionEventActions.Down;
                            //imageView.DispatchTouchEvent(e);
                        }
                    }

                    oldX = e.GetX();
                    break;
            }

            return false;
        }*/
    }
}