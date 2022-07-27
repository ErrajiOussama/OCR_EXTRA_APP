using Google.Cloud.Vision.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Npgsql;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using OCR_EXTRA_APP.models;

namespace OCR_EXTRA_APP.CS
{
    internal class Process_OCR
    {
        public Delimitateur GetExtraction_delimitateur (string[] acte_ocriser)
        {
            string _connexionbase = "";

            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"./config.json").Build();
                _connexionbase = builder["ConnexionString"];
                var sql = (new StreamReader(@"SQL/Get_delimitateur.sql")).ReadToEnd();
                using (var npgadapter= new NpgsqlDataAdapter(sql, _connexionbase))
                {
                    DataTable dt = new DataTable();
                    npgadapter.Fill(dt);
                    foreach(DataRow dr in dt.Rows)
                    {
                        if (dr["mot_cles"].ToString() == String.Join("\n", acte_ocriser)) ;
                    }
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return  ;
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
