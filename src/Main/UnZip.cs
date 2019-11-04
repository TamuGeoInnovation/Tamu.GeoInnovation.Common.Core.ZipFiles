// SharpZipLibrary samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace USC.GISResearchLab.Common.UnZipFile
{
    public class UnZip
    {
        private string _ZipFilePath;
        private string _ZipPassword;
        private ZipInputStream streamReader;
        private ZipEntry theEntry;
        private FileStream streamWriter;
        private string _OutputDirectory;

        public string OutputDirectory
        {
            get { return _OutputDirectory; }
            set
            {
                if (Directory.Exists(value))
                {
                    _OutputDirectory = value;
                    if (_OutputDirectory.Substring(_OutputDirectory.Length - 1, 1) != "\\") _OutputDirectory = _OutputDirectory + "\\";
                }
                else throw new IOException("Output directory not exsist.");
            }
        }
        public string ZipPassword
        {
            set { _ZipPassword = value; }
        }
        public string ZipFilePath
        {
            get { return _ZipFilePath; }
            set { if (File.Exists(value)) _ZipFilePath = value; else throw new IOException("Zip file not found"); }
        }

        public UnZip(string zipFilePath)
        {
            _ZipPassword = "";
            streamReader = null;
            streamWriter = null;
            theEntry = null;
            ZipFilePath = zipFilePath;
        }

        public bool NeedPassword()
        {
            bool ret = false;
            try
            {
                streamReader = new ZipInputStream(File.OpenRead(this._ZipFilePath));
                while ((theEntry = streamReader.GetNextEntry()) != null)
                    if (theEntry.IsCrypted)
                    {
                        ret = true;
                        break;
                    }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during opening/checking the zip file for encryption.", ex);
            }
            finally
            {
                theEntry = null;
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader = null;
                }
            }
            return ret;
        }

        public void DoUnZip()
        {
            try
            {
                string entryDirectoryName = "";
                string entryFileName = "";
                int size = 2048;
                byte[] data = null;

                if (_OutputDirectory == string.Empty) OutputDirectory = Path.GetDirectoryName(_ZipFilePath);

                using (streamReader = new ZipInputStream(File.OpenRead(_ZipFilePath)))
                {
                    theEntry = null;
                    streamReader.Password = this._ZipPassword;
                    while ((theEntry = streamReader.GetNextEntry()) != null)
                    {
                        entryDirectoryName = Path.GetDirectoryName(theEntry.Name);
                        entryFileName = Path.GetFileName(theEntry.Name);

                        // create directory
                        if (entryDirectoryName.Length > 0) Directory.CreateDirectory(OutputDirectory + entryDirectoryName);

                        if (entryFileName != String.Empty)
                        {
                            using (streamWriter = File.Create(OutputDirectory + theEntry.Name))
                            {
                                data = new byte[size];
                                while (true)
                                {
                                    size = streamReader.Read(data, 0, data.Length);
                                    if (size > 0) streamWriter.Write(data, 0, size);
                                    else break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ZipException("Error during unzip.", e);
            }
            finally
            {
                theEntry = null;
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader = null;
                }
            }
        }
    }
}