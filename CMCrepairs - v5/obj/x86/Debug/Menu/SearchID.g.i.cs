﻿#pragma checksum "..\..\..\..\Menu\SearchID.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6D86D873EFF6F8E29F3BEBD440301F31"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CMCrepairs.Menu {
    
    
    /// <summary>
    /// SearchID
    /// </summary>
    public partial class SearchID : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 75 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtOne;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chbCompleted;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chbItemWithCustomer;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chbRWPA;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chbNowSold;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgdIDs;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSearch;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLoad;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\Menu\SearchID.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CMCrepairs;component/menu/searchid.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Menu\SearchID.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtOne = ((System.Windows.Controls.TextBox)(target));
            
            #line 75 "..\..\..\..\Menu\SearchID.xaml"
            this.txtOne.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtOne_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.chbCompleted = ((System.Windows.Controls.CheckBox)(target));
            
            #line 78 "..\..\..\..\Menu\SearchID.xaml"
            this.chbCompleted.Checked += new System.Windows.RoutedEventHandler(this.chbCompleted_Checked);
            
            #line default
            #line hidden
            
            #line 78 "..\..\..\..\Menu\SearchID.xaml"
            this.chbCompleted.Unchecked += new System.Windows.RoutedEventHandler(this.chbCompleted_Unchecked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.chbItemWithCustomer = ((System.Windows.Controls.CheckBox)(target));
            
            #line 79 "..\..\..\..\Menu\SearchID.xaml"
            this.chbItemWithCustomer.Checked += new System.Windows.RoutedEventHandler(this.chbItemWithCustomer_Checked);
            
            #line default
            #line hidden
            
            #line 79 "..\..\..\..\Menu\SearchID.xaml"
            this.chbItemWithCustomer.Unchecked += new System.Windows.RoutedEventHandler(this.chbItemWithCustomer_Unchecked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.chbRWPA = ((System.Windows.Controls.CheckBox)(target));
            
            #line 80 "..\..\..\..\Menu\SearchID.xaml"
            this.chbRWPA.Checked += new System.Windows.RoutedEventHandler(this.chbRWPA_Checked);
            
            #line default
            #line hidden
            
            #line 80 "..\..\..\..\Menu\SearchID.xaml"
            this.chbRWPA.Unchecked += new System.Windows.RoutedEventHandler(this.chbRWPA_Unchecked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.chbNowSold = ((System.Windows.Controls.CheckBox)(target));
            
            #line 83 "..\..\..\..\Menu\SearchID.xaml"
            this.chbNowSold.Checked += new System.Windows.RoutedEventHandler(this.chbNowSold_Checked);
            
            #line default
            #line hidden
            
            #line 83 "..\..\..\..\Menu\SearchID.xaml"
            this.chbNowSold.Unchecked += new System.Windows.RoutedEventHandler(this.chbNowSold_Unchecked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dgdIDs = ((System.Windows.Controls.DataGrid)(target));
            
            #line 91 "..\..\..\..\Menu\SearchID.xaml"
            this.dgdIDs.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.dgdIDs_DoubleClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnSearch = ((System.Windows.Controls.Button)(target));
            
            #line 93 "..\..\..\..\Menu\SearchID.xaml"
            this.btnSearch.Click += new System.Windows.RoutedEventHandler(this.btnSearch_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnLoad = ((System.Windows.Controls.Button)(target));
            
            #line 94 "..\..\..\..\Menu\SearchID.xaml"
            this.btnLoad.Click += new System.Windows.RoutedEventHandler(this.btnLoad_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 95 "..\..\..\..\Menu\SearchID.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

