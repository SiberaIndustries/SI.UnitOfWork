using System;

namespace SI.UnitOfWork.Tests
{
    public static class GuidExtensions
    {
        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static int ToInt(this Guid value)
        {
            byte[] bytes = value.ToByteArray();
            int bint = BitConverter.ToInt32(bytes, 0);
            return bint;
        }
    }
}
