﻿#pragma checksum "..\..\AddClassRoomDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D10B8147C81E58304F99760B24B3FF4A7310D9BF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Schedule_WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace Schedule_WPF {
    
    
    /// <summary>
    /// AddClassRoomDialog
    /// </summary>
    public partial class AddClassRoomDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 31 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Building_Text;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Number_Text;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Seats_Text;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Building_Required;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Building_Invalid;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Number_Required;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Number_Invalid;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\AddClassRoomDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Seats_Invalid;
        
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
            System.Uri resourceLocater = new System.Uri("/Schedule_WPF;component/addclassroomdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AddClassRoomDialog.xaml"
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
            this.Building_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.Number_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.Seats_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            
            #line 41 "..\..\AddClassRoomDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SubmitData);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Building_Required = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.Building_Invalid = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 7:
            this.Number_Required = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 8:
            this.Number_Invalid = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.Seats_Invalid = ((System.Windows.Controls.StackPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

