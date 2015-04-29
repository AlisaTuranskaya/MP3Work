using System;
using System.IO;
using System.Text;

namespace MP3Work
{
	internal class Program
	{
		private static void Main(string[] args)
		{
		}
	}

	public class ID3v1
	{
		public string Filename;
		public string Title;
		public string Artist;
		public string Album;
		public string Year;
		public string Comment;
		public int GenreID;
		public int Track;
		public bool hasTag;
		
		public ID3v1()
		{
			hasTag = false;
			Filename = "";
			Title = "";
			Artist = "";
			Album = "";
			Year = "";
			Comment = "";
			GenreID = 0;
			Track = 0;
		}

		public ID3v1(string filename)
		{
			Filename = filename;
		}

		public void Read()
		{
			// Read the 128 byte ID3 tag into a byte array

			var oFileStream = new FileStream(Filename, FileMode.Open);
			byte[] bBuffer = new byte[128];

			oFileStream.Seek(-128, SeekOrigin.End);

			oFileStream.Read(bBuffer, 0, 128);

			oFileStream.Close();

			// Convert the Byte Array to a String

			Encoding instEncoding = new ASCIIEncoding(); // NB: Encoding is an Abstract class

			string id3Tag = instEncoding.GetString(bBuffer);

			// If there is an attched ID3 v1.x TAG then read it

			if (id3Tag.Substring(0, 3) == "TAG")
			{
				Title = id3Tag.Substring(3, 30).Trim();
				Artist = id3Tag.Substring(33, 30).Trim();
				Album = id3Tag.Substring(63, 30).Trim();
				Year = id3Tag.Substring(93, 4).Trim();
				Comment = id3Tag.Substring(97, 28).Trim();
				// Get the track number if TAG conforms to ID3 v1.1
				Track = id3Tag[125] == 0 ? bBuffer[126] : 0;
				GenreID = bBuffer[127];
hasTag = true;

				// ********* IF USED IN ANGER: ENSURE to test for non-numeric year
			}
			else
			{
				hasTag = false;
			}
		}

		public void UpdateMP3Tag()
		{
			// Trim any whitespace

			Title = Title.Trim();
			Artist = Artist.Trim();
			Album = Album.Trim();
			Year = Year.Trim();
			Comment = this.Comment.Trim();

			// Ensure all properties are correct size
			if (Title.Length > 30) this.Title = this.Title.Substring(0, 30);
			if (Artist.Length > 30) this.Artist = this.Artist.Substring(0, 30);
			if (Album.Length > 30) this.Album = this.Album.Substring(0, 30);
			if (Year.Length > 4) this.Year = this.Year.Substring(0, 4);
			if (Comment.Length > 28) this.Comment = this.Comment.Substring(0, 28);

			// Build a new ID3 Tag (128 Bytes)

			byte[] tagByteArray = new byte[128];

			for (int i = 0; i < tagByteArray.Length; i++)
				tagByteArray[i] = 0; // Initialise array to nulls

			// Convert the Byte Array to a String
			Encoding instEncoding = new ASCIIEncoding(); // NB: Encoding is an Abstract class // ************ To DO: Make a shared instance of ASCIIEncoding so we don't keep creating/destroying it

			// Copy "TAG" to Array
			byte[] workingByteArray = instEncoding.GetBytes("TAG");
			Array.Copy(workingByteArray, 0, tagByteArray, 0, workingByteArray.Length);

			// Copy Title to Array
			workingByteArray = instEncoding.GetBytes(this.Title);
			Array.Copy(workingByteArray, 0, tagByteArray, 3, workingByteArray.Length);

			// Copy Artist to Array
			workingByteArray = instEncoding.GetBytes(this.Artist);
			Array.Copy(workingByteArray, 0, tagByteArray, 33, workingByteArray.Length);

			// Copy Album to Array
			workingByteArray = instEncoding.GetBytes(this.Album);
			Array.Copy(workingByteArray, 0, tagByteArray, 63, workingByteArray.Length);
			
			// Copy Year to Array
			workingByteArray = instEncoding.GetBytes(this.Year);
			Array.Copy(workingByteArray, 0, tagByteArray, 93, workingByteArray.Length);

			// Copy Comment to Array
			workingByteArray = instEncoding.GetBytes(this.Comment);
			Array.Copy(workingByteArray, 0, tagByteArray, 97, workingByteArray.Length);

			// Copy Track and Genre to Array
			tagByteArray[126] = System.Convert.ToByte(this.Track);
			tagByteArray[127] = System.Convert.ToByte(this.GenreID);

			// SAVE TO DISK: Replace the final 128 Bytes with our new ID3 tag

			var oFileStream = new FileStream(this.Filename, FileMode.Open);

			if (hasTag)
				oFileStream.Seek(-128, SeekOrigin.End);
			else
				oFileStream.Seek(0, SeekOrigin.End);

			oFileStream.Write(tagByteArray, 0, 128);
			oFileStream.Close();

			this.hasTag = true;
		}
	}
}