#nullable disable
#pragma warning disable 0168, 8618, 8600, 8625, 8603

﻿using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Modbus TCP common driver class. 
/// </summary>
namespace ModbusTCP
{
    /// <summary>
    /// Modbus TCP common driver class. 
    /// </summary>
    /// 
    /// This class implements a modbus TCP master driver. It supports the following commands:
    /// 
    /// Read coils
    /// Read discrete inputs
    /// Write single coil
    /// Write multiple cooils
    /// Read holding register
    /// Read input register
    /// Write single register
    /// Write multiple register
    /// 
    /// All commands can be sent in synchronous or asynchronous mode. If a value is accessed
    /// in synchronous mode the program will stop and wait for slave to response. If the 
    /// slave didn't answer within a specified time a timeout exception is called.
    /// The class uses multi threading for both synchronous and asynchronous access. For
    /// the communication two lines are created. This is necessary because the synchronous
    /// thread has to wait for a previous command to finish.
    /// The synchronous channel can be disabled during connection. This can be necessary when
    /// the slave only supports one connection.
    /// 
    public class Master
    {
        // ------------------------------------------------------------------------
        // Constants for access
        private const byte fctReadCoil = 1;
        private const byte fctReadDiscreteInputs = 2;
        private const byte fctReadHoldingRegister = 3;
        private const byte fctReadInputRegister = 4;
        private const byte fctWriteSingleCoil = 5;
        private const byte fctWriteSingleRegister = 6;
        private const byte fctWriteMultipleCoils = 15;
        private const byte fctWriteMultipleRegister = 16;
        private const byte fctReadWriteMultipleRegister = 23;

        /// <summary>Constant for exception illegal function.</summary>
        public const byte excIllegalFunction = 1;
        /// <summary>Constant for exception illegal data address.</summary>
        public const byte excIllegalDataAdr = 2;
        /// <summary>Constant for exception illegal data value.</summary>
        public const byte excIllegalDataVal = 3;
        /// <summary>Constant for exception slave device failure.</summary>
        public const byte excSlaveDeviceFailure = 4;
        /// <summary>Constant for exception acknowledge. This is triggered if a write request is executed while the watchdog has expired.</summary>
        public const byte excAck = 5;
        /// <summary>Constant for exception slave is busy/booting up.</summary>
        public const byte excSlaveIsBusy = 6;
        /// <summary>Constant for exception gate path unavailable.</summary>
        public const byte excGatePathUnavailable = 10;
        /// <summary>Constant for exception not connected.</summary>
        public const byte excExceptionNotConnected = 253;
        /// <summary>Constant for exception connection lost.</summary>
        public const byte excExceptionConnectionLost = 254;
        /// <summary>Constant for exception response timeout.</summary>
        public const byte excExceptionTimeout = 255;
        /// <summary>Constant for exception wrong offset.</summary>
        private const byte excExceptionOffset = 128;
        /// <summary>Constant for exception send failt.</summary>
        private const byte excSendFailt = 100;

        // ------------------------------------------------------------------------
        // Private declarations
        private static ushort _timeout = 500;
        private static ushort _refresh = 10;
        private bool _connected = false;
        private static bool _no_sync_connection = false;

        private Socket tcpAsyCl;
        private byte[] tcpAsyClBuffer = new byte[2048];

        private Socket tcpSynCl;
        private byte[] tcpSynClBuffer = new byte[2048];

        // ------------------------------------------------------------------------
        /// <summary>Response data event. This event is called when new data arrives</summary>
        public delegate void ResponseData(ushort id, byte unit, byte function, byte[] data);
        /// <summary>Response data event. This event is called when new data arrives</summary>
        public event ResponseData OnResponseData;
        /// <summary>Exception data event. This event is called when the data is incorrect</summary>
        public delegate void ExceptionData(ushort id, byte unit, byte function, byte exception);
        /// <summary>Exception data event. This event is called when the data is incorrect</summary>
        public event ExceptionData OnException;

        // ------------------------------------------------------------------------
        /// <summary>Response timeout. If the slave didn't answers within in this time an exception is called.</summary>
        /// <value>The default value is 500ms.</value>
        public ushort timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        // ------------------------------------------------------------------------
        /// <summary>Refresh timer for slave answer. The class is polling for answer every X ms.</summary>
        /// <value>The default value is 10ms.</value>
        public ushort refresh
        {
            get { return _refresh; }
            set { _refresh = value; }
        }

        // ------------------------------------------------------------------------
        /// <summary>Displays the state of the synchronous channel</summary>
        /// <value>True if channel was diabled during connection.</value>
        public bool NoSyncConnection
        {
            get { return _no_sync_connection; }
        }

        // ------------------------------------------------------------------------
        /// <summary>Shows if a connection is active.</summary>
        public bool connected
        {
            get { return _connected; }
        }

        // ------------------------------------------------------------------------
        /// <summary>Create master instance without parameters.</summary>
        public Master()
        {
        }

        // ------------------------------------------------------------------------
        /// <summary>Create master instance with parameters.</summary>
        /// <param name="ip">IP adress of modbus slave.</param>
        /// <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
        public Master(string ip, ushort port)
        {
            connect(ip, port, false);
        }

        // ------------------------------------------------------------------------
        /// <summary>Create master instance with parameters.</summary>
        /// <param name="ip">IP adress of modbus slave.</param>
        /// <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
        /// <param name="no_sync_connection">Disable second connection for synchronous requests</param>
        public Master(string ip, ushort port, bool no_sync_connection)
        {
            connect(ip, port, no_sync_connection);
        }

        // ------------------------------------------------------------------------
        /// <summary>Start connection to slave.</summary>
        /// <param name="ip">IP adress of modbus slave.</param>
        /// <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
        /// <param name="no_sync_connection">Disable sencond connection for synchronous requests</param>
        public void connect(string ip, ushort port, bool no_sync_connection)
        {
            try
            {
                IPAddress _ip;
                _no_sync_connection = no_sync_connection;
                if (IPAddress.TryParse(ip, out _ip) == false)
                {
                    IPHostEntry hst = Dns.GetHostEntry(ip);
                    ip = hst.AddressList[0].ToString();
                }
                // ----------------------------------------------------------------
                // Connect synchronous client
                if (_no_sync_connection)
                {
                    // ----------------------------------------------------------------
                    // Connect asynchronous client
                    // ✅ Async 소켓 생성 및 연결
                    tcpAsyCl = new Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // ✅ 연결 타임아웃 설정
                    IAsyncResult connectResult = tcpAsyCl.BeginConnect(
                        new IPEndPoint(IPAddress.Parse(ip), port), null, null);

                    bool success = connectResult.AsyncWaitHandle.WaitOne(_timeout, true);

                    if (!success || !tcpAsyCl.Connected)
                    {
                        tcpAsyCl.Close();
                        tcpAsyCl = null;
                        _connected = false;
                        throw new SocketException(10060); // WSAETIMEDOUT
                    }

                    //tcpAsyCl.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    tcpAsyCl.EndConnect(connectResult);
                    tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
                    tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
                    tcpAsyCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                    // 2025.10.23. GSM : Keep Alive 기능 추가
                    EnableKeepAlive(tcpAsyCl, 10000, 2000);
                }
                else
                {
                    // ✅ Sync 소켓도 동일하게 개선
                    tcpSynCl = new Socket(IPAddress.Parse(ip).AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    IAsyncResult connectResult = tcpSynCl.BeginConnect(
                new IPEndPoint(IPAddress.Parse(ip), port), null, null);

                    bool success = connectResult.AsyncWaitHandle.WaitOne(_timeout, true);

                    if (!success || !tcpSynCl.Connected)
                    {
                        tcpSynCl.Close();
                        tcpSynCl = null;
                        _connected = false;
                        throw new SocketException(10060);
                    }

                    //tcpSynCl.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    tcpSynCl.EndConnect(connectResult);
                    tcpSynCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
                    tcpSynCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
                    tcpSynCl.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);

                    // 2025.10.23. GSM : Keep Alive 기능 추가
                    EnableKeepAlive(tcpSynCl, 10000, 2000);
                }
                _connected = true;
            }
            catch (SocketException ex)
            {
                // ✅ 소켓 생성/연결 실패
                _connected = false;

                if (tcpAsyCl != null)
                {
                    try { tcpAsyCl.Close(); } catch { }
                    tcpAsyCl = null;
                }
                if (tcpSynCl != null)
                {
                    try { tcpSynCl.Close(); } catch { }
                    tcpSynCl = null;
                }

                throw; // ✅ 예외 재던지기 (CModbusPLC.Connect에서 처리)
            }
            catch (Exception ex)
            {
                // ✅ 기타 예외
                _connected = false;

                if (tcpAsyCl != null)
                {
                    try { tcpAsyCl.Close(); } catch { }
                    tcpAsyCl = null;
                }
                if (tcpSynCl != null)
                {
                    try { tcpSynCl.Close(); } catch { }
                    tcpSynCl = null;
                }

                throw;
            }
        }

        // 2025.10.23. GSM : Keep Alive 기능 추가
        private void EnableKeepAlive(Socket sock, uint keepAliveTime, uint keepAliveInterval)
        {
            // 기본 KeepAlive 옵션 활성화
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // SIO_KEEPALIVE_VALS 구조체 = [onoff(4)][time(ms)(4)][interval(ms)(4)]
            byte[] inOptionValues = new byte[12];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);      // onoff = 1
            BitConverter.GetBytes(keepAliveTime).CopyTo(inOptionValues, 4);  // KeepAliveTime
            BitConverter.GetBytes(keepAliveInterval).CopyTo(inOptionValues, 8); // KeepAliveInterval

            sock.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        // ------------------------------------------------------------------------
        /// <summary>Stop connection to slave.</summary>
        public void disconnect()
        {
            _connected = false;
            Dispose();
        }

        // ------------------------------------------------------------------------
        /// <summary>Destroy master instance.</summary>
        ~Master()
        {
            Dispose();
        }

        // ------------------------------------------------------------------------
        /// <summary>Destroy master instance</summary>
        public void Dispose()
        {
            _connected = false; // ✅ 먼저 플래그 설정

            // ✅ Async 소켓 정리
            if (tcpAsyCl != null)
            {
                try
                {
                    if (tcpAsyCl.Connected)
                    {
                        try { tcpAsyCl.Shutdown(SocketShutdown.Both); }
                        catch { }
                    }
                    tcpAsyCl.Close(); // ✅ Connected 여부와 상관없이 Close
                }
                catch (ObjectDisposedException) { }
                catch { }
                finally
                {
                    tcpAsyCl = null;
                }
            }

            // ✅ Sync 소켓 정리
            if (tcpSynCl != null)
            {
                try
                {
                    if (tcpSynCl.Connected)
                    {
                        try { tcpSynCl.Shutdown(SocketShutdown.Both); }
                        catch { }
                    }
                    tcpSynCl.Close();
                }
                catch (ObjectDisposedException) { }
                catch { }
                finally
                {
                    tcpSynCl = null;
                }
            }

            //if (tcpAsyCl != null)
            //{
            //    if (tcpAsyCl.Connected)
            //    {
            //        try { tcpAsyCl.Shutdown(SocketShutdown.Both); }
            //        catch { }
            //        tcpAsyCl.Close();
            //    }
            //    tcpAsyCl = null;
            //}
            //if (tcpSynCl != null)
            //{
            //    if (tcpSynCl.Connected)
            //    {
            //        try { tcpSynCl.Shutdown(SocketShutdown.Both); }
            //        catch { }
            //        tcpSynCl.Close();
            //    }
            //    tcpSynCl = null;
            //}
        }

        internal void CallException(ushort id, byte unit, byte function, byte exception)
        {
            if ((tcpAsyCl == null && _no_sync_connection) || (tcpSynCl == null && !_no_sync_connection)) return;
            if (exception == excExceptionConnectionLost)
            {
                disconnect();
                //tcpSynCl = null;
                //tcpAsyCl = null;
            }
            if (OnException != null) OnException(id, unit, function, exception);
        }

        internal static UInt16 SwapUInt16(UInt16 inValue)
        {
            return (UInt16)(((inValue & 0xff00) >> 8) |
                     ((inValue & 0x00ff) << 8));
        }

        // ------------------------------------------------------------------------
        /// <summary>Read coils from slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        public void ReadCoils(ushort id, byte unit, ushort startAddress, ushort numInputs)
        {
            if (numInputs > 2000)
            {
                CallException(id, unit, fctReadCoil, excIllegalDataVal);
                return;
            }
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadCoil), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read coils from slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="values">Contains the result of function.</param>
        public void ReadCoils(ushort id, byte unit, ushort startAddress, ushort numInputs, ref byte[] values)
        {
            if (numInputs > 2000)
            {
                CallException(id, unit, fctReadCoil, excIllegalDataVal);
                return;
            }
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadCoil), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read discrete inputs from slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        public void ReadDiscreteInputs(ushort id, byte unit, ushort startAddress, ushort numInputs)
        {
            if (numInputs > 2000)
            {
                CallException(id, unit, fctReadDiscreteInputs, excIllegalDataVal);
                return;
            }
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadDiscreteInputs), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read discrete inputs from slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="values">Contains the result of function.</param>
        public void ReadDiscreteInputs(ushort id, byte unit, ushort startAddress, ushort numInputs, ref byte[] values)
        {
            if (numInputs > 2000)
            {
                CallException(id, unit, fctReadDiscreteInputs, excIllegalDataVal);
                return;
            }

            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadDiscreteInputs), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read holding registers from slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        public void ReadHoldingRegister(ushort id, byte unit, ushort startAddress, ushort numInputs)
        {
            if (numInputs > 125)
            {
                CallException(id, unit, fctReadHoldingRegister, excIllegalDataVal);
                return;
            }
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadHoldingRegister), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read holding registers from slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="values">Contains the result of function.</param>
        public void ReadHoldingRegister(ushort id, byte unit, ushort startAddress, ushort numInputs, ref byte[] values)
        {
            if (numInputs > 125)
            {
                CallException(id, unit, fctReadHoldingRegister, excIllegalDataVal);
                return;
            }
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadHoldingRegister), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read input registers from slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        public void ReadInputRegister(ushort id, byte unit, ushort startAddress, ushort numInputs)
        {
            if (numInputs > 125)
            {
                CallException(id, unit, fctReadInputRegister, excIllegalDataVal);
                return;
            }
            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadInputRegister), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read input registers from slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="values">Contains the result of function.</param>
        public void ReadInputRegister(ushort id, byte unit, ushort startAddress, ushort numInputs, ref byte[] values)
        {
            if (numInputs > 125)
            {
                CallException(id, unit, fctReadInputRegister, excIllegalDataVal);
                return;
            }
            values = WriteSyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadInputRegister), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write single coil in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="OnOff">Specifys if the coil should be switched on or off.</param>
        public void WriteSingleCoils(ushort id, byte unit, ushort startAddress, bool OnOff)
        {
            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleCoil);
            if (OnOff == true) data[10] = 255;
            else data[10] = 0;
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write single coil in slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="OnOff">Specifys if the coil should be switched on or off.</param>
        /// <param name="result">Contains the result of the synchronous write.</param>
        public void WriteSingleCoils(ushort id, byte unit, ushort startAddress, bool OnOff, ref byte[] result)
        {
            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleCoil);
            if (OnOff == true) data[10] = 255;
            else data[10] = 0;
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write multiple coils in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numBits">Specifys number of bits.</param>
        /// <param name="values">Contains the bit information in byte format.</param>
        public void WriteMultipleCoils(ushort id, byte unit, ushort startAddress, ushort numBits, byte[] values)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250 || numBits > 2000)
            {
                CallException(id, unit, fctWriteMultipleCoils, excIllegalDataVal);
                return;
            }

            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, numBits, (byte)(numBytes + 2), fctWriteMultipleCoils);
            Array.Copy(values, 0, data, 13, numBytes);
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write multiple coils in slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="numBits">Specifys number of bits.</param>
        /// <param name="values">Contains the bit information in byte format.</param>
        /// <param name="result">Contains the result of the synchronous write.</param>
        public void WriteMultipleCoils(ushort id, byte unit, ushort startAddress, ushort numBits, byte[] values, ref byte[] result)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250 || numBits > 2000)
            {
                CallException(id, unit, fctWriteMultipleCoils, excIllegalDataVal);
                return;
            }

            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, numBits, (byte)(numBytes + 2), fctWriteMultipleCoils);
            Array.Copy(values, 0, data, 13, numBytes);
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write single register in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        public void WriteSingleRegister(ushort id, byte unit, ushort startAddress, byte[] values)
        {
            if (values.GetUpperBound(0) != 1)
            {
                CallException(id, unit, fctReadCoil, excIllegalDataVal);
                return;
            }
            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleRegister);
            data[10] = values[0];
            data[11] = values[1];
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write single register in slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        /// <param name="result">Contains the result of the synchronous write.</param>
        public void WriteSingleRegister(ushort id, byte unit, ushort startAddress, byte[] values, ref byte[] result)
        {
            if (values.GetUpperBound(0) != 1)
            {
                CallException(id, unit, fctReadCoil, excIllegalDataVal);
                return;
            }
            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleRegister);
            data[10] = values[0];
            data[11] = values[1];
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        public void WriteMultipleRegister(ushort id, byte unit, ushort startAddress, byte[] values)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateWriteHeader(id, unit, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister);
            Array.Copy(values, 0, data, 13, values.Length);
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write multiple registers in slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        /// <param name="result">Contains the result of the synchronous write.</param>
        public void WriteMultipleRegister(ushort id, byte unit, ushort startAddress, byte[] values, ref byte[] result)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateWriteHeader(id, unit, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister);
            Array.Copy(values, 0, data, 13, values.Length);
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read/Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startReadAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="startWriteAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        public void ReadWriteMultipleRegister(ushort id, byte unit, ushort startReadAddress, ushort numInputs, ushort startWriteAddress, byte[] values)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctReadWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateReadWriteHeader(id, unit, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes / 2));
            Array.Copy(values, 0, data, 17, values.Length);
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read/Write multiple registers in slave synchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startReadAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="startWriteAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        /// <param name="result">Contains the result of the synchronous command.</param>
        public void ReadWriteMultipleRegister(ushort id, byte unit, ushort startReadAddress, ushort numInputs, ushort startWriteAddress, byte[] values, ref byte[] result)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctReadWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateReadWriteHeader(id, unit, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes / 2));
            Array.Copy(values, 0, data, 17, values.Length);
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        // Create modbus header for read action
        private byte[] CreateReadHeader(ushort id, byte unit, ushort startAddress, ushort length, byte function)
        {
            byte[] data = new byte[12];

            byte[] _id = BitConverter.GetBytes((short)id);
            data[0] = _id[1];			    // Slave id high byte
            data[1] = _id[0];				// Slave id low byte
            data[5] = 6;					// Message size
            data[6] = unit;					// Slave address
            data[7] = function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = _adr[0];				// Start address
            data[9] = _adr[1];				// Start address
            byte[] _length = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)length));
            data[10] = _length[0];			// Number of data to read
            data[11] = _length[1];			// Number of data to read
            return data;
        }

        // ------------------------------------------------------------------------
        // Create modbus header for write action
        private byte[] CreateWriteHeader(ushort id, byte unit, ushort startAddress, ushort numData, ushort numBytes, byte function)
        {
            byte[] data = new byte[numBytes + 11];

            byte[] _id = BitConverter.GetBytes((short)id);
            data[0] = _id[1];				// Slave id high byte
            data[1] = _id[0];				// Slave id low byte
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(5 + numBytes)));
            data[4] = _size[0];				// Complete message size in bytes
            data[5] = _size[1];				// Complete message size in bytes
            data[6] = unit;					// Slave address
            data[7] = function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = _adr[0];				// Start address
            data[9] = _adr[1];				// Start address
            if (function >= fctWriteMultipleCoils)
            {
                byte[] _cnt = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numData));
                data[10] = _cnt[0];			// Number of bytes
                data[11] = _cnt[1];			// Number of bytes
                data[12] = (byte)(numBytes - 2);
            }
            return data;
        }

        // ------------------------------------------------------------------------
        // Create modbus header for read/write action
        private byte[] CreateReadWriteHeader(ushort id, byte unit, ushort startReadAddress, ushort numRead, ushort startWriteAddress, ushort numWrite)
        {
            byte[] data = new byte[numWrite * 2 + 17];

            byte[] _id = BitConverter.GetBytes((short)id);
            data[0] = _id[1];						// Slave id high byte
            data[1] = _id[0];						// Slave id low byte
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(11 + numWrite * 2)));
            data[4] = _size[0];						// Complete message size in bytes
            data[5] = _size[1];						// Complete message size in bytes
            data[6] = unit;							// Slave address
            data[7] = fctReadWriteMultipleRegister;	// Function code
            byte[] _adr_read = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startReadAddress));
            data[8] = _adr_read[0];					// Start read address
            data[9] = _adr_read[1];					// Start read address
            byte[] _cnt_read = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numRead));
            data[10] = _cnt_read[0];				// Number of bytes to read
            data[11] = _cnt_read[1];				// Number of bytes to read
            byte[] _adr_write = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startWriteAddress));
            data[12] = _adr_write[0];				// Start write address
            data[13] = _adr_write[1];				// Start write address
            byte[] _cnt_write = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numWrite));
            data[14] = _cnt_write[0];				// Number of bytes to write
            data[15] = _cnt_write[1];				// Number of bytes to write
            data[16] = (byte)(numWrite * 2);

            return data;
        }

        // ------------------------------------------------------------------------
        // Write asynchronous data
        private void WriteAsyncData(byte[] write_data, ushort id)
        {
            if (tcpAsyCl == null || !tcpAsyCl.Connected)
            {
                _connected = false;
                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                return;
            }

            try
            {
                tcpAsyCl.BeginSend(write_data, 0, write_data.Length,
                    SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (ObjectDisposedException)
            {
                _connected = false;
                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            }
            catch (SocketException)
            {
                _connected = false;
                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            }
            catch (Exception)
            {
                _connected = false;
                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            }
            
            //if ((tcpAsyCl != null) && (tcpAsyCl.Connected))
            //{
            //    try
            //    {
            //        tcpAsyCl.BeginSend(write_data, 0, write_data.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            //    }
            //    catch (SystemException)
            //    {
            //        CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            //    }
            //}
            //else
            //{
            //    _connected = false;
            //    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            //}
        }

        // ------------------------------------------------------------------------
        // Write asynchronous data acknowledge
        private void OnSend(System.IAsyncResult result)
        {
            if (tcpAsyCl == null) return; // ✅ null 체크

            try
            {
                Int32 size = tcpAsyCl.EndSend(result);

                // ✅ size == 0 또는 미완료 체크
                if (size == 0 || !result.IsCompleted)
                {
                    CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
                    return;
                }

                // ✅ 수신 대기 시작
                tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length,
                    SocketFlags.None, new AsyncCallback(OnReceive), tcpAsyCl);
            }
            catch (ObjectDisposedException)
            {
                // 소켓이 이미 닫힌 경우 - 조용히 종료
                return;
            }
            catch (SocketException ex)
            {
                CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
            }
            catch (Exception ex)
            {
                CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
            }

            //try
            //{
            //    Int32 size = tcpAsyCl.EndSend(result);
            //    if (result.IsCompleted == false) CallException(0xFFFF, 0xFF, 0xFF, excSendFailt);
            //    else tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), tcpAsyCl);
            //}
            //catch (Exception)
            //{
            //}
        }

        // ------------------------------------------------------------------------
        // Write asynchronous data response
        private void OnReceive(System.IAsyncResult result)
        {
            if (tcpAsyCl == null) return;

            try
            {
                int bytesRead = tcpAsyCl.EndReceive(result);
                // bytesRead == 0 : 원격이 정상적으로 연결을 닫았음 -> 연결 끊김 처리
                if (bytesRead == 0)
                {
                    CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
                    return;
                }

                // ✅ 최소 Modbus TCP 헤더 크기 검증 (9 bytes)
                if (bytesRead < 9)
                {
                    CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
                    return;
                }


                // 기존 처리: result.IsCompleted 검사 대신 읽은 바이트 수로 판단하는 것이 안전함
                // (예외 발생 시 아래 catch에서 처리됨)
                ushort id = SwapUInt16(BitConverter.ToUInt16(tcpAsyClBuffer, 0));
                byte unit = tcpAsyClBuffer[6];
                byte function = tcpAsyClBuffer[7];
                byte[] data;

                if ((function >= fctWriteSingleCoil) && (function != fctReadWriteMultipleRegister))
                {
                    // ✅ Write 응답 길이 검증
                    if (bytesRead < 12)
                    {
                        CallException(id, unit, function, excExceptionConnectionLost);
                        return;
                    }
                    data = new byte[2];
                    Array.Copy(tcpAsyClBuffer, 10, data, 0, 2);
                }
                else
                {
                    int len = tcpAsyClBuffer[8];
                    // ✅ Read 응답 길이 검증
                    if (bytesRead < 9 + len || len < 0)
                    {
                        CallException(id, unit, function, excExceptionConnectionLost);
                        return;
                    }

                    data = new byte[len];
                    Array.Copy(tcpAsyClBuffer, 9, data, 0, len);
                }

                if (function > excExceptionOffset)
                {
                    function -= excExceptionOffset;
                    CallException(id, unit, function, tcpAsyClBuffer[8]);
                }
                else if (OnResponseData != null)
                {
                    OnResponseData(id, unit, function, data);
                }

                // ✅ 다음 수신 대기 (소켓 상태 체크)
                if (tcpAsyCl != null && tcpAsyCl.Connected)
                {
                    tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length,
                        SocketFlags.None, new AsyncCallback(OnReceive), tcpAsyCl);
                }
                //// 계속해서 다음 비동기 수신을 걸어두려면 다시 BeginReceive 호출
                //tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), tcpAsyCl);
            }
            catch (ObjectDisposedException)
            {
                // 소켓이 이미 닫힌 경우 - 조용히 종료
                return;
            }
            catch (SocketException)
            {
                CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
            }
            catch (Exception)
            {
                CallException(0xFFFF, 0xFF, 0xFF, excExceptionConnectionLost);
            }
        }

        // ------------------------------------------------------------------------
        // Write data and and wait for response
        private byte[] WriteSyncData(byte[] write_data, ushort id)
        {
            // ✅ null 체크 추가
            if (tcpSynCl == null || !tcpSynCl.Connected)
            {
                _connected = false;
                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                return null;
            }
            try
            {
                // ✅ Send 결과 체크
                int sent = tcpSynCl.Send(write_data, 0, write_data.Length, SocketFlags.None);
                if (sent == 0)
                {
                    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                    return null;
                }

                SocketError ecode;
                int result = tcpSynCl.Receive(tcpSynClBuffer, 0, tcpSynClBuffer.Length,
                    SocketFlags.None, out ecode);

                // ✅ result == 0을 먼저 체크
                if (result == 0)
                {
                    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                    return null;
                }

                // ✅ SocketError 체크
                if (ecode != SocketError.Success)
                {
                    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                    return null;
                }

                // ✅ 최소 헤더 크기 체크
                if (result < 9)
                {
                    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                    return null;
                }

                byte unit = tcpSynClBuffer[6];
                byte function = tcpSynClBuffer[7];
                byte[] data;

                // Response data is slave exception
                if (function > excExceptionOffset)
                {
                    function -= excExceptionOffset;
                    CallException(id, unit, function, tcpSynClBuffer[8]);
                    return null;
                }
                // Write response data
                else if ((function >= fctWriteSingleCoil) && (function != fctReadWriteMultipleRegister))
                {
                    data = new byte[2];
                    Array.Copy(tcpSynClBuffer, 10, data, 0, 2);
                }
                // Read response data
                else
                {
                    data = new byte[tcpSynClBuffer[8]];
                    Array.Copy(tcpSynClBuffer, 9, data, 0, tcpSynClBuffer[8]);
                }
                return data;
            }
            catch (SocketException ex)
            {
                // ✅ 소켓 에러 (연결 끊김 등)
                _connected = false;

                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                return null;
            }
            catch (ObjectDisposedException)
            {
                // ✅ 소켓이 Dispose된 경우
                _connected = false;

                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                return null;
            }
            catch (Exception ex)
            {
                // ✅ 기타 예외
                _connected = false;

                CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                return null;
            }

            //if (tcpSynCl.Connected)
            //{
            //    try
            //    {
            //        tcpSynCl.Send(write_data, 0, write_data.Length, SocketFlags.None);
            //        SocketError ecode = new SocketError();
            //        int result = tcpSynCl.Receive(tcpSynClBuffer, 0, tcpSynClBuffer.Length, SocketFlags.None, out ecode);

            //        byte unit = tcpSynClBuffer[6];
            //        byte function = tcpSynClBuffer[7];
            //        byte[] data;

            //        if (result == 0) CallException(id, unit, write_data[7], excExceptionConnectionLost);

            //        // ------------------------------------------------------------
            //        // Response data is slave exception
            //        if (function > excExceptionOffset)
            //        {
            //            function -= excExceptionOffset;
            //            CallException(id, unit, function, tcpSynClBuffer[8]);
            //            return null;
            //        }
            //        // ------------------------------------------------------------
            //        // Write response data
            //        else if ((function >= fctWriteSingleCoil) && (function != fctReadWriteMultipleRegister))
            //        {
            //            data = new byte[2];
            //            Array.Copy(tcpSynClBuffer, 10, data, 0, 2);
            //        }
            //        // ------------------------------------------------------------
            //        // Read response data
            //        else
            //        {
            //            data = new byte[tcpSynClBuffer[8]];
            //            Array.Copy(tcpSynClBuffer, 9, data, 0, tcpSynClBuffer[8]);
            //        }
            //        return data;
            //    }
            //    catch (SystemException)
            //    {
            //        CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            //    }
            //}
            //else CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            //return null;
        }
    }
}
