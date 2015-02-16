using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marv.Common;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public partial class LineDataChart : INotifyPropertyChanged
    {
        private const double Tolerance = 2;

        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (LineDataChart), new PropertyMetadata(2010));

        public static readonly DependencyProperty HorizontalAxisQuantityProperty =
            DependencyProperty.Register("HorizontalAxisQuantity", typeof (HorizontalAxisQuantity), typeof (LineDataChart), new PropertyMetadata(HorizontalAxisQuantity.Sections, OnHorizontalAxisQuantityChanged));

        public static readonly DependencyProperty IsEvidenceEditEnabledProperty =
            DependencyProperty.Register("IsEvidenceEditEnabled", typeof (bool), typeof (LineDataChart), new PropertyMetadata(false));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (ILineData), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty SectionIdsProperty =
            DependencyProperty.Register("SectionIds", typeof (ObservableCollection<string>), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (LineDataChart), new PropertyMetadata(2010));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis", typeof (CartesianAxis), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty XTitleProperty =
            DependencyProperty.Register("XTitle", typeof (string), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof (int), typeof (LineDataChart), new PropertyMetadata(int.MinValue));

        private readonly LinearAxis linearAxis = new LinearAxis();
        private readonly LogarithmicAxis logarightmicAxis = new LogarithmicAxis();

        private ObservableCollection<CategoricalDataPoint> anchorPoints;
        private ObservableCollection<ObservableCollection<ProbabilityDataPoint>> baseDistributionSeries;
        private ObservableCollection<CategoricalDataPoint> baseNumberPoints;
        private bool isLog;
        private ObservableCollection<CategoricalDataPoint> maxPoints;
        private ObservableCollection<CategoricalDataPoint> minPoints;
        private ObservableCollection<CategoricalDataPoint> modePoints;
        private CategoricalDataPoint trackedPoint;

        public ObservableCollection<CategoricalDataPoint> AnchorPoints
        {
            get { return this.anchorPoints; }

            set
            {
                if (value != this.anchorPoints)
                {
                    this.anchorPoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ObservableCollection<ProbabilityDataPoint>> BaseDistributionSeries
        {
            get { return this.baseDistributionSeries; }

            set
            {
                if (value != this.baseDistributionSeries)
                {
                    this.baseDistributionSeries = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<CategoricalDataPoint> BaseNumberPoints
        {
            get { return this.baseNumberPoints; }

            set
            {
                if (value != this.baseNumberPoints)
                {
                    this.baseNumberPoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int EndYear
        {
            get { return (int) GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }

        public HorizontalAxisQuantity HorizontalAxisQuantity
        {
            get { return (HorizontalAxisQuantity) GetValue(HorizontalAxisQuantityProperty); }
            set { SetValue(HorizontalAxisQuantityProperty, value); }
        }

        public bool IsEvidenceEditEnabled
        {
            get { return (bool) GetValue(IsEvidenceEditEnabledProperty); }
            set { SetValue(IsEvidenceEditEnabledProperty, value); }
        }

        public bool IsLog
        {
            get { return this.isLog; }

            set
            {
                if (value.Equals(this.isLog))
                {
                    return;
                }

                this.isLog = value;
                this.RaisePropertyChanged();

                this.VerticalAxis = this.IsLog ? (CartesianAxis) this.logarightmicAxis : this.linearAxis;

                var numericalAxis = this.VerticalAxis as NumericalAxis;
                numericalAxis.Minimum = this.Vertex.SafeMin;
                numericalAxis.Maximum = this.Vertex.SafeMax;
            }
        }

        public ILineData LineData
        {
            get { return (ILineData) GetValue(LineDataProperty); }

            set { SetValue(LineDataProperty, value); }
        }

        public ObservableCollection<CategoricalDataPoint> MaxPoints
        {
            get { return this.maxPoints; }

            set
            {
                if (value != this.maxPoints)
                {
                    this.maxPoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<CategoricalDataPoint> MinPoints
        {
            get { return this.minPoints; }

            set
            {
                if (value != this.minPoints)
                {
                    this.minPoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<CategoricalDataPoint> ModePoints
        {
            get { return this.modePoints; }

            set
            {
                if (value != this.modePoints)
                {
                    this.modePoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> SectionIds
        {
            get { return (ObservableCollection<string>) GetValue(SectionIdsProperty); }
            set { SetValue(SectionIdsProperty, value); }
        }

        public string SelectedSectionId
        {
            get { return (string) GetValue(SelectedSectionIdProperty); }
            set { SetValue(SelectedSectionIdProperty, value); }
        }

        public int StartYear
        {
            get { return (int) GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public CategoricalDataPoint TrackedPoint
        {
            get { return this.trackedPoint; }

            set
            {
                if (value != null && value.Equals(this.trackedPoint))
                {
                    return;
                }

                this.trackedPoint = value;
                this.RaisePropertyChanged();
            }
        }

        public Vertex Vertex
        {
            get { return (Vertex) GetValue(VertexProperty); }
            set { SetValue(VertexProperty, value); }
        }

        public CartesianAxis VerticalAxis
        {
            get { return (CartesianAxis) GetValue(VerticalAxisProperty); }
            set { SetValue(VerticalAxisProperty, value); }
        }

        public string XTitle
        {
            get { return (string) GetValue(XTitleProperty); }
            set { SetValue(XTitleProperty, value); }
        }

        public int Year
        {
            get { return (int) GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        public LineDataChart()
        {
            InitializeComponent();

            this.Loaded -= LineDataChart_Loaded;
            this.Loaded += LineDataChart_Loaded;
        }

        private static void OnHorizontalAxisQuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;
            control.RaiseHorizontalAxisQuantityChanged(control.HorizontalAxisQuantity);
        }

        public void RemoveSelectedEvidence()
        {
            this.RemoveEvidence(this.SelectedSectionId, this.Year);
        }

        public void SetUserEvidence(object category, VertexEvidence vertexEvidence)
        {
            if (category == null)
            {
                return;
            }

            this.BaseNumberPoints.Remove(point => point.Category.Equals(category));
            this.BaseDistributionSeries.Remove(point => point.Category.Equals(category));

            var paramValues = vertexEvidence.Params;
            var type = vertexEvidence.Type;

            switch (type)
            {
                case VertexEvidenceType.Number:
                {
                    this.BaseNumberPoints.Add(new CategoricalDataPoint
                    {
                        Category = category,
                        Value = paramValues[0]
                    });

                    break;
                }

                case VertexEvidenceType.Range:
                {
                    while (this.BaseDistributionSeries.Count < 2)
                    {
                        this.BaseDistributionSeries.Add(new ObservableCollection<ProbabilityDataPoint>());
                    }

                    this.BaseDistributionSeries[0].Add(new ProbabilityDataPoint
                    {
                        Category = category,
                        Value = paramValues[0],
                        Probability = 0
                    });

                    this.BaseDistributionSeries[1].Add(new ProbabilityDataPoint
                    {
                        Category = category,
                        Value = paramValues[1] - paramValues[0],
                        Probability = 1
                    });

                    break;
                }

                case VertexEvidenceType.Distribution:
                case VertexEvidenceType.Normal:
                case VertexEvidenceType.Triangular:
                {
                    if (vertexEvidence.Value == null)
                    {
                        break;
                    }

                    var maxProb = vertexEvidence.Value.Max();

                    this.Vertex.States.ForEach((state, i) =>
                    {
                        while (this.BaseDistributionSeries.Count < i + 2)
                        {
                            this.BaseDistributionSeries.Add(new ObservableCollection<ProbabilityDataPoint>());
                        }

                        if (i == 0)
                        {
                            this.BaseDistributionSeries[i].Add(new ProbabilityDataPoint
                            {
                                Category = category,
                                Value = state.SafeMin,
                                Probability = 0
                            });
                        }

                        this.BaseDistributionSeries[i + 1].Add(new ProbabilityDataPoint
                        {
                            Category = category,
                            Value = state.SafeMax - state.SafeMin,
                            Probability = vertexEvidence.Value[i] / maxProb
                        });
                    });

                    break;
                }
            }
        }

        public void SetUserEvidence(Dict<object, VertexEvidence> vertexEvidences)
        {
            this.AnchorPoints = new ObservableCollection<CategoricalDataPoint>();
            this.BaseDistributionSeries = new ObservableCollection<ObservableCollection<ProbabilityDataPoint>>();
            this.BaseNumberPoints = new ObservableCollection<CategoricalDataPoint>();

            foreach (var category in vertexEvidences.Keys)
            {
                this.AnchorPoints.Add(new CategoricalDataPoint
                {
                    Category = category,
                    Value = null
                });

                this.SetUserEvidence(category, vertexEvidences[category]);
            }
        }

        public void SetVerticalAxis(double min, double max)
        {
            this.VerticalAxis = this.IsLog ? (CartesianAxis) this.logarightmicAxis : this.linearAxis;

            var numericalAxis = this.VerticalAxis as NumericalAxis;
            numericalAxis.Minimum = min;
            numericalAxis.Maximum = max;
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            var data = this.Chart.ConvertPointToData(e.GetPosition(sender as IInputElement));

            if (data == null || data.FirstValue == null || data.SecondValue == null)
            {
                return;
            }

            this.TrackedPoint = new CategoricalDataPoint
            {
                Category = data.FirstValue,
                Value = (double) data.SecondValue
            };
        }

        private void EvidenceClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.InitializeEvidence();
        }

        private void EvidenceDoneButton_Click(object sender, RoutedEventArgs e)
        {
            var series = new ObservableCollection<ObservableCollection<CategoricalDataPoint>>
            {
                this.MaxPoints,
                this.ModePoints,
                this.MinPoints
            };

            var splines = series.Select(s =>
            {
                var xCoords = s.Select(point => (double) this.GetAnchorIndex(point));
                var yCoords = s.Select(point => point.Value != null ? point.Value.Value : 0);

                return new LinearInterpolator(xCoords.ToArray(), yCoords.ToArray());
            });

            this.AnchorPoints.ForEach((point, i) =>
            {
                var values = splines.Select(spline => spline.Eval(i)).ToArray();

                var evidenceString = string.Format("TRI({0:F2},{1:F2},{2:F2})", values[0], values[1], values[2]);

                var sectionId = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Sections ? point.Category as string : this.SelectedSectionId;
                var year = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Sections ? this.Year : (int) point.Category;

                this.RaiseEvidenceGenerated(new EvidenceGeneratedEventArgs
                {
                    EvidenceString = evidenceString,
                    SectionId = sectionId,
                    Year = year
                });
            });
        }

        private int GetAnchorIndex(CategoricalDataPoint point)
        {
            return this.AnchorPoints.IndexOf(anchorPoint => anchorPoint.Category.Equals(point.Category));
        }

        private void InitializeEvidence()
        {
            this.MaxPoints = new ObservableCollection<CategoricalDataPoint>();
            this.ModePoints = new ObservableCollection<CategoricalDataPoint>();
            this.MinPoints = new ObservableCollection<CategoricalDataPoint>();

            if (this.AnchorPoints == null || this.Vertex == null)
            {
                return;
            }

            var first = this.AnchorPoints.First().Category;
            var last = this.AnchorPoints.Last().Category;

            var size = this.Vertex.SafeMax - this.Vertex.SafeMin;

            var maxValue = this.Vertex.SafeMax - size * 0.1;
            var minValue = this.Vertex.SafeMin == 0 ? 1 : this.Vertex.SafeMin + size * 0.1;
            var modeValue = (maxValue + minValue) / 2;

            this.MaxPoints.Add(new CategoricalDataPoint
            {
                Category = first,
                Value = maxValue
            });

            this.MaxPoints.Add(new CategoricalDataPoint
            {
                Category = last,
                Value = maxValue
            });

            this.ModePoints.Add(new CategoricalDataPoint
            {
                Category = first,
                Value = modeValue
            });

            this.ModePoints.Add(new CategoricalDataPoint
            {
                Category = last,
                Value = modeValue
            });

            this.MinPoints.Add(new CategoricalDataPoint
            {
                Category = first,
                Value = minValue
            });

            this.MinPoints.Add(new CategoricalDataPoint
            {
                Category = last,
                Value = minValue
            });
        }

        private void LineDataChart_Loaded(object sender, RoutedEventArgs e)
        {
            this.Chart.MouseMove -= Chart_MouseMove;
            this.Chart.MouseMove += Chart_MouseMove;

            this.MaxSeries.MouseDown -= MaxSeries_MouseDown;
            this.MaxSeries.MouseDown += MaxSeries_MouseDown;

            this.MaxSeries.MouseMove -= MaxSeries_MouseMove;
            this.MaxSeries.MouseMove += MaxSeries_MouseMove;

            this.MaxSeries.MouseUp -= Series_MouseUp;
            this.MaxSeries.MouseUp += Series_MouseUp;

            this.ModeSeries.MouseDown -= ModeSeries_MouseDown;
            this.ModeSeries.MouseDown += ModeSeries_MouseDown;

            this.ModeSeries.MouseMove -= ModeSeries_MouseMove;
            this.ModeSeries.MouseMove += ModeSeries_MouseMove;

            this.ModeSeries.MouseUp -= Series_MouseUp;
            this.ModeSeries.MouseUp += Series_MouseUp;

            this.MinSeries.MouseDown -= MinSeries_MouseDown;
            this.MinSeries.MouseDown += MinSeries_MouseDown;

            this.MinSeries.MouseMove -= MinSeries_MouseMove;
            this.MinSeries.MouseMove += MinSeries_MouseMove;

            this.MinSeries.MouseUp -= Series_MouseUp;
            this.MinSeries.MouseUp += Series_MouseUp;
        }

        private void MaxSeries_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as IInputElement);

            var position = e.GetPosition(this.Chart);
            var points = this.MaxPoints;
            var isPointRemoved = e.ChangedButton == MouseButton.Right;

            this.UpdateEvidencePoint(position, points, isPointRemoved);
        }

        private void MaxSeries_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this.Chart);
            var points = this.MaxPoints;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.UpdateEvidencePoint(position, points, false);
            }
        }

        private void MinSeries_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as IInputElement);

            var position = e.GetPosition(this.Chart);
            var points = this.MinPoints;
            var isPointRemoved = e.ChangedButton == MouseButton.Right;

            this.UpdateEvidencePoint(position, points, isPointRemoved);
        }

        private void MinSeries_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this.Chart);
            var points = this.MinPoints;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.UpdateEvidencePoint(position, points, false);
            }
        }

        private void ModeSeries_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as IInputElement);

            var position = e.GetPosition(this.Chart);
            var points = this.ModePoints;
            var isPointRemoved = e.ChangedButton == MouseButton.Right;

            this.UpdateEvidencePoint(position, points, isPointRemoved);
        }

        private void ModeSeries_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this.Chart);
            var points = this.ModePoints;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.UpdateEvidencePoint(position, points, false);
            }
        }

        private void RaiseEvidenceGenerated(EvidenceGeneratedEventArgs e)
        {
            if (this.EvidenceGenerated != null)
            {
                this.EvidenceGenerated(this, e);
            }
        }

        private void RaiseHorizontalAxisQuantityChanged(HorizontalAxisQuantity horizontalAxisQuantity)
        {
            if (this.HorizontalAxisQuantityChanged != null)
            {
                this.HorizontalAxisQuantityChanged(this, horizontalAxisQuantity);
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RemoveEvidence(string sectionId, int year)
        {
            var category = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Sections ? sectionId : year as object;

            if (category == null)
            {
                return;
            }

            this.BaseNumberPoints.Remove(point => point.Category.Equals(category));
            this.BaseDistributionSeries.Remove(point => point.Category.Equals(category));
        }

        private void Series_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (sender as IInputElement).ReleaseMouseCapture();
        }

        private void UpdateEvidencePoint(Point position, ObservableCollection<CategoricalDataPoint> points, bool isPointRemoved)
        {
            var data = this.Chart.ConvertPointToData(position);

            if (data == null || data.FirstValue == null || data.SecondValue == null)
            {
                return;
            }

            var userPoint = new CategoricalDataPoint
            {
                Category = data.FirstValue,
                Value = (double) data.SecondValue
            };

            var userPointAnchorIndex = this.GetAnchorIndex(userPoint);

            var pointsWithinTolerance = points.Where(point => Utils.Distance(this.Chart.ConvertDataToPoint(new DataTuple(point.Category, point.Value)), position) < Tolerance ||
                                                              point.Category.Equals(userPoint.Category)).ToList();

            if (isPointRemoved)
            {
                // Can't remove first and last points.
                foreach (var point in pointsWithinTolerance.Except(points.First()).Except(points.Last()))
                {
                    points.Remove(point);
                }

                return;
            }

            if (pointsWithinTolerance.Count == 0)
            {
                var nearestPointAnchorIndex = 0;

                foreach (var nearestPoint in points)
                {
                    nearestPointAnchorIndex = this.GetAnchorIndex(nearestPoint);

                    if (nearestPointAnchorIndex > userPointAnchorIndex)
                    {
                        break;
                    }
                }

                var userPointInsertIndex = points.IndexOf(point => point.Category.Equals(this.AnchorPoints[nearestPointAnchorIndex].Category));

                points.Insert(userPointInsertIndex, userPoint);
            }
            else
            {
                foreach (var point in pointsWithinTolerance)
                {
                    point.Value = (double) data.SecondValue;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<EvidenceGeneratedEventArgs> EvidenceGenerated;

        public event EventHandler<HorizontalAxisQuantity> HorizontalAxisQuantityChanged;
    }
}