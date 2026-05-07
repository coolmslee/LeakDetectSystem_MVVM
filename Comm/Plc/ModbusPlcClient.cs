using ModbusTCP;
using System.Globalization;

namespace LeakDetectSystem_MVVM.Comm.Plc
{
    public sealed class ModbusPlcClient : IDisposable
    {
        private Master? _master;
        private readonly byte _unit = 0;

        public bool IsConnected => _master?.connected == true;

        public void Connect(string ip, ushort port)
        {
            Disconnect();
            _master = new Master(ip, port, false);
        }

        public void Disconnect()
        {
            try
            {
                _master?.disconnect();
            }
            finally
            {
                _master = null;
            }
        }

        public bool ReadCoilBit(ushort address)
        {
            EnsureConnected();

            byte[]? values = null;
            _master!.ReadCoils(address, _unit, address, 1, ref values!);
            return values != null && values.Length > 0 && (values[0] & 0x01) == 0x01;
        }

        public void WriteCoilBit(ushort address, bool value)
        {
            EnsureConnected();

            byte[]? result = null;
            _master!.WriteSingleCoils(address, _unit, address, value, ref result!);
        }

        public ushort[] ReadHoldingRegisters(ushort startAddress, ushort length)
        {
            EnsureConnected();

            byte[]? bytes = null;
            _master!.ReadHoldingRegister(startAddress, _unit, startAddress, length, ref bytes!);
            if (bytes == null || bytes.Length == 0)
            {
                return Array.Empty<ushort>();
            }

            int wordCount = bytes.Length / 2;
            ushort[] words = new ushort[wordCount];
            for (int i = 0; i < wordCount; i++)
            {
                words[i] = (ushort)((bytes[i * 2] << 8) | bytes[(i * 2) + 1]);
            }

            return words;
        }

        public void WriteHoldingRegisters(ushort startAddress, ushort[] values)
        {
            EnsureConnected();

            byte[] bytes = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                bytes[i * 2] = (byte)((values[i] >> 8) & 0xFF);
                bytes[(i * 2) + 1] = (byte)(values[i] & 0xFF);
            }

            byte[]? result = null;
            _master!.WriteMultipleRegister(startAddress, _unit, startAddress, bytes, ref result!);
        }

        public static bool TryMapLegacyCoilAddress(string baseAddress, string bitIndex, out ushort mappedAddress)
        {
            mappedAddress = 0;

            if (!TryParseUShort(baseAddress, out ushort baseAddr) || !TryParseUShort(bitIndex, out ushort bit))
            {
                return false;
            }

            if (bit > 0x0F)
            {
                return false;
            }

            int address = (baseAddr * 16) + bit;
            if (address < ushort.MinValue || address > ushort.MaxValue)
            {
                return false;
            }

            mappedAddress = (ushort)address;
            return true;
        }

        private static bool TryParseUShort(string value, out ushort result)
        {
            value = value.Trim();
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return ushort.TryParse(value[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
            }

            return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        private void EnsureConnected()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("PLC is not connected.");
            }
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}
