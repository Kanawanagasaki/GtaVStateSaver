using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaVStateSaver
{
    public class Main : Script
    {
        private const string SAVE_FILE = "scripts/GtaVStateSaver.bin";
        private const string SETTINGS_FILE = "scripts/GtaVStateSaver.ini";

        private State _state = null;

        private Keys[] _saveKB;
        private Keys[] _loadKB;
        private bool _isKeyPressed = false;

        public Main()
        {
            if (!File.Exists(SETTINGS_FILE))
                File.WriteAllText(SETTINGS_FILE, $"[Keybindings]\nKB_SAVE={(int)Keys.ShiftKey}+{(int)Keys.F10}\nKB_LOAD={(int)Keys.F10}");

            var parser = new IniParser(SETTINGS_FILE);
            _saveKB = parser
                .GetSetting("Keybindings", "KB_SAVE")
                .Split('+')
                .Select(x => new string(x.Where(ch => char.IsDigit(ch)).ToArray()))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => (Keys)int.Parse(x))
                .OrderBy(x => x)
                .Distinct()
                .ToArray();
            if (_saveKB.Length == 0)
                _saveKB = new Keys[] { Keys.ShiftKey, Keys.F10 };

            _loadKB = parser
                .GetSetting("Keybindings", "KB_LOAD")
                .Split('+')
                .Select(x => new string(x.Where(ch => char.IsDigit(ch)).ToArray()))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => (Keys)int.Parse(x))
                .OrderBy(x => x)
                .Distinct()
                .ToArray();
            if (_loadKB.Length == 0)
                _loadKB = new Keys[] { Keys.F10 };

            if (_loadKB.Length == _saveKB.Length && _loadKB.All(x => _saveKB.Contains(x)))
            {
                _saveKB = new Keys[] { Keys.ShiftKey, Keys.F10 };
                _loadKB = new Keys[] { Keys.F10 };
            }

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            bool isSavePressed = _saveKB.All(x => Game.IsKeyPressed(x));
            bool isLoadPressed = _loadKB.All(x => Game.IsKeyPressed(x));

            bool isSavePriority = _loadKB.Length < _saveKB.Length;
            if (isSavePressed && isLoadPressed)
            {
                if (isSavePriority)
                    isLoadPressed = false;
                else isSavePressed = false;
            }

            if (isSavePressed || isLoadPressed)
            {
                if (!_isKeyPressed)
                {
                    if (isSavePressed)
                    {
                        if (_state == null)
                            _state = new State();

                        _state.Save();

                        using (var fileStream = File.OpenWrite(SAVE_FILE))
                        using (var writer = new BinaryWriter(fileStream))
                            _state.Write(writer);
                    }
                    else if (isLoadPressed)
                    {
                        if (!File.Exists(SAVE_FILE))
                            return;

                        if (_state == null)
                        {
                            _state = new State();
                            using (var fileStream = File.OpenRead(SAVE_FILE))
                            using (var reader = new BinaryReader(fileStream))
                                _state.Read(reader);
                        }

                        _state.Load();
                    }

                    _isKeyPressed = true;
                }
            }
            else if (_isKeyPressed)
                _isKeyPressed = false;
        }
    }
}
