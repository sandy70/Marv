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
        private const double Tolerance = 50;

        public static readonly DependencyProperty IsEvidenceEditEnabledProperty =
            DependencyProperty.Register("IsEvidenceEditEnabled", typeof (bool), typeof (LineDataChart), new PropertyMetadata(false, ChangedEvidenceEditEnabled));

        public static readonly DependencyProperty IsXAxisSectionsProperty =
            DependencyProperty.Register("IsXAxisSections", typeof (bool), typeof (LineDataChart), new PropertyMetadata(true, ChangedLineData));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (ILineData), typeof (LineDataChart), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (LineDataChart), new PropertyMetadata(null, ChangedSelectedSectionId));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (LineDataChart), new PropertyMetadata(null, ChangedVertex));

        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis", typeof (CartesianAxis), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty XTitleProperty =
            DependencyProperty.Register("XTitle", typeof (string), typeof (LineDataChart), new PropertyMetadata(null));

        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof (int), typeof (LineDataChart), new PropertyMetadata(int.MinValue, ChangedLineData));

        private readonly LinearAxis linearAxis = new LinearAxis();
        private readonly LogarithmicAxis logarightmicAxis = new LogarithmicAxis();

        private ObservableCollection<CategoricalDataPoint> anchorPoints;
        private ObservableCollection<ObservableCollection<ProbabilityDataPoint>> baseDistributionSeries;
        private ObservableCollection<CategoricalDataPoint> baseNumberPoints;
        private ObservableCollection<ObservableCollection<CategoricalDataPoint>> evidenceSeries;
        private ObservableCollection<CategoricalDataPoint> maxPoints;
        private ObservableCollection<CategoricalDataPoint> minPoints;
        private ObservableCollection<CategoricalDataPoint> modePoints;

        public ObservableCollection<CategoricalDataPoint> AnchorPoints
        {
            get
            {
                return this.anchorPoints;
            }

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
            get
            {
                return this.baseDistributionSeries;
            }

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
            get
            {
                return this.baseNumberPoints;
            }

            set
            {
                if (value != this.baseNumberPoints)
                {
                    this.baseNumberPoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ObservableCollection<CategoricalDataPoint>> EvidenceSeries
        {
            get
            {
                return this.evidenceSeries;
            }

            set
            {
                if (value.Equals(this.evidenceSeries))
                {
                    return;
                }

                this.evidenceSeries = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsEvidenceEditEnabled
        {
            get
            {
                return (bool) GetValue(IsEvidenceEditEnabledProperty);
            }
            set
            {
                SetValue(IsEvidenceEditEnabledProperty, value);
            }
        }

        public bool IsXAxisSections
        {
            get
            {
                return (bool) GetValue(IsXAxisSectionsProperty);
            }
            set
            {
                SetValue(IsXAxisSectionsProperty, value);
            }
        }

        public ILineData LineData
        {
            get
            {
                return (ILineData) GetValue(LineDataProperty);
            }

            set
            {
                SetValue(LineDataProperty, value);
            }
        }

        public ObservableCollection<CategoricalDataPoint> MaxPoints
        {
            get
            {
                return this.maxPoints;
            }

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
            get
            {
                return this.minPoints;
            }

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
            get
            {
                return this.modePoints;
            }

            set
            {
                if (value != this.modePoints)
                {
                    this.modePoints = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string SelectedSectionId
        {
            get
            {
                return (string) GetValue(SelectedSectionIdProperty);
            }
            set
            {
                SetValue(SelectedSectionIdProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string) GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public Vertex Vertex
        {
            get
            {
                return (Vertex) GetValue(VertexProperty);
            }
            set
            {
                SetValue(VertexProperty, value);
            }
        }

        public CartesianAxis VerticalAxis
        {
            get
            {
                return (CartesianAxis) GetValue(VerticalAxisProperty);
            }
            set
            {
                SetValue(VerticalAxisProperty, value);
            }
        }

        public string XTitle
        {
            get
            {
                return (string) GetValue(XTitleProperty);
            }
            set
            {
                SetValue(XTitleProperty, value);
            }
        }

        public int Year
        {
            get
            {
                return (int) GetValue(YearProperty);
            }
            set
            {
                SetValue(YearProperty, value);
            }
        }

        public LineDataChart()
        {
            InitializeComponent();

            this.Loaded -= LineDataChart_Loaded;
            this.Loaded += LineDataChart_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void ChangedEvidenceEditEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;

            if (!control.IsEvidenceEditEnabled)
            {
                control.UpdateLineData();
                control.UpdateBasePoints();
            }
        }

        private static void ChangedLineData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;
            control.UpdateBasePoints();
            control.InitializeEvidence();
        }

        private static void ChangedSelectedSectionId(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;

            if (!control.IsXAxisSections || e.OldValue == null)
            {
                control.UpdateBasePoints();
                control.InitializeEvidence();
            }
        }

        private static void ChangedVertex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;

            control.InitializeVerticalAxis();
            control.UpdateBasePoints();
            control.InitializeEvidence();
        }

        public void UpdateBasePoints()
        {
            if (this.LineData == null ||
                this.SelectedSectionId == null ||
                this.Year < 0 ||
                !this.LineData.ContainsSection(this.SelectedSectionId) ||
                this.Vertex == null)
            {
                return;
            }

            this.AnchorPoints = new ObservableCollection<CategoricalDataPoint>();
            this.BaseDistributionSeries = new ObservableCollection<ObservableCollection<ProbabilityDataPoint>>();
            this.BaseNumberPoints = new ObservableCollection<CategoricalDataPoint>();

            foreach (var state in this.Vertex.States)
            {
                this.BaseDistributionSeries.Add(new ObservableCollection<ProbabilityDataPoint>());
            }

            this.BaseDistributionSeries.Add(new ObservableCollection<ProbabilityDataPoint>());

            this.Title = this.IsXAxisSections ? "Year: " + this.Year : "Section: " + this.SelectedSectionId;
            this.XTitle = this.IsXAxisSections ? "Sections" : "Years";
            var categories = this.IsXAxisSections ? this.LineData.GetSectionIds() : Enumerable.Range(this.LineData.StartYear, this.LineData.EndYear - this.LineData.StartYear + 1).Select(i => i as object);

            foreach (var category in categories.ToList())
            {
                var sectionId = this.IsXAxisSections ? category as string : this.SelectedSectionId;
                var year = this.IsXAxisSections ? this.Year : (int) category;

                this.AnchorPoints.Add(new CategoricalDataPoint
                {
                    Category = category,
                    Value = null
                });

                var vertexData = this.LineData.GetSectionEvidence(sectionId)[year][this.Vertex.Key];

                this.AddBasePoint(category, vertexData);
            }
        }

        public void UpdateEvidence(VertexEvidence vertexEvidence, CellModel cellModel = null)
        {
            var sectionId = cellModel == null ? this.SelectedSectionId : cellModel.SectionId;
            var year = cellModel == null ? this.Year : cellModel.Year;

            if (this.BaseNumberPoints == null || this.BaseNumberPoints.Count == 0)
            {
                this.UpdateBasePoints();
            }

            var category = this.IsXAxisSections ? sectionId : year as object;

            if (category == null)
            {
                return;
            }

            this.BaseNumberPoints.Remove(point => point.Category.Equals(category));
            this.BaseDistributionSeries.Remove(point => point.Category.Equals(category));

            this.AddBasePoint(category, vertexEvidence);
        }

        private void AddBasePoint(object category, VertexEvidence vertexEvidence)
        {
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

                    if (this.BaseDistributionSeries == null)
                    {
                        this.UpdateBasePoints();
                    }

                    this.Vertex.States.ForEach((state, i) =>
                    {
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

        private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this.Chart);
            this.UpdateEvidence(position, e.ChangedButton == MouseButton.Right);
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(this.Chart);
                this.UpdateEvidence(position);
            }
        }

        private int GetAnchorIndex(CategoricalDataPoint point)
        {
            return this.AnchorPoints.IndexOf(anchorPoint => anchorPoint.Category.Equals(point.Category));
        }

        private ObservableCollection<CategoricalDataPoint> GetNearestSeries(CategoricalDataPoint userPoint)
        {
            var userPointAnchorIndex = this.GetAnchorIndex(userPoint);

            var series = new ObservableCollection<ObservableCollection<CategoricalDataPoint>>
            {
                this.MaxPoints,
                this.ModePoints,
                this.MinPoints
            };

            return series.MinBy(s =>
            {
                var xCoords = s.Select(point => (double) this.GetAnchorIndex(point));
                var yCoords = s.Select(point => point.Value.Value);

                var spline = new LinearInterpolator(xCoords, yCoords);

                return Math.Abs(spline.Eval(userPointAnchorIndex) - userPoint.Value.Value);
            });
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

            var maxValue = this.Vertex.SafeMax * 0.9;
            var minValue = this.Vertex.SafeMin == 0 ? 1 : this.Vertex.SafeMin * 1.1;
            var modeValue = (maxValue + minValue) / 2;

            this.MaxPoints.Add(new CategoricalDataPoint
            {
                Category = first, Value = maxValue
            });

            this.MaxPoints.Add(new CategoricalDataPoint
            {
                Category = last, Value = maxValue
            });

            this.ModePoints.Add(new CategoricalDataPoint
            {
                Category = first, Value = modeValue
            });

            this.ModePoints.Add(new CategoricalDataPoint
            {
                Category = last, Value = modeValue
            });

            this.MinPoints.Add(new CategoricalDataPoint
            {
                Category = first, Value = minValue
            });

            this.MinPoints.Add(new CategoricalDataPoint
            {
                Category = last, Value = minValue
            });
        }

        private void InitializeVerticalAxis()
        {
            if (this.Vertex == null)
            {
                return;
            }

            this.VerticalAxis = this.Vertex.IsLogScale ? (CartesianAxis) this.logarightmicAxis : this.linearAxis;

            var numericalAxis = this.VerticalAxis as NumericalAxis;
            numericalAxis.Minimum = this.Vertex.SafeMin;
            numericalAxis.Maximum = this.Vertex.SafeMax;
        }

        private void LineDataChart_Loaded(object sender, RoutedEventArgs e)
        {
            this.Chart.MouseDown -= Chart_MouseDown;
            this.Chart.MouseDown += Chart_MouseDown;

            this.Chart.MouseMove -= Chart_MouseMove;
            this.Chart.MouseMove += Chart_MouseMove;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateEvidence(Point position, bool removePoints = false)
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

            var nearestSeries = this.GetNearestSeries(userPoint);

            var pointsWithinTolerance = nearestSeries.Where(point => Utils.Distance(this.Chart.ConvertDataToPoint(new DataTuple(point.Category, point.Value)), position) < Tolerance).ToList();

            if (removePoints)
            {
                foreach (var point in pointsWithinTolerance)
                {
                    nearestSeries.Remove(point);
                }

                return;
            }

            if (pointsWithinTolerance.Count == 0)
            {
                var nearestPointAnchorIndex = 0;

                foreach (var nearestPoint in nearestSeries)
                {
                    nearestPointAnchorIndex = this.GetAnchorIndex(nearestPoint);

                    if (nearestPointAnchorIndex > userPointAnchorIndex)
                    {
                        break;
                    }
                }

                var userPointInsertIndex = nearestSeries.IndexOf(point => point.Category.Equals(this.AnchorPoints[nearestPointAnchorIndex].Category));

                nearestSeries.Insert(userPointInsertIndex, userPoint);
            }
            else
            {
                foreach (var point in pointsWithinTolerance)
                {
                    point.Value = (double) data.SecondValue;
                }
            }
        }

        private void UpdateLineData()
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
                var yCoords = s.Select(point => point.Value.Value);

                return new LinearInterpolator(xCoords.ToArray(), yCoords.ToArray());
            });

            this.AnchorPoints.ForEach((point, i) =>
            {
                var values = splines.Select(spline => spline.Eval(i)).ToArray();

                var evidenceString = string.Format("TRI({0:F2},{1:F2},{2:F2})", values[0], values[1], values[2]);

                var sectionId = this.IsXAxisSections ? point.Category as string : this.SelectedSectionId;
                var year = this.IsXAxisSections ? this.Year : (int) point.Category;

                var vertexEvidence = this.Vertex.States.ParseEvidenceString(evidenceString);

                this.LineData.GetSectionEvidence(sectionId)[year][this.Vertex.Key] = vertexEvidence;
            });

            this.LineData.RaiseDataChanged();
        }
    }
}