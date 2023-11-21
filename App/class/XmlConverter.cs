using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Converter
{
    class XmlConverter {
        public void Convert(string[] xmlFiles, string[] jsonFiles){
            for (int i = 0; i < 6; i++){   
                try
                {
                    //path dos arquivos de input e output
                    string XMLpath = xmlFiles[i];
                    string JSONpath = jsonFiles[i];

                    //inicializa um XmlDocument com o conteudo do arquivo XML
                    XmlDocument XMLdoc = new XmlDocument();
                    XMLdoc.Load(XMLpath);

                    //converte para uma string json
                    string jsonS = JsonConvert.SerializeXmlNode(XMLdoc, Newtonsoft.Json.Formatting.Indented);

                    //IMPORTANTE
                    //passo importante na conversão, o objeto det precisa ser um array
                    JObject jsonObj = JObject.Parse(jsonS);
                    //força o objeto 'det' para sempre ser um array
                    JToken dets = jsonObj["nfeProc"]!["NFe"]!["infNFe"]!["det"]!;
                    if (dets is JObject)
                    {
                        JArray array = [dets];
                        jsonObj["nfeProc"]!["NFe"]!["infNFe"]!["det"] = array;
                        jsonS = jsonObj.ToString();
                    }

                    //salva o arquivo
                    File.WriteAllText(JSONpath, jsonS);

                    Console.WriteLine($"\t-> Arquivo XML {i+1} convertido para JSON com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\t\t-- correu um erro: " + ex.Message);
                }
            }
        }

        internal void Convert(object xmlFiles, object jsonFiles)
        {
            throw new NotImplementedException();
        }
    }
}