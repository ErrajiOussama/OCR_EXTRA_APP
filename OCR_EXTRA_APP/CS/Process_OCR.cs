using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace OCR_EXTRA_APP.CS
{
    internal class Process_OCR
    {
        public string GetModel(string[] acte_ocriser)
        {
            
            return "hello";
        }
        public static async Task<string[]> getOCRImage(string pathimage)
        {
            try
            {
                if (!string.IsNullOrEmpty(pathimage))
                {
                    FlowDocument flowDocument = new FlowDocument();
                    var client = ImageAnnotatorClient.Create();
                    Random random = new Random();
                    var image = Image.FromFile(pathimage);
                    var response = await Task.Run(() => client.DetectText(image));
                    System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph();
                    string[] responseStr = new string[response.Count];
                    int i = 0;
                    foreach (var annotation in response)
                    {
                        if (annotation.Description != null)
                        {

                            responseStr[i] = annotation.Description;
                        }
                        i++;
                    }

                    return responseStr;
                }
                else
                {
                    throw new Exception("Charger une image");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
