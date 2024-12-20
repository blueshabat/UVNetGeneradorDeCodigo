﻿#pragma checksum "..\..\..\Dialogos\ModificacionProceso.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "36775000AD351F8C81F9D422C46907012080870D689688A1ECE460DACBFD7983"
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
using UVNetGeneradorDeCodigo.API;
using UVNetGeneradorDeCodigo.Generador.ScriptsSQL;


namespace UVNetGeneradorDeCodigo.Dialogos {
    
    
    /// <summary>
    /// ModificacionProceso
    /// </summary>
    public partial class ModificacionProceso : UVNetGeneradorDeCodigo.API.CDialogo<UVNetGeneradorDeCodigo.Generador.ScriptsSQL.CModificacionProceso>, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\Dialogos\ModificacionProceso.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CbxProcesos;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Dialogos\ModificacionProceso.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgParametros;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Dialogos\ModificacionProceso.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ChbCrearProcedimientosObtencionPRE;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Dialogos\ModificacionProceso.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ChbCrearProcedimientosObtencionEN;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Dialogos\ModificacionProceso.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button1;
        
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
            System.Uri resourceLocater = new System.Uri("/UVNetGeneradorDeCodigo;component/dialogos/modificacionproceso.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Dialogos\ModificacionProceso.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.CbxProcesos = ((System.Windows.Controls.ComboBox)(target));
            
            #line 13 "..\..\..\Dialogos\ModificacionProceso.xaml"
            this.CbxProcesos.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.SeHaSeleccionadoUnProceso);
            
            #line default
            #line hidden
            return;
            case 2:
            this.dgParametros = ((System.Windows.Controls.DataGrid)(target));
            
            #line 17 "..\..\..\Dialogos\ModificacionProceso.xaml"
            this.dgParametros.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.VerificarPosibilidadCreacionProcedimientosObtencion);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ChbCrearProcedimientosObtencionPRE = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.ChbCrearProcedimientosObtencionEN = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\Dialogos\ModificacionProceso.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.BtnModificar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

