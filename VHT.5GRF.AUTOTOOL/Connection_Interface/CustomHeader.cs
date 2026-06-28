using System;
using System.Collections.Generic;
using System.Linq;

namespace _5GAutoTool
{
    public interface ICustomHeader
    {
        byte[] GetHeader();
    }

    #region OAM Header
    public class OamHeader : ICustomHeader
    {
        public const ushort Length = 16;
        public ushort TransactionId { get; set; }
        public ushort SourceModuleId { get; set; }
        public ushort DestinationModuleId { get; set; }
        public ushort ApiId { get; set; }
        public byte[] Reserver { get; set; }
        public ushort MessageLength { get; set; }

        public OamHeader()
        {
            TransactionId = 0x802D;
            SourceModuleId = 0xA9;
            DestinationModuleId = 0x1D;
            ApiId = 0xBB;
            Reserver = new byte[6];
        }

        public OamHeader(byte[] header) : this()
        {
            SetHeader(header);
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel);
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============

        public void SetHeader(byte[] header)
        {
            if (header == null) return;

            TransactionId = (ushort)(header[0] << 8 + header[1]);
            SourceModuleId = (ushort)(header[2] << 8 + header[3]);
            DestinationModuleId = (ushort)(header[4] << 8 + header[5]);
            ApiId = (ushort)(header[6] << 8 + header[7]);
            MessageLength = (ushort)(header[8] << 8 + header[9] - Length);
            Reserver = new ArraySegment<byte>(header, 10, 6).ToArray();
        }

        public byte[] GetHeader()
        {
            
            var header = ToBytes(TransactionId)
                .Concat(ToBytes(SourceModuleId))
                .Concat(ToBytes(DestinationModuleId))
                .Concat(ToBytes(ApiId))
                .Concat(ToBytes((ushort)(MessageLength + Length)))
                .Concat(Reserver)
                .Concat(new byte[MessageLength]);
            return header.ToArray();
        }

        private static IEnumerable<byte> ToBytes(ushort fragment)
        {
            return BitConverter.GetBytes(fragment).Reverse();
        }

        public override string ToString()
        {
            return $"OamHeader: {{{TransactionId}, {SourceModuleId}, {DestinationModuleId}, {ApiId}, {MessageLength}+16, {Reserver}}}";
        }
    }
    #endregion
}
