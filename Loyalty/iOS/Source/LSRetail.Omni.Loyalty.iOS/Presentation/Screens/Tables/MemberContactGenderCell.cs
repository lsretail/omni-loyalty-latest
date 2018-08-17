using System;
using UIKit;
using CoreGraphics;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class MemberContactGenderCell : UITableViewCell
    {
        public static string KEY = "GENDERCELL";

        private UILabel captionLabel;
        private UIView segmentedContainerView;
        private UISegmentedControl segmentedControl;

        private MemberContactAttributesDTO memberContactAttributes;

        public MemberContactGenderCell() : base(UITableViewCellStyle.Default, KEY)
        {
            this.captionLabel = new UILabel();
            this.captionLabel.Font = UIFont.BoldSystemFontOfSize(14f);
            this.ContentView.AddSubview(this.captionLabel);

            this.segmentedContainerView = new UIView();
            this.segmentedContainerView.BackgroundColor = UIColor.Clear;
            this.ContentView.AddSubview(this.segmentedContainerView);

            // have to set the frame of the segmented control here, otherwise the frame changes after being displayed
            nfloat segmentedMargin = 11f;
            nfloat cellHeight = 44f;
            nfloat segmentedWidth = 0.50f * UIScreen.MainScreen.Bounds.Width - 15f;

            this.segmentedControl = new UISegmentedControl(new CGRect(2 * segmentedMargin, segmentedMargin, segmentedWidth - 4 * segmentedMargin, cellHeight - 2 * segmentedMargin));
            this.segmentedControl.InsertSegment(LocalizationUtilities.LocalizedString("Account_Male", "Male"), 0, true);
            this.segmentedControl.InsertSegment(LocalizationUtilities.LocalizedString("Account_Female", "Female"), 1, true);
            this.segmentedControl.TintColor = AppColors.PrimaryColor;
            this.segmentedControl.SelectedSegment = 0;
            this.segmentedControl.ValueChanged += (sender, e) =>
            {
                var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;

                if (selectedSegmentId == 0)
                    this.memberContactAttributes.Value = "Male";
                else
                    this.memberContactAttributes.Value = "Female";
            };
            this.segmentedContainerView.AddSubview(this.segmentedControl);


            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            const float margin = 15f;

            this.captionLabel.Frame = new CGRect(margin, 0f, 0.5 * this.ContentView.Frame.Width - margin, this.ContentView.Frame.Height);
            this.segmentedContainerView.Frame = new CGRect(this.captionLabel.Frame.Right, 0f, 0.5 * this.ContentView.Frame.Width - margin, this.ContentView.Frame.Height);
        }

        public void SetValues(MemberContactAttributesDTO memberContactAttributes)
        {
            this.memberContactAttributes = memberContactAttributes;
            this.captionLabel.Text = this.memberContactAttributes.Caption;
        }
    }
}

