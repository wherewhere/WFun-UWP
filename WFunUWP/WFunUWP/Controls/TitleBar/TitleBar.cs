using System;
using WFunUWP.Helpers;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WFunUWP.Controls
{
    [ContentProperty(Name = "CustomContent")]
    [TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "TitleText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "CustomContentPresenter", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "DragRegion", Type = typeof(Grid))]
    [TemplatePart(Name = "BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "Icon", Type = typeof(Viewbox))]
    public partial class TitleBar : Control
    {
        //private Grid m_layoutRoot;
        //private TextBlock m_titleTextBlock;
        //private FrameworkElement m_customArea;
        //private Viewbox m_icon;

        //private bool m_isTitleSquished = false;
        //private bool m_isIconSquished = false;

        //private double m_titleWidth;
        //private double m_iconWidth;

        public TitleBar()
        {
            this.DefaultStyleKey = typeof(TitleBar);

            SizeChanged += OnSizeChanged;

            CoreApplicationView currentView = CoreApplication.GetCurrentView();
            if (currentView != null)
            {
                CoreApplicationViewTitleBar coreTitleBar = currentView.TitleBar;
                if (coreTitleBar != null)
                {
                    coreTitleBar.LayoutMetricsChanged += OnTitleBarMetricsChanged;
                    coreTitleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;
                }
            }

            Window window = Window.Current;
            if (window != null)
            {
                window.Activated += OnWindowActivated;
            }

            //ActualThemeChanged += (FrameworkElement sender, object args) => UpdateTheme();
        }

        protected override void OnApplyTemplate()
        {
            CoreApplicationView currentView = CoreApplication.GetCurrentView();
            if (currentView != null)
            {
                CoreApplicationViewTitleBar coreTitleBar = currentView.TitleBar;
                if (coreTitleBar != null)
                {
                    coreTitleBar.ExtendViewIntoTitleBar = true;
                }
            }

            //m_layoutRoot = (Grid)GetTemplateChild("LayoutRoot");

            //m_icon = (Viewbox)GetTemplateChild("Icon");
            //m_titleTextBlock = (TextBlock)GetTemplateChild("TitleText");
            //m_customArea = (FrameworkElement)GetTemplateChild("CustomContentPresenter");

            Window window = Window.Current;
            if (window != null)
            {
                Grid dragRegion = (Grid)GetTemplateChild("DragRegion");
                if (dragRegion != null)
                {
                    window.SetTitleBar(dragRegion);
                }
                else
                {
                    window.SetTitleBar(null);
                }
            }

            Button backButton = (Button)GetTemplateChild("BackButton");
            if (backButton != null)
            {
                backButton.Click += OnBackButtonClick;

                // Do localization for the back button
                if (string.IsNullOrEmpty(AutomationProperties.GetName(backButton)))
                {
                    string backButtonName = ResourceLoader.GetForViewIndependentUse().GetString("NavigationBackButtonName");
                    AutomationProperties.SetName(backButton, backButtonName);
                }

                // Setup the tooltip for the back button
                ToolTip tooltip = new ToolTip();
                string backButtonTooltipText = ResourceLoader.GetForViewIndependentUse().GetString("NavigationBackButtonToolTip");
                tooltip.Content = backButtonTooltipText;
                ToolTipService.SetToolTip(backButton, tooltip);
            }

            UpdateVisibility();
            UpdateHeight();
            UpdatePadding();
            UpdateIcon();
            UpdateBackButton();
            //UpdateTheme();
            UpdateTitle();

            base.OnApplyTemplate();
        }

        public void OnBackButtonClick(object sender, RoutedEventArgs args)
        {
            BackRequested?.Invoke(this, null);
        }

        public void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateIcon();
        }

        public void OnIsBackButtonVisiblePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateBackButton();
        }

        public void OnCustomContentPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateHeight();
        }

        public void OnTitlePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateTitle();
        }

        public void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            //var titleTextBlock = m_titleTextBlock;
            //var customArea = m_customArea;
            //if (titleTextBlock != null && !string.IsNullOrEmpty(titleTextBlock.Text) && customArea != null)
            //{
            //	if (m_isTitleSquished)
            //	{
            //		var icon = m_icon;
            //		var source = IconSource;
            //		if (icon != null && source != null)
            //		{
            //			if (m_isIconSquished)
            //			{
            //				if (customArea.DesiredSize.Width + m_iconWidth + 16 < customArea.ActualWidth)
            //				{
            //					VisualStateManager.GoToState(this, "IconVisible", true);
            //					m_isIconSquished = false;

            //					if (customArea.DesiredSize.Width + m_iconWidth + m_titleWidth + 32 < customArea.ActualWidth)
            //					{
            //						VisualStateManager.GoToState(this, "TitleTextVisible", true);
            //						m_isTitleSquished = false;
            //					}
            //				}
            //			}
            //			else
            //			{
            //				if (customArea.DesiredSize.Width + m_titleWidth + 16 < customArea.ActualWidth)
            //				{
            //					VisualStateManager.GoToState(this, "TitleTextVisible", true);
            //					m_isTitleSquished = false;
            //				}

            //				if (!m_isIconSquished && customArea.DesiredSize.Width >= customArea.ActualWidth)
            //				{
            //					VisualStateManager.GoToState(this, "IconCollapsed", true);
            //					m_iconWidth = titleTextBlock.ActualWidth;
            //					m_isIconSquished = true;
            //				}
            //			}
            //		}
            //		else
            //		{
            //			if (customArea.DesiredSize.Width + m_titleWidth + 16 < customArea.ActualWidth)
            //			{
            //				VisualStateManager.GoToState(this, "TitleTextVisible", true);
            //				m_isTitleSquished = false;
            //			}
            //		}
            //	}
            //	else
            //	{
            //		if (!m_isTitleSquished && customArea.DesiredSize.Width >= customArea.ActualWidth)
            //		{
            //			VisualStateManager.GoToState(this, "TitleTextCollapsed", true);
            //			m_titleWidth = titleTextBlock.ActualWidth;
            //			m_isTitleSquished = true;
            //		}
            //	}
            //}
        }

        public void OnWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            VisualStateManager.GoToState(this, (args.WindowActivationState == CoreWindowActivationState.Deactivated) ? "Deactivated" : "Activated", false);
        }

        public void OnTitleBarMetricsChanged(object UnnamedParameter, object UnnamedParameter2)
        {
            UpdatePadding();
        }

        public void OnTitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object UnnamedParameter)
        {
            UpdateVisibility();
        }

        public void UpdateIcon()
        {
            TitleBarTemplateSettings templateSettings = TemplateSettings;
            Microsoft.UI.Xaml.Controls.IconSource source = IconSource;
            if (source != null)
            {
                templateSettings.IconElement = SharedHelpers.MakeIconElementFrom(source);
                VisualStateManager.GoToState(this, "IconVisible", false);
            }
            else
            {
                templateSettings.IconElement = null;
                VisualStateManager.GoToState(this, "IconCollapsed", false);
            }
        }

        public void UpdateBackButton()
        {
            VisualStateManager.GoToState(this, IsBackButtonVisible ? "BackButtonVisible" : "BackButtonCollapsed", false);
        }

        public void UpdateVisibility()
        {
            CoreApplicationView currentView = CoreApplication.GetCurrentView();
            if (currentView != null)
            {
                CoreApplicationViewTitleBar coreTitleBar = currentView.TitleBar;
                if (coreTitleBar != null)
                {
                    VisualStateManager.GoToState(this, coreTitleBar.IsVisible ? "TitleBarVisible" : "TitleBarCollapsed", false);
                }
            }
        }

        public void UpdateHeight()
        {
            VisualStateManager.GoToState(this, (CustomContent == null && AutoSuggestBox == null && PaneFooter == null) ? "CompactHeight" : "ExpandedHeight", false);
        }

        public void UpdatePadding()
        {
            TitleBarTemplateSettings templateSettings = TemplateSettings;
            CoreApplicationView currentView = CoreApplication.GetCurrentView();
            if (currentView != null)
            {
                CoreApplicationViewTitleBar coreTitleBar = currentView.TitleBar;
                if (coreTitleBar != null)
                {
                    templateSettings.LeftPaddingColumnGridLength = new GridLength(coreTitleBar.SystemOverlayLeftInset);
                    templateSettings.RightPaddingColumnGridLength = new GridLength(coreTitleBar.SystemOverlayRightInset);
                }
            }
        }

        public void UpdateTheme()
        {
            ApplicationView appView = ApplicationView.GetForCurrentView();
            if (appView != null)
            {
                ApplicationViewTitleBar titleBar = appView.TitleBar;
                if (titleBar != null)
                {
                    ResourceDictionary ResourceDictionary = new ResourceDictionary();
                    ResourceDictionary.Source = new Uri("ms-appx:///Controls/TitleBar/TitleBar_themeresources.xaml");

                    // rest colors
                    Color buttonForegroundColor = (Color)ResourceDictionary["TitleBarButtonForegroundColor"];
                    titleBar.ButtonForegroundColor = buttonForegroundColor;

                    Color buttonBackgroundColor = (Color)ResourceDictionary["TitleBarButtonBackgroundColor"];
                    titleBar.ButtonBackgroundColor = buttonBackgroundColor;
                    titleBar.ButtonInactiveBackgroundColor = buttonBackgroundColor;

                    // hover colors
                    Color buttonHoverForegroundColor = (Color)ResourceDictionary["TitleBarButtonHoverForegroundColor"];
                    titleBar.ButtonHoverForegroundColor = buttonHoverForegroundColor;

                    Color buttonHoverBackgroundColor = (Color)ResourceDictionary["TitleBarButtonHoverBackgroundColor"];
                    titleBar.ButtonHoverBackgroundColor = buttonHoverBackgroundColor;

                    // pressed colors
                    Color buttonPressedForegroundColor = (Color)ResourceDictionary["TitleBarButtonPressedForegroundColor"];
                    titleBar.ButtonPressedForegroundColor = buttonPressedForegroundColor;

                    Color buttonPressedBackgroundColor = (Color)ResourceDictionary["TitleBarButtonPressedBackgroundColor"];
                    titleBar.ButtonPressedBackgroundColor = buttonPressedBackgroundColor;

                    // inactive foreground
                    Color buttonInactiveForegroundColor = (Color)ResourceDictionary["TitleBarButtonInactiveForegroundColor"];
                    titleBar.ButtonInactiveForegroundColor = buttonInactiveForegroundColor;
                }
            }
        }

        public void UpdateTitle()
        {
            string titleText = Title;
            if (string.IsNullOrEmpty(titleText))
            {
                VisualStateManager.GoToState(this, "TitleTextCollapsed", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "TitleTextVisible", false);
            }
        }
    }
}
