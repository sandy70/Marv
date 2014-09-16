using Marv;
using Marv.LineAndSectionOverviewService;
using System.Collections.Generic;

namespace Marv
{
    public class SynergiModel : Model
    {
        private SelectableCollection<LineSummaryDTO> lines;
        private string password = "Password01";
        private SelectableCollection<SectionSummaryDTO> sections;
        private Dictionary<string, string> segmentData;
        private string ticket;
        private string username = "LAML";

        public SelectableCollection<LineSummaryDTO> Lines
        {
            get
            {
                return this.lines;
            }

            set
            {
                if (value != this.lines)
                {
                    this.lines = value;
                    this.RaisePropertyChanged("Lines");
                }
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (value != this.password)
                {
                    this.password = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }

        public SelectableCollection<SectionSummaryDTO> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                if (value != this.sections)
                {
                    this.sections = value;
                    this.RaisePropertyChanged("Sections");
                }
            }
        }

        public Dictionary<string, string> SegmentData
        {
            get
            {
                return this.segmentData;
            }

            set
            {
                if (value != this.segmentData)
                {
                    this.segmentData = value;
                    this.RaisePropertyChanged("SegmentData");
                }
            }
        }

        public string Ticket
        {
            get
            {
                return this.ticket;
            }

            set
            {
                if (value != this.ticket)
                {
                    this.ticket = value;
                    this.RaisePropertyChanged("Ticket");
                }
            }
        }

        public string UserName
        {
            get
            {
                return this.username;
            }

            set
            {
                if (value != this.username)
                {
                    this.username = value;
                    this.RaisePropertyChanged("UserName");
                }
            }
        }
    }
}