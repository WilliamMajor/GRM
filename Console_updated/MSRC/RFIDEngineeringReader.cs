using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RFID.ReaderComm;
using RFID.Reader;

#pragma warning disable CS1591

namespace MSRC
{
    public class RFIDEngineeringReader : RFIDReader
    {
        private enum EngModuleCommands
        {
            SET_HOPPING_FREQS = 0x14,
            GET_HOPPING_FREQS = 0x16,
            SET_CW_SETTING = 0x24,
            GET_CW_SETTING = 0x26,
            ENTER_MODULE_FW_UPDATE_MODE = 0x30,
            GET_MOD_EXTD_ERROR_CODE = 0x40,
            CLR_MOD_EXTD_ERROR_CODE = 0x42,
        }

        private enum EngSensorCommands
        {
            SET_SENSARRAY_SERNUM = 0x0C,
            RFID_MOD_ENABLE_RD = 0x30,
            RFID_MOD_ENABLE_CHANGE = 0x32,
            GET_BAUD_RATE = 0x38,
            SET_BAUD_RATE = 0x3A,
            POE_GET_NCSW_STATUS = 0x40,
            POE_SET_NCSW_STATUS = 0x42,
            POE_GET_NOSW_STATUS = 0x44,
            POE_SET_NOSW_STATUS = 0x46,
            POE_GET_PSESW_STATUS = 0x48,
            POE_SET_PSESW_STATUS = 0x4A,
            PSE_GET_IV_READINGS = 0x50,
            POE_GET_SOURCE_COUNTS = 0x52,
            READ_5980_REGISTER_VALUES = 0x54,
            WRITE_5980_REGISTER_VALUES = 0x56,
            SET_ANTENNA_CONN_NOTIFICATION = 0xC0,
            GET_PROFILE_TIME = 0xD0,
            RESET_I2C_BUS = 0xD2,
            READ_ETHCHIP_REGISTER_VALUES = 0xD4,
            WRITE_ETHCHIP_REGISTER_VALUES = 0xD6,
            TEST_WDT = 0xD8,
            RFID_MOD_DISABLE_AT_STARTUP_CHANGE = 0xDA,
            depSET_READER_TYPE = 0xE6,
            SET_READER_TYPE = 0xA6,
            depSET_READER_CONFIGURATION = 0xE8,
            SET_READER_CONFIGURATION = 0xAA,
            GET_MODULE_REGISTER_INFO = 0xDC,
            GET_BOOTLOADER_CONFIG = 0xEA,
            SET_BOOTLOADER_CONFIG = 0xEC,
            ENTER_BOOTLOADER_MODE = 0xF2,
            ENTER_BTL_PROGRAMMING_MODE = 0xF4,
        }

        Int32 savedReceiveTimeout;

        public Byte BaudRate
        {
            get
            {
                Byte baudRate;
                if (this.GetBaudRate(out baudRate) == RFIDStatus.OK)
                    return baudRate;
                else
                    throw new Exception("Get Baud Rate returned error status.");
            }

            set
            {
                if (this.SetBaudRate(value) != RFIDStatus.OK)
                    throw new Exception("Set Baud Rate returned error status.");
            }
        }

        public RFIDEngineeringReader(String sIpAddress, UInt16 iPortNumber)
        : base(sIpAddress, iPortNumber)
        {
        }

        public RFIDEngineeringReader(String sIpAddress, UInt16 iPortNumber, RFID.Reader.TagReadHandler continuousTagReadHandler)
        : base(sIpAddress, iPortNumber, continuousTagReadHandler)
        {
        }

        #region Module Interface Functions

        // Get & Set the baud rate
        public RFIDStatus GetBaudRate(out Byte baudRate)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (Byte)EngSensorCommands.GET_BAUD_RATE);

            baudRate = this.responseBuffer[0];

            return RFIDStatus.OK;
        }

        public RFIDStatus SetBaudRate(Byte baudRate)
        {
            Byte[] sendBuffer = new Byte[1];

            sendBuffer[0] = baudRate;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (Byte)EngSensorCommands.SET_BAUD_RATE,
                                                           sendBuffer, sendBuffer.Length);

            return (responseLength == 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        // Functions used primarily for interaction with module around FCC testing

        public RFIDStatus GetHoppingFrequencies(Int32 antennaId, ref Int32 numberOfFreqs, ref UInt32[] freqs)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.GET_HOPPING_FREQS);

            numberOfFreqs = responseBuffer[0];

            for (int idx = 0; idx < numberOfFreqs; idx++)
            {
                freqs[idx] = (UInt32)((responseBuffer[idx * 3 + 1] * 256 + responseBuffer[idx * 3 + 2]) * 256 + responseBuffer[idx * 3 + 3]);
            }

            return RFIDStatus.OK;
        }

        public RFIDStatus GetHoppingFrequencies(Int32 antennaId, ref Int32 numberOfFreqs, ref Int32[] freqs)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.GET_HOPPING_FREQS);

            numberOfFreqs = responseBuffer[0];

            for (int idx = 0; idx < numberOfFreqs; idx++)
            {
                freqs[idx] = (responseBuffer[idx * 3 + 1] * 256 + responseBuffer[idx * 3 + 2]) * 256 + responseBuffer[idx * 3 + 3];
            }

            return RFIDStatus.OK;
        }

        public RFIDStatus SetHoppingFrequencies(UInt32[] frequencies, Byte numberOfFrequencies)
        {
            Int32 sendBufLength = numberOfFrequencies * 3 + 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            Int32 bufLoc = 0;
            sendBuffer[bufLoc++] = numberOfFrequencies;

            for (Byte idx = 0; idx < numberOfFrequencies; idx++)
            {
                sendBuffer[bufLoc++] = (Byte)((frequencies[idx] >> 16) & 0xFF);
                sendBuffer[bufLoc++] = (Byte)((frequencies[idx] >> 8) & 0xFF);
                sendBuffer[bufLoc++] = (Byte)(frequencies[idx] & 0xFF);
            }

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.SET_HOPPING_FREQS,
                                                           sendBuffer, sendBufLength);

            return (responseLength == 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetCWSetting(ref Boolean cwOn)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.GET_CW_SETTING);

            cwOn = (responseBuffer[0] == 1);

            return RFIDStatus.OK;
        }

        public RFIDStatus SetCWSetting(Boolean cwSetting)
        {
            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = (Byte)(cwSetting ? 1 : 0);

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.SET_CW_SETTING,
                                                           sendBuffer, sendBufLength);

            return (responseBuffer[0] == 0x01) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus SetCWSetting(Boolean cwSetting, Byte portId, Double powerLevel)
        {
            const Int32 sendBufLength = 4;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = (Byte)(cwSetting ? 1 : 0);
            sendBuffer[1] = portId;
            sendBuffer[2] = (Byte)(((int)(powerLevel * 100) >> 8) & 0x00FF);
            sendBuffer[3] = (Byte)((int)(powerLevel * 100) & 0x00FF);

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.SET_CW_SETTING,
                                                           sendBuffer, sendBufLength);

            return (responseBuffer[0] == 0x01) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetModuleExtendedErrorCode(Byte errorType, out UInt16 extendedErrorCode)
        {
            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = errorType;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (Byte)EngModuleCommands.GET_MOD_EXTD_ERROR_CODE,
                                                           sendBuffer, sendBufLength);

            extendedErrorCode = (UInt16)((responseBuffer[1] << 8) | responseBuffer[2]);

            return (responseBuffer[0] == 0x01) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus ClearModuleExtendedErrorCode()
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                          RFIDReader.maxResponseSize,
                                                          ReaderSubsystem.RFID_Module,
                                                          (Byte)EngModuleCommands.CLR_MOD_EXTD_ERROR_CODE);

            return (responseBuffer[0] == 0x01) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetRegisterContents(Byte registerId, Byte[] registerData, out Byte[] registerContents)
        {
            Byte[] sendBuffer = new Byte[registerData.Length + 2];

            sendBuffer[0] = registerId;
            sendBuffer[1] = (Byte)registerData.Length;

            for (int idx = 0; idx < registerData.Length; idx++)
                sendBuffer[2 + idx] = registerData[idx];

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (Byte)EngSensorCommands.GET_MODULE_REGISTER_INFO,
                                                           sendBuffer, sendBuffer.Length);

            registerContents = new Byte[responseLength];
            Array.Copy(responseBuffer, registerContents, responseLength);

            return (responseLength == 16) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public void SetLongReceiveDelay()
        {
            this.savedReceiveTimeout = this.client.ReceiveTimeout;
            this.client.ReceiveTimeout = RFIDReaderComm.INFINITE_TIMEOUT;
        }

        public void RestoreReceiveDelay()
        {
            this.client.ReceiveTimeout = this.savedReceiveTimeout;
        }

        public RFIDStatus GetAccessResponse(out Byte[] responseData)
        {
            Byte[] buffer = new byte[64];
            Int32 responseLength = this.client.DoReceive(ref buffer,
                                                         buffer.Length,
                                                         ReaderSubsystem.RFID_Controller, 
                                                         (Byte)EngSensorCommands.GET_MODULE_REGISTER_INFO);

            responseData = new byte[responseLength];

            Array.Copy(buffer, responseData, responseLength);

            return RFIDStatus.OK;
        }

        #endregion Module Interface Functions

        #region BiPoE Management

        // BiPoE management

        public RFIDStatus GetNcSwitchStatus(ref Byte[] ncSwitchStatus)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.POE_GET_NCSW_STATUS);

            if (responseLength == 1)
            {
                for (int idx = 0; idx < 4; idx++)
                {
                    ncSwitchStatus[3 - idx] = (Byte)(((responseBuffer[0] & (0x01 << idx)) != 0) ? 1 : 0);
                }
            }
            else
                return RFIDStatus.FAILED;

            return RFIDStatus.OK;
        }

        public RFIDStatus SetNcSwitchStatus(Byte[] ncSwitchStatus)
        {
            int switchSetup = 0;

            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            for (int idx = 0; idx < 4; idx++)
            {
                switchSetup = (switchSetup << 1) | ncSwitchStatus[idx];
            }

            sendBuffer[0] = (Byte)switchSetup;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.POE_SET_NCSW_STATUS,
                                                           sendBuffer, sendBufLength);

            return (responseLength == 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetNoSwitchStatus(ref Byte[] noSwitchStatus)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.POE_GET_NOSW_STATUS);

            if (responseLength == 1)
            {
                for (int idx = 0; idx < 4; idx++)
                {
                    noSwitchStatus[3 - idx] = (Byte)(((responseBuffer[0] & (0x01 << idx)) != 0) ? 1 : 0);
                }
            }
            else
                return RFIDStatus.FAILED;

            return RFIDStatus.OK;
        }

        public RFIDStatus SetNoSwitchStatus(Byte[] noSwitchStatus)
        {
            int switchSetup = 0;

            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            for (int idx = 0; idx < 4; idx++)
            {
                switchSetup = (switchSetup << 1) | noSwitchStatus[idx];
            }

            sendBuffer[0] = (Byte)switchSetup;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.POE_SET_NOSW_STATUS,
                                                           sendBuffer, sendBufLength);

            return (responseLength == 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetPseSwitchStatus(ref Byte[] pseSwitchStatus)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.POE_GET_PSESW_STATUS);

            if (responseLength == 2)
            {
                for (int idx = 0; idx < 4; idx++)
                {
                    pseSwitchStatus[3 - idx] = (Byte)(((responseBuffer[1] & (0x01 << idx * 2)) != 0) ? 1 : 0);
                }
            }
            else
                return RFIDStatus.FAILED;

            return (responseLength == 2 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus SetPseSwitchStatus(Byte[] pseSwitchStatus)
        {
            int switchSetup = 0;

            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            for (int idx = 0; idx < 4; idx++)
            {
                switchSetup = (switchSetup << 2) | pseSwitchStatus[idx];
            }

            sendBuffer[0] = (Byte)switchSetup;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.POE_SET_PSESW_STATUS,
                                                           sendBuffer, sendBufLength);

            return (responseLength == 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus Read5980Register(Byte registerId, out Byte registerValue)
        {
            const Int32 sendBufLength = 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = 1;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.READ_5980_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            Console.WriteLine(BitConverter.ToString(this.responseBuffer, 0, responseLength));

            if (responseBuffer[0] == 1)
                registerValue = this.responseBuffer[3];
            else
                registerValue = 0xFF;

            return (responseLength >= 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus Read5980Registers(Byte registerId, Byte dataLength, ref Byte[] registerValues)
        {
            const Int32 sendBufLength = 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = dataLength;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.READ_5980_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            Console.WriteLine(BitConverter.ToString(this.responseBuffer, 0, responseLength));

            if (responseBuffer[0] == 1)
            {
                for (int idx = 0; idx < dataLength; idx++)
                {
                    registerValues[idx] = this.responseBuffer[idx + 3];
                }
            }

            return (responseLength >= 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus Write5980Register(Byte registerId, Byte registerValue)
        {
            Int32 sendBufLength = 3;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = 1;
            sendBuffer[2] = registerValue;

            Console.WriteLine(BitConverter.ToString(sendBuffer, 0, sendBuffer.Length));

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.WRITE_5980_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            return (responseLength >= 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus Write5980Registers(Byte registerId, Byte dataLength, Byte[] registerValues)
        {
            Int32 sendBufLength = dataLength + 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = dataLength;

            for (int idx = 0; idx < dataLength; idx++)
            {
                sendBuffer[idx + 2] = registerValues[idx];
            }

            Console.WriteLine(BitConverter.ToString(sendBuffer, 0, dataLength + 2));

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.WRITE_5980_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            return (responseLength >= 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus ReadEthChipRegister(Byte registerId, out Byte registerValue)
        {
            const Int32 sendBufLength = 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = 1;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.READ_ETHCHIP_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            Console.WriteLine(BitConverter.ToString(this.responseBuffer, 0, responseLength));
            registerValue = responseBuffer[0];

            return (responseLength >= 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus ReadEthChipRegisters(Byte registerId, Byte dataLength, ref Byte[] registerValues)
        {
            const Int32 sendBufLength = 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = dataLength;

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.READ_ETHCHIP_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            Console.WriteLine(BitConverter.ToString(this.responseBuffer, 0, responseLength));
            registerValues[0] = responseBuffer[0];

            return (responseLength >= 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus WriteEthChipRegister(Byte registerId, Byte writeMask, Byte registerValue)
        {
            Int32 sendBufLength = 3;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = writeMask;

            sendBuffer[2] = registerValue;

            Console.WriteLine(BitConverter.ToString(sendBuffer, 0, sendBufLength));

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.WRITE_ETHCHIP_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            return (responseLength >= 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus WriteEthChipRegisters(Byte registerId, Byte writeMask, Byte[] registerValues)
        {
            Int32 sendBufLength = 3;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = registerId;
            sendBuffer[1] = writeMask;

            sendBuffer[2] = registerValues[0];

            Console.WriteLine(BitConverter.ToString(sendBuffer, 0, sendBufLength));

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.WRITE_ETHCHIP_REGISTER_VALUES,
                                                           sendBuffer, sendBufLength);

            return (responseLength >= 1 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetPSEIVReadings(ref UInt32[] pseCurrents, ref UInt32[] pseVoltages)
        {
            int responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                         RFIDReader.maxResponseSize,
                                                         ReaderSubsystem.RFID_Controller,
                                                         (int)EngSensorCommands.PSE_GET_IV_READINGS);

            for (int idx = 0; idx < 4; idx++)
            {
                pseCurrents[idx] = (UInt32)((responseBuffer[1 + idx * 8] << 24) |
                                            (responseBuffer[2 + idx * 8] << 16) |
                                            (responseBuffer[3 + idx * 8] << 8) |
                                             responseBuffer[4 + idx * 8]);

                pseVoltages[idx] = (UInt32)((responseBuffer[5 + idx * 8] << 24) |
                                            (responseBuffer[6 + idx * 8] << 16) |
                                            (responseBuffer[7 + idx * 8] << 8) |
                                             responseBuffer[8 + idx * 8]);
            }

            return (responseLength == 33 && responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetPoECounts(ref Byte[] PoECounts)
        {
            this.client.SendMessage(ref this.responseBuffer,
                                     RFIDReader.maxResponseSize,
                                     ReaderSubsystem.RFID_Controller,
                                     (int)EngSensorCommands.POE_GET_SOURCE_COUNTS);


            for (int idx = 0; idx < 5; idx++)
                PoECounts[idx] = responseBuffer[idx];

            return RFIDStatus.OK;
        }

        #endregion BiPoE Management

        #region Firmware Update Support

        // Firmware Update Support

        //public RFIDStatus GetBootloaderInfo(ref UInt16 bootloaderPort)
        //{
        //    Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
        //                                                   RFIDReader.maxResponseSize,
        //                                                   ReaderSubsystem.RFID_Controller,
        //                                                   (int)EngSensorCommands.GET_BOOTLOADER_CONFIG);

        //    if (responseLength == 2)
        //    {
        //        bootloaderPort = (UInt16)((this.responseBuffer[0] << 8) | this.responseBuffer[1]);
        //    }
        //    else
        //    {
        //        bootloaderPort = 0;
        //        return RFIDStatus.FAILED;
        //    }

        //    return RFIDStatus.OK;
        //}

        public RFIDStatus SetBootloaderInfo(UInt16 bootloaderPort)
        {
            const Int32 sendBufLength = 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = (Byte)((bootloaderPort >> 8) & 0xFF);
            sendBuffer[1] = (Byte)(bootloaderPort & 0xFF);

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.SET_BOOTLOADER_CONFIG,
                                                           sendBuffer, sendBufLength);

            return (responseLength == 1 && this.responseBuffer[0] == 1) ?
                        RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus EnterBootloaderMode()
        {
            // There won't be any direct response to this command.
            this.client.SendMessageNoReply(ReaderSubsystem.RFID_Controller,
                                           (int)EngSensorCommands.ENTER_BOOTLOADER_MODE);

            // The client will restart after handling firmware updates.
            // Close the socket so that it has to be properly reopened 
            // the next time we try to communicate with the device.
            this.client.Shutdown();

            return RFIDStatus.OK;
        }

        public RFIDStatus EnterBootloaderProgrammingMode()
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.ENTER_BTL_PROGRAMMING_MODE);

            if (this.responseBuffer[0] != 0)
                this.client.Shutdown();

            return (this.responseBuffer[0] != 0) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus EnterModuleProgrammingMode()
        {
            // Command for putting the M700 module into firmware update mode.

            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Module,
                                                           (int)EngModuleCommands.ENTER_MODULE_FW_UPDATE_MODE);

            return (responseBuffer[0] != 0) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        #endregion Firmware Update Support

        #region Manufacturing Reader Configuration Functions

        // Manufacturing-time reader configuration functions

        //////////////////////////////////////////////////////////////////////////////////////////
        ///
        /// <Method>
        ///   RFIDStatus GetSerialNumber(ref String SensArraySerNum)
        /// </Method>
        ///
        /// <Summary>
        ///   This method retrieves the hardware Major.Minor.Revision number of the sensor.
        /// </Summary>
        ///
        /// <Parameters>
        ///   <Parameter>
        ///     <ParameterName>
        ///       SensArraySerNum
        ///     </ParameterName>
        ///     <ParameterType>
        ///       ref String
        ///     </ParameterType>
        ///     <ParamDescription>
        ///       Output returning the serial number of the sensor.
        ///     </ParamDescription>
        ///   </Parameter>
        /// </Parameters>
        ///
        /// <ReturnType>
        ///   This function will return RFIDStatus.OK if it is able to read and return the
        ///   the sensor's firmware version. It will return RFIDStatus.FAILED otherwise.
        /// </ReturnType>
        ///
        /// <Exceptions>
        ///   <ul>A System.TimeoutException will be thrown if a connection to the sensor cannot
        ///   be established or the module doesn't respond.</ul>
        ///   <ul>A System.IndexOutOfRangeException will be thrown if the length of the
        ///   SensARraySerNum string is longer than 8 characters.</ul>
        /// </Exceptions>
        ///
        //////////////////////////////////////////////////////////////////////////////////////////

        public RFIDStatus SetSensArraySerialNumber(String SensArraySerNum)
        {
            Int32 sendBufLength = 8;
            Byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(SensArraySerNum);

            if (sendBuffer.Length > 8)
                throw new System.IndexOutOfRangeException();
            else if (sendBuffer.Length < 8)
                Array.Resize<Byte>(ref sendBuffer, 8);

            Array.Clear(sendBuffer, SensArraySerNum.Length, sendBufLength - SensArraySerNum.Length);

            Int32 responseLength = this.client.SendMessage(ref responseBuffer, maxResponseSize,
                                                            ReaderSubsystem.RFID_Controller,
                                                            (int)EngSensorCommands.SET_SENSARRAY_SERNUM,
                                                            sendBuffer, sendBufLength);


            return (responseBuffer[0] == 0x01) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus SetReaderType(string readerType)
        {
            Int32 sendBufLength = readerType.Length + 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            Byte[] converted = System.Text.Encoding.UTF8.GetBytes(readerType);

            for (int idx = 0; idx < readerType.Length; idx++)
                sendBuffer[idx] = converted[idx];
            sendBuffer[readerType.Length] = 0x00;

            Boolean tryDeprecatedCommand = false;

            try
            {
                this.client.SendMessage(ref this.responseBuffer,
                                        RFIDReader.maxResponseSize,
                                        ReaderSubsystem.RFID_Controller,
                                        (int)EngSensorCommands.SET_READER_TYPE,
                                        sendBuffer, sendBufLength);
            }
            catch (RFID.ReaderComm.BadCommandException)
            {
                tryDeprecatedCommand = true;
            }

            if (tryDeprecatedCommand)
            {
                this.client.SendMessage(ref this.responseBuffer,
                                        RFIDReader.maxResponseSize,
                                        ReaderSubsystem.RFID_Controller,
                                        (int)EngSensorCommands.depSET_READER_TYPE,
                                        sendBuffer, sendBufLength);
            }

            return (responseBuffer[0] == 0x01) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus SetReaderConfiguration(int readerConfiguration)
        {
            RFIDStatus returnCode = RFIDStatus.OK;

            Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = (Byte)readerConfiguration;

            Boolean tryDeprecatedCommand = false;

            try
            {
                this.client.SendMessage(ref this.responseBuffer,
                                        RFIDReader.maxResponseSize,
                                        ReaderSubsystem.RFID_Controller,
                                        (int)EngSensorCommands.SET_READER_CONFIGURATION,
                                        sendBuffer, sendBufLength);
            }
            catch (RFID.ReaderComm.BadCommandException)
            {
                tryDeprecatedCommand = true;
            }

            if (tryDeprecatedCommand)
            {
                this.client.SendMessage(ref this.responseBuffer,
                                        RFIDReader.maxResponseSize,
                                        ReaderSubsystem.RFID_Controller,
                                        (int)EngSensorCommands.depSET_READER_CONFIGURATION,
                                        sendBuffer, sendBufLength);
            }

            if (responseBuffer[0] == 0x01)
            {
                returnCode = RFIDStatus.OK;
            }
            else
            {
                returnCode = RFIDStatus.FAILED;
            }

            return returnCode;
        }

        #endregion

        #region Miscellaneous functions

        // Miscellaneous

        public RFIDStatus ClearInputBuffer()
        {
            this.client.ClearBufferedInput();

            return RFIDStatus.OK;
        }

        public RFIDStatus ResetI2CBus()
        {
            int responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                         RFIDReader.maxResponseSize,
                                                         ReaderSubsystem.RFID_Controller,
                                                         (int)EngSensorCommands.RESET_I2C_BUS);

            return (this.responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus TestWDT()
        {
            // We don't expect a response back from this command, so we will
            // will always return OK.

            int responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                         RFIDReader.maxResponseSize,
                                                         ReaderSubsystem.RFID_Controller,
                                                         (int)EngSensorCommands.TEST_WDT);

            return RFIDStatus.OK;
        }

        public RFIDStatus SetAntennaConnectionConfiguration(Byte enableIndicator, Byte saveAsStartupConfig)
        {
            const Int32 sendBufLength = 2;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = enableIndicator;
            sendBuffer[1] = saveAsStartupConfig;

            int responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                         RFIDReader.maxResponseSize,
                                                         ReaderSubsystem.RFID_Controller,
                                                         (int)EngSensorCommands.SET_ANTENNA_CONN_NOTIFICATION,
                                                         sendBuffer, sendBufLength);

            return (responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus GetProfileTime(ref UInt32 profileTime)
        {
            int responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                         RFIDReader.maxResponseSize,
                                                         ReaderSubsystem.RFID_Controller,
                                                         (int)EngSensorCommands.GET_PROFILE_TIME);

            profileTime = (UInt32)((this.responseBuffer[0] << 24) |
                                   (this.responseBuffer[1] << 16) |
                                   (this.responseBuffer[2] << 8) |
                                    this.responseBuffer[3]);

            return RFIDStatus.OK;
        }

        public RFIDStatus GetModuleEnabledState(ref Boolean moduleEnabled)
        {
            Int32 responseLength = this.client.SendMessage(ref this.responseBuffer,
                                                           RFIDReader.maxResponseSize,
                                                           ReaderSubsystem.RFID_Controller,
                                                           (int)EngSensorCommands.RFID_MOD_ENABLE_RD);
            moduleEnabled = (responseBuffer[0] != 0);

            return RFIDStatus.OK;
        }

        public RFIDStatus SetModuleEnabledState(Boolean enabled)
        {
            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            sendBuffer[0] = (Byte)(enabled ? 0x01 : 0x00);

            this.client.SendMessage(ref this.responseBuffer,
                                    RFIDReader.maxResponseSize,
                                    ReaderSubsystem.RFID_Controller,
                                    (int)EngSensorCommands.RFID_MOD_ENABLE_CHANGE,
                                    sendBuffer, sendBufLength);

            return (this.responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus SetModuleStartupEnabledState(Boolean enabled)
        {
            const Int32 sendBufLength = 1;
            Byte[] sendBuffer = new Byte[sendBufLength];

            // The logic here is inverted since firmware logic is to disable the module at
            // startup if the configuration bit is set to 1.
            sendBuffer[0] = (Byte)(enabled ? 0x00 : 0x01);

            this.client.SendMessage(ref this.responseBuffer,
                                    RFIDReader.maxResponseSize,
                                    ReaderSubsystem.RFID_Controller,
                                    (int)EngSensorCommands.RFID_MOD_DISABLE_AT_STARTUP_CHANGE,
                                    sendBuffer, sendBufLength);

            return (this.responseBuffer[0] == 1) ? RFIDStatus.OK : RFIDStatus.FAILED;
        }

        public RFIDStatus SendBogusSensArrayMessage(Byte messageId, UInt16 dataLength = 0)
        {
            Byte[] sendBuffer = null;

            if (dataLength > 0)
                sendBuffer = new Byte[dataLength];

            int responsLength = this.client.SendMessage(ref this.responseBuffer,
                                                        RFIDReader.maxResponseSize,
                                                        ReaderSubsystem.RFID_Controller,
                                                        messageId,
                                                        sendBuffer, dataLength);

            return (responsLength == 2 && this.responseBuffer[0] == 0xFE && this.responseBuffer[1] == 0x20) ?
                                                                        RFIDStatus.OK : RFIDStatus.FAILED;
        }

        #endregion

        #region M700 Firmware Update Support

        // Public passthrough methods so underlying client object doesn't need to be exposed.

        public Int32 ReadByte(ref Byte[] byteRead)
        {
            // Returns number of bytes read & byteRead array filled in.
            return this.client.DoReceiveByte(ref byteRead, byteRead.Length);
        }

        public void SendData(Byte[] messageData, Int32 dataLength)
        {
            this.client.SendMessageNoReply(ReaderSubsystem.SendRawData, 0,
                                           messageData, dataLength);
        }

        #endregion M700 Firmware Update Support
    }
}
