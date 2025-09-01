using GoodSeat.Nime.Core;
using GoodSeat.Nime.Core.KeySequences;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodSeat.Nime.Controls
{
    public partial class SettingPanelTargetApplication : UserControl, ISettingPanel
    {
        internal SettingPanelTargetApplication(ApplicationSetting s)
        {
            InitializeComponent();

            if (s == ApplicationSetting.DefaultSetting)
            {
                _comboBoxBase.Enabled = false;
                _checkBoxInputMethodDerived.Enabled = false;
                _checkBoxDeleteMethodDerived.Enabled = false;

                _checkBoxEnabledDerived.Enabled = false;
                _checkBoxIgnoreCaretPositionChangeDerived.Enabled = false;
                _checkBoxVisibleInputNaviDerived.Enabled = false;
                _checkBoxEnabledInputSuggestDerived.Enabled = false;
                _checkBoxAutoConvertOnInputCommmaDerived.Enabled = false;
                _checkBoxAutoConvertOnInputPeriodDerived.Enabled = false;
                _checkBoxForceConvertCtrlUDerived.Enabled = false;
                _checkBoxForceConvertCtrlIDerived.Enabled = false;
                _checkBoxForceConvertCtrlODerived.Enabled = false;
                _checkBoxForceConvertCtrlPDerived.Enabled = false;
                _checkBoxForceConvertF6Derived.Enabled = false;
                _checkBoxForceConvertF7Derived.Enabled = false;
                _checkBoxForceConvertF8Derived.Enabled = false;
                _checkBoxForceConvertF9Derived.Enabled = false;
            }

            foreach (var entry in InputText.AllCandidates()) _comboBoxInputMethod.Items.Add(entry);
            foreach (var entry in DeleteCurrent.AllCandidates()) _comboBoxDeleteMethod.Items.Add(entry);

            Target = s;

            _checkBoxInputMethodDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxInputMethodDerived.Checked)
                {
                    Target.InputOrg = null;
                    _comboBoxInputMethod.Enabled = false;
                    foreach (var entry in _comboBoxInputMethod.Items)
                    {
                        if (entry.ToString() == Target.Input.Title)
                        {
                            _comboBoxInputMethod.SelectedItem = entry;
                            break;
                        }
                    }
                }
                else
                {
                    Target.InputOrg = Target.Input;
                    _comboBoxInputMethod.Enabled = true;
                }
            };

            _checkBoxDeleteMethodDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxDeleteMethodDerived.Checked)
                {
                    Target.DeleteOrg = null;
                    _comboBoxDeleteMethod.Enabled = false;
                    foreach (var entry in _comboBoxDeleteMethod.Items)
                    {
                        if (entry.ToString() == Target.Delete.Title)
                        {
                            _comboBoxDeleteMethod.SelectedItem = entry;
                            break;
                        }
                    }
                }
                else
                {
                    Target.DeleteOrg = Target.Delete;
                    _comboBoxDeleteMethod.Enabled = true;
                }
            };

            _checkBoxEnabledDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxEnabledDerived.Checked)
                {
                    Target.EnabledOrg = null;
                    _checkBoxEnabled.Enabled = false;
                    _checkBoxEnabled.Checked = Target.Enabled;
                }
                else
                {
                    Target.EnabledOrg = Target.Enabled;
                    _checkBoxEnabled.Enabled = true;
                }
            };

            _checkBoxIgnoreCaretPositionChangeDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxIgnoreCaretPositionChangeDerived.Checked)
                {
                    Target.IgnoreCaretChangedOrg = null;
                    _checkBoxIgnoreCaretPositionChange.Enabled = false;
                    _checkBoxIgnoreCaretPositionChange.Checked = Target.IgnoreCaretChanged;
                }
                else
                {
                    Target.IgnoreCaretChangedOrg = Target.IgnoreCaretChanged;
                    _checkBoxIgnoreCaretPositionChange.Enabled = true;
                }
            };

            _checkBoxVisibleInputNaviDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxVisibleInputNaviDerived.Checked)
                {
                    Target.VisibleInputViewOrg = null;
                    _checkBoxVisibleInputNavi.Enabled = false;
                    _checkBoxVisibleInputNavi.Checked = Target.VisibleInputView;
                }
                else
                {
                    Target.VisibleInputViewOrg = Target.VisibleInputView;
                    _checkBoxVisibleInputNavi.Enabled = true;
                }
            };

            _checkBoxEnabledInputSuggestDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxEnabledInputSuggestDerived.Checked)
                {
                    Target.VisibleInputSuggstionOrg = null;
                    _checkBoxEnabledInputSuggest.Enabled = false;
                    _checkBoxEnabledInputSuggest.Checked = Target.VisibleInputSuggstion;
                }
                else
                {
                    Target.VisibleInputSuggstionOrg = Target.VisibleInputSuggstion;
                    _checkBoxEnabledInputSuggest.Enabled = true;
                }
            };

            _checkBoxAutoConvertOnInputCommmaDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxAutoConvertOnInputCommmaDerived.Checked)
                {
                    Target.AutoConvertOnInputCommmaOrg = null;
                    _checkBoxAutoConvertOnInputCommma.Enabled = false;
                    _checkBoxAutoConvertOnInputCommma.Checked = Target.AutoConvertOnInputCommma;
                }
                else
                {
                    Target.AutoConvertOnInputCommmaOrg = Target.AutoConvertOnInputCommma;
                    _checkBoxAutoConvertOnInputCommma.Enabled = true;
                }
            };

            _checkBoxAutoConvertOnInputPeriodDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxAutoConvertOnInputPeriodDerived.Checked)
                {
                    Target.AutoConvertOnInputPeriodOrg = null;
                    _checkBoxAutoConvertOnInputPeriod.Enabled = false;
                    _checkBoxAutoConvertOnInputPeriod.Checked = Target.AutoConvertOnInputPeriod;
                }
                else
                {
                    Target.AutoConvertOnInputPeriodOrg = Target.AutoConvertOnInputPeriod;
                    _checkBoxAutoConvertOnInputPeriod.Enabled = true;
                }
            };

            _checkBoxForceConvertCtrlUDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlUDerived.Checked)
                {
                    Target.UseForceModeOnlyHiraganaWithCtrlUOrg = null;
                    _checkBoxForceConvertCtrlU.Enabled = false;
                    _checkBoxForceConvertCtrlU.Checked = Target.UseForceModeOnlyHiraganaWithCtrlU;
                }
                else
                {
                    Target.UseForceModeOnlyHiraganaWithCtrlUOrg = Target.UseForceModeOnlyHiraganaWithCtrlU;
                    _checkBoxForceConvertCtrlU.Enabled = true;
                }
            };
            _checkBoxForceConvertCtrlIDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlIDerived.Checked)
                {
                    Target.UseForceModeOnlyKatakanaWithCtrlIOrg = null;
                    _checkBoxForceConvertCtrlI.Enabled = false;
                    _checkBoxForceConvertCtrlI.Checked = Target.UseForceModeOnlyKatakanaWithCtrlI;
                }
                else
                {
                    Target.UseForceModeOnlyKatakanaWithCtrlIOrg = Target.UseForceModeOnlyKatakanaWithCtrlI;
                    _checkBoxForceConvertCtrlI.Enabled = true;
                }
            };
            _checkBoxForceConvertCtrlODerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlODerived.Checked)
                {
                    Target.UseForceModeOnlyHalfKatakanaWithCtrlOOrg = null;
                    _checkBoxForceConvertCtrlO.Enabled = false;
                    _checkBoxForceConvertCtrlO.Checked = Target.UseForceModeOnlyHalfKatakanaWithCtrlO;
                }
                else
                {
                    Target.UseForceModeOnlyHalfKatakanaWithCtrlOOrg = Target.UseForceModeOnlyHalfKatakanaWithCtrlO;
                    _checkBoxForceConvertCtrlO.Enabled = true;
                }
            };
            _checkBoxForceConvertCtrlPDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlPDerived.Checked)
                {
                    Target.UseForceModeOnlyWideRomajiWithCtrlPOrg = null;
                    _checkBoxForceConvertCtrlP.Enabled = false;
                    _checkBoxForceConvertCtrlP.Checked = Target.UseForceModeOnlyWideRomajiWithCtrlP;
                }
                else
                {
                    Target.UseForceModeOnlyWideRomajiWithCtrlPOrg = Target.UseForceModeOnlyWideRomajiWithCtrlP;
                    _checkBoxForceConvertCtrlP.Enabled = true;
                }
            };

            _checkBoxForceConvertF6Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF6Derived.Checked)
                {
                    Target.UseForceModeOnlyHiraganaWithF6Org = null;
                    _checkBoxForceConvertF6.Enabled = false;
                    _checkBoxForceConvertF6.Checked = Target.UseForceModeOnlyHiraganaWithF6;
                }
                else
                {
                    Target.UseForceModeOnlyHiraganaWithF6Org = Target.UseForceModeOnlyHiraganaWithF6;
                    _checkBoxForceConvertF6.Enabled = true;
                }
            };
            _checkBoxForceConvertF7Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF7Derived.Checked)
                {
                    Target.UseForceModeOnlyKatakanaWithF7Org = null;
                    _checkBoxForceConvertF7.Enabled = false;
                    _checkBoxForceConvertF7.Checked = Target.UseForceModeOnlyKatakanaWithF7;
                }
                else
                {
                    Target.UseForceModeOnlyKatakanaWithF7Org = Target.UseForceModeOnlyKatakanaWithF7;
                    _checkBoxForceConvertF7.Enabled = true;
                }
            };
            _checkBoxForceConvertF8Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF8Derived.Checked)
                {
                    Target.UseForceModeOnlyHalfKatakanaWithF8Org = null;
                    _checkBoxForceConvertF8.Enabled = false;
                    _checkBoxForceConvertF8.Checked = Target.UseForceModeOnlyHalfKatakanaWithF8;
                }
                else
                {
                    Target.UseForceModeOnlyHalfKatakanaWithF8Org = Target.UseForceModeOnlyHalfKatakanaWithF8;
                    _checkBoxForceConvertF8.Enabled = true;
                }
            };
            _checkBoxForceConvertF9Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF9Derived.Checked)
                {
                    Target.UseForceModeOnlyWideRomajiWithF9Org = null;
                    _checkBoxForceConvertF9.Enabled = false;
                    _checkBoxForceConvertF9.Checked = Target.UseForceModeOnlyWideRomajiWithF9;
                }
                else
                {
                    Target.UseForceModeOnlyWideRomajiWithF9Org = Target.UseForceModeOnlyWideRomajiWithF9;
                    _checkBoxForceConvertF9.Enabled = true;
                }
            };


        }

        public string TitleOfContents => Target.Name;

        /// <summary>
        /// 全体設定を取得します。
        /// </summary>
        internal Setting Setting { get; private set; }

        /// <summary>
        /// 対象とするアプリケーション設定を取得します。
        /// </summary>
        internal ApplicationSetting Target { get; private set; }

        public void OnCancel(Setting setting)
        {
            throw new NotImplementedException();
        }

        bool NowDownloading { get; set; } = false;

        void DownloadSetting()
        {
            try
            {
                NowDownloading = true;
                _checkBoxEnabled.Checked = Target.Enabled;
                _checkBoxEnabledDerived.Checked = !Target.EnabledOrg.HasValue;

                foreach (var entry in _comboBoxInputMethod.Items)
                {
                    if (entry.ToString() == Target.Input.Title)
                    {
                        _comboBoxInputMethod.SelectedItem = entry;
                        break;
                    }
                }
                foreach (var entry in _comboBoxDeleteMethod.Items)
                {
                    if (entry.ToString() == Target.Delete.Title)
                    {
                        _comboBoxDeleteMethod.SelectedItem = entry;
                        break;
                    }
                }

                _checkBoxInputMethodDerived.Checked = Target.InputOrg == null;
                _checkBoxDeleteMethodDerived.Checked = Target.DeleteOrg == null;

                _checkBoxIgnoreCaretPositionChange.Checked = Target.IgnoreCaretChanged;
                _checkBoxIgnoreCaretPositionChangeDerived.Checked = !Target.IgnoreCaretChangedOrg.HasValue;

                _checkBoxVisibleInputNavi.Checked = Target.VisibleInputView;
                _checkBoxVisibleInputNaviDerived.Checked = !Target.VisibleInputViewOrg.HasValue;

                _checkBoxEnabledInputSuggest.Checked = Target.VisibleInputSuggstion;
                _checkBoxEnabledInputSuggestDerived.Checked = !Target.VisibleInputSuggstionOrg.HasValue;

                _checkBoxAutoConvertOnInputCommma.Checked = Target.AutoConvertOnInputCommma;
                _checkBoxAutoConvertOnInputCommmaDerived.Checked = !Target.AutoConvertOnInputCommmaOrg.HasValue;

                _checkBoxAutoConvertOnInputPeriod.Checked = Target.AutoConvertOnInputPeriod;
                _checkBoxAutoConvertOnInputPeriodDerived.Checked = !Target.AutoConvertOnInputPeriodOrg.HasValue;

                _checkBoxForceConvertCtrlU.Checked = Target.UseForceModeOnlyHiraganaWithCtrlU;
                _checkBoxForceConvertCtrlUDerived.Checked = !Target.UseForceModeOnlyHiraganaWithCtrlUOrg.HasValue;
                _checkBoxForceConvertCtrlI.Checked = Target.UseForceModeOnlyKatakanaWithCtrlI;
                _checkBoxForceConvertCtrlIDerived.Checked = !Target.UseForceModeOnlyKatakanaWithCtrlIOrg.HasValue;
                _checkBoxForceConvertCtrlO.Checked = Target.UseForceModeOnlyHalfKatakanaWithCtrlO;
                _checkBoxForceConvertCtrlODerived.Checked = !Target.UseForceModeOnlyHalfKatakanaWithCtrlOOrg.HasValue;
                _checkBoxForceConvertCtrlP.Checked = Target.UseForceModeOnlyWideRomajiWithCtrlP;
                _checkBoxForceConvertCtrlPDerived.Checked = !Target.UseForceModeOnlyWideRomajiWithCtrlPOrg.HasValue;

                _checkBoxForceConvertF6.Checked = Target.UseForceModeOnlyHiraganaWithF6;
                _checkBoxForceConvertF6Derived.Checked = !Target.UseForceModeOnlyHiraganaWithF6Org.HasValue;
                _checkBoxForceConvertF7.Checked = Target.UseForceModeOnlyKatakanaWithF7;
                _checkBoxForceConvertF7Derived.Checked = !Target.UseForceModeOnlyKatakanaWithF7Org.HasValue;
                _checkBoxForceConvertF8.Checked = Target.UseForceModeOnlyHalfKatakanaWithF8;
                _checkBoxForceConvertF8Derived.Checked = !Target.UseForceModeOnlyHalfKatakanaWithF8Org.HasValue;
                _checkBoxForceConvertF9.Checked = Target.UseForceModeOnlyWideRomajiWithF9;
                _checkBoxForceConvertF9Derived.Checked = !Target.UseForceModeOnlyWideRomajiWithF9Org.HasValue;
            }
            finally
            {
                NowDownloading = false;
            }
        }

        public void OnLoading(Setting setting)
        {
            Setting = setting;

            _comboBoxBase_DropDown(this, EventArgs.Empty);

            DownloadSetting();
        }
        public void OnComeback(Setting setting)
        {
            OnLoading(setting);
        }

        public void OnOK(Setting setting)
        {
            throw new NotImplementedException();
        }


        private void _checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxEnabled.Enabled) Target.EnabledOrg = _checkBoxEnabled.Checked;
        }

        private void _comboBoxInputMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (!_comboBoxInputMethod.Enabled) return;

            Target.InputOrg = (_comboBoxInputMethod.SelectedItem as Entry<InputText>).Value;
        }

        private void _comboBoxDeleteMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (!_comboBoxDeleteMethod.Enabled) return;

            Target.DeleteOrg = (_comboBoxDeleteMethod.SelectedItem as Entry<DeleteCurrent>).Value;
        }

        private void _comboBoxBase_DropDown(object sender, EventArgs e)
        {
            _comboBoxBase.Items.Clear();

            var lstIgnore = new List<ApplicationSetting>
            {
                Target
            };

            bool retry = true;
            while (retry)
            {
                retry = false;
                foreach (var s in Setting.AppSettings)
                {
                    if (lstIgnore.Contains(s)) continue;
                    if (lstIgnore.Any(x => x == s.Parent))
                    {
                        lstIgnore.Add(s);
                        retry = true;
                    }
                }
            }


            {
                var s = ApplicationSetting.DefaultSetting;
                var entry = new Entry<ApplicationSetting> { Title = s.Name, Value = s };
                _comboBoxBase.Items.Add(entry);
                if (Target.Parent == s) _comboBoxBase.SelectedItem = entry;
            }
            foreach (var s in Setting.AppSettings)
            {
                if (lstIgnore.Contains(s)) continue;

                var entry = new Entry<ApplicationSetting> { Title = s.Name, Value = s };
                _comboBoxBase.Items.Add(entry);
                if (Target.Parent == s) _comboBoxBase.SelectedItem = entry;
            }
        }

        private void _comboBoxBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            var p = (_comboBoxBase.SelectedItem as Entry<ApplicationSetting>).Value;
            if (p == ApplicationSetting.DefaultSetting)
            {
                Target.ParentOrg = null;
            }
            else
            {
                Target.ParentOrg = p;
            }

            DownloadSetting();
        }

        private void _checkBoxIgnoreCaretPositionChange_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxIgnoreCaretPositionChange.Enabled) Target.IgnoreCaretChangedOrg = _checkBoxIgnoreCaretPositionChange.Checked;
        }

        private void _checkBoxVisibleInputNavi_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxVisibleInputNavi.Enabled) Target.VisibleInputViewOrg = _checkBoxVisibleInputNavi.Checked;
        }

        private void _checkBoxEnabledInputSuggest_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxEnabledInputSuggest.Enabled) Target.VisibleInputSuggstionOrg = _checkBoxEnabledInputSuggest.Checked;
        }

        private void _checkBoxAutoConvertOnInputCommma_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxAutoConvertOnInputCommma.Enabled) Target.AutoConvertOnInputCommmaOrg = _checkBoxAutoConvertOnInputCommma.Checked;
        }

        private void _checkBoxAutoConvertOnInputPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxAutoConvertOnInputPeriod.Enabled) Target.AutoConvertOnInputPeriodOrg = _checkBoxAutoConvertOnInputPeriod.Checked;
        }

        private void _checkBoxForceConvertCtrlU_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertCtrlU.Enabled) Target.UseForceModeOnlyHiraganaWithCtrlUOrg = _checkBoxForceConvertCtrlU.Checked;
        }

        private void _checkBoxForceConvertCtrlI_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertCtrlI.Enabled) Target.UseForceModeOnlyKatakanaWithCtrlIOrg = _checkBoxForceConvertCtrlI.Checked;
        }

        private void _checkBoxForceConvertCtrlO_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertCtrlO.Enabled) Target.UseForceModeOnlyHalfKatakanaWithCtrlOOrg = _checkBoxForceConvertCtrlO.Checked;
        }

        private void _checkBoxForceConvertCtrlP_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertCtrlP.Enabled) Target.UseForceModeOnlyWideRomajiWithCtrlPOrg = _checkBoxForceConvertCtrlP.Checked;
        }

        private void _checkBoxForceConvertF6_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertF6.Enabled) Target.UseForceModeOnlyHiraganaWithF6Org = _checkBoxForceConvertF6.Checked;
        }

        private void _checkBoxForceConvertF7_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertF7.Enabled) Target.UseForceModeOnlyKatakanaWithF7Org = _checkBoxForceConvertF7.Checked;
        }

        private void _checkBoxForceConvertF8_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertF8.Enabled) Target.UseForceModeOnlyHalfKatakanaWithF8Org = _checkBoxForceConvertF8.Checked;
        }

        private void _checkBoxForceConvertF9_CheckedChanged(object sender, EventArgs e)
        {
            if (NowDownloading) return;
            if (_checkBoxForceConvertF9.Enabled) Target.UseForceModeOnlyWideRomajiWithF9Org = _checkBoxForceConvertF9.Checked;
        }

    }
}
