using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public partial class LineDataChart : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CellModelsProperty =
            DependencyProperty.Register("CellModels", typeof (IEnumerable<CellModel>), typeof (LineDataChart), new PropertyMetadata(null, ChangedCellModels));

        public static readonly DependencyProperty IsEvidenceEditEnabledProperty =
            DependencyProperty.Register("IsEvidenceEditEnabled", typeof (bool), typeof (LineDataChart), new PropertyMetadata(false));

        public static readonly DependencyProperty IsXAxisSectionsProperty =
            DependencyProperty.Register("IsXAxisSections", typeof (bool), typeof (LineDataChart), new PropertyMetadata(true, ChangedLineData));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (LineData), typeof (LineDataChart), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty SectionIdProperty =
            DependencyProperty.Register("SectionId", typeof (string), typeof (LineDataChart), new PropertyMetadata(null, ChangedLineData));

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
        private ObservableCollection<ObservableCollection<ProbabilityDataPoint>> baseDistributionPoints;
        private ObservableCollection<CategoricalDataPoint> baseNumberPoints;
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

        public ObservableCollection<ObservableCollection<ProbabilityDataPoint>> BaseDistributionPoints
        {
            get
            {
                return this.baseDistributionPoints;
            }

            set
            {
                if (value != this.baseDistributionPoints)
                {
                    this.baseDistributionPoints = value;
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

        public IEnumerable<CellModel> CellModels
        {
            get
            {
                return (IEnumerable<CellModel>) GetValue(CellModelsProperty);
            }
            set
            {
                SetValue(CellModelsProperty, value);
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

        public LineData LineData
        {
            get
            {
                return (LineData) GetValue(LineDataProperty);
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

        public string SectionId
        {
            get
            {
                return (string) GetValue(SectionIdProperty);
            }
            set
            {
                SetValue(SectionIdProperty, value);
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void ChangedCellModels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;

            if (control.CellModels == null)
            {
                return;
            }

            control.AnchorPoints = new ObservableCollection<CategoricalDataPoint>();

            foreach (var cellModel in control.CellModels)
            {
                control.AnchorPoints.Add(new CategoricalDataPoint
                {
                    Category = cellModel.SectionId, Value = null
                });
            }

            control.UpdateBasePoints();
            control.InitializeEvidence();
        }

        private static void ChangedLineData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;
            control.InitializeEvidence();
            control.UpdateBasePoints();
        }

        private static void ChangedVertex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataChart;

            control.InitializeVerticalAxis();
            control.InitializeEvidence();
            control.UpdateBasePoints();
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

            var first = this.AnchorPoints.First().Category as string;
            var last = this.AnchorPoints.Last().Category as string;

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

        private void UpdateBasePoints()
        {
            if (this.LineData == null ||
                this.SectionId == null ||
                this.Year < 0 ||
                !this.LineData.Sections.ContainsKey(this.SectionId))
            {
                return;
            }

            this.AnchorPoints = new ObservableCollection<CategoricalDataPoint>();
            this.BaseDistributionPoints = new ObservableCollection<ObservableCollection<ProbabilityDataPoint>>();

            var categories = this.IsXAxisSections ? this.LineData.Sections.Keys : Enumerable.Range(this.LineData.StartYear, this.LineData.EndYear - this.LineData.StartYear + 1).Select(i => i as object);

            foreach (var category in categories)
            {
                var sectionId = this.IsXAxisSections ? category as string : this.SectionId;
                var year = this.IsXAxisSections ? this.Year : (int) category;

                this.AnchorPoints.Add(new CategoricalDataPoint
                {
                    Category = category,
                    Value = null
                });

                var vertexData = this.LineData.Sections[sectionId][year][this.Vertex.Key];

                var vertexEvidenceType = this.Vertex.GetEvidenceType(vertexData.String);
                var paramValues = VertexData.ParseValues(vertexData.String);

                switch (vertexEvidenceType)
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
                        paramValues.Sort();

                        while (this.BaseDistributionPoints.Count < 2)
                        {
                            this.BaseDistributionPoints.Add(new ObservableCollection<ProbabilityDataPoint>());
                        }

                        this.BaseDistributionPoints[0].Add(new ProbabilityDataPoint
                        {
                            Category = category,
                            Value = paramValues[0],
                            Probability = 0
                        });

                        this.BaseDistributionPoints[1].Add(new ProbabilityDataPoint
                        {
                            Category = category,
                            Value = paramValues[1],
                            Probability = 1
                        });

                        break;
                    }

                    case VertexEvidenceType.Distribution:
                    case VertexEvidenceType.Normal:
                    case VertexEvidenceType.Triangular:
                    {
                        var maxProb = vertexData.Evidence.Max();

                        this.Vertex.States.ForEach((state, i) =>
                        {
                            if (this.BaseDistributionPoints.Count < i + 1)
                            {
                                this.BaseDistributionPoints.Add(new ObservableCollection<ProbabilityDataPoint>());
                            }

                            this.BaseDistributionPoints[i].Add(new ProbabilityDataPoint
                            {
                                Category = category,
                                Value = state.SafeMax - state.SafeMin,
                                Probability = vertexData.Evidence[i] / maxProb
                            });
                        });

                        break;
                    }
                }
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}