using System.Xml.Linq;
using Newtonsoft.Json;
using NJsonSchema;

namespace Generator
{
    class SchemaGenerator {
        public void Generate(string XmlPath, string schemaFile){
            try
            {
                //carrega o XML original
                XDocument xmlDoc = XDocument.Load(XmlPath);
                //converte o XML para JSON
                string jsonS = JsonConvert.SerializeXNode(xmlDoc);
                //gera o JSON Schema a partir da string Json
                JsonSchema schema = JsonSchema.FromSampleJson(jsonS);

                //IMPORTANTE
                //edita o tipo do objeto "det" para Array
                schema.Definitions["Det"].Type = JsonObjectType.Array;

                // Converter o JSON Schema para string
                string schemaS = schema.ToJson();

                //Cria o arquivo Json Schema
                File.WriteAllText(schemaFile, schemaS);
                Console.WriteLine("\t-> JSON Schema gerado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t\t-- Ocorreu um erro: " + ex.Message);
            }
        }
    }
}