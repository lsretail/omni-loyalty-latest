using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
namespace Presentation
{
	public class RadioGroup : UIView
	{
		private List<Radio> radios;
		private UIView radiobtnCtn;
		public nfloat rHeight = 60f;
		public nfloat padding = 20f;
		public delegate void SelectedEventHandler(nint row);
		public SelectedEventHandler Selected;
		public RadioGroup(List<string> values)
		{
			radios = new List<Radio>();
			radiobtnCtn = new UIView();
			BackgroundColor = UIColor.White;
			for (var i = 0; i < values.Count; i++)
			{
				Radio radio = new Radio(values[i], i);
				radios.Add(radio);
				radio.radio.ValueChanged += valueChanged;
				radiobtnCtn.AddSubview(radio);
			}
			radios[0].radio.SetState(true, false);
			AddSubview(radiobtnCtn);
		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			radiobtnCtn.Frame = new CGRect(padding, padding, Frame.Width - padding * 2, Frame.Height - padding * 2);
			for (var i = 0; i < radios.Count; i++)
			{
				radios[i].Frame = new CGRect(0f, i * rHeight, Frame.Width, rHeight);
			}
		}

		private void valueChanged(object sender, EventArgs e) {
			UISwitch s = (UISwitch)sender;
			if (!s.On)
			{
				s.SetState(true, true);
			}
			else 
			{
				for (var i = 0; i < radios.Count; i++)
				{
					if (s.Tag != radios[i].radio.Tag)
					{
						radios[i].radio.SetState(false, true);
					}
				}
			}
			if (Selected != null)
				Selected(s.Tag);
		}
	}

	public class Radio : UIView
	{ 
		private UITextView value;
		public UISwitch radio;

		public Radio(string val, int i)
		{
			radio = new UISwitch();
			radio.Tag = i;

			value = new UITextView()
			{
				Text = val,
				TextColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.SystemFontOfSize(14)
			};
			AddSubviews(radio, value);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			radio.Frame = new CGRect(0, 0, 60f, Frame.Height);
			value.Frame = new CGRect(60f, 0, Frame.Width - 60f, Frame.Height);

		}

		public string getValue()
		{
			return value.Text;
		}
	}
}

