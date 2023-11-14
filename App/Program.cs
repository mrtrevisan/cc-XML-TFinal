using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace trabalhoFinal
{   
    class Program
    {
        static void convertXml(){
            for (int i = 1; i <= 6; i++){   
                try
                {
                    string caminhoArquivoXML = "xml/nota" + i.ToString() + ".xml";
                    string caminhoArquivoJSON = "json/nota" + i.ToString() + ".json";

                    XmlDocument documentoXML = new XmlDocument();
                    documentoXML.Load(caminhoArquivoXML);

                    string json = JsonConvert.SerializeXmlNode(documentoXML, Newtonsoft.Json.Formatting.Indented);

                    File.WriteAllText(caminhoArquivoJSON, json);

                    Console.WriteLine($"\t-> Arquivo XML {i} convertido para JSON com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\t\t-- correu um erro: " + ex.Message);
                }
            }
        }

        static void generateSchema(){
            try
            {
                string caminhoSchema = "json/schema.json";
                string caminhoArquivoXML = "xml/nota1.xml";

                // Carregar o XML original
                XDocument xml = XDocument.Load(caminhoArquivoXML);

                // Converter XML para JSON
                string json = JsonConvert.SerializeXNode(xml);

                // Converter o JSON para um objeto JToken
                JToken jToken = JToken.Parse(json);

                // Gerar o JSON Schema a partir do JToken
                NJsonSchema.JsonSchema schema = NJsonSchema.JsonSchema.FromSampleJson(json);

                //edita o tipo do "Det" para Array ou Object para validar todos os json
                schema.Definitions["Det"].Type = JsonObjectType.Array | JsonObjectType.Object;

                // Converter o JSON Schema para string
                string schemaString = schema.ToJson();

                //Cria o arquivo Json Schema
                File.WriteAllText(caminhoSchema, schemaString);
                Console.WriteLine("\t-> JSON Schema gerado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t\t-- Ocorreu um erro: " + ex.Message);
            }
        }

        static void validateJson() {
            for (int i = 1; i <= 6; i++){   
                try
                {
                    // Carregar o JSON Schema que você gerou anteriormente
                    string caminhoSchema = "json/schema.json";
                    string jsonSchema = File.ReadAllText(caminhoSchema);

                    // Carregar o arquivo JSON que você deseja validar
                    string caminhoArquivoJSON = "json/nota" + i.ToString() + ".json";
                    string json = File.ReadAllText(caminhoArquivoJSON);

                    // Parse do JSON Schema
                    JSchema schema = JSchema.Parse(jsonSchema);

                    // Parse do JSON para um objeto JToken
                    JToken jToken = JToken.Parse(json);

                    // Validar o JSON com base no esquema
                    IList<string> messages;
                    bool isValid = jToken.IsValid(schema, out messages);

                    if (isValid)
                    {
                        Console.WriteLine($"\t-> O arquivo JSON {i} é válido de acordo com o esquema!");
                    }
                    else
                    {
                        Console.WriteLine($"\t-> O arquivo JSON {i} NÃO é válido de acordo com o esquema.");
                        foreach (var error in messages)
                        {
                            Console.WriteLine($"\t\t-- Erro: {error}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }
        }

        static void Main(string[] args)
        {
            convertXml();
            Console.WriteLine("===");

            generateSchema();
            Console.WriteLine("===");
            
            validateJson();
        }
    }
}