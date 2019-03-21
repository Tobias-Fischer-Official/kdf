using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.KDFObjects
{
    public class ProgramInfo
    {
        public enum SoftwareType : int
        {
            Game = 0,
            Tool = 1,            
            StayOnline = 2,
            Mafia1 = 3,
            Mafia2 = 4,
            FotoContest = 5,
            MauMau = 6,
            Bingo = 7,
            Poker = 8,
            Fifty = 9,
            Quiz = 10,
            WordMix = 11,
            MultiBot = 12,
            Sonstiges = 13
        }

        private Version _version;

        public Version Version
        {
            get { return _version; }
            set { _version = value; }
        }
        private string _author;

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }
        private string _programName;

        public string ProgramName
        {
            get { return _programName; }
            set { _programName = value; }
        }
        private string _downloadLink;

        public string DownloadLink
        {
            get { return _downloadLink; }
            set { _downloadLink = value; }
        }
        private string _releaseLink;

        public string ReleaseLink
        {
            get { return _releaseLink; }
            set { _releaseLink = value; }
        }
        private string _supportLink;

        public string SupportLink
        {
            get { return _supportLink; }
            set { _supportLink = value; }
        }
        private string _iconLink;
        private SoftwareType _softwareType;

        public SoftwareType SoftwareType1
        {
            get { return _softwareType; }
            set { _softwareType = value; }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _howto;

        public string Howto
        {
            get { return _howto; }
            set { _howto = value; }
        }
        private string[] _screens;

        public string[] Screens
        {
            get { return _screens; }
            set { _screens = value; }
        }

        private string _features;
        public string Features
        {
            get { return _features; }
            set { _features = value; }
        }
        
        public ProgramInfo(Version version, string author, string programName, SoftwareType softwareType)
        {
            _version = version;
            _author = author;
            _programName = programName;
            _softwareType = softwareType;
        }
    }
}