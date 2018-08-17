using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Utils;
using Presentation.Views;

namespace Presentation.Activities.SlideControl
{
    class SlideControlFragment : BaseFragment, ViewPager.IOnPageChangeListener
    {
        public enum ViewTypes
        {
            None = 0,
            Store = 1,
            Menu = 2,
            MenuItem = 3,
            MenuGroup = 4,
            Offer = 5,
            Coupon = 6,
        }

        private MenuService menuService;
        private ScrollableChildViewPager pager;
        private int currentMenu = -1;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            menuService = new MenuService();

            var view = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.SlideControlScreen);
            pager = view.FindViewById<ScrollableChildViewPager>(Resource.Id.SlideControlPager);

            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
            {
                pager.ChildId = Resource.Id.HeaderImageContainer;
            }

            var viewType = (ViewTypes)Arguments.GetInt(BundleUtils.ViewType);
            var items = new List<SlideControlPagerAdapter.SlideControlItem>();
                 
            string currentItemId = string.Empty;

            if (viewType == ViewTypes.Store)
            {
                currentItemId = Arguments.GetString(BundleUtils.Id);
                var ids = Arguments.GetStringArray(BundleUtils.Ids).ToList();

                foreach (var id in ids)
                {
                    items.Add(new SlideControlPagerAdapter.SlideControlItem()
                    {
                        Id1 = id,
                        ViewType = ViewTypes.Store
                    });
                }
            }
            else if (viewType == ViewTypes.Menu)
            {
                currentItemId = Arguments.GetString(BundleUtils.NodeId);
                var menuId = Arguments.GetString(BundleUtils.MenuId);
                var parentId = string.Empty;
                if (Arguments.ContainsKey(BundleUtils.ParentNodeId))
                {
                    parentId = Arguments.GetString(BundleUtils.ParentNodeId);
                }

                var menu = AppData.MobileMenu.MenuNodes.FirstOrDefault(x => x.Id == menuId);
                if (string.IsNullOrEmpty(parentId))
                {
                    foreach (var menuNode in menu.MenuNodes)
                    {
                        items.Add(new SlideControlPagerAdapter.SlideControlItem()
                        {
                            Id1 = menuNode.Id,
                            Id2 = menuId,
                            ViewType = ViewTypes.MenuGroup,
                            Type = 0
                        });
                    }
                }
                else
                {
                    var parentNode = menuService.GetMenuGroupNode(menu, parentId);
                    foreach (var menuNode in parentNode.MenuGroupNodes)
                    {
                        ViewTypes type;
                        int itemType = 0;
                        if (menuNode.NodeIsItem)
                        {
                            type = ViewTypes.MenuItem;
                            var menuNodeLine = parentNode.MenuNodeLines.FirstOrDefault(x => x.Id == menuNode.Id);

                            itemType = (int)menuNodeLine.NodeLineType;
                        }
                        else
                        {
                            type = ViewTypes.MenuGroup;
                        }

                        items.Add(new SlideControlPagerAdapter.SlideControlItem()
                        {
                            Id1 = menuNode.Id,
                            Id2 = menuId,
                            ViewType = type,
                            Type = itemType
                        });
                    }
                }
            }
            else if (viewType == ViewTypes.Offer)
            {
                currentItemId = Arguments.GetString(BundleUtils.OfferId);
                var ids = Arguments.GetStringArray(BundleUtils.Ids).ToList();

                foreach (var id in ids)
                {
                    items.Add(new SlideControlPagerAdapter.SlideControlItem()
                    {
                        Id1 = id,
                        ViewType = ViewTypes.Offer
                    });
                }
            }
            else if (viewType == ViewTypes.Coupon)
            {
                currentItemId = Arguments.GetString(BundleUtils.CouponId);
                var ids = Arguments.GetStringArray(BundleUtils.Ids).ToList();

                foreach (var id in ids)
                {
                    items.Add(new SlideControlPagerAdapter.SlideControlItem()
                    {
                        Id1 = id,
                        ViewType = ViewTypes.Coupon
                    });
                }
            }
            
            pager.PageMargin = Resources.GetDimensionPixelSize(Resource.Dimension.OneDP);
            pager.SetPageMarginDrawable(Resource.Color.backgroundcolor);

            /** Instantiating FragmentPagerAdapter */
            var pagerAdapter = new SlideControlPagerAdapter(ChildFragmentManager, items);

            if (Build.VERSION.SdkInt >= Utils.Utils.ViewPagerUtils.ImageTransformPager.MinVersion)
            {
                pager.SetPageTransformer(false, new Utils.Utils.ViewPagerUtils.ImageTransformPager());
            }

            /** Setting the pagerAdapter to the pager object */
            pager.Adapter = pagerAdapter;

            if (!String.IsNullOrEmpty(currentItemId))
            {
                var index = items.IndexOf(items.FirstOrDefault(x => x.Id1 == currentItemId));
                pager.SetCurrentItem(index, true);
            }

            OnPageSelected(0);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            pager.AddOnPageChangeListener(this);
        }

        public override void OnPause()
        {
            pager.RemoveOnPageChangeListener(this);

            base.OnPause();
        }

        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            if(currentMenu == position)
                return;

            var pagerAdapter = pager.Adapter as SlideControlPagerAdapter;
            var fragment = pagerAdapter.GetFragment(position);

            if (fragment is SlideControlBaseFragment)
            {
                currentMenu = position;
                (fragment as SlideControlBaseFragment).SetSupportActionBar();
            }
        }

        public void OnPageSelected(int position)
        {
        }
    }
}