using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PerfectPopup
{
    public class HomePage : ContentPage
    {
        private double _height;

        private double _width;

        public HomePage(string title)
        {
            BackgroundColor = Color.Gray.WithLuminosity(.9);
            Title = title;
            Padding = 0;
            PopupButton = new ToolbarItem
            {
                Text = "Show Popup",
                Priority = 0
            };

            ToolbarItems.Add(PopupButton);
            PopupButton.Clicked += ((object sender, EventArgs e) =>
            {
                if (!PopupContent.PopupVisible)
                {
                    #region popup content

                    //optional remove popup toolbaritem from navigation bar
                    ToolbarItems.Remove(PopupButton);

                    TableRoot = new TableRoot();
                    TableView = new TableView(TableRoot)
                    {
                        Intent = TableIntent.Data,
                        HasUnevenRows = false
                    };
                    TableSection = new TableSection("");
                    TableRoot.Add(TableSection);
                    for (var i = 0; i < 20; i++)
                    {
                        TableSection.Add(new SwitchCell { Text = "Switch Cell #" + i });
                    }

                    //scale the size of the modal popup min=.25 max=1 default=.80 optional title
                    //if the size=1 the dialog will fill the content area like a sheet window

                    PopupContent.ShowPopup(TableView, 1, modal: true, title: "Perfect Popup ");
                    foreach (var cell1 in TableSection)
                    {
                        var cell = (SwitchCell)cell1;
                        cell.OnChanged += ((cellsender, cellevent) =>
                        {
                            PopupContent.PopupChanged = true;
                            
                        });
                    }

                    #endregion
                }
                else
                {
                    PopupContent.DismisPopup();
                }
            });
        }

        public ToolbarItem PopupButton { get; set; }

        public PopupDialog PopupContent { get; set; }

        public TableView TableView { get; set; }

        public TableRoot TableRoot { get; set; }

        public TableSection TableSection { get; set; }

        public ScrollView PageScroll { get; set; }

        public StackLayout PageStack { get; set; }

        void DrawScreen()
        {
            PageStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                BackgroundColor = Color.White
            };
            for (var i = 0; i < 100; i++)
            {
                var cellStack = new StackLayout
                {
                    HeightRequest = 20,
                    BackgroundColor = Color.White,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = 1,
                    Spacing = 1
                };
                var cellText = new Label
                {
                    Text = i + " - Now is the time for all good men to come to the aid of their country.",
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 20,
                    LineBreakMode = LineBreakMode.TailTruncation
                };
                cellStack.Children.Add(cellText);
                PageStack.Children.Add(cellStack);
                PageStack.Children.Add(new BoxView
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.Gray.WithLuminosity(.9)
                    });
            }
            PageScroll = new ScrollView
            {
                Content = PageStack
            };
            if (PopupContent == null)
            {
                //first parameter will take any View 
                //second parameter a reference to the current content window
                PopupContent = new PopupDialog(PageScroll, this);
            }
            Content = PopupContent;
            PopupContent.Closed += ((bool changed) =>
            {
                if (!ToolbarItems.Contains(PopupButton))
                {
                    //put the toolbaritem back in navigation bar
                    ToolbarItems.Add(PopupButton);
                }
                if (changed)
                {
                    //call code to respond to the controls changed within the popup view.
                    DisplayAlert("Popup Status", "This popup has changed!", "OK");
                }
            });
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            //this instantiation and creation of the window’s content should be called from OnSizeAllocated event.  This is the only way to insure that the window and popup will be dynamically resized on orientation changed.
            if (Math.Abs(this._width - width) > .001 || Math.Abs(this._height - height) > .001)
            {
                this._width = width;
                this._height = height;
                if (PopupContent == null || !PopupContent.PopupVisible)
                {
                    DrawScreen();
                }
            }
        }
    }
}
