﻿#pragma checksum "..\..\AddClassDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A3C281759B2D54505A5C61ECD7DC3C27918F258B"
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
    /// AddClassDialog
    /// </summary>
    public partial class AddClassDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 52 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CRN_Text;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Dept_Text;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ClassNum_Text;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Section_Text;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Name_Text;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Credits_Text;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Prof_Text;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\AddClassDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Online_Box;
        
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
            System.Uri resourceLocater = new System.Uri("/Schedule_WPF;component/addclassdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AddClassDialog.xaml"
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
            this.CRN_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.Dept_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ClassNum_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.Section_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.Name_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.Credits_Text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.Prof_Text = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.Online_Box = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            
            #line 73 "..\..\AddClassDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

