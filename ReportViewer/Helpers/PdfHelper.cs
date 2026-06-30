
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ReportViewer.Helpers
{
    public class PdfHelper
    {
        public static Stream MergeFiles(List<Stream> fileNamesInTemp)
        {

            // Open the output document
            PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (var str in fileNamesInTemp)
            {
                //string filePath = tempPath + fileName;


                PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(str, PdfDocumentOpenMode.Import);
                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }
            var ms = new MemoryStream();
            // Save the document...
            outputDocument.Save(ms);
            return ms;
        }
    }
}