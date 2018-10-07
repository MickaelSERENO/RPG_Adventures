using System;
using System.IO;

namespace DandDAdventures.Database
{
    /// <summary>
    /// Class representing a Resource (like image) to add to the database
    /// </summary>
    public class DBResource
    {
        /// <summary>
        /// The resource data
        /// </summary>
        private byte[] m_data;

        /// <summary>
        /// The data length
        /// </summary>
        private long m_length;

        /// <summary>
        /// The key associated with this Resource
        /// </summary>
        private String m_key;

        /// <summary>
        /// Constructor. Store the data contained in a Stream and the associated key
        /// </summary>
        /// <param name="input">The Stream to read at</param>
        /// <param name="key">The associated key</param>
        public DBResource(Stream input, String key)
        {
            m_key = key;

            byte[] buffer = new byte[16*1024];

            using(MemoryStream memStream = new MemoryStream())
            {
                int read;
                while((read = input.Read(buffer, 0, buffer.Length)) != 0)
                    memStream.Write(buffer, 0, read);
                m_data = memStream.GetBuffer();
                m_length = memStream.Length;
            }
        }

        /// <summary>
        /// Constructor. Store the data contained in a byte array and the associated key
        /// </summary>
        /// <param name="input">The data as a byte array</param>        
        /// <param name="length">The data length</param>
        /// <param name="key">The associated key</param>
        public DBResource(byte[] input, long length, String key)
        {
            Buffer.BlockCopy(input, 0, m_data, 0, (int)Math.Min(m_length, length));
            m_length = length;
        }

        /// <summary>
        /// Copy the content of the resource into a Stream
        /// </summary>
        /// <param name="s">The destination stream</param>
        public void CopyTo(Stream s)
        {
            s.Write(m_data, 0, (int)m_length);
        }

        /// <summary>
        /// The Key of the Resource
        /// </summary>
        public String Key    { get => m_key; set => m_key = value; }

        /// <summary>
        /// The Data of the Resource. See "Length" for knowing the size of the array (some unused byte can be in this array)
        /// </summary>
        public byte[] Data   { get =>m_data; }

        /// <summary>
        /// The Length of the Resource.
        /// </summary>
        public long   Length { get => m_length; }
    }
}
