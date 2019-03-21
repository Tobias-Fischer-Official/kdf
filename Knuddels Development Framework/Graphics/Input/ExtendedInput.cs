using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KDF.Graphics
{
    /// <summary>
    /// Erweitertes Texteingabe-Control
    /// </summary>
    public partial class ExtendedInput : TextBox
    {
        /// <summary>
        /// Das Event welches ausgelöst wird, wenn die Enter/Return gedrückt wurde
        /// </summary>
        public event EventHandler<OnReturnPressedEventArgs> OnReturnPressed;

        private Dictionary<int, string> _lastInputs = new Dictionary<int, string>();

        /// <summary>
        /// Eine Liste der zuletzt getätigten Eingaben in die Textbox
        /// </summary>
        public Dictionary<int, string> LastInputs
        {
            get { return _lastInputs; }
            set { _lastInputs = value; }
        }

        private List<string> _autoCompleteEntrys = new List<string>();
        /// <summary>
        /// Die Autovervollständigungseinträge
        /// </summary>
        public List<string> AutoCompleteEntrys
        {
            get { return _autoCompleteEntrys; }
            set { _autoCompleteEntrys = value; }
        }

        private List<string> _tabCacheEntrys = new List<string>();
        /// <summary>
        /// Die Tabulator Einträge
        /// </summary>
        public List<string> TabCacheEntrys
        {
            get { return _tabCacheEntrys; }
            set { _tabCacheEntrys = value; }
        }

        private int _maxInputCache = 100;
        private int? _selectedCacheEntry = null;
        private int _inputCacheSize = 0;

        /// <summary>
        /// Kontruktor
        /// </summary>
        public ExtendedInput()
        {
            this.AcceptsTab = false;
            this.KeyDown += new KeyEventHandler(_onKeyDown);
            this.KeyPress += new KeyPressEventHandler(_onKeyPress);
        }
        /* <--- Zu Analysezwecken --->
        ListBox lbx;
        Label sCS;
        Label sSCE;
        public DynamicInputLine(ListBox lbx, Label showCacheSize, Label showSelectedCacheEntry)
        {
            components = new System.ComponentModel.Container();
            this.lbx = lbx;
            this.sCS = showCacheSize;
            this.sSCE = showSelectedCacheEntry;
            this.KeyDown += new KeyEventHandler(onKeyDown);
            this.KeyPress += new KeyPressEventHandler(onKeyPress);
        } */
        #region Enter Handlen
        private bool _skipEnter;
        private void _onKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                if (_tabCacheEntrys.Count >= 1)
                    this.Text = _tabCacheEntrys[0];
                e.Handled = true;
            }
            if (_skipEnter)
                e.Handled = true;
        }
        #endregion
        private void _onKeyDown(object sender, KeyEventArgs e)
        {
            _skipEnter = (e.KeyCode == Keys.Enter);

            #region Entertaste
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (OnReturnPressed != null && this.Text != string.Empty)
                    OnReturnPressed(this, new OnReturnPressedEventArgs(this.Text));
                if (!_lastInputs.ContainsValue(this.Text) && this.Text != string.Empty)
                {
                    _lastInputs.Add(_lastInputs.Keys.Count, this.Text);
                    _inputCacheSize = _lastInputs.Keys.Count;
                    //sCS.Text = inputCacheSize.ToString();
                    //lbx.Items.Add(this.Text);
                    if (_lastInputs.Count >= _maxInputCache)
                        _lastInputs.Remove(0);
                }
                _selectedCacheEntry = null;
                this.ResetText();
            }
            #endregion
            #region Hochtaste
            else if (e.KeyCode == Keys.Up)
            {
                if (_selectedCacheEntry == null && _lastInputs.Count >= 0 && _lastInputs.ContainsKey(_inputCacheSize - 1))
                {
                    this.Text = _lastInputs[_inputCacheSize - 1];
                    _selectedCacheEntry = _inputCacheSize - 1;
                    //sSCE.Text = selectedCacheEntry.ToString();
                }
                else if (_lastInputs.ContainsKey((int)_selectedCacheEntry - 1) && _inputCacheSize > 0)
                {
                    _selectedCacheEntry--;
                    this.Text = _lastInputs[(int)_selectedCacheEntry];
                    //sSCE.Text = selectedCacheEntry.ToString();
                }
            }
            #endregion
            #region Runtertaste
            else if (e.KeyCode == Keys.Down)
            {
                if (_selectedCacheEntry != null)
                {
                    if (_lastInputs.ContainsKey((int)_selectedCacheEntry + 1))
                    {
                        _selectedCacheEntry++;
                        this.Text = _lastInputs[(int)_selectedCacheEntry];
                        //sSCE.Text = selectedCacheEntry.ToString();
                    }
                    else
                    {
                        this.ResetText();
                        _selectedCacheEntry = _lastInputs.Count;
                        //sSCE.Text = selectedCacheEntry.ToString();
                    }
                }
            }
            #endregion
            #region Escapetaste
            else if (e.KeyCode == Keys.Escape)
            {
                string refText = string.Empty;
                if (this.Text.Contains(' '))
                {
                    int startIndex = this.Text.LastIndexOf(' ');
                    int length = this.TextLength - this.Text.LastIndexOf(' ');
                    refText = this.Text.Substring(startIndex, length).Replace(" ", "");
                }
                else
                    refText = this.Text;

                string replaceTo = refText;
                string toReplace = refText;
                refText = refText.ToLower();
                int found = 0;
                foreach (string s in _autoCompleteEntrys)
                    if ((found == 0 || found == 1) && s.ToLower().StartsWith(refText))
                        if (found == 1)
                            found = 2;
                        else
                        {
                            toReplace = s;
                            found = 1;
                        }
                if (found == 1 && replaceTo != string.Empty)
                    this.Text = this.Text.Replace(replaceTo, toReplace) + " ";
                this.SelectionStart = this.Text.Length;
            }
            #endregion
        }

        /// <summary>
        /// Fügt einen neuen Autovervollständigungseintrag hinzu
        /// </summary>
        /// <param name="Entry">Der hinzuzufügende Eintrag</param>
        public void AddAutoCompleteEntry(string Entry)
        {
            if (!this._autoCompleteEntrys.Contains(Entry) && Entry != " " && Entry != string.Empty)
                this._autoCompleteEntrys.Add(Entry);
        }

        /// <summary>
        /// Entfernt einen Autovervollständigungseintrag
        /// </summary>
        /// <param name="Entry">Der zu entfernende Eintrag</param>
        public void RemoveAutoCompleteEntry(string Entry)
        {
            if (this._autoCompleteEntrys.Contains(Entry) && Entry != " " && Entry != string.Empty)
                this._autoCompleteEntrys.Remove(Entry);
        }

        /// <summary>
        /// Fügt einen Eintrag zu dem Tab-Cache hinzu
        /// </summary>
        /// <param name="EntryString">Der hinzuzufügende Eintrag</param>
        public void AddTabCacheEntry(string EntryString)
        {
            _tabCacheEntrys.Insert(0, EntryString);
            if (_tabCacheEntrys.Count >= _maxInputCache)
                _tabCacheEntrys.RemoveAt(_tabCacheEntrys.Count - 1);
        }

        /// <summary>
        /// Entfernt einen Eintrag aus dem Tab-Cache
        /// </summary>
        /// <param name="EntryString">Der zu entfernende Eintrag</param>
        public void RemoveTabCacheEntry(string EntryString)
        {
            _tabCacheEntrys.RemoveAll(new Predicate<string>(x => x == EntryString));
        }
    }
    /// <summary>
    /// Stellt Informationen für das OnReturnPressed Event bereit
    /// </summary>
    public class OnReturnPressedEventArgs : EventArgs
    {
        private string _OutData;
        /// <summary>
        /// Die Daten die von der Textbox zurückgegeben werden
        /// </summary>
        public string OutData { get { return _OutData; } }
        /// <summary>
        /// Die Eventargumente für das OnReturnPressed Event
        /// </summary>
        /// <param name="OutData">Die Daten die von der Textbox zurückgegeben wurden</param>
        public OnReturnPressedEventArgs(string OutData) { this._OutData = OutData; }
    }
}
