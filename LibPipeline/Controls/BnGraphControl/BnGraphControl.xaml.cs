﻿using LibNetwork;
using Marv.Common;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class BnGraphControl : UserControl
    {
        public static readonly DependencyProperty ConnectionColorProperty =
        DependencyProperty.Register("ConnectionColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register("Graph", typeof(Graph), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly RoutedEvent GroupButtonClickedEvent =
        EventManager.RegisterRoutedEvent("GroupButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
        DependencyProperty.Register("IncomingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsBackButtonVisibleProperty =
        DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSensorButtonVisibleProperty =
        DependencyProperty.Register("IsSensorButtonVisible", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(true));

        public static readonly RoutedEvent NewEvidenceAvailableEvent =
        EventManager.RegisterRoutedEvent("NewEvidenceAvailable", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
        DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.Red));

        public static readonly RoutedEvent RetractButtonClickedEvent =
        EventManager.RegisterRoutedEvent("RetractButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty SelectedVertexViewModelProperty =
        DependencyProperty.Register("SelectedVertexViewModel", typeof(BnVertexViewModel), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly RoutedEvent SensorButtonCheckedEvent =
        EventManager.RegisterRoutedEvent("SensorButtonChecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly RoutedEvent SensorButtonUncheckedEvent =
        EventManager.RegisterRoutedEvent("SensorButtonUnchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty ShapeOpacityProperty =
        DependencyProperty.Register("ShapeOpacity", typeof(double), typeof(BnGraphControl), new PropertyMetadata(1.0));

        public static readonly RoutedEvent StateDoubleClickedEvent =
        EventManager.RegisterRoutedEvent("StateDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<BnGraphControlEventArgs>), typeof(BnGraphControl));

        public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register("Zoom", typeof(double), typeof(BnGraphControl), new PropertyMetadata(1.0));

        private Dictionary<Graph, string> selectedGroups = new Dictionary<Graph, string>();

        public BnGraphControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> GroupButtonClicked
        {
            add { AddHandler(GroupButtonClickedEvent, value); }
            remove { RemoveHandler(GroupButtonClickedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> NewEvidenceAvailable
        {
            add { AddHandler(NewEvidenceAvailableEvent, value); }
            remove { RemoveHandler(NewEvidenceAvailableEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> RetractButtonClicked
        {
            add { AddHandler(RetractButtonClickedEvent, value); }
            remove { RemoveHandler(RetractButtonClickedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> SensorButtonChecked
        {
            add { AddHandler(SensorButtonCheckedEvent, value); }
            remove { RemoveHandler(SensorButtonCheckedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> SensorButtonUnchecked
        {
            add { AddHandler(SensorButtonUncheckedEvent, value); }
            remove { RemoveHandler(SensorButtonUncheckedEvent, value); }
        }

        public event RoutedEventHandler<BnGraphControlEventArgs> StateDoubleClicked
        {
            add { AddHandler(StateDoubleClickedEvent, value); }
            remove { RemoveHandler(StateDoubleClickedEvent, value); }
        }

        public Color ConnectionColor
        {
            get { return (Color)GetValue(ConnectionColorProperty); }
            set { SetValue(ConnectionColorProperty, value); }
        }

        public Graph Graph
        {
            get { return (Graph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color)GetValue(IncomingConnectionHighlightColorProperty); }
            set { SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public bool IsBackButtonVisible
        {
            get { return (bool)GetValue(IsBackButtonVisibleProperty); }
            set { SetValue(IsBackButtonVisibleProperty, value); }
        }

        public bool IsSensorButtonVisible
        {
            get { return (bool)GetValue(IsSensorButtonVisibleProperty); }
            set { SetValue(IsSensorButtonVisibleProperty, value); }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get { return (Color)GetValue(OutgoingConnectionHighlightColorProperty); }
            set { SetValue(OutgoingConnectionHighlightColorProperty, value); }
        }

        public Dictionary<Graph, string> SelectedGroups
        {
            get
            {
                return selectedGroups;
            }

            set
            {
                selectedGroups = value;
            }
        }

        public BnVertexViewModel SelectedVertexViewModel
        {
            get { return (BnVertexViewModel)GetValue(SelectedVertexViewModelProperty); }
            set { SetValue(SelectedVertexViewModelProperty, value); }
        }

        public double ShapeOpacity
        {
            get { return (double)GetValue(ShapeOpacityProperty); }
            set { SetValue(ShapeOpacityProperty, value); }
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }
    }
}