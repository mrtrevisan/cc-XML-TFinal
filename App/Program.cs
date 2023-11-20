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
        //path dos arquivos xml
        static string[] xmlFiles = {
            "xml/nota1.xml",
            "xml/nota2.xml",
            "xml/nota3.xml",
            "xml/nota4.xml",
            "xml/nota5.xml",
            "xml/nota6.xml"
        };

        //path dos arquivos json
        static string[] jsonFiles = {
            "json/nota1.json",
            "json/nota2.json",
            "json/nota3.json",
            "json/nota4.json",
            "json/nota5.json",
            "json/nota6.json"
        };

        //path do json schema
        static string schemaFile = "json/schema.json";

        static void convertXml(){
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

        static void generateSchema(){
            try
            {
                //escolhe um arquivo xml qualquer para gerar o json schema
                string XMLpath = "xml/nota1.xml";

                //carrega o XML original
                XDocument xmlDoc = XDocument.Load(XMLpath);
                //converte o XML para JSON
                string jsonS = JsonConvert.SerializeXNode(xmlDoc);
                //gera o JSON Schema a partir da string Json
                NJsonSchema.JsonSchema schema = NJsonSchema.JsonSchema.FromSampleJson(jsonS);

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

        static void validateJson() {
            //carrega o JSON Schema 
            string jsonSchema = File.ReadAllText(schemaFile);
            //parse do JSON Schema
            JSchema schema = JSchema.Parse(jsonSchema);

            //valida cada arquivo Json
            for (int i = 0; i < 6; i++){   
                try
                {
                    //carregar o arquivo JSON 
                    string JSONpath = jsonFiles[i];
                    string jsonS = File.ReadAllText(JSONpath);

                    //parse do JSON para um objeto JToken
                    JToken jToken = JToken.Parse(jsonS);

                    //valida o JSON com base no schema, os erros serão gravados numa lista
                    IList<string> messages;
                    bool isValid = jToken.IsValid(schema, out messages);

                    if (isValid)
                    {
                        Console.WriteLine($"\t-> O arquivo JSON {i+1} é válido de acordo com o esquema!");
                    }
                    else
                    {
                        Console.WriteLine($"\t-> O arquivo JSON {i+1} NÃO é válido de acordo com o esquema.");
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

        static void query1(){
            Console.WriteLine("Query 1: (a)Número de produtos em todas as notas e (b)Valor total dos produtos:");
            int nProd = 0;
            float tValue = .0f;
            
            for (int i = 0; i < 6; i++){
                try
                {
                    string caminhoArquivoJSON = jsonFiles[i];
                    string json = File.ReadAllText(caminhoArquivoJSON);
                    // Parse do JSON para um objeto JObject
                    JObject jsonObj = JObject.Parse(json);

                    //JToken dets = jsonObj["nfeProc"]["NFe"]["infNFe"]["det"];
                    JToken prods = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.det")!;
                    nProd += prods.Count();

                    foreach(var prod in prods){
                        tValue += (float)prod["prod"]!["vProd"]!;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }
            Console.WriteLine($"\ta) Total de produtos: {nProd}.\n\tb) Valor total dos produtos: {tValue}");
        }

        static void query2(){
            Console.WriteLine("Query 2: (a)Total do ICMS, (b)Total de frete dos produtos:");
            float vIcms = .0f;
            float vFrete = .0f;

            for (int i = 0; i < 6; i++){
                try
                {
                    string caminhoArquivoJSON = jsonFiles[i];
                    string json = File.ReadAllText(caminhoArquivoJSON);
                    // Parse do JSON para um objeto JObject
                    JObject jsonObj = JObject.Parse(json);

                    JToken totais = jsonObj.SelectToken("$.nfeProc.NFe.infNFe.total.ICMSTot")!;
                    vFrete += (float) totais["vFrete"]!;
                    vIcms += (float) totais["vICMS"]!;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }
            Console.WriteLine($"\ta) Total de ICSM: {vIcms}.\n\tb) Valor total de frete: {vFrete}.");

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Conversão dos arquivos XML para JSON:");
            convertXml();

            Console.WriteLine("-\nGeração do arquivo JSON Schema:");
            generateSchema();

            Console.WriteLine("-\nValidação dos arquivos JSON:");
            validateJson();
            /*

            Console.WriteLine("-\nQueries no JSON:");
            query1();
            query2();
            */
        }
    }
}