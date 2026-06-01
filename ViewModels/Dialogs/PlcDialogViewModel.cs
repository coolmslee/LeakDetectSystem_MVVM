using LeakDetectSystem_MVVM.Commands;
using LeakDetectSystem_MVVM.Comm.Plc;
using LeakDetectSystem_MVVM.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace LeakDetectSystem_MVVM.ViewModels.Dialogs
{
    public enum PlcDisplayMode
    {
        Bit,
        Hex,
        Ascii
    }

    public class PlcDialogViewModel : DialogViewModelBase
    {
        private const int BitsPerWord = 16;
        private const int MaxReceiveLogCount = 256;

        private static readonly Brush ConnectedBrush = CreateFrozenBrush(0x2E, 0xCC, 0x71);
        private static readonly Brush DisconnectedBrush = CreateFrozenBrush(0xE7, 0x4C, 0x3C);

        private readonly ModbusPlcClient _plcClient;
        private readonly IDialogService _dialogService;

        private string _startAddress = "0";
        private string _startLength = "10";
        private string _writeData = string.Empty;

        private PlcDisplayMode _displayMode = PlcDisplayMode.Ascii;

        private bool _isConnected;
        private string _ipAddress = "192.168.100.130";
        private string _port = "502";
        private string _readAddress = "31000";
        private string _readLength = "1000";
        private string _readUnit = "0";
        private string _writeAddressOcr = "140";
        private string _writeAddress1 = "640";
        private string _writeAddress2 = "120";
        private string _writeUnit = "0";
        private string _readInterval = "300";
        private string _heartbeatAddress = "621";

        private string _barcode = "-";
        private string _seqNo = "-";

        public PlcDialogViewModel()
            : this(new ModbusPlcClient(), new DialogService())
        {
        }

        public PlcDialogViewModel(ModbusPlcClient plcClient, IDialogService dialogService)
        {
            _plcClient = plcClient ?? throw new ArgumentNullException(nameof(plcClient));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            ConnectCommand = new RelayCommand(Connect);
            ClearReceiveCommand = new RelayCommand(() => ReceiveData.Clear());
            ReadBitCommand = new RelayCommand(ReadBit);
            WriteBitCommand = new RelayCommand(WriteBit);
            ReadDataCommand = new RelayCommand(ReadData);
            WriteDataCommand = new RelayCommand(WriteDataToPlc);
            ReadInspectionResultCommand = new RelayCommand(ReadInspectionResult, () => IsConnected);
        }

        public ObservableCollection<string> ReceiveData { get; } = new();

        public ICommand ConnectCommand { get; }
        public ICommand ClearReceiveCommand { get; }
        public ICommand ReadBitCommand { get; }
        public ICommand WriteBitCommand { get; }
        public ICommand ReadDataCommand { get; }
        public ICommand WriteDataCommand { get; }
        public RelayCommand ReadInspectionResultCommand { get; }

        public string StartAddress
        {
            get => _startAddress;
            set => SetProperty(ref _startAddress, value);
        }

        public string StartLength
        {
            get => _startLength;
            set => SetProperty(ref _startLength, value);
        }

        public string WriteData
        {
            get => _writeData;
            set => SetProperty(ref _writeData, value);
        }

        public bool IsBitMode
        {
            get => DisplayMode == PlcDisplayMode.Bit;
            set
            {
                if (value)
                {
                    DisplayMode = PlcDisplayMode.Bit;
                }
            }
        }

        public bool IsHexMode
        {
            get => DisplayMode == PlcDisplayMode.Hex;
            set
            {
                if (value)
                {
                    DisplayMode = PlcDisplayMode.Hex;
                }
            }
        }

        public bool IsAsciiMode
        {
            get => DisplayMode == PlcDisplayMode.Ascii;
            set
            {
                if (value)
                {
                    DisplayMode = PlcDisplayMode.Ascii;
                }
            }
        }

        public PlcDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (SetProperty(ref _displayMode, value))
                {
                    OnPropertyChanged(nameof(IsBitMode));
                    OnPropertyChanged(nameof(IsHexMode));
                    OnPropertyChanged(nameof(IsAsciiMode));
                }
            }
        }

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(ConnectionBrush));
                    ReadInspectionResultCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public Brush ConnectionBrush => IsConnected ? ConnectedBrush : DisconnectedBrush;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public string ReadAddress
        {
            get => _readAddress;
            set => SetProperty(ref _readAddress, value);
        }

        public string ReadLength
        {
            get => _readLength;
            set => SetProperty(ref _readLength, value);
        }

        public string ReadUnit
        {
            get => _readUnit;
            set => SetProperty(ref _readUnit, value);
        }

        public string WriteAddressOcr
        {
            get => _writeAddressOcr;
            set => SetProperty(ref _writeAddressOcr, value);
        }

        public string WriteAddress1
        {
            get => _writeAddress1;
            set => SetProperty(ref _writeAddress1, value);
        }

        public string WriteAddress2
        {
            get => _writeAddress2;
            set => SetProperty(ref _writeAddress2, value);
        }

        public string WriteUnit
        {
            get => _writeUnit;
            set => SetProperty(ref _writeUnit, value);
        }

        public string ReadInterval
        {
            get => _readInterval;
            set => SetProperty(ref _readInterval, value);
        }

        public string HeartbeatAddress
        {
            get => _heartbeatAddress;
            set => SetProperty(ref _heartbeatAddress, value);
        }

        public string Barcode
        {
            get => _barcode;
            set => SetProperty(ref _barcode, value);
        }

        public string SeqNo
        {
            get => _seqNo;
            set => SetProperty(ref _seqNo, value);
        }

        private void Connect()
        {
            if (!TryParseUShort(Port, out ushort port))
            {
                AddReceiveStr($"[Error] Invalid Port: {Port}");
                _dialogService.ShowError($"포트 값이 올바르지 않습니다: {Port}", "PLC 연결 오류");
                return;
            }

            try
            {
                _plcClient.Connect(IpAddress.Trim(), port);
                IsConnected = _plcClient.IsConnected;
                AddReceiveStr($"Connect [{IpAddress}:{port}] => {IsConnected}");
                if (!IsConnected)
                {
                    _dialogService.ShowError("PLC 연결에 실패했습니다. IP/Port를 확인하세요.", "PLC 연결 실패");
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                AddReceiveStr($"[Error] Connect failed: {ex.Message}");
                _dialogService.ShowError($"PLC 연결 중 오류가 발생했습니다.\n{ex.Message}", "PLC 연결 실패");
            }
        }

        private void ReadBit()
        {
            if (!EnsureConnected())
            {
                return;
            }

            if (!TryParseLegacyCoilAddress(StartAddress, out ushort address, out string addressDisplay))
            {
                AddReceiveStr($"[Error] Invalid Start Address for bit read: {StartAddress}");
                return;
            }

            try
            {
                bool bit = _plcClient.ReadCoilBit(address);
                AddReceiveStr($"Get Bit [{addressDisplay}] : {bit}");
            }
            catch (Exception ex)
            {
                AddReceiveStr($"[Error] Bit read failed: {ex.Message}");
                IsConnected = _plcClient.IsConnected;
            }
        }

        private void WriteBit()
        {
            if (!EnsureConnected())
            {
                return;
            }

            if (!TryParseLegacyCoilAddress(StartAddress, out ushort address, out string addressDisplay))
            {
                AddReceiveStr($"[Error] Invalid Start Address for bit write: {StartAddress}");
                _dialogService.ShowError($"비트 쓰기 주소가 올바르지 않습니다: {StartAddress}", "PLC 쓰기 오류");
                return;
            }

            bool bitValue = TryParseWriteBitValue(WriteData, out bool on);
            if (!bitValue)
            {
                AddReceiveStr($"[Error] Invalid Write Data for bit write: {WriteData}");
                _dialogService.ShowError($"비트 쓰기 값이 올바르지 않습니다: {WriteData}", "PLC 쓰기 오류");
                return;
            }

            try
            {
                _plcClient.WriteCoilBit(address, on);
                AddReceiveStr($"Set Bit [{addressDisplay}] : {on}");
            }
            catch (Exception ex)
            {
                AddReceiveStr($"[Error] Bit write failed: {ex.Message}");
                IsConnected = _plcClient.IsConnected;
                _dialogService.ShowError($"비트 쓰기에 실패했습니다.\n{ex.Message}", "PLC 쓰기 실패");
            }
        }

        private void ReadData()
        {
            if (!EnsureConnected())
            {
                return;
            }

            if (!TryParseAddress(StartAddress, out ushort startAddress))
            {
                AddReceiveStr($"[Error] Invalid Start Address: {StartAddress}");
                return;
            }

            if (!TryParseUShort(StartLength, out ushort length))
            {
                AddReceiveStr($"[Error] Invalid Length: {StartLength}");
                return;
            }

            try
            {
                ushort[] values = _plcClient.ReadHoldingRegisters(startAddress, length);
                AddReceiveStr(FormatReadData(values));
            }
            catch (Exception ex)
            {
                AddReceiveStr($"[Error] Data read failed: {ex.Message}");
                IsConnected = _plcClient.IsConnected;
            }
        }

        private void WriteDataToPlc()
        {
            if (!EnsureConnected())
            {
                return;
            }

            if (!TryParseAddress(StartAddress, out ushort startAddress))
            {
                AddReceiveStr($"[Error] Invalid Start Address: {StartAddress}");
                _dialogService.ShowError($"쓰기 시작 주소가 올바르지 않습니다: {StartAddress}", "PLC 쓰기 오류");
                return;
            }

            if (!TryBuildWriteWords(out ushort[] words, out string error))
            {
                AddReceiveStr($"[Error] {error}");
                _dialogService.ShowError(error, "PLC 쓰기 오류");
                return;
            }

            if (!_dialogService.ShowConfirmation(
                    $"시작 주소 {startAddress}에 {words.Length}개 워드를 기록합니다. 계속하시겠습니까?",
                    "PLC 쓰기 확인"))
            {
                AddReceiveStr("[Info] PLC write canceled by user.");
                return;
            }

            try
            {
                _plcClient.WriteHoldingRegisters(startAddress, words);
                AddReceiveStr($"Write Data [{startAddress}] Count:{words.Length}");
            }
            catch (Exception ex)
            {
                AddReceiveStr($"[Error] Data write failed: {ex.Message}");
                IsConnected = _plcClient.IsConnected;
                _dialogService.ShowError($"데이터 쓰기에 실패했습니다.\n{ex.Message}", "PLC 쓰기 실패");
            }
        }

        private void ReadInspectionResult()
        {
            if (!EnsureConnected())
            {
                return;
            }

            if (!TryParseAddress(WriteAddress1, out ushort responseAddress))
            {
                AddReceiveStr($"[Error] Invalid inspection response address: {WriteAddress1}");
                _dialogService.ShowError($"공병정보응답 주소가 올바르지 않습니다: {WriteAddress1}", "PLC 읽기 오류");
                return;
            }

            try
            {
                ushort[] values = _plcClient.ReadHoldingRegisters(responseAddress, 4);
                string formatted = FormatReadData(values);
                AddReceiveStr($"Inspection Result [{responseAddress}]: {formatted}");
            }
            catch (Exception ex)
            {
                AddReceiveStr($"[Error] Inspection result read failed: {ex.Message}");
                IsConnected = _plcClient.IsConnected;
                _dialogService.ShowError($"검사 결과 읽기에 실패했습니다.\n{ex.Message}", "PLC 읽기 실패");
            }
        }


        private bool TryBuildWriteWords(out ushort[] words, out string error)
        {
            words = Array.Empty<ushort>();
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(WriteData))
            {
                if (TryParseUShort(StartLength, out ushort emptyLength))
                {
                    words = new ushort[emptyLength];
                    return true;
                }

                error = $"Invalid Length: {StartLength}";
                return false;
            }

            string text = WriteData.Trim();

            if (DisplayMode == PlcDisplayMode.Bit)
            {
                text = text.Replace(" ", string.Empty);
                if (text.Any(ch => ch != '0' && ch != '1'))
                {
                    error = "Bit mode write data must contain only 0 or 1.";
                    return false;
                }

                int wordSize = text.Length / BitsPerWord;
                if ((text.Length % BitsPerWord) > 0)
                {
                    wordSize++;
                }

                words = new ushort[wordSize];
                for (int i = 0; i < wordSize; i++)
                {
                    int tmp = 0;
                    for (int j = 0; j < BitsPerWord; j++)
                    {
                        int index = text.Length - ((i * BitsPerWord) + j) - 1;
                        if (index < 0)
                        {
                            break;
                        }

                        if (text[index] == '1')
                        {
                            tmp |= (1 << j);
                        }
                    }

                    words[i] = (ushort)tmp;
                }

                return true;
            }

            if (DisplayMode == PlcDisplayMode.Hex)
            {
                string[] tokens = text.Split([' ', ',', ';', '\t'], StringSplitOptions.RemoveEmptyEntries);
                List<ushort> parsed = new();

                if (tokens.Length > 1)
                {
                    foreach (string token in tokens)
                    {
                        if (!TryParseHexWord(token, out ushort value))
                        {
                            error = $"Invalid hex word: {token}";
                            return false;
                        }

                        parsed.Add(value);
                    }
                }
                else
                {
                    string compact = text.Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
                    if ((compact.Length % 4) != 0)
                    {
                        error = "Hex mode write data must be 4-hex chunks (or separated by spaces).";
                        return false;
                    }

                    for (int i = 0; i < compact.Length; i += 4)
                    {
                        if (!ushort.TryParse(compact.Substring(i, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort value))
                        {
                            error = $"Invalid hex word: {compact.Substring(i, 4)}";
                            return false;
                        }

                        parsed.Add(value);
                    }
                }

                words = parsed.ToArray();
                return true;
            }

            words = AsciiToWords(text);
            return true;
        }

        private static ushort[] AsciiToWords(string value)
        {
            List<ushort> words = new();

            for (int i = 0; i < value.Length; i += 2)
            {
                byte low = (byte)value[i];
                byte high = (i + 1) < value.Length ? (byte)value[i + 1] : (byte)0;
                words.Add((ushort)(low | (high << 8)));
            }

            return words.ToArray();
        }

        private string FormatReadData(ushort[] values)
        {
            if (values.Length == 0)
            {
                return string.Empty;
            }

            if (DisplayMode == PlcDisplayMode.Bit)
            {
                StringBuilder bits = new();
                foreach (ushort word in values)
                {
                    for (int j = 0; j < BitsPerWord; j++)
                    {
                        if (j > 0 && (j % 4) == 0)
                        {
                            bits.Append(' ');
                        }

                        bits.Append(((word & (1 << ((BitsPerWord - 1) - j))) > 0) ? '1' : '0');
                    }
                }

                return bits.ToString();
            }

            if (DisplayMode == PlcDisplayMode.Hex)
            {
                return string.Join(' ', values.Select(v => v.ToString("X4", CultureInfo.InvariantCulture)));
            }

            StringBuilder ascii = new();
            foreach (ushort value in values)
            {
                ascii.Append((char)(value & 0xFF));
                ascii.Append((char)((value >> 8) & 0xFF));
            }

            return ascii.ToString().Trim('\0');
        }

        private bool EnsureConnected()
        {
            IsConnected = _plcClient.IsConnected;
            if (IsConnected)
            {
                return true;
            }

            AddReceiveStr("[Error] PLC is not connected.");
            _dialogService.ShowError("PLC가 연결되어 있지 않습니다. 먼저 연결을 수행하세요.", "PLC 연결 필요");
            return false;
        }

        private void AddReceiveStr(string text)
        {
            if (ReceiveData.Count > MaxReceiveLogCount)
            {
                ReceiveData.Clear();
            }

            ReceiveData.Add(text);
        }

        private static bool TryParseAddress(string value, out ushort address)
        {
            value = value.Trim();
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return ushort.TryParse(value[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out address);
            }

            return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out address);
        }

        private static bool TryParseUShort(string value, out ushort result)
            => ushort.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

        private static bool TryParseWriteBitValue(string value, out bool on)
        {
            value = value.Trim();
            if (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                on = true;
                return true;
            }

            if (value == "0" || value.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                on = false;
                return true;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number))
            {
                on = number > 0;
                return true;
            }

            on = false;
            return false;
        }

        private static bool TryParseHexWord(string token, out ushort value)
        {
            token = token.Trim();
            if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                token = token[2..];
            }

            return ushort.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryParseLegacyCoilAddress(string input, out ushort coilAddress, out string displayAddress)
        {
            displayAddress = input.Trim();
            coilAddress = 0;

            if (displayAddress.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return TryParseAddress(displayAddress, out coilAddress);
            }

            if (displayAddress.Length >= 5)
            {
                string baseAddr = displayAddress[..4];
                string bitHex = displayAddress.Substring(4, 1);

                if (byte.TryParse(bitHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte bitIndex)
                    && ModbusPlcClient.TryMapLegacyCoilAddress(baseAddr, bitIndex.ToString(CultureInfo.InvariantCulture), out coilAddress))
                {
                    return true;
                }
            }

            return TryParseAddress(displayAddress, out coilAddress);
        }

        private static Brush CreateFrozenBrush(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }
    }
}
