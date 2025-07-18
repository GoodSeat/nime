﻿using GoodSeat.Nime.Core;
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

            Target = s;

            _checkBoxEnabledDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxEnabledDerived.Checked)
                {
                    _checkBoxEnabled.Enabled = false;
                    _checkBoxEnabled.Checked = Target.Enabled;
                    Target.EnabledOrg = null;
                }
                else
                {
                    _checkBoxEnabled.Enabled = true;
                    Target.EnabledOrg = Target.Enabled;
                }
            };

            _checkBoxIgnoreCaretPositionChangeDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxIgnoreCaretPositionChangeDerived.Checked)
                {
                    _checkBoxIgnoreCaretPositionChange.Enabled = false;
                    _checkBoxIgnoreCaretPositionChange.Checked = Target.IgnoreCaretChanged;
                    Target.IgnoreCaretChangedOrg = null;
                }
                else
                {
                    _checkBoxIgnoreCaretPositionChange.Enabled = true;
                    Target.IgnoreCaretChangedOrg = Target.IgnoreCaretChanged;
                }
            };

            _checkBoxVisibleInputNaviDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxVisibleInputNaviDerived.Checked)
                {
                    _checkBoxVisibleInputNavi.Enabled = false;
                    _checkBoxVisibleInputNavi.Checked = Target.VisibleInputView;
                    Target.VisibleInputViewOrg = null;
                }
                else
                {
                    _checkBoxVisibleInputNavi.Enabled = true;
                    Target.VisibleInputViewOrg = Target.VisibleInputView;
                }
            };

            _checkBoxEnabledInputSuggestDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxEnabledInputSuggestDerived.Checked)
                {
                    _checkBoxEnabledInputSuggest.Enabled = false;
                    _checkBoxEnabledInputSuggest.Checked = Target.VisibleInputSuggstion;
                    Target.VisibleInputSuggstionOrg = null;
                }
                else
                {
                    _checkBoxEnabledInputSuggest.Enabled = true;
                    Target.VisibleInputSuggstionOrg = Target.VisibleInputSuggstion;
                }
            };

            _checkBoxAutoConvertOnInputCommmaDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxAutoConvertOnInputCommmaDerived.Checked)
                {
                    _checkBoxAutoConvertOnInputCommma.Enabled = false;
                    _checkBoxAutoConvertOnInputCommma.Checked = Target.AutoConvertOnInputCommma;
                    Target.AutoConvertOnInputCommmaOrg = null;
                }
                else
                {
                    _checkBoxAutoConvertOnInputCommma.Enabled = true;
                    Target.AutoConvertOnInputCommmaOrg = Target.AutoConvertOnInputCommma;
                }
            };

            _checkBoxAutoConvertOnInputPeriodDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxAutoConvertOnInputPeriodDerived.Checked)
                {
                    _checkBoxAutoConvertOnInputPeriod.Enabled = false;
                    _checkBoxAutoConvertOnInputPeriod.Checked = Target.AutoConvertOnInputPeriod;
                    Target.AutoConvertOnInputPeriodOrg = null;
                }
                else
                {
                    _checkBoxAutoConvertOnInputPeriod.Enabled = true;
                    Target.AutoConvertOnInputPeriodOrg = Target.AutoConvertOnInputPeriod;
                }
            };

            _checkBoxForceConvertCtrlUDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlUDerived.Checked)
                {
                    _checkBoxForceConvertCtrlU.Enabled = false;
                    _checkBoxForceConvertCtrlU.Checked = Target.UseForceModeOnlyHiraganaWithCtrlU;
                    Target.UseForceModeOnlyHiraganaWithCtrlUOrg = null;
                }
                else
                {
                    _checkBoxForceConvertCtrlU.Enabled = true;
                    Target.UseForceModeOnlyHiraganaWithCtrlUOrg = Target.UseForceModeOnlyHiraganaWithCtrlU;
                }
            };
            _checkBoxForceConvertCtrlIDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlIDerived.Checked)
                {
                    _checkBoxForceConvertCtrlI.Enabled = false;
                    _checkBoxForceConvertCtrlI.Checked = Target.UseForceModeOnlyKatakanaWithCtrlI;
                    Target.UseForceModeOnlyKatakanaWithCtrlIOrg = null;
                }
                else
                {
                    _checkBoxForceConvertCtrlI.Enabled = true;
                    Target.UseForceModeOnlyKatakanaWithCtrlIOrg = Target.UseForceModeOnlyKatakanaWithCtrlI;
                }
            };
            _checkBoxForceConvertCtrlODerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlODerived.Checked)
                {
                    _checkBoxForceConvertCtrlO.Enabled = false;
                    _checkBoxForceConvertCtrlO.Checked = Target.UseForceModeOnlyHalfKatakanaWithCtrlO;
                    Target.UseForceModeOnlyHalfKatakanaWithCtrlOOrg = null;
                }
                else
                {
                    _checkBoxForceConvertCtrlO.Enabled = true;
                    Target.UseForceModeOnlyHalfKatakanaWithCtrlOOrg = Target.UseForceModeOnlyHalfKatakanaWithCtrlO;
                }
            };
            _checkBoxForceConvertCtrlPDerived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertCtrlPDerived.Checked)
                {
                    _checkBoxForceConvertCtrlP.Enabled = false;
                    _checkBoxForceConvertCtrlP.Checked = Target.UseForceModeOnlyWideRomajiWithCtrlP;
                    Target.UseForceModeOnlyWideRomajiWithCtrlPOrg = null;
                }
                else
                {
                    _checkBoxForceConvertCtrlP.Enabled = true;
                    Target.UseForceModeOnlyWideRomajiWithCtrlPOrg = Target.UseForceModeOnlyWideRomajiWithCtrlP;
                }
            };

            _checkBoxForceConvertF6Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF6Derived.Checked)
                {
                    _checkBoxForceConvertF6.Enabled = false;
                    _checkBoxForceConvertF6.Checked = Target.UseForceModeOnlyHiraganaWithF6;
                    Target.UseForceModeOnlyHiraganaWithF6Org = null;
                }
                else
                {
                    _checkBoxForceConvertF6.Enabled = true;
                    Target.UseForceModeOnlyHiraganaWithF6Org = Target.UseForceModeOnlyHiraganaWithF6;
                }
            };
            _checkBoxForceConvertF7Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF7Derived.Checked)
                {
                    _checkBoxForceConvertF7.Enabled = false;
                    _checkBoxForceConvertF7.Checked = Target.UseForceModeOnlyKatakanaWithF7;
                    Target.UseForceModeOnlyKatakanaWithF7Org = null;
                }
                else
                {
                    _checkBoxForceConvertF7.Enabled = true;
                    Target.UseForceModeOnlyKatakanaWithF7Org = Target.UseForceModeOnlyKatakanaWithF7;
                }
            };
            _checkBoxForceConvertF8Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF8Derived.Checked)
                {
                    _checkBoxForceConvertF8.Enabled = false;
                    _checkBoxForceConvertF8.Checked = Target.UseForceModeOnlyHalfKatakanaWithF8;
                    Target.UseForceModeOnlyHalfKatakanaWithF8Org = null;
                }
                else
                {
                    _checkBoxForceConvertF8.Enabled = true;
                    Target.UseForceModeOnlyHalfKatakanaWithF8Org = Target.UseForceModeOnlyHalfKatakanaWithF8;
                }
            };
            _checkBoxForceConvertF9Derived.CheckedChanged += (s, e) =>
            {
                if (_checkBoxForceConvertF9Derived.Checked)
                {
                    _checkBoxForceConvertF9.Enabled = false;
                    _checkBoxForceConvertF9.Checked = Target.UseForceModeOnlyWideRomajiWithF9;
                    Target.UseForceModeOnlyWideRomajiWithF9Org = null;
                }
                else
                {
                    _checkBoxForceConvertF9.Enabled = true;
                    Target.UseForceModeOnlyWideRomajiWithF9Org = Target.UseForceModeOnlyWideRomajiWithF9;
                }
            };


        }

        public string TitleOfContents => Target.Name;

        /// <summary>
        /// 対象とするアプリケーション設定を取得します。
        /// </summary>
        internal ApplicationSetting Target { get; private set; }

        public void OnCancel(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void OnLoading(Setting setting)
        {
            _checkBoxEnabled.Checked = Target.Enabled;
            _checkBoxEnabledDerived.Checked = !Target.EnabledOrg.HasValue;

            // TODO!:入力方法と削除方法を読み込み

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

        public void OnOK(Setting setting)
        {
            throw new NotImplementedException();
        }


    }
}
