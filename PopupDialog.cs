using System;
using Xamarin.Forms;

//Copyright 2015 By David Bagno dbagno@mac.com

namespace PerfectPopup
{
    public class PopupDialog : AbsoluteLayout
    {
        public delegate void ClosedEventHandler(bool changed);

        private double _popupHeight;

        private double _popupLeft;
        private double _popupTop;
        private double _popupWidth;
        private double _viewScale = .75;
        private double _lastheight;
        private double _lastwidth;

        public bool PopupChanged;

        public View PopupView;

        public bool PopupVisible;

        public Label TitleLabel{ get; set; }

        public PopupDialog(View view, ContentPage parentPage)
        {
            ParentPage = parentPage;
            PopupParent = view;
            _lastwidth = parentPage.Width;
            _lastwidth = parentPage.Height;

            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.Transparent;
            SetLayoutFlags(view, AbsoluteLayoutFlags.None);
            SetLayoutBounds(view, new Rectangle(0f, 0f, parentPage.Width - parentPage.Padding.HorizontalThickness, parentPage.Height - parentPage.Padding.VerticalThickness));
            Children.Add(view);
            ParentPage.SizeChanged += ((object sender, EventArgs e) =>
            {
                if (PopupParent != null && ParentPage.Width > 0)
                {
                    if (Math.Abs(_lastwidth - ParentPage.Width) > .001)
                    {
                        #region ReSizing the poupup

                        _lastwidth = ParentPage.Width;
                        _lastheight = ParentPage.Height;

                        SetLayoutBounds(PopupParent,
                            new Rectangle(0f, 0f, parentPage.Width - parentPage.Padding.HorizontalThickness,
                                parentPage.Height - parentPage.Padding.VerticalThickness));

                        if (PopupView != null && PopupVisible)
                        {
                            _popupWidth = ParentPage.Width * _viewScale;
                            _popupHeight = ParentPage.Height * _viewScale;
                            _popupLeft = ((ParentPage.Width - ParentPage.Padding.HorizontalThickness) - _popupWidth) / 2;
                            _popupTop = ((ParentPage.Height - ParentPage.Padding.VerticalThickness) - _popupHeight) / 2;
                            if (IsModal)
                            {
                                SetLayoutBounds(PopupShield, new Rectangle(0, 0, ParentPage.Width, ParentPage.Height));

                                if (!Children.Contains(PopupShield))
                                {
                                    Children.Add(PopupShield);
                                }
                            }
                            SetLayoutBounds(PopupFramer, new Rectangle(_popupLeft, _popupTop, _popupWidth, _popupHeight));
                            TitleLabel.WidthRequest = _popupWidth - 21;
                            if (!Children.Contains(PopupFramer))
                            {
                                Children.Add(PopupFramer);
                            }
                        }
                    }

                    #endregion
                }
            });
        }

        private ContentPage ParentPage { get; }

        private View PopupFramer { get; set; }

        private Frame PopupFrame { get; set; }

        private StackLayout PopupStack{ get; set; }

        public View PopupParent { get; set; }

        private StackLayout PopupShield { get; set; }

        private bool IsModal { get; set; }

        public event ClosedEventHandler Closed;



        protected virtual void OnClosed(bool changed)
        {
            Closed?.Invoke(PopupChanged);
        }

        public async void DismisPopup()
        {
            await PopupFramer.ScaleTo(0, 120, Easing.Linear);
            Children.Remove(PopupFramer);
            PopupFramer = null;
            PopupFrame = null;
            PopupStack = null;
            if (IsModal)
            {
                Children.Remove(PopupShield);
                PopupShield = null;
            }
            PopupVisible = false;
            OnClosed(PopupChanged);
        }



        public void ShowPopup(View view, double scale = .80, bool modal = true, string title = "")
        {
            if (scale <= 1 && scale >= .25)
            {
                _viewScale = scale;
            }
            else
            {
                _viewScale = .80;
            }
            IsModal = modal;
            PopupView = view;
            PopupChanged = false;
            _popupWidth = ParentPage.Width * _viewScale;
            _popupHeight = ParentPage.Height * _viewScale;
            if (_viewScale < 1)
            {
                PopupFrame = new Frame
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HasShadow = true,
                    Padding = 4,
                    BackgroundColor = Color.Gray.WithLuminosity(.98)
                };
            }
            else
            {
                IsModal = false;
                PopupStack = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = 0,
                    Spacing = 0,
                    BackgroundColor = Color.Gray.WithLuminosity(.98)
                };
            }
            var titleBarStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(5, 1, 5, 1),
                Spacing = 1,
                HeightRequest = 26,
                BackgroundColor = Color.Transparent,
            };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => DismisPopup();
            titleBarStack.GestureRecognizers.Add(tapGestureRecognizer);
            var seperator = new BoxView{ HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Start, HeightRequest = .5, BackgroundColor = Color.Gray.WithLuminosity(.8) };
            var closeButton = new Button
            {
                Text = "X",
                FontSize = 18,
                WidthRequest = 20,
                HeightRequest = 24,
                TextColor = Color.Gray.WithLuminosity(.3),
                HorizontalOptions = LayoutOptions.End,
                BorderRadius = 0,
                VerticalOptions = LayoutOptions.Center,
            };
            TitleLabel = new Label
            {
                Text = (title == "") ? "   " : title,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Start,
                FontSize = 18,
                HeightRequest = 24,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = _popupWidth - 21,
                LineBreakMode = LineBreakMode.TailTruncation,
                TextColor = Color.Gray.WithLuminosity(.3), 
            };
            titleBarStack.Children.Add(TitleLabel);
            titleBarStack.Children.Add(closeButton);
            if (_viewScale < 1)
            {
                PopupFrame.Content = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = 0,
                    Spacing = 0,
                    HeightRequest = 26,
                    Children =
                    {
                        titleBarStack,
                        seperator,
                        view
                    }
                };
            }
            else
            {
                PopupStack.Children.Add(titleBarStack);
                PopupStack.Children.Add(seperator);
                PopupStack.Children.Add(view);
            }
            closeButton.Clicked += ((object sender, EventArgs e) =>
            {
                DismisPopup();
                return;
            });
            if (_viewScale < 1)
            {
                PopupFramer = PopupFrame;
            }
            else
            {
                PopupFramer = PopupStack;
            }

            _popupLeft = ((ParentPage.Width - ParentPage.Padding.HorizontalThickness) - _popupWidth) / 2;
            _popupTop = ((ParentPage.Height - ParentPage.Padding.VerticalThickness) - _popupHeight) / 2;

            SetLayoutFlags(PopupFramer, AbsoluteLayoutFlags.None);
            SetLayoutBounds(PopupFramer, new Rectangle(_popupLeft + _popupWidth / 2, _popupTop + _popupHeight / 2, 0, 0));
            if (IsModal)
            {
                PopupShield = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = new Color(0, 0, 0, 0.4)
                };
                SetLayoutFlags(PopupShield, AbsoluteLayoutFlags.None);
                Children.Add(PopupShield, new Rectangle(0, 0, ParentPage.Width, ParentPage.Height));
            }
            PopupVisible = true;
            Children.Add(PopupFramer);
            PopupFramer.LayoutTo(new Rectangle(_popupLeft, _popupTop, _popupWidth, _popupHeight), 150, Easing.Linear);
        }
    }
}
